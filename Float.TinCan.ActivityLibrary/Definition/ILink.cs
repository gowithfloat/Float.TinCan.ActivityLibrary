using System;
using System.ComponentModel;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// A deep link to a piece of content inside an activity.
    /// </summary>
    public interface ILink : ISearchableContent, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the UUID.
        /// </summary>
        /// <value>The UUID.</value>
        string UUID { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <value>The activity.</value>
        IActivity Activity { get; }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>The URI.</value>
        Uri Uri { get; }

        /// <summary>
        /// Gets the activity group.
        /// </summary>
        /// <value>The activity group.</value>
        IActivityGroup ActivityGroup { get; }

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <value>The section.</value>
        ISection Section { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the type of link.  This should be a classification of the content of the link.
        /// </summary>
        /// <value>The type.</value>
        string Type { get; }
    }
}
