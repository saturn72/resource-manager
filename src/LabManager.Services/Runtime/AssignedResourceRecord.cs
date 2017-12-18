using System;

namespace LabManager.Services.Runtime
{
    internal class AssignedResourceRecord
    {
        public long ResourceId { get; set; }
        public DateTime AssignedOnUtc { get; set; }
        public DateTime LastAccessedOnUtc { get; set; }
    }
}