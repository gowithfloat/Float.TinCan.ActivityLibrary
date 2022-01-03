namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Represents something that can be completed.
    /// </summary>
    public interface ICompletable
    {
        /// <summary>
        /// Gets or sets the percent complete.
        /// </summary>
        /// <value>The percent complete.</value>
        double PercentComplete { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ICompletable"/> is complete.
        /// </summary>
        /// <value><c>true</c> if is complete; otherwise, <c>false</c>.</value>
        bool IsComplete { get; }
    }
}
