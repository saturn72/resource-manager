using System;
using System.Linq;
using System.Threading;
using Saturn72.Extensions;

namespace LabManager.Services.Runtime
{
    public class CleanTimer
    {
        private static Timer _timer;
        private const int CleanTimerInterval = 10 * 1000;
        private const int ResourceMaxUnAccessTime = 60;


        public static void Start()
        {
            _timer = new Timer(_ => CleanAssignedResourceRecords(), null, 0, CleanTimerInterval);
        }

        private static void CleanAssignedResourceRecords()
        {
            if (AssignedResourceRecordCollection.All.IsEmptyOrNull())
                return;
            var curDateTime = DateTime.UtcNow;

            var toClear = AssignedResourceRecordCollection.All.Where(
                rar => curDateTime.Subtract(rar.LastAccessedOnUtc).TotalSeconds >
                       ResourceMaxUnAccessTime).ToArray();
            foreach (var tr in toClear)
                AssignedResourceRecordCollection.Remove(tr);
        }
    }
}
