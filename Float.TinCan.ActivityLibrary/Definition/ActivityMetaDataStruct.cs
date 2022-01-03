using System;
using System.Collections.Generic;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Activity meta data struct.
    /// </summary>
    public struct ActivityMetaDataStruct : IEquatable<ActivityMetaDataStruct>, IActivityMetaData
    {
        /// <inheritdoc />
        public string StartLocation { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public string UUID { get; set; }

        /// <inheritdoc />
        public DateTimeOffset LastModificationTime { get; set; }

        /// <summary>
        /// Compares two <see cref="ActivityMetaDataStruct"/> objects using value equality.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if the two values are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(ActivityMetaDataStruct left, ActivityMetaDataStruct right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="ActivityMetaDataStruct"/> objects using value inequality.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if the two values are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(ActivityMetaDataStruct left, ActivityMetaDataStruct right) => !(left == right);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is ActivityMetaDataStruct meta))
            {
                return false;
            }

            return Equals(meta);
        }

        /// <inheritdoc />
        public bool Equals(ActivityMetaDataStruct other)
        {
            return StartLocation == other.StartLocation && Title == other.Title && UUID == other.UUID;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = -709967909;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(StartLocation);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(UUID);
            return hashCode;
        }
    }
}
