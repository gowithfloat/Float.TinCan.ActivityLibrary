using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Float.Core.UI;
using Float.Core.UX;
using Float.FileDownloader;
using Float.TinCan.ActivityLibrary.Definition;
using Float.TinCan.LocalLRSServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TinCan;
using Xamarin.Forms;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Activity launch coordinator.
    /// </summary>
    public abstract class ActivityLaunchCoordinator : Coordinator
    {
        readonly IRemoteFileProvider remoteFileProvider;
        readonly IActivityMetaDataProvider metaDataProvider;
        readonly ILRS lrs;
        readonly ILRSServerDelegate serverDelegate;
        readonly IRemoteFileProcessor remoteFileProcessor;
        readonly Uri localServerBaseUri;
        readonly LRSServerInfo lrsServerInfo;

        string startLocation;
        DownloadStatus downloadStatus;
        bool isHidingDownloadPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLaunchCoordinator"/> class.
        /// </summary>
        /// <param name="activity">The Activity.</param>
        /// <param name="remoteFileProvider">Remote file provider.</param>
        /// <param name="metaDataProvider">Meta data provider.</param>
        /// <param name="lrs">xAPI LRS for storing statements.</param>
        /// <param name="serverDelegate">LRS server delegate.</param>
        /// <param name="localServerBaseUri">Provides a base Uri for start location.</param>
        /// <param name="remoteFileProcessor">The remote file processor.  Optional; if ignored, <see cref="TinCanLessonFileProcessor"/> will be used.</param>
        protected ActivityLaunchCoordinator(IActivity activity, IRemoteFileProvider remoteFileProvider, IActivityMetaDataProvider metaDataProvider, ILRS lrs, ILRSServerDelegate serverDelegate, Uri localServerBaseUri, IRemoteFileProcessor remoteFileProcessor = null)
        {
            this.Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            this.remoteFileProvider = remoteFileProvider ?? throw new ArgumentNullException(nameof(remoteFileProvider));
            this.metaDataProvider = metaDataProvider ?? throw new ArgumentNullException(nameof(metaDataProvider));
            this.lrs = lrs ?? throw new ArgumentNullException(nameof(lrs));
            this.serverDelegate = serverDelegate ?? throw new ArgumentNullException(nameof(serverDelegate));
            this.localServerBaseUri = localServerBaseUri ?? throw new ArgumentNullException(nameof(localServerBaseUri));
            this.remoteFileProcessor = remoteFileProcessor ?? new TinCanLessonFileProcessor(metaDataProvider, activity);
            this.lrsServerInfo = new LRSServerInfo(null, null);
        }

        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <value>The activity.</value>
        public IActivity Activity { get; }

        /// <summary>
        /// Gets the available post assessments.
        /// </summary>
        /// <value>The available post assessments.</value>
        protected List<Activity> AvailablePostAssessments { get; } = new List<Activity>();

        /// <summary>
        /// Gets the runner.
        /// </summary>
        /// <value>The runner.</value>
        protected ActivityRunner Runner { get; private set; }

        /// <summary>
        /// Gets or sets the managed html activity runner page.
        /// </summary>
        /// <value>The managed html activity runner page.</value>
        protected BaseContentPage ManagedHtmlActivityRunnerPage { get; set; }

        /// <inheritdoc />
        public override void Start()
        {
            base.Start();

            startLocation = metaDataProvider.GetMetaData(Activity)?.Result?.StartLocation;

            var startPath = new Uri(startLocation).PathAndQuery.Substring(1);

            if (string.IsNullOrEmpty(FileStorage.PackagedContentDirectory))
            {
                var e = new MissingMemberException(nameof(FileStorage), nameof(FileStorage.PackagedContentDirectory));
                OnActivityLaunchException(e);
                Finish(EventArgs.Empty);
                return;
            }

            var fileLocation = Path.Combine(FileStorage.PackagedContentDirectory, startPath);

            if (DownloadChecker.IsActivityDownloaded(fileLocation))
            {
                CreateRunnerAndHandleErrors();
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        downloadStatus = ActivityDownloader.Download(Activity, remoteFileProvider, metaDataProvider, remoteFileProcessor);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                    {
                        OnActivityLaunchException(e);
                        Finish(EventArgs.Empty);
                        return;
                    }

                    downloadStatus.DownloadsCompleted += HandleDownloadCompleted;
                    downloadStatus.DownloadsCancelled += HandleDownloadCancelled;

                    NavigationContext.PresentPage(CreateDownloadStatusPage(downloadStatus));
                });
            }
        }

        /// <summary>
        /// Creates the download status page.
        /// </summary>
        /// <returns>The download status page.</returns>
        /// <param name="downloadStatus">Download status.</param>
        protected abstract BaseContentPage CreateDownloadStatusPage(DownloadStatus downloadStatus);

        /// <summary>
        /// Creates the activity complete page.
        /// </summary>
        /// <returns>The activity complete page.</returns>
        /// <param name="hasPostAssessment">If set to <c>true</c> has post assessment.</param>
        protected abstract BaseContentPage CreateActivityCompletePage(bool hasPostAssessment);

        /// <summary>
        /// Awards the points for assessment.
        /// </summary>
        /// <param name="statement">The Statement.</param>
        protected abstract void AwardPointsForAssessment(Statement statement);

        /// <summary>
        /// Sets the activity page base location.
        /// </summary>
        /// <param name="uri">The URI.</param>
        protected abstract void SetActivityPageBaseLocation(Uri uri);

        /// <inheritdoc />
        protected override void Finish(EventArgs args)
        {
            {
                return;
            }

            base.Finish(args);
            try
            {
                downloadStatus?.CancelDownload();
                ManagedHtmlActivityRunnerPage?.Navigation.RemovePage(ManagedHtmlActivityRunnerPage);
            }
            catch
            {
                throw;
            }
            finally
            {
                startLocation = null;

                Runner?.Dispose();
                Runner = null;
            }
        }

        /// <inheritdoc />
        protected override Page PresentInitialPage()
        {
            return ManagedHtmlActivityRunnerPage;
        }

        /// <summary>
        /// Invoked when the activity has finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">Completion arguments.</param>
        protected virtual void OnActivityFinished(object sender, EventArgs args)
        {
            ShowCompletionScreen();
        }

        /// <summary>
        /// Invoked when the activity could not be launched due to an exception.
        /// </summary>
        /// <param name="exception">The exception that prevented the activity from launching.</param>
        protected abstract void OnActivityLaunchException(Exception exception);

        /// <summary>
        /// Gets the current actor.
        /// </summary>
        /// <returns>The current actor.</returns>
        protected abstract Agent GetCurrentActor();

        /// <summary>
        /// Shows the completion screen.
        /// </summary>
        protected void ShowCompletionScreen()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var completionPage = CreateActivityCompletePage(AvailablePostAssessments != null && AvailablePostAssessments.Any());
                NavigationContext.PresentPage(completionPage);
            });
        }

        /// <summary>
        /// Occurs when a user has requested the download be canceled.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleCancelDownloadRequested(object sender, EventArgs args)
        {
            if (downloadStatus == null || downloadStatus.State != DownloadStatus.DownloadState.Downloading)
            {
                return;
            }

            downloadStatus.CancelDownload();
            Finish(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the activity finishes.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleActivityFinished(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.Activity.CompletionDate = DateTimeOffset.Now;
            });

            if (args is not StatementEventArgs statementArgs)
            {
                return;
            }

            var otherContextActivities = statementArgs.Statement?.context?.contextActivities?.other;

            if (otherContextActivities != null)
            {
                var postAssessments = otherContextActivities.Where(activity => activity.definition?.type?.OriginalString == "http://adlnet.gov/expapi/activities/assessment");
                AvailablePostAssessments.AddRange(postAssessments);
            }

            OnActivityFinished(sender, args);
        }

        /// <summary>
        /// Occurs when the user progresses within the activity.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleActivityProgressed(object sender, EventArgs args)
        {
            if (args is not StatementEventArgs statementArgs)
            {
                return;
            }

            if (statementArgs.Statement.result == null || statementArgs.Statement.result.extensions == null)
            {
                return;
            }

            var obj = JsonConvert.DeserializeObject<JObject>(statementArgs.Statement.result.extensions.ToJSON());
            var percentComplete = obj.Property("https://w3id.org/xapi/cmi5/result/extensions/progress", StringComparison.OrdinalIgnoreCase);

            if (percentComplete == null)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                this.Activity.PercentComplete = (double)percentComplete / 100;
            });
        }

        /// <summary>
        /// Occurs when the user passes the assessment.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleAssessmentPassed(object sender, EventArgs args)
        {
            if (args is StatementEventArgs statementArgs)
            {
                AwardPointsForAssessment(statementArgs.Statement);
            }
        }

        /// <summary>
        /// Occurs when the user fails the assessment.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleAssessmentFailed(object sender, EventArgs args)
        {
            if (args is StatementEventArgs statementArgs)
            {
                AwardPointsForAssessment(statementArgs.Statement);
            }
        }

        /// <summary>
        /// Occurs when the download completes.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual async void HandleDownloadCompleted(object sender, EventArgs args)
        {
            if (downloadStatus != null && downloadStatus.State != DownloadStatus.DownloadState.Error)
            {
                downloadStatus.DownloadsCompleted -= HandleDownloadCompleted;
                downloadStatus.DownloadsCancelled -= HandleDownloadCancelled;

                startLocation = Activity.MetaData.StartLocation;
                isHidingDownloadPage = true;

                await NavigationContext.DismissPage();

                CreateRunnerAndHandleErrors();

                downloadStatus = null;
                isHidingDownloadPage = false;
            }
            else
            {
                HandleDownloadError(sender, args);
            }
        }

        /// <summary>
        /// Occurs when the user cancels the download.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleDownloadCancelled(object sender, EventArgs args)
        {
            if (downloadStatus != null)
            {
                downloadStatus.DownloadsCompleted -= HandleDownloadCompleted;
                downloadStatus.DownloadsCancelled -= HandleDownloadCancelled;
            }

            NavigationContext.DismissPage();
            downloadStatus = null;
        }

        /// <summary>
        /// Occurs when the download encounters an error.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleDownloadError(object sender, EventArgs args)
        {
            if (downloadStatus != null && downloadStatus.Exception != null)
            {
                OnActivityLaunchException(downloadStatus.Exception);
            }

            HandleDownloadCancelled(sender, args);
            Finish(EventArgs.Empty);
        }

        void CreateRunnerAndHandleErrors()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    CreateRunner();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    OnActivityLaunchException(e);
                    Finish(EventArgs.Empty);
                }
            });
        }

        /// <summary>
        /// Creates the runner. Note that this method expects to be called on the main thread.
        /// </summary>
        void CreateRunner()
        {
            // because this method is always invoked on the main thread, it's possible this coordinator was finished on a different thread first
            // this can cause all sorts of problems because we create a runner and never dispose it
            if (IsFinished)
            {
                return;
            }

            // we aggressively null check in this method due to some crashes seen in production
            var actor = GetCurrentActor();

            if (actor == null)
            {
                throw new NullReferenceException("Actor is required to create a runner");
            }

            Runner = new TinCanActivityRunner(Activity, lrs, actor, serverDelegate, lrsServerInfo);
            Runner.ActivityFinished += HandleActivityFinished;
            Runner.ActivityProgressed += HandleActivityProgressed;
            Runner.AssessmentFailed += HandleAssessmentFailed;
            Runner.AssessmentPassed += HandleAssessmentPassed;
            Runner.Run();

            startLocation = metaDataProvider.GetMetaData(Activity)?.Result?.StartLocation;

            if (string.IsNullOrWhiteSpace(startLocation))
            {
                throw new NullReferenceException($"No start location for activity {Activity.Name}");
            }

            Uri launchUri;

            if (startLocation.StartsWith("/", StringComparison.Ordinal))
            {
                // While `startLocation` is expected to be an absolute file path, it may already have a query or fragment string.
                // To ensure we handle it correctly, we assume a "http://" scheme if there is not already one provided.
                launchUri = new Uri(localServerBaseUri, startLocation);
            }
            else
            {
                launchUri = new Uri(startLocation);
            }

            launchUri = Runner.AddRunnerParameters(launchUri);
            SetActivityPageBaseLocation(launchUri);

            if (ManagedHtmlActivityRunnerPage == null)
            {
                throw new InvalidOperationException($"Invalid managed HTML page for activity {Activity.Name}");
            }

            if (NavigationContext == null)
            {
                throw new InvalidOperationException($"No navigation context for activity {Activity.Name}");
            }

            NavigationContext.PushPage(ManagedHtmlActivityRunnerPage);
        }
    }
}
