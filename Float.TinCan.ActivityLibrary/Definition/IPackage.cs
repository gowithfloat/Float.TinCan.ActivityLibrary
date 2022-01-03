using System.Collections.Generic;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Package. Top level of content.
    /// </summary>
    public interface IPackage
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
        /// Gets the activity groups.
        /// </summary>
        /// <value>The activity groups.</value>
        IEnumerable<IActivityGroup> ActivityGroups { get; }

        /// <summary>
        /// Gets the thumbnail.
        /// </summary>
        /// <value>The thumbnail.</value>
        IFile Thumbnail { get; }
    }
}
