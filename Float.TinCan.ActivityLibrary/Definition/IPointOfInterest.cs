namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// An interface for a point of interest.
    /// </summary>
    public interface IPointOfInterest : ISearchableContent
    {
        /// <summary>
        /// Gets the uuid.
        /// </summary>
        /// <value>The uuid.</value>
        string UUID { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the formatted name.
        /// </summary>
        /// <value>The formatted name.</value>
        string FormattedName { get; }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>The destination.</value>
        string Destination { get; }

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        string Keywords { get; }

        /// <summary>
        /// Gets the formatted keywords.
        /// </summary>
        /// <value>The formatted keywords.</value>
        string FormattedKeywords { get; }

        /// <summary>
        /// Gets the ID of the activity that contains the point of interest.
        /// </summary>
        /// <value>The activity ID.</value>
        string ActivityId { get; }

        /// <summary>
        /// Gets the name of the activity that contains the point of interest.
        /// </summary>
        /// <value>The activity name.</value>
        string ActivityName { get; }
    }
}
