using System;

namespace QAutomation.Core.Audit
{
    public interface IDeleteAudit : ICreateAudit
    {
        DateTime? DeletedOnUtc
        {
            get; set;
        }
        long? DeletedByUserId { get; set; }
    }
}