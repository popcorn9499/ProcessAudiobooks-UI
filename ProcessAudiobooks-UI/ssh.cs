using Renci.SshNet;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessAudiobooks_UI
{
    public class ssh
    {
        private string ip { get; set; }
        private int port { get; set; }
        private string username { get; set; }
        private string password { get; set; }

        private SshClient client;

        public  ssh(string ip, int port, string username, string password) {
            this.ip = ip;
            this.port = port;
            this.username = username;
            this.password = password;
            try
            {
                var tsk = RunCommand("echo 'SSH Connection Test Successful'");
                tsk.Wait();
            }
            catch
            {
                System.Windows.MessageBox.Show("SSH Server down or The connection settings are wrong");
                ConsoleWindow.WriteInfo("SSH Server down or The connection settings are wrong");
                throw new SSHConnectionFailed();
            }
            ConsoleWindow.WriteInfo("SSH connected!");
        }

        public async Task RunCommand(string command)
        {
            using (var client = new SshClient(this.ip, this.port, this.username, this.password))
            {
                client.Connect();
                var cmd = client.CreateCommand(command);
                var result = cmd.BeginExecute();

                using (var reader =
                   new StreamReader(cmd.OutputStream, Encoding.UTF8, true, -1, true))
                {
                    while (!result.IsCompleted || !reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            ConsoleWindow.WriteInfo(line);
                            await Task.Delay(2);
                        }
                    }
                }

                cmd.EndExecute(result);
                ConsoleWindow.WriteInfo("Finished");
            }
        }
    }

    public class SSHConnectionFailed : Exception
    {
        public SSHConnectionFailed()
            : base("SSH Server down or The connection settings are wrong") { }
    }
}
