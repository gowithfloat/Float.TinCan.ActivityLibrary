using System;
using Float.FileDownloader;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// A File.
    /// </summary>
    public interface IFile : IRemoteFile
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
        /// Gets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        double FileSize { get; }

        /// <summary>
        /// Gets the last updated date.
        /// </summary>
        /// <value>The last updated date.</value>
        DateTimeOffset LastModificationTime { get; }

        /// <summary>
        /// Gets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        string MimeType { get; }
    }
}
