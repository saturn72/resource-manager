using System.Collections.Generic;

namespace LabManager.Services.Runtime
{
    internal class AssignedResourceRecordCollection
    {
        private static readonly ICollection<AssignedResourceRecord> ResourceAssignRecords =
            new List<AssignedResourceRecord>();

        internal static void Add(AssignedResourceRecord assignedResourceRecord)
        {
            ResourceAssignRecords.Add(assignedResourceRecord);
        }

        internal static bool Remove(AssignedResourceRecord assignedResourceRecord)
        {
            return ResourceAssignRecords.Remove(assignedResourceRecord);
        }

        public static IEnumerable<AssignedResourceRecord> All => ResourceAssignRecords;
    }
}