using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using LabManager.Common.Domain.Resource;
using LabManager.Services.Ssh;
using QAutomation.Extensions;

namespace LabManager.Services.Commanders
{
    public class LvpResourceCommander : IResourceCommander
    {
        #region Consts

        private const string QcGui = "QC_GUI";

        #endregion

        #region Statics

        private static readonly ICollection<ActiveRuntime> ActiveRutimes = new List<ActiveRuntime>();

        #endregion

        public int Start(ResourceModel resource)
        {
            if (ActiveRutimes.Any(ar => ar.ResourceId == resource.Id))
                throw new InvalidOperationException("Resource already started");

            if (StartAutViaSsh(resource) != 0)
                return -666;
            var activeRuntime = new ActiveRuntime {ResourceId = resource.Id};
            try
            {
                StartLocalServices(activeRuntime, resource);
            }
            catch (Exception ex)
            {
                return Failure;
            }
            ActiveRutimes.Add(activeRuntime);
            return 0;
        }

        private void StartSquishServer(ActiveRuntime activeRuntime, ResourceModel resource)
        {
            var squishServerPath = @"C:\Program Files\Squish\bin\squishserver.exe";
            //First configure squish server
            var configPsi = new ProcessStartInfo
            {
                FileName = squishServerPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = string.Format("--config addAttachableAUT {0}_{1} {2}:{3}", QcGui,resource.Id,
                    resource.IpAddress, resource.SquishServerPort),
            };
            var squishServerConfigProcess = Process.Start(configPsi);
            squishServerConfigProcess.WaitForExit();

            //second Launch squish server
            var startSquishServerPsi = new ProcessStartInfo
            {
                FileName = squishServerPath,
                Arguments = "--port=" + resource.SquishServerPort,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var squishServerProcess = Process.Start(startSquishServerPsi);
            Thread.Sleep(2000);

            activeRuntime.LocalProcessesIds.Add(squishServerProcess.Id);
        }


        public int Stop(ResourceModel resource)
        {
            if (StopAutViaSsh(resource) != 0)
                return Failure;

            var activeRuntime = ActiveRutimes.FirstOrDefault(ar => ar.ResourceId == resource.Id);
            if (activeRuntime.IsNull())
                return 0;

            //stop local services
            foreach (var pId in activeRuntime.LocalProcessesIds)
                Process.GetProcessById(pId).Kill();

            return 0;
        }

        public bool IsAlive(ResourceModel resource)
        {
            return IsAutProcessAlive(resource);
        }

        private void StartLocalServices(ActiveRuntime activeRuntime, ResourceModel resource)
        {
            CheckResourceCompatibilityForSquish(resource);
            StartSquishServer(activeRuntime, resource);
            throw new NotImplementedException("Start Squish CMD not implemented");
        }


        #region consts

        private const string NoHupCommandFormat = "nohup {0} >/dev/null 2>&1 &";
        private const ushort AutPort = 8002;
        private const int Failure = -666;

        #endregion

        #region Utilities

        private bool IsAutProcessAlive(ResourceModel resource)
        {
            CheckResourceCompatibilityForSsh(resource);

            var getProcesses = "pgrep " + QcGui;
            var startLocalMonitorCommands = new[]
            {
                getProcesses
            };
            var sshResponse = RunSshCommads(resource, startLocalMonitorCommands, 100);
            if (!sshResponse.HasValue())
                return false;

            var lines = sshResponse.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
            var commandLineIndex = 0;
            int pId;

            //when the process id is the only value returned
            if (lines.Length == 1)
                return int.TryParse(lines[commandLineIndex], out pId);

            //when the response has full ssh content
            do
            {
                commandLineIndex++;
                if (lines[commandLineIndex].Trim().EndsWith(QcGui))
                    break;
            } while (commandLineIndex < lines.Length);

            return commandLineIndex != lines.Length && int.TryParse(lines[0], out pId);
        }

        private int StopAutViaSsh(ResourceModel resource)
        {
            CheckResourceCompatibilityForSsh(resource);

            var stopAllProcesses = new[]
            {
                "kill -9 $(pgrep QC_)"
            };
            RunSshCommads(resource, stopAllProcesses, 2000);
            return IsAlive(resource) ? -666 : 0;
        }

        private int StartAutViaSsh(ResourceModel resource)
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
            var squishCommand = "./../squish/bin/startaut --port={0} {1}".AsFormat(AutPort, QcGui);
            var startAutCommands = new[]
            {
                "export DISPLAY=:0",
                "cd ~/LVPPump/X86/",
                "export LD_LIBRARY_PATH=.:/home/qcore/Qt5.6.2/5.6/gcc_64/lib",
                NoHupCommandFormat.AsFormat(squishCommand)
            };
            var res = RunSshCommads(resource, startAutCommands, 2000);
            return res.HasValue()?0:1;
        }

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
        private void CheckResourceCompatibilityForSquish(ResourceModel resource)
        {
            var path = resource.SquishServerLocalPath;
            if (resource.SquishServerPort > 0 && path.HasValue() && !(File.Exists(path) || Directory.Exists(path)))
                throw new MissingMemberException("Missing resource data for connecting to squish");
        }
        #endregion
    }
}