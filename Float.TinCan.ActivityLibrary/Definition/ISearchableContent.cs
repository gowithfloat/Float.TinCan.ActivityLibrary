namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Search activity status.
    /// </summary>
    public enum ContentStatus
    {
        /// <summary>
        /// The new content status.
        /// </summary>
        NewContent,

        /// <summary>
        /// The complete status.
        /// </summary>
        Complete,

        /// <summary>
        /// The other status.
        /// </summary>
        Other,
    }

    /// <summary>
    /// The Searchable content.
    /// </summary>
    public interface ISearchableContent : IContent
    {
        /// <summary>
        /// Gets the status of this piece of content.
        /// </summary>
        /// <value>The status of this content.</value>
        ContentStatus ContentCompletableStatus { get; }
    }
}
