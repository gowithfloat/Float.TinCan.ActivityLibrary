using System;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Activity meta data.
    /// </summary>
    public interface IActivityMetaData
    {
        /// <summary>
        /// Gets the start location.
        /// </summary>
        /// <value>The start location.</value>
        string StartLocation { get; }

        /// <summary>
        /// Gets the UUID.
        /// </summary>
        /// <value>The UUID.</value>
        string UUID { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; }

        /// <summary>
        /// Gets the time of last modification.
        /// </summary>
        /// <value>The last modification time.</value>
        DateTimeOffset LastModificationTime { get; }
    }
}
