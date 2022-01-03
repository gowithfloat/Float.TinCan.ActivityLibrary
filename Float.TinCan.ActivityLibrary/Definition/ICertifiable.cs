namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// The interface for certifiable items.
    /// </summary>
    public interface ICertifiable : ICompletable
    {
        /// <summary>
        /// Gets a value indicating whether this activity group is certifiable.
        /// </summary>
        /// <value>If it is certifiable.</value>
        bool IsCertificateAvailable { get; }
    }
}
