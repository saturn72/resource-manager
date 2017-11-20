using System;
using System.Collections.Generic;
using LabManager.Common.Domain.Resource;
using LabManager.Services.Ssh;
using QAutomation.Extensions;

namespace LabManager.Services.Commanders
{
    public class SshResourceCommander : IResourceCommander
    {
        public int Start(ResourceModel resource)
        {
            CheckResourceCompatibilityForSsh(resource);
            if (IsAlive(resource))
                return 0;

            var startLocalMonitorCommands = new[]
            {
                "export DISPLAY=:0",
                "cd ~/LVPPump/X86/",
                "export LD_LIBRARY_PATH=.:/home/qcore/Qt5.6.2/5.6/gcc_64/lib",
                NoHupCommandFormat.AsFormat("./QC_LOCAL_MONITOR")
            };
            RunSshCommads(resource, startLocalMonitorCommands, 6000);
            var squishCommand = "./../squish/bin/startaut --port={0} QC_GUI".AsFormat(AutPort);
            var startAutCommands = new[]
            {
                "export DISPLAY=:0",
                "cd ~/LVPPump/X86/",
                "export LD_LIBRARY_PATH=.:/home/qcore/Qt5.6.2/5.6/gcc_64/lib",
                NoHupCommandFormat.AsFormat(squishCommand)
            };
            RunSshCommads(resource, startAutCommands, 2000);
            return IsAlive(resource) ? 0 : 1;
        }

        public int Stop(ResourceModel resource)
        {
            CheckResourceCompatibilityForSsh(resource);

            var stopAllProcesses = new[]
            {
                "kill -9 $(pgrep QC_)"
            };
            RunSshCommads(resource, stopAllProcesses, 2000);
            return IsAlive(resource) ? -666 : 0;
        }

        public bool IsAlive(ResourceModel resource)
        {
            CheckResourceCompatibilityForSsh(resource);

            var getProcesses = "pgrep QC_GUI";
            var startLocalMonitorCommands = new[]
            {
                getProcesses
            };
            var sshResponse = RunSshCommads(resource, startLocalMonitorCommands, 100);
            if (sshResponse.IsEmptyOrNull() || sshResponse.Trim().IsEmptyOrNull())
                return false;
            var lines = sshResponse.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //if (lines.Length > 1)
            //{
            //    int commandLinesNumberInSshResponse = 0;
            //    for (; commandLinesNumberInSshResponse < lines.Length; commandLinesNumberInSshResponse++)
            //        if (lines[commandLinesNumberInSshResponse].Trim().EndsWith(getProcesses))
            //            break;
            //    if (commandLinesNumberInSshResponse == lines.Length)
            //        return false;
            //}

            int pId;
            return int.TryParse(lines[0], out pId);
        }

        #region consts

        private const string NoHupCommandFormat = "nohup {0} >/dev/null 2>&1 &";
        private const ushort AutPort = 8002;

        #endregion

        #region Utilities

        private string RunSshCommads(ResourceModel resource, IEnumerable<string> commandArray, uint preCloseDelay)
        {
            var sshClient = new SshClient(resource.SshUsername, resource.SshPassword, resource.IpAddress);
            return sshClient.ExecuteCommands(commandArray, preCloseDelay);
        }

        private void CheckResourceCompatibilityForSsh(ResourceModel resource)
        {
            if (resource.IpAddress.IsEmptyOrNull() || resource.SshUsername.IsEmptyOrNull() ||
                   resource.SshPassword.IsEmptyOrNull())
                throw new MissingMemberException("Missing resource data for ssh connection");
        }

        #endregion
    }
}