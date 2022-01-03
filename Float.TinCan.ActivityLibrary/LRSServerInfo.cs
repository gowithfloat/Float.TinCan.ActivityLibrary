using System;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Creates object that contains Server Information.
    /// </summary>
    public class LRSServerInfo : IEquatable<LRSServerInfo>
    {
        const string DefaultAddress = "http://127.0.0.1";
        readonly string address;
        readonly ushort port;

        /// <summary>
        /// Initializes a new instance of the <see cref="LRSServerInfo"/> class.
        /// </summary>
        /// <param name="address">The address for the LRS server.</param>
        /// <param name="port">The port for the LRS server.</param>
        public LRSServerInfo(string address = null, ushort? port = null)
        {
            this.address = address ?? DefaultAddress;
            this.port = port ?? PortSelector.SelectForAddress(this.address).SelectedPort;
        }

        /// <summary>
        /// Gets the address for the LRS server.
        /// </summary>
        /// <value>
        /// The address of the LRS server.
        /// </value>
        public string Address => address;

        /// <summary>
        /// Gets the port for the LRS server.
        /// </summary>
        /// <value>
        /// The port of the LRS server.
        /// </value>
        public ushort Port => port;

        /// <summary>
        /// Compares to LRSServerInfos.
        /// </summary>
        /// <param name="left">The left hand LRSServerInfo.</param>
        /// <param name="right">The right hand LRSServerInfo.</param>
        /// <returns><c>True</c> if they're equal.</returns>
        public static bool operator ==(LRSServerInfo left, LRSServerInfo right)
        {
            return left == null || right == null ? left == null && right == null : left.Equals(right);
        }

        /// <summary>
        /// Compares to LRSServerInfos.
        /// </summary>
        /// <param name="left">The left hand LRSServerInfo.</param>
        /// <param name="right">The right hand LRSServerInfo.</param>
        /// <returns><c>True</c> if they're not equal.</returns>
        public static bool operator !=(LRSServerInfo left, LRSServerInfo right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public bool Equals(LRSServerInfo other) => address == other?.address && port == other?.port;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is LRSServerInfo info && Equals(info);

        /// <inheritdoc/>
        public override int GetHashCode() => (address, port).GetHashCode();
    }
}
