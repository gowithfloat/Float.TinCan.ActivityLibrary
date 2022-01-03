using System;
using System.Diagnostics.Contracts;
using System.IO;
using Float.TinCan.ActivityLibrary.Definition;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// File storage.
    /// </summary>
    public static class FileStorage
    {
        /// <summary>
        /// Gets or sets the application data directory.
        /// </summary>
        /// <value>The application data directory.</value>
        public static string ApplicationDataDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory, which contains unzipped packaged content.
        /// </summary>
        /// <value>The packaged content directory.</value>
        public static string PackagedContentDirectory { get; set; }

        /// <summary>
        /// Gets the download destination.
        /// </summary>
        /// <returns>The download destination.</returns>
        /// <param name="file">The File.</param>
        public static Uri GetDownloadDestination(IFile file)
        {
            Contract.Requires(file != null);
            return new Uri(Path.Combine(ApplicationDataDirectory, file.Name));
        }

        /// <summary>
        /// Delete the specified file.
        /// </summary>
        /// <param name="file">The File.</param>
        public static void Delete(IFile file)
        {
            Contract.Requires(file != null);

            if (Exists(file))
            {
                File.Delete(GetDownloadDestination(file).LocalPath);
            }
        }

        /// <summary>
        /// Determines if the specified file exists on the device.
        /// </summary>
        /// <returns>True if the file exists.</returns>
        /// <param name="file">The File.</param>
        public static bool Exists(IFile file)
        {
            Contract.Requires(file != null);
            return File.Exists(GetDownloadDestination(file).LocalPath);
        }
    }
}
