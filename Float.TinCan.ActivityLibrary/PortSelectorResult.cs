using System;
using System.Collections.Generic;
using System.Linq;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Storage for the results of selecting an available port for use.
    /// </summary>
    public struct PortSelectorResult : IPortSelectorResult, IEquatable<PortSelectorResult>
    {
        readonly HashSet<ushort> rejectedPorts;

        internal PortSelectorResult(ushort selected = PortSelector.DefaultStartPort, IEnumerable<ushort> rejected = null)
        {
            this.SelectedPort = selected;
            this.rejectedPorts = new HashSet<ushort>(rejected ?? Enumerable.Empty<ushort>());
        }

        /// <inheritdoc/>
        public ushort SelectedPort { get; }

        /// <inheritdoc/>
        public IEnumerable<ushort> RejectedPorts => rejectedPorts;

        /// <summary>
        /// Checks two port selector results for value equality.
        /// </summary>
        /// <param name="left">The first result.</param>
        /// <param name="right">The second result.</param>
        /// <returns><c>true</c> if the results are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(PortSelectorResult left, PortSelectorResult right) => left.Equals(right);

        /// <summary>
        /// Checks two port selector results for value inequality.
        /// </summary>
        /// <param name="left">The first result.</param>
        /// <param name="right">The second result.</param>
        /// <returns><c>true</c> if the results are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(PortSelectorResult left, PortSelectorResult right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is PortSelectorResult other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(PortSelectorResult other) => other.GetHashCode() == GetHashCode();

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -123090624;
            hashCode = (hashCode * -1521134295) + SelectedPort.GetHashCode();

            var comparer = HashSet<ushort>.CreateSetComparer();
            hashCode = (hashCode * -1521134295) + comparer.GetHashCode(rejectedPorts);
            return hashCode;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{GetType()}<Selected: {SelectedPort}, Rejected: {string.Join(",", rejectedPorts.OrderBy(p => p))}>";
        }
    }
}
