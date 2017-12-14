using Saturn72.Core;

namespace LabManager.WebService
{
    public sealed class LabManagerWorkContext:IWorkContext
    {
        public long CurrentUserId => 123;

        public string CurrentUserIpAddress => "ipAddress";

        public string ClientId => "client-id";
    }
}