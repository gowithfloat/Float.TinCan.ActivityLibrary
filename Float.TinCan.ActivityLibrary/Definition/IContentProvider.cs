using System.Collections.Generic;
using System.Threading.Tasks;

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Content provider.
    /// </summary>
    public interface IContentProvider
    {
        /// <summary>
        /// Retrieves the activity by UUID.
        /// </summary>
        /// <returns>The activity.</returns>
        /// <param name="activityUUID">Activity UUID.</param>
        IActivity RetrieveActivity(string activityUUID);

        /// <summary>
        /// Retrieves the packages.
        /// </summary>
        /// <returns>The packages.</returns>
        IEnumerable<IPackage> RetrievePackages();

        /// <summary>
        /// Retrieves the activity groups.
        /// </summary>
        /// <returns>The activity groups.</returns>
        /// <param name="packageId">Package identifier.</param>
        /// <param name="parentUUID">The parent identifier.</param>
        IEnumerable<IActivityGroup> RetrieveActivityGroups(string packageId = null, string parentUUID = null);

        /// <summary>
        /// Retrieves the parent activity groups.
        /// </summary>
        /// <returns>The parent activity groups.</returns>
        /// <param name="packageId">Package identifier.</param>
        IEnumerable<IActivityGroup> RetrieveParentActivityGroups(string packageId = null);

        /// <summary>
        /// Retrieves the parent activity groups that exist within a specific parent.
        /// </summary>
        /// <returns>The parent activity groups.</returns>
        /// <param name="packageId">Requesting parent's package identifier.</param>
        IEnumerable<IActivityGroup> RetrieveParentActivityGroupsByParent(string packageId);

        /// <summary>
        /// Retrieves all activities.
        /// </summary>
        /// <returns>The activities.</returns>
        IEnumerable<IActivity> RetrieveActivities();

        /// <summary>
        /// Retrieves the activity group.
        /// </summary>
        /// <returns>The activity group.</returns>
        /// <param name="groupUUID">Group UUID.</param>
        IActivityGroup RetrieveActivityGroup(string groupUUID);

        /// <summary>
        /// Retrieves the points of interest.
        /// </summary>
        /// <returns>The points of interest.</returns>
        IEnumerable<IPointOfInterest> RetrievePointsOfInterest();

        /// <summary>
        /// Retrieves the links.
        /// </summary>
        /// <returns>The links.</returns>
        IEnumerable<ILink> RetrieveLinks();

        /// <summary>
        /// Refreshes the package data.
        /// </summary>
        /// <returns>The updated package data.</returns>
        Task<IEnumerable<IPackage>> RefreshPackages();

        /// <summary>
        /// Deletes all content.
        /// </summary>
        void DeleteAllContent();
    }
}
