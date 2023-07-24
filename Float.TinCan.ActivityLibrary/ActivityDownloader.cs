using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Float.FileDownloader;
using Float.TinCan.ActivityLibrary.Definition;
#if NETSTANDARD
using Xamarin.Forms;
#endif

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Activity downloader.
    /// </summary>
    public static class ActivityDownloader
    {
        static readonly Dictionary<IActivity, DownloadStatus> ActiveDownloads = new ();

        /// <summary>
        /// Download the specified activity using the file and metadata providers.
        /// </summary>
        /// <returns>The download.</returns>
        /// <param name="activity">The Activity.</param>
        /// <param name="fileProvider">File provider.</param>
        /// <param name="metaDataProvider">Meta data provider.</param>
        /// <param name="remoteFileProcessor">Provide a custom remote file processor.</param>
        public static DownloadStatus Download(IActivity activity, IRemoteFileProvider fileProvider, IActivityMetaDataProvider metaDataProvider, IRemoteFileProcessor remoteFileProcessor)
        {
            Contract.Requires(activity != null);
            Contract.Requires(fileProvider != null);
            Contract.Requires(metaDataProvider != null);
            Contract.Requires(remoteFileProcessor != null);

            Contract.Requires(activity.Files.Any(), $"The activity has no files that can be downloaded ({activity.UUID})");

            if (ActiveDownloads.ContainsKey(activity))
            {
                return ActiveDownloads[activity];
            }

            var status = new DownloadStatus(activity.Name);

            ActiveDownloads[activity] = status;
            status.DownloadsCompleted += (sender, args) =>
            {
                ActiveDownloads.Remove(activity);
            };

            status.DownloadsCancelled += (sender, args) =>
            {
                ActiveDownloads.Remove(activity);

                // Delete all the downloads associated with the activity
                DeleteDownloadsForActivity(activity, metaDataProvider);
            };

            var downloadActivityFiles = activity.Files.Select(file => DownloadFile(file, status, fileProvider, metaDataProvider, activity, remoteFileProcessor));

            Task.WhenAll(downloadActivityFiles).ContinueWith(
                tasks =>
                {
                    tasks?.Exception?.Handle(exc =>
                    {
#if NETSTANDARD
                        Device.BeginInvokeOnMainThread(() =>
#else
                        MainThread.BeginInvokeOnMainThread(() =>
#endif
                        {
                            DeleteDownloadsForActivity(activity, metaDataProvider);
                            ActiveDownloads.Remove(activity);
                        });

                        return true;
                    });
                }, TaskScheduler.FromCurrentSynchronizationContext());

            return status;
        }

        /// <summary>
        /// Deletes the downloads for activity.
        /// </summary>
        /// <param name="activity">The Activity.</param>
        /// <param name="metaDataProvider">Meta data provider.</param>
        public static void DeleteDownloadsForActivity(IActivity activity, IActivityMetaDataProvider metaDataProvider)
        {
            Contract.Requires(activity != null);
            Contract.Requires(metaDataProvider != null);

            foreach (var file in activity.Files)
            {
                FileStorage.Delete(file);
            }

            metaDataProvider.RemoveMetaData(activity);
        }

        static async Task DownloadFile(
            IFile activityFile,
            DownloadStatus status,
            IRemoteFileProvider fileProvider,
            IActivityMetaDataProvider metaDataProvider,
            IActivity activity,
            IRemoteFileProcessor fileProcessor)
        {
            var response = await FileDownloadRequest.DownloadFile(
                fileProvider,
                activityFile,
                FileStorage.GetDownloadDestination(activityFile),
                status,
                fileProcessor).ConfigureAwait(false);

            await metaDataProvider.SaveFileCachingMetadata(activity, activityFile, response).ConfigureAwait(false);
        }
    }
}
