using System;

namespace QAutomation.Core.Audit
{
    public interface ICreateAudit
    {
        DateTime CreatedOnUtc { get; set; }
        long CreatedByUserId { get; set; }
    }
}