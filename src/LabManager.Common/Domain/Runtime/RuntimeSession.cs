using System.Collections.Generic;
using LabManager.Common.Domain.Resource;
using QAutomation.Core.Domain;

namespace LabManager.Common.Domain.Runtime
{
    public class RuntimeSession:DomainModelBase
    {
        public RuntimeSession(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
        public IEnumerable<ResourceModel> Resources { get; set; }
    }
}
