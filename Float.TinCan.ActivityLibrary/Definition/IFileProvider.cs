using System.Net.Http;
using System.Threading.Tasks;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Provider used to access files such as thumbnails and adapt courses.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Builds the request to download a file.
        /// </summary>
        /// <returns>The request to download file.</returns>
        /// <param name="file">The File to download.</param>
        Task<HttpRequestMessage> BuildRequestToDownloadFile(IFile file);
    }
}
