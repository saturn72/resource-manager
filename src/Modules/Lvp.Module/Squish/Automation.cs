using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Saturn72.Extensions;

namespace Lvp.Module.Squish
{
    public class Automation
    {
        #region ctor/dtor
        public Automation(string squishPrefix = "", string host = "localhost", ushort port = 4322)
        {
            Guard.HasValue(squishPrefix,
                () => throw new SquishException("SQUISH_PREFIX is not set and no prefix was passed to Init()"));

            StartSquishCmdProcess(squishPrefix, host, port);
            SetIOStreams();

            WaitForSquishCmdInitStartupRespone();
        }

        ~Automation()
        {
            if (!_wasSquisCmdStarted || _squishCmdProcess.HasExited)
                return;
            _squishCmdProcess.Close();
        }
        #endregion

       
        public void Shutdown()
        {
            VerifySquishCmdStillRunning();
            _squishCmdProcess.StandardInput.WriteLine("q");
            _squishCmdOutput.Close();
            _squishCmdInput.Close();
        }

        public ObjectRef Eval(string command)
        {
            Guard.HasValue(command, ()=> throw new SquishException("Code string to evaluate is empty."));

            VerifySquishCmdStillRunning();

            _squishCmdInput.WriteLine("eval " + command);
            _squishCmdInput.Flush();
            
            //Parse rsponse
            var response = _squishCmdOutput.ReadLine();
            
            var statusIndex = response.IndexOf(ResponseFieldDelimiter);
            var contenIndex = response.IndexOf(ResponseFieldDelimiter, statusIndex + 1);
            var status = response.Substring(0, statusIndex);
            string str3 = null;
            string val = null;

            Guard.Equals(status, ResponseStatus.Ok, ()=> throw new SquishException("Eval failed: " + response.Substring(statusIndex + 1)));

            if (contenIndex > -1)
            {
                str3 = response.Substring(statusIndex + 1, contenIndex - statusIndex - 1);
                val = response.Substring(contenIndex + 1);
            }
            else
            {
                str3 = response.Substring(statusIndex + 1);
            }


            ObjectRef objectRef;
            switch (str3)
            {
                case "boolean":
                    objectRef = new ObjectRef(this, val == "true" ? "True" : "False", RefType.Boolean);
                    break;
                case "float":
                    objectRef = new ObjectRef(this, val, RefType.Double);
                    break;
                case "int":
                    objectRef = new ObjectRef(this, val, RefType.Long);
                    break;
                case "ref":
                    objectRef = new ObjectRef(this, val, RefType.Object);
                    break;
                case "string":
                    objectRef = new ObjectRef(this, val, RefType.String);
                    break;
                case "void":
                    objectRef = new ObjectRef(this, val, RefType.Void);
                    break;
                default:
                    objectRef = new ObjectRef(this, response.Substring(statusIndex), RefType.Long);
                    break;
            }
            if (objectRef.Type == RefType.Exception)
                throw new SquishException(objectRef.ToString());
            return objectRef;
        }

        private void VerifySquishCmdStillRunning()
        {
            if (_squishCmdProcess.HasExited)
                throw new SquishException("The squishcmd process needs to be running for this operation.");
        }

        # region Utilities

        private void WaitForSquishCmdInitStartupRespone()
        {
            string output;

            do
            {
                output = _squishCmdOutput.ReadLine();
            } while (output != SquishInitSuccessMessage && output != SquishInitErrorMessage);

            if (output == SquishInitErrorMessage)
                throw new SquishException("squishcmd process intialization failed.");
            _wasSquisCmdStarted = true;
        }

        private void SetIOStreams()
        {
            _squishCmdInput =
                TextWriter.Synchronized(new StreamWriter(_squishCmdProcess.StandardInput.BaseStream,
                    new UTF8Encoding(false)));
            _squishCmdOutput =
                TextReader.Synchronized(new StreamReader(_squishCmdProcess.StandardOutput.BaseStream,
                    new UTF8Encoding(false)));
        }

        private void StartSquishCmdProcess(string localSquishPath, string host, ushort port)
        {
            var squishCmdExecutable = Path.Combine(localSquishPath, SquishCmdPathSuffix);
            var squishCmdArgs = SquishCmdArgumentsFormat.AsFormat(localSquishPath, host, port);
            var psi = new ProcessStartInfo
            {
                FileName = "\"" + squishCmdExecutable + "\"",
                Arguments = squishCmdArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = false
            };

            _squishCmdProcess = Process.Start(psi);

            try
            {
                if (!_squishCmdProcess.IsRunning())
                    throw new SquishException("Failed to start squishcmd at: \"" + squishCmdExecutable + "\"");
            }
            catch (Win32Exception ex)
            {
                throw new SquishException("Failed to start squishcmd at: \"" + squishCmdExecutable + "\". Reason: " + ex.Message);
            }
        }

        #endregion

        #region consts
        private const char ResponseFieldDelimiter = ' ';

        private const string SquishInitSuccessMessage = "Hello!";
        private const string SquishInitErrorMessage = "Error!";

        private const string SquishCmdPathSuffix = @"lib\exec\squishcmd.exe";

        private const string SquishCmdArgumentsFormat =
            "--lang javascript --squish-prefix \"{0}\" --server-host {1} --server-port {2}";

        #endregion

        #region Fields

        private Process _squishCmdProcess;
        private bool _wasSquisCmdStarted;
        private TextWriter _squishCmdInput;
        private TextReader _squishCmdOutput;

        #endregion
    }
}