using System.Collections.Generic;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// An interface for the result of identifying available ports.
    /// </summary>
    public interface IPortSelectorResult
    {
        /// <summary>
        /// Gets the selected port which is available for use.
        /// </summary>
        /// <value>The recommended port.</value>
        ushort SelectedPort { get; }

        /// <summary>
        /// Gets the ports that were tested and were already in use.
        /// </summary>
        /// <value>Ports which are unavailable.</value>
        IEnumerable<ushort> RejectedPorts { get; }
    }
}
