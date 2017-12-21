using System.Collections.Generic;

namespace LabManager.Services.Commanders
{
    public class ActiveRuntime
    {
        private ICollection<int> _localProcessesIds;
        public long ResourceId { get; set; }

        public ICollection<int> LocalProcessIds
        {
            get => _localProcessesIds ?? (_localProcessesIds = new List<int>());
            internal set => _localProcessesIds = value;
        }

    }
}