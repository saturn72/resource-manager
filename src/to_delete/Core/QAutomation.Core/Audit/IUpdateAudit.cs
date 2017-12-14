using System;

namespace QAutomation.Core.Audit
{
    public interface IUpdateAudit : ICreateAudit
    {
        DateTime? UpdatedOnUtc { get; set; }
        long? UpdatedByUserId { get; set; }
    }
}