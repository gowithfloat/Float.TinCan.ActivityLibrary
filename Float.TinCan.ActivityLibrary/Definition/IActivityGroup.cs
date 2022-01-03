using System.Collections.Generic;
using System.ComponentModel;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Activity group.
    /// </summary>
    public interface IActivityGroup : ITinCanActivity, INotifyPropertyChanged, ISearchableContent, ICertifiable
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
        /// Gets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        string Keywords { get; }

        /// <summary>
        /// Gets the weight (relative order) of this group.
        /// </summary>
        /// <value>The weight.</value>
        int Weight { get; }

        /// <summary>
        /// Gets the thumbnail.
        /// </summary>
        /// <value>The thumbnail.</value>
        IFile Thumbnail { get; }

        /// <summary>
        /// Gets the audiences.
        /// </summary>
        /// <value>The audiences.</value>
        IEnumerable<IAudience> Audiences { get; }

        /// <summary>
        /// Gets the activities.
        /// </summary>
        /// <value>The activities.</value>
        IEnumerable<IActivity> Activities { get; }

        /// <summary>
        /// Gets the package.
        /// </summary>
        /// <value>The package.</value>
        IPackage Package { get; }

        /// <summary>
        /// Gets the parent activity group.
        /// </summary>
        /// <value>The parent activity group.</value>
        IActivityGroup ParentActivityGroup { get; }

        /// <summary>
        /// Gets the child activity groups.
        /// </summary>
        /// <value>The child activity groups.</value>
        IEnumerable<IActivityGroup> ChildActivityGroups { get; }

        /// <summary>
        /// Gets the links. These are deep links to specific sections of Activities.
        /// </summary>
        /// <value>The links.</value>
        IEnumerable<ILink> Links { get; }

        /// <summary>
        /// Gets the number new activities.
        /// </summary>
        /// <value>The number new activities.</value>
        int NumNewActivities { get; }
    }
}
