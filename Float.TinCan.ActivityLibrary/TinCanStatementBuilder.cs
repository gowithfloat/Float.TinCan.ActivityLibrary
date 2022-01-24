using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using TinCan;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Tin can statement builder.
    /// </summary>
    public class TinCanStatementBuilder
    {
        const string DefaultLocale = "en-US";

        /// <summary>
        /// The agent for the statement to be built. Optional.
        /// </summary>
        Agent agent;

        /// <summary>
        /// The activity for the statement to be built. Optional.
        /// </summary>
        Activity activity;

        /// <summary>
        /// The context.
        /// </summary>
        Context context;

        /// <summary>
        /// The verb for the statement to be built. Optional.
        /// </summary>
        Verb verb;

        /// <summary>
        /// Sets the name of the agent for the generated statement.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="name">The agent name.</param>
        public TinCanStatementBuilder SetName(string name)
        {
            if (agent == null)
            {
                agent = new Agent();
            }

            agent.name = name;
            return this;
        }

        /// <summary>
        /// Sets the email of the agent for the generated statement.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="email">The agent email.</param>
        public TinCanStatementBuilder SetEmail(string email)
        {
            if (agent == null)
            {
                agent = new Agent();
            }

            agent.mbox = $"mailto:{email}";
            return this;
        }

        /// <summary>
        /// Sets the agent.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="agent">The agent to include in the built statement.</param>
        public TinCanStatementBuilder SetAgent(Agent agent)
        {
            this.agent = agent;

            return this;
        }

        /// <summary>
        /// Sets the activity identifier for the generated statement.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="activityId">The activity identifier.</param>
        public TinCanStatementBuilder SetActivityId(Uri activityId)
        {
            Contract.Requires(activityId != null);

            if (activity == null)
            {
                activity = new Activity
                {
                    id = activityId,
                };
            }
            else
            {
                activity = new Activity
                {
                    id = activityId,
                    definition = activity.definition,
                };
            }

            return this;
        }

        /// <summary>
        /// Sets the name of the activity.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="activityName">Activity name.</param>
        /// <param name="localeCode">Locale code.</param>
        public TinCanStatementBuilder SetActivityName(string activityName, string localeCode = DefaultLocale)
        {
            if (activity == null)
            {
                activity = new Activity();
            }

            activity.definition.name = new LanguageMap
            {
                { localeCode, activityName },
            };

            return this;
        }

        /// <summary>
        /// Sets the type of the activity.
        /// </summary>
        /// <returns>The activity type.</returns>
        /// <param name="activityType">Activity type.</param>
        public TinCanStatementBuilder SetActivityType(Uri activityType)
        {
            if (activity == null)
            {
                activity = new Activity();
            }

            activity.definition.type = activityType;

            return this;
        }

        /// <summary>
        /// Sets the name of the parent context.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="name">The parent context's name.</param>
        /// <param name="localeCode">The local code for the language map.</param>
        /// <param name="activityId">The activityId for the given parent.</param>
        /// <param name="activityType">The activity type.</param>
        public TinCanStatementBuilder SetParentContext(string name, string localeCode, Uri activityId, Uri activityType)
        {
            if (context == null)
            {
                context = new Context();
            }

            var activities = context.contextActivities ?? new ContextActivities();

            activities.parent = new List<Activity>
            {
                new Activity
                {
                    definition = new ActivityDefinition
                    {
                        name = new LanguageMap
                        {
                            { localeCode, name },
                        },
                        type = activityType,
                    },
                    id = activityId,
                },
            };

            context.contextActivities = activities;

            return this;
        }

        /// <summary>
        /// Sets group context.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="name">The group context's name.</param>
        /// <param name="activityId">The activityId for the given group.</param>
        /// <param name="activityType">The activity type.</param>
        public TinCanStatementBuilder SetGroupContext(string name, Uri activityId, Uri activityType)
        {
            if (string.IsNullOrWhiteSpace(name) || activityId == null || activityType == null)
            {
                return this;
            }

            if (context == null)
            {
                context = new Context();
            }

            var contextActivities = context.contextActivities ?? new ContextActivities();
            var groupingContext = contextActivities.grouping ?? new List<Activity>();
            groupingContext.Add(GetActivity(name, activityType, activityId));
            contextActivities.grouping = groupingContext;

            context.contextActivities = contextActivities;

            return this;
        }

        /// <summary>
        /// Adds a parent category.
        /// </summary>
        /// <returns>The parent category.</returns>
        /// <param name="activityName">Activity name.</param>
        /// <param name="localeCode">Locale code.</param>
        /// <param name="activityId">Activity identifier.</param>
        /// <param name="activityType">Activity type.</param>
        public TinCanStatementBuilder AddParentCategory(string activityName, string localeCode, Uri activityId, Uri activityType)
        {
            if (context == null)
            {
                context = new Context();
            }

            if (context.contextActivities.category == null)
            {
                context.contextActivities.category = new List<Activity>();
            }

            var newActivity = new Activity()
            {
                id = activityId,
                definition = new ActivityDefinition()
                {
                    name = new LanguageMap
                    {
                        { localeCode, activityName },
                    },
                    type = activityType,
                },
            };

            context.contextActivities.category.Add(newActivity);

            return this;
        }

        /// <summary>
        /// Sets the verb.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="verbString">Verb string.</param>
        /// <param name="verbName">Verb name.</param>
        /// <param name="localeCode">Locale code.</param>
        public TinCanStatementBuilder SetVerb(string verbString, string verbName, string localeCode = DefaultLocale)
        {
            verb = new Verb(new Uri(verbString))
            {
                display = new LanguageMap
                {
                    { localeCode, verbName },
                },
            };

            return this;
        }

        /// <summary>
        /// Sets the verb.
        /// </summary>
        /// <returns>This object, useful for chaining.</returns>
        /// <param name="verb">Verb to use.</param>
        public TinCanStatementBuilder SetVerb(Verb verb)
        {
            this.verb = verb;

            return this;
        }

        /// <summary>
        /// Build a statement using the parameters given so far.
        /// </summary>
        /// <returns>The built statement.</returns>
        public Statement Build()
        {
            var statement = new Statement();

            if (context == null)
            {
                context = new Context();
            }

            if (agent != null)
            {
                statement.actor = agent;
            }

            if (activity != null)
            {
                statement.target = activity;
            }

            statement.context = context;
            statement.verb = verb;
            statement.Stamp();

            return statement;
        }

        static Activity GetActivity(string contextActivityName, Uri contextActivityType, Uri contextActivityId)
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
    }
}
