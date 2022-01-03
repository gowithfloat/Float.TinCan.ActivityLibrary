using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Float.FileDownloader;
using Float.TinCan.ActivityLibrary.Definition;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Tin can lesson file processor.
    /// </summary>
    public class TinCanLessonFileProcessor : IRemoteFileProcessor
    {
        readonly IActivityMetaDataProvider metaProvider;
        readonly IActivity activity;

        /// <summary>
        /// Initializes a new instance of the <see cref="TinCanLessonFileProcessor"/> class.
        /// </summary>
        /// <param name="metaProvider">Meta provider.</param>
        /// <param name="activity">The Activity.</param>
        public TinCanLessonFileProcessor(IActivityMetaDataProvider metaProvider, IActivity activity)
        {
            Contract.Requires(metaProvider != null);
            Contract.Requires(activity != null);

            this.metaProvider = metaProvider;
            this.activity = activity;
        }

        /// <summary>
        /// Processes the download.
        /// </summary>
        /// <returns>The download.</returns>
        /// <param name="remoteFile">Remote file.</param>
        /// <param name="downloadPath">Download path.</param>
        /// <param name="response">The Response.</param>
        public async Task ProcessDownload(IRemoteFile remoteFile, string downloadPath, HttpResponseMessage response)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(downloadPath));

            if (Path.GetExtension(downloadPath).ToUpperInvariant() == ".ZIP")
            {
                var directory = await FileUnzipper.UnzipFile(downloadPath).ConfigureAwait(false);
                var meta = ActivityMetaDataGenerator.CreateMetaData(new Uri(directory), null);
                await metaProvider.SaveMetaData(activity, meta).ConfigureAwait(false);
            }
        }
    }
}
