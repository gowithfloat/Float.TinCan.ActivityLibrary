using System.Net.Http;
using System.Threading.Tasks;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Activity meta data provider.
    /// </summary>
    public interface IActivityMetaDataProvider
    {
        /// <summary>
        /// Saves the meta data.
        /// </summary>
        /// <returns>The meta data.</returns>
        /// <param name="activity">The Activity.</param>
        /// <param name="metaData">Activity meta data.</param>
        Task<IActivityMetaData> SaveMetaData(IActivity activity, IActivityMetaData metaData);

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <returns>The meta data.</returns>
        /// <param name="activity">The Activity.</param>
        Task<IActivityMetaData> GetMetaData(IActivity activity);

        /// <summary>
        /// Removes the meta data.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="activity">The Activity.</param>
        Task RemoveMetaData(IActivity activity);

        /// <summary>
        /// Saves caching metadata for one of the files in the activity.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="activity">The activity owning the file.</param>
        /// <param name="file">The that was successfully downloaded.</param>
        /// <param name="response">The HTTP response received when the file was requested.</param>
        /// <remarks>
        /// An activity can be comprised of multiple files.
        /// This method is invoked after each file is successfully downloaded in the activity
        /// so that caching metadata can be saved.
        /// For example, the implementing application may choose to save the ETag from the response
        /// and include it in the download request to avoid re-downloading a file that has no changes.
        /// </remarks>
        Task SaveFileCachingMetadata(IActivity activity, IFile file, HttpResponseMessage response);
    }
}
