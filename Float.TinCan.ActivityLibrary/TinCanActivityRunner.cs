using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Float.Core.Extensions;
using Float.TinCan.ActivityLibrary.Definition;
using Float.TinCan.LocalLRSServer;
using TinCan;
using TinCan.Documents;
using TinCan.Json;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Tin can activity runner.
    /// </summary>
    public class TinCanActivityRunner : ActivityRunner
    {
        readonly Uri contextActivityId;
        readonly Uri contextActivityType;
        readonly string contextActivityName;
        readonly Uri activityId;
        readonly LRSServer lrsServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TinCanActivityRunner"/> class.
        /// </summary>
        /// <param name="activity">The Activity.</param>
        /// <param name="lrs">The LRS to which to persist xAPI statements.</param>
        /// <param name="actor">The current actor.</param>
        /// <param name="serverDelegate">The server delegate.</param>
        /// <param name="lrsServerInfo">Contains server information for the LRS.</param>
        public TinCanActivityRunner(IActivity activity, ILRS lrs, Agent actor, ILRSServerDelegate serverDelegate, LRSServerInfo lrsServerInfo = null) : base(activity, actor, lrs, lrsServerInfo)
        {
            Contract.Requires(activity != null);
            Contract.Requires(lrs != null);
            Contract.Requires(actor != null);
            Contract.Requires(serverDelegate != null);
            lrsServer = new LRSServer(LrsServerInfo.Address, LrsServerInfo.Port, serverDelegate)
            {
                StateGetRequest = HandleStateGetRequest,
                StatePutRequest = HandleStatePutRequest,
                StatePostRequest = HandleStatePutRequest,
                StateDeleteRequest = HandleStateDeleteRequest,
            };

            lrsServer.StatementReceived += HandleStatementEvent;
            lrsServer.AgentProfileDocumentReceived += HandleAgentProfileRequest;
            contextActivityId = activity.ActivityGroup?.TinCanActivityId;
            contextActivityType = activity.ActivityGroup?.TinCanActivityType;
            contextActivityName = activity.ActivityGroup?.Name;
            activityId = activity.TinCanActivityId;
        }

        /// <inheritdoc />
        public override event EventHandler ActivityFinished;

        /// <inheritdoc />
        public override event EventHandler ActivityProgressed;

        /// <inheritdoc />
        public override event EventHandler AssessmentPassed;

        /// <inheritdoc />
        public override event EventHandler AssessmentFailed;

        /// <inheritdoc />
        public override void Run()
        {
            base.Run();
            lrsServer.Start();
        }

        /// <inheritdoc />
        public override void Stop()
        {
            base.Stop();
            lrsServer.Stop();
        }

        /// <inheritdoc />
        public override Uri AddRunnerParameters(Uri launchUri)
        {
            if (launchUri == null)
            {
                throw new ArgumentNullException(nameof(launchUri));
            }

            if (!launchUri.IsAbsoluteUri)
            {
                throw new ArgumentException($"{launchUri.OriginalString} is not an absolute URI", nameof(launchUri));
            }

            var uriBuilder = new UriBuilder(launchUri);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            // The query parameters here are based on what is supported by ADLs xAPI wrapper:
            // https://github.com/adlnet/xAPIWrapper/#launch-parameters
            // And Rustici's (old) TinCan.js:
            // https://github.com/RusticiSoftware/TinCanJS/blob/master/src/TinCan.js#L195
            query["endpoint"] = lrsServer.Url.ToString();
            query["auth"] = "none";
            query["actor"] = Agent.ToJSON();
            query["activity_id"] = Activity.TinCanActivityId.ToString();
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            lrsServer.Close();
        }

        /// <inheritdoc />
        protected override void OnSuspended()
        {
            if (!Completed)
            {
                base.OnSuspended();
            }
        }

        /// <inheritdoc />
        protected override void OnCompleted()
        {
            // We are assuming the content will provide it's own completed statement.
            Completed = true;
        }

        /// <summary>
        /// Extracts the parameter from query.
        /// </summary>
        /// <returns>The value associated with the provided key in the http request.</returns>
        /// <param name="key">The name of the value you wish to extract.</param>
        /// <param name="request">The Http request containing the information needed.</param>
        static string ExtractParamFromQuery(string key, HttpListenerRequest request)
        {
            var queryValue = request.QueryString[key];
            return string.IsNullOrEmpty(queryValue) ? string.Empty : WebUtility.UrlDecode(queryValue);
        }

        /// <summary>
        /// Get a state document containing all request parameters needed.
        /// </summary>
        /// <returns>The request parameters.</returns>
        /// <param name="request">The Http request containing the information needed.</param>
        static StateDocument GetRequestParameters(HttpListenerRequest request)
        {
            var state = new StateDocument();
            var agent = ExtractParamFromQuery("agent", request);
            state.id = ExtractParamFromQuery("stateId", request);
            state.agent = new Agent(new StringOfJSON(agent));

            var id = ExtractParamFromQuery("activityId", request);

            if (Uri.TryCreate(id, UriKind.RelativeOrAbsolute, out var uri))
            {
                var activity = new Activity
                {
                    id = uri,
                };

                state.activity = activity;
            }

            var registrationString = ExtractParamFromQuery("registration", request);

            if (!string.IsNullOrEmpty(registrationString))
            {
                state.registration = new System.Guid(registrationString);
            }

            return state;
        }

        static void UpdateResponse(HttpListenerResponse response, string contentType = null, byte[] outputStreamContent = null)
        {
            try
            {
                if (contentType != null)
                {
                    response.ContentType = contentType;
                }

                if (outputStreamContent != null)
                {
                    response.ContentLength64 = outputStreamContent.Length;
                    response.OutputStream.Write(outputStreamContent, 0, outputStreamContent.Length);
                }

                response.OutputStream.Close();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        static void UpdateResponse(HttpListenerResponse response, HttpStatusCode statusCode, string contentType = null, byte[] outputStreamContent = null)
        {
            try
            {
                response.StatusCode = (int)statusCode;
                UpdateResponse(response, contentType, outputStreamContent);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Invoked when an xAPI statement was sent from the content.
        /// </summary>
        /// <param name="sender">Reference to the LRS server.</param>
        /// <param name="args">The xAPI statement and additional context.</param>
        void HandleStatementEvent(object sender, StatementEventArgs args)
        {
            var statement = args.Statement;

            // Don't forward launched statements from the activity to the server.  We manually send launched statements for every activity.
            var curActivity = statement.target as Activity;
            if (curActivity != null)
            {
                if (statement.verb.id == Verb.Launched.id && curActivity.id == activityId)
                {
                    return;
                }
            }

            if (contextActivityId != null)
            {
                var context = statement.context ?? new Context();
                var contextActivities = context.contextActivities ?? new ContextActivities();

                // The ActivityGroup containing this activity should be considered the "grouping" context.
                // This allows us to analyze user behavior by sections ("group") of content.
                var groupingContext = contextActivities.grouping ?? new System.Collections.Generic.List<Activity>();
                groupingContext.Add(GetContextActivity());
                contextActivities.grouping = groupingContext;

                context.contextActivities = contextActivities;
                statement.context = context;
            }

            Lrs.SaveStatement(statement);

            if (args.Statement.verb.id == Verb.Completed.id)
            {
                if (curActivity != null && curActivity.id == activityId)
                {
                    OnCompleted();
                    ActivityFinished?.Invoke(this, new StatementEventArgs(statement));
                }
            }
            else if (args.Statement.verb.id.OriginalString == "http://adlnet.gov/expapi/verbs/progressed")
            {
                ActivityProgressed?.Invoke(this, new StatementEventArgs(statement));
            }
            else if (args.Statement.verb.id.OriginalString == "http://adlnet.gov/expapi/verbs/passed")
            {
                AssessmentPassed?.Invoke(this, new StatementEventArgs(statement));
            }
            else if (args.Statement.verb.id.OriginalString == "http://adlnet.gov/expapi/verbs/failed")
            {
                AssessmentFailed?.Invoke(this, new StatementEventArgs(statement));
            }
        }

        /// <summary>
        /// Invoked when an Agent Profile POST/PUT Request was sent from the content.
        /// </summary>
        /// <param name="sender">Reference to the LRS server.</param>
        /// <param name="args">The agent profile document and additional context.</param>
        void HandleAgentProfileRequest(object sender, AgentProfileDocumentEventArgs args)
        {
            var profile = args.AgentProfileDocument;
            Lrs.SaveAgentProfile(profile);
        }

        /// <summary>
        /// Gets a contextual activity for the current activity (likely a "grouping" context).
        /// </summary>
        /// <returns>The context activity.</returns>
        Activity GetContextActivity()
        {
            return new Activity
            {
                id = contextActivityId,
                definition = new ActivityDefinition
                {
                    type = contextActivityType,
                    name = new LanguageMap
                    {
                        { CultureInfo.CurrentCulture.Name, contextActivityName },
                    },
                },
            };
        }

        /// <summary>
        /// Handles requests that are specifically related to xAPI Get State Resources.
        /// </summary>
        /// <param name="request">The HTTP request to get payload data from.</param>
        /// <param name="response">The HTTP response to write the response to.</param>
        void HandleStateGetRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var state = GetRequestParameters(request);
            if (string.IsNullOrEmpty(state.id))
            {
                Lrs.RetrieveStateIds(state.activity, state.agent, state.registration).OnSuccess(task =>
                {
                    if (task.Result.success)
                    {
                        if (task.Result.content != null)
                        {
                            var list = task.Result.content.ToString();
                            var listBytes = Encoding.UTF8.GetBytes(list);
                            UpdateResponse(response, HttpStatusCode.OK, "application/json", listBytes);
                        }
                        else
                        {
                            UpdateResponse(response, HttpStatusCode.NotFound);
                        }
                    }
                    else
                    {
                        UpdateResponse(response, HttpStatusCode.NotFound);
                    }
                }).OnFailure(task =>
                {
                    UpdateResponse(response, HttpStatusCode.BadGateway);
                });
            }
            else
            {
                Lrs.RetrieveState(state.id, state.activity, state.agent, state.registration).OnSuccess(task =>
                {
                    if (task.Result.success)
                    {
                        state = task.Result.content;
                        if (state.content != null)
                        {
                            UpdateResponse(response, HttpStatusCode.OK, state.contentType, state.content);
                        }
                        else
                        {
                            UpdateResponse(response, HttpStatusCode.NotFound);
                        }
                    }
                    else
                    {
                        UpdateResponse(response, HttpStatusCode.NotFound);
                    }
                }).OnFailure(task =>
                {
                    UpdateResponse(response, HttpStatusCode.BadGateway);
                });
            }
        }

        /// <summary>
        /// Handles requests that are specifically related to xAPI Put State Resources.
        /// </summary>
        /// <param name="request">The HTTP request to get payload data from.</param>
        /// <param name="response">The HTTP response to write the response to.</param>
        void HandleStatePutRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var state = GetRequestParameters(request);

            state.contentType = request.ContentType;

            using (var stream = new MemoryStream())
            {
                request.InputStream.CopyTo(stream);
                state.content = stream.ToArray();
            }

            Lrs.SaveState(state).OnSuccess(task =>
            {
                UpdateResponse(response, HttpStatusCode.NoContent);
            }).OnFailure(task =>
            {
                UpdateResponse(response, HttpStatusCode.BadRequest);
            });
        }

        /// <summary>
        /// Handles requests that are specifically related to xAPI Delete State Resources.
        /// </summary>
        /// <param name="request">The HTTP request to get payload data from.</param>
        /// <param name="response">The HTTP response to write the response to.</param>
        void HandleStateDeleteRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var state = GetRequestParameters(request);

            // If stateID is null this is a clear statement otherwise it is a Delete statement
            if (string.IsNullOrEmpty(state.id))
            {
                Lrs.ClearState(state.activity, state.agent, state.registration).OnSuccess(task =>
                {
                    UpdateResponse(response, HttpStatusCode.NoContent);
                }).OnFailure((task) =>
                {
                    UpdateResponse(response, HttpStatusCode.BadRequest);
                });
            }
            else
            {
                Lrs.DeleteState(state).OnSuccess(task =>
                {
                    UpdateResponse(response, HttpStatusCode.NoContent);
                }).OnFailure((task) =>
                {
                    UpdateResponse(response, HttpStatusCode.BadRequest);
                });
            }
        }
    }
}
