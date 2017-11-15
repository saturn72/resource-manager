using System;
using QAutomation.Extensions;

namespace QAutomation.Core.Audit
{
    public static class AuditExtensions
    {
        public static bool WasUpdated(this IUpdateAudit audit)
        {
            return audit.UpdatedOnUtc != default(DateTime);
        }

        public static bool WasDeleted(this IDeleteAudit audit)
        {
            return audit.DeletedOnUtc.NotNull();
        }
    }
}