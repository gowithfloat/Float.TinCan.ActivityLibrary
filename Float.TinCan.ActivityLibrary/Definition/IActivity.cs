using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Tin Can activity.
    /// </summary>
    public interface IActivity : ITinCanActivity, INotifyPropertyChanged, ISearchableContent, ICompletable
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
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the thumbnail.
        /// </summary>
        /// <value>The thumbnail.</value>
        IFile Thumbnail { get; }

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <value>The section.</value>
        string Section { get; }

        /// <summary>
        /// Gets the type of the activity.
        /// </summary>
        /// <value>The type of the activity.</value>
        string ActivityType { get; }

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        string Keywords { get; }

        /// <summary>
        /// Gets the audiences.
        /// </summary>
        /// <value>The audiences.</value>
        IEnumerable<IAudience> Audiences { get; }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        IEnumerable<IFile> Files { get; }

        /// <summary>
        /// Gets the activity group.
        /// </summary>
        /// <value>The activity group.</value>
        IActivityGroup ActivityGroup { get; }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        IActivityMetaData MetaData { get; }

        /// <summary>
        /// Gets the last file modification time.
        /// </summary>
        /// <value>The last file modification time.</value>
        DateTimeOffset? LastFileModificationTime { get; }

        /// <summary>
        /// Gets or sets the completion date.
        /// </summary>
        /// <value>The completion date.</value>
        DateTimeOffset? CompletionDate { get; set; }

        /// <summary>
        /// Gets the date the activity is new till.
        /// </summary>
        /// <value>The new till.</value>
        DateTimeOffset? NewUntilDate { get; }

        /// <summary>
        /// Gets the last updated.
        /// </summary>
        /// <value>The last updated.</value>
        DateTimeOffset? LastUpdatedDate { get; }

        /// <summary>
        /// Gets the points of interest.
        /// </summary>
        /// <value>The points of interest.</value>
        IEnumerable<IPointOfInterest> PointsOfInterest { get; }
    }
}
