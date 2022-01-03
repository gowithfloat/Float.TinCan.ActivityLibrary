namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// A Section. Meta data that groups Links or Activities.
    /// </summary>
    public interface ISection
    {
        /// <summary>
        /// Gets the UUID.
        /// </summary>
        /// <value>The UUID.</value>
        string UUID { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }
}
