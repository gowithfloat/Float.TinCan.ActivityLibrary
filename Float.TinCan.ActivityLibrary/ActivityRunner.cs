using System;
using System.Diagnostics.Contracts;
using Float.TinCan.ActivityLibrary.Definition;
using TinCan;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Base class for running activities, both static and xAPI compliant.
    /// </summary>
    /// <remarks>
    /// When a user is performing an activity within the app, a "runner"
    /// observes the user's progress through the activity while providing any
    /// background processes the activity is dependent on.
    /// </remarks>
    public abstract class ActivityRunner : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityRunner"/> class.
        /// </summary>
        /// <param name="activity">Activity to run.</param>
        /// <param name="agent">Agent using the activity.</param>
        /// <param name="lrs">The LRS to which to persist xAPI statements.</param>
        /// <param name="lrsServerInfo">Contains server information for the LRS.</param>
        protected ActivityRunner(IActivity activity, Agent agent, ILRS lrs, LRSServerInfo lrsServerInfo = null)
        {
            Contract.Requires(activity != null);
            Contract.Requires(agent != null);
            Contract.Requires(lrs != null);

            if (string.IsNullOrEmpty(agent.mbox) && string.IsNullOrEmpty(agent.openid) && agent.account == null)
            {
                throw new ArgumentException("The actor must have an inverse functional identifier");
            }

            Activity = activity;
            Agent = agent;
            Lrs = lrs;
            LrsServerInfo = lrsServerInfo ?? new LRSServerInfo(null, null);
        }

        /// <summary>
        /// Occurs when completion screen should be shown.
        /// </summary>
        public abstract event EventHandler ActivityFinished;

        /// <summary>
        /// Occurs when an activity has progressed.
        /// </summary>
        public abstract event EventHandler ActivityProgressed;

        /// <summary>
        /// Occurs when an assessment is passed.
        /// </summary>
        public abstract event EventHandler AssessmentPassed;

        /// <summary>
        /// Occurs when an assessment is failed.
        /// </summary>
        public abstract event EventHandler AssessmentFailed;

        /// <summary>
        /// Gets the activity to run.
        /// </summary>
        /// <value>The current activity.</value>
        protected IActivity Activity { get; }

        /// <summary>
        /// Gets the current agent.
        /// </summary>
        /// <value>The current agent.</value>
        protected Agent Agent { get; }

        /// <summary>
        /// Gets the LRS to which to send statements.
        /// </summary>
        /// <value>The current LRS.</value>
        protected ILRS Lrs { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this lesson has been completed.
        /// Implementing classes are responsible for maintaining the state of this property.
        /// </summary>
        /// <value><c>true</c> if completed, <c>false</c> otherwise.</value>
        protected bool Completed { get; set; }

        /// <summary>
        /// Gets the LRS server info such as address and port.
        /// </summary>
        /// <value>Contains address and port of LRS server.</value>
        protected LRSServerInfo LrsServerInfo { get; }

        /// <summary>
        /// Starts any processes required by this runner.
        /// </summary>
        public virtual void Run()
        {
            OnLaunched();
        }

        /// <summary>
        /// Temporarily stops processes being used by this runner.
        /// They can be started again by using <see cref="Run"/>.
        /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
        public virtual void Stop()
#pragma warning restore CA1716 // Identifiers should not match keywords
        {
            OnSuspended();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="ActivityRunner"/> object.
        /// </summary>
        /// <remarks>
        /// Call when you are finished using the <see cref="ActivityRunner"/>. This method leaves the
        /// runner in an unusable state. After calling dispose, you must release all references to the
        /// runner so the garbage collector can reclaim the memory that this was occupying.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds runner parameters to a given path.
        /// </summary>
        /// <returns>The given path as a URI with runner parameters added.</returns>
        /// <param name="path">The path.</param>
        /// <remarks>
        /// To avoid encoding issues with query or fragments, it's much safer to treat
        /// the incoming parameter as a URI. Thus, this method is now obsolete.
        /// </remarks>
        [Obsolete("Use AddRunnerParamaters(Uri) instead")]
        public virtual string AddRunnerParametersToPath(string path)
        {
            // we URI encode here to ensure that the web view can properly load the resource
            // at any point before this, we should treat activity locations as file paths
            return AddRunnerParameters(new Uri(path)).OriginalString;
        }

        /// <summary>
        /// Adds additional runner-specific parameters to the provided launch URI.
        /// </summary>
        /// <param name="launchUri">The launch URI for the content; must be absolute.</param>
        /// <returns>A launch URI with runner-specific parameters appended.</returns>
        public abstract Uri AddRunnerParameters(Uri launchUri);

        /// <summary>
        /// Releases all resource used by the <see cref="ActivityRunner"/> object.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this object is being disposed, <c>false</c> otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            Stop();
        }

        /// <summary>
        /// Called when this lesson is launched.
        /// </summary>
        protected virtual void OnLaunched()
        {
            SendLaunchedStatement();
        }

        /// <summary>
        /// Called when this lesson is suspended.
        /// </summary>
        protected virtual void OnSuspended()
        {
            SendSuspendedStatement();
        }

        /// <summary>
        /// Called when this lesson is completed.
        /// </summary>
        protected virtual void OnCompleted()
        {
            SendCompletedStatement();
        }

        /// <summary>
        /// Convenience function to send a launched statement to the LRS with the current activity ID and user info.
        /// </summary>
        protected void SendLaunchedStatement()
        {
            var statement = new TinCanStatementBuilder()
                .SetAgent(Agent)
                .SetVerb(Verb.Launched)
                .SetActivityId(Activity.TinCanActivityId)
                .SetActivityType(Activity.TinCanActivityType)
                .SetActivityName(Activity.Name)
                .SetGroupContext(Activity.ActivityGroup?.Name, Activity.ActivityGroup?.TinCanActivityId, Activity.ActivityGroup?.TinCanActivityType)
                .Build();

            Lrs.SaveStatement(statement);
        }

        /// <summary>
        /// Convenience function to send a suspended statement to the LRS with the current activity ID and user info.
        /// </summary>
        protected void SendSuspendedStatement()
        {
            // it's possible that the activity was removed from the database while viewed
            try
            {
                _ = Activity.TinCanActivityId;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return;
            }

            var statement = new TinCanStatementBuilder()
                .SetAgent(Agent)
                .SetVerb(Verb.Suspended)
                .SetActivityId(Activity.TinCanActivityId)
                .SetActivityName(Activity.Name)
                .SetActivityType(Activity.TinCanActivityType)
                .SetGroupContext(Activity.ActivityGroup?.Name, Activity.ActivityGroup?.TinCanActivityId, Activity.ActivityGroup?.TinCanActivityType)
                .Build();

            Lrs.SaveStatement(statement);
        }

        /// <summary>
        /// Convenience function to send a completed statement to the LRS with the current activity ID and user info.
        /// </summary>
        protected void SendCompletedStatement()
        {
            // it's possible that the activity was removed from the database while viewed
            try
            {
                _ = Activity.TinCanActivityId;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return;
            }

            var statement = new TinCanStatementBuilder()
                .SetAgent(Agent)
                .SetVerb(Verb.Completed)
                .SetActivityId(Activity.TinCanActivityId)
                .SetActivityName(Activity.Name)
                .SetActivityType(Activity.TinCanActivityType)
                .SetGroupContext(Activity.ActivityGroup?.Name, Activity.ActivityGroup?.TinCanActivityId, Activity.ActivityGroup?.TinCanActivityType)
                .Build();

            Lrs.SaveStatement(statement);
        }
    }
}
