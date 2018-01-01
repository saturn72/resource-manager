using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Renci.SshNet.Common;

namespace LabManager.Services.Ssh
{
    public class SshClient
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;
        private readonly StringBuilder _inputStream = new StringBuilder();

        public SshClient(string username, string password, string hostname)
        {
            _username = username;
            _password = password;
            _hostname = hostname;
        }

        public string ExecuteCommands(IEnumerable<string> commands, uint milisecDelayBeforeChannelClosing = 0)
        {
            _inputStream.Clear();
            using (var sshClient = new Renci.SshNet.SshClient(_hostname, _username, _password))
            {
                sshClient.Connect();

                if (commands.Count() == 1)
                    return sshClient.RunCommand(commands.ElementAt(0)).Result;

                using (var shellStream = sshClient.CreateShellStream("QCore-Automation_Ssh", 0, 0, 0, 0, 1024))
                {
                    shellStream.DataReceived += OnShellStreamDataRecieved;
                    foreach (var cmd in commands)
                        shellStream.WriteLine(cmd);

                    var startTime = DateTime.UtcNow;
                    shellStream.Flush();

                    while (DateTime.UtcNow.Subtract(startTime).TotalMilliseconds < milisecDelayBeforeChannelClosing)
                    {
                        Thread.Sleep(150);
                    }
                    shellStream.DataReceived -= OnShellStreamDataRecieved;
                    shellStream.Close();
                }
                sshClient.Disconnect();
            }

            return _inputStream.ToString();
        }

        private void OnShellStreamDataRecieved(object sender, ShellDataEventArgs e)
        {
            var value = Encoding.Default.GetString(e.Data);
            _inputStream.Append(value);
        }
    }
}