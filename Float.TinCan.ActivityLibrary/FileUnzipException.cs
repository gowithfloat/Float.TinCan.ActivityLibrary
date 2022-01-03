using System;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// File unzip exception.
    /// </summary>
    public class FileUnzipException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileUnzipException"/> class.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="innerException">The inner exception, if any.</param>
        internal FileUnzipException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUnzipException"/> class.
        /// </summary>
        private FileUnzipException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUnzipException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        private FileUnzipException(string message) : base(message)
        {
        }
    }
}
