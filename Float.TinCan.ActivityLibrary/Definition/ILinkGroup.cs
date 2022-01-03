using System.Collections.Generic;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// A link group.
    /// </summary>
    public interface ILinkGroup
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
        /// Gets the audiences.
        /// </summary>
        /// <value>The audiences.</value>
        IEnumerable<IAudience> Audiences { get; }

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>The links.</value>
        IEnumerable<ILink> Links { get; }

        /// <summary>
        /// Gets the package.
        /// </summary>
        /// <value>The package.</value>
        IPackage Package { get; }
    }
}
