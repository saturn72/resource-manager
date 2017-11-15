using System;

namespace QAutomation.Core.Audit
{
    public interface IAccessAudit
    {
        DateTime LastAccessedOnUtc { get; set; }
        long LastAccessedByUserId { get; set; }
        string LastAccessedIpAddress { get; set; }
        string LastAccessedAppId { get; set; }
    }
}