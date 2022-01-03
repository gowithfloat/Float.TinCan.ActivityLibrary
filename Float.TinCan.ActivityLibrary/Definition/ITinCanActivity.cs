using System;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Defines a TinCan activity, which must have a unique identifier.
    /// </summary>
    public interface ITinCanActivity
    {
        /// <summary>
        /// Gets the TinCan activity identifier.
        /// </summary>
        /// <value>The TinCan activity identifier.</value>
        Uri TinCanActivityId { get; }

        /// <summary>
        /// Gets the type of the tin can activity.
        /// </summary>
        /// <value>The type of the tin can activity.</value>
        Uri TinCanActivityType { get; }
    }
}
