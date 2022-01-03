using System;
using System.IO;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Checks the download state of activities.
    /// </summary>
    public static class DownloadChecker
    {
        /// <summary>
        /// Determines if the activity is downloaded by checking if there is a file at the given path.
        /// </summary>
        /// <returns><c>true</c>, if the activity is downloaded, <c>false</c> otherwise.</returns>
        /// <param name="activityAbsolutePath">The absolute path to an Activity.</param>
        public static bool IsActivityDownloaded(string activityAbsolutePath)
        {
            if (string.IsNullOrWhiteSpace(activityAbsolutePath))
            {
                return false;
            }

            // `activityAbsolutePath` may already have query parameters or a fragment which should
            // not be included when checking for file existence.
            Uri fileUri;
            if (activityAbsolutePath.StartsWith("/", StringComparison.Ordinal))
            {
                fileUri = new Uri($"file://{activityAbsolutePath}");
            }
            else
            {
                fileUri = new Uri(activityAbsolutePath);
            }

            return File.Exists(fileUri.LocalPath);
        }
    }
}
