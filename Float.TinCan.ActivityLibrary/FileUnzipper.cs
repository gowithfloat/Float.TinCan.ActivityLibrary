using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// File unzipper.
    /// </summary>
    public static class FileUnzipper
    {
        /// <summary>
        /// Unzips the file.
        /// </summary>
        /// <returns>The directory the file was unzipped to.</returns>
        /// <param name="filePath">File path.</param>
        /// <param name="destination">The directory in which the file is unzipped.
        ///                           If not set, the file will be extracted in the same directory as the zip file
        ///                           into a directory with the name of the file.
        /// </param>
        /// <param name="shouldDelete">If set to <c>true</c> should delete.</param>
        public static async Task<string> UnzipFile(string filePath, string destination = null, bool shouldDelete = true)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(filePath));

            var unzipDestination = destination != null ? destination
                : Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));

            CreateDirectory(unzipDestination, true);

            try
            {
                // Open a stream to the zip file
                using var reader = File.OpenRead(filePath);
                using var archive = new ZipArchive(reader, ZipArchiveMode.Read);

                // loop through every entry in the zip archive. an entry could be a directories or files
                foreach (var entry in archive.Entries)
                {
                    var entryFilePath = Path.Combine(unzipDestination, entry.FullName);
                    CreateDirectory(Path.GetDirectoryName(entryFilePath), false);

                    // If the write path has an extension assume its a file.  otherwise its a directory
                    if (Path.HasExtension(entryFilePath))
                    {
                        //// Open a stream to read the entry , and to write to the desired file
                        using var entryReader = entry.Open();
                        using var writer = File.Create(entryFilePath);
                        await entryReader.CopyToAsync(writer).ConfigureAwait(false);
                    }
                }
            }
            catch (InvalidDataException e)
            {
                // Delete the zip file if it can't be unzipped
                File.Delete(filePath);
                Directory.Delete(unzipDestination);
                throw new FileUnzipException("There could be a problem with your file", e);
            }
            catch (DirectoryNotFoundException e)
            {
                // Delete the zip file if it can't be unzipped
                File.Delete(filePath);
                Directory.Delete(unzipDestination);
                throw new FileUnzipException("There could be a problem with your file", e);
            }

            if (shouldDelete)
            {
                File.Delete(filePath);
            }

            return Path.GetFileNameWithoutExtension(filePath);
        }

        static void CreateDirectory(string path, bool overwriteIfExists)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(path));

            if (Directory.Exists(path))
            {
                if (overwriteIfExists)
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    return;
                }
            }

            Directory.CreateDirectory(path);
        }
    }
}
