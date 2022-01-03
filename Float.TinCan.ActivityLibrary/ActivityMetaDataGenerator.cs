using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using Float.TinCan.ActivityLibrary.Definition;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Activity meta data generator.
    /// </summary>
    public static class ActivityMetaDataGenerator
    {
        /// <summary>
        /// Creates the meta data.
        /// </summary>
        /// <returns>The meta data.</returns>
        /// <param name="activityFolder">Activity folder.</param>
        /// <param name="baseUri">The base URI from which to launch the activity.</param>
        public static ActivityMetaDataStruct CreateMetaData(Uri activityFolder, Uri baseUri)
        {
            Contract.Requires(activityFolder != null);

            try
            {
                return ParseMetaData(activityFolder, baseUri);
            }
            catch (XmlException e)
            {
                var ex = new InvalidOperationException("Something appears to be wrong with this activity's metadata", e);
                throw ex;
            }
        }

        /// <summary>
        /// Parses the meta data.
        /// </summary>
        /// <returns>The meta data.</returns>
        /// <param name="activityFolder">Activity folder.</param>
        /// <param name="baseUri">The base URI from which to launch the activity.</param>
        static ActivityMetaDataStruct ParseMetaData(Uri activityFolder, Uri baseUri)
        {
            var activityUrl = activityFolder.OriginalString;
            var path = Path.Combine(activityUrl, "tincan.xml");
            var metaStruct = new ActivityMetaDataStruct();
            string baseUrl;
            if (baseUri != null)
            {
                string lastSegment = activityFolder.Segments.Last();
                baseUrl = Path.Combine(baseUri?.OriginalString, lastSegment);
            }
            else
            {
                baseUrl = activityUrl;
            }

            if (!File.Exists(path))
            {
                metaStruct.StartLocation = Path.Combine(baseUrl, "index.html");
                return metaStruct;
            }

            using var reader = XmlReader.Create(File.OpenRead(path));
            reader.ReadToFollowing("launch");
            reader.Read();

            var launch = reader.Value;
            metaStruct.StartLocation = Path.Combine(baseUrl, launch ?? "index.html");

            reader.ReadToFollowing("name");
            reader.Read();

            metaStruct.Title = reader.Value;

            return metaStruct;
        }
    }
}
