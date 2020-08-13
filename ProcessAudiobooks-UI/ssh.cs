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
        public string ip { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }


        public ssh(string ip, int port, string username, string password) {
            this.ip = ip;
            this.port = port;
            this.username = username;
            this.password = password;
            
        }

        public async Task sshStartTest()
        {
            try
            {
                await RunCommand("echo 'SSH Connection Test Successful'");
                
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
                ConsoleWindow.WriteInfo(command);
                var cmd = client.CreateCommand(command);
                var result = cmd.BeginExecute();

                using (var reader =
                   new StreamReader(cmd.OutputStream, Encoding.UTF8, true, -1, true))
                {
                    while (!result.IsCompleted || !reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        if (line != null)
                        {
                            ConsoleWindow.WriteInfo("[SSH] " + line);
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
