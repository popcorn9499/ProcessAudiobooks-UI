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
    //This is ment to be a easy object to handle ssh connections
    //It will handle storing the data required to make ssh connections such as. Username, Password, ip and port. 
    //
    public class ssh
    {
        public string ip { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        //empty constructor so deserializating from json to a object works correctly
        public ssh()
        {
            this.ip = "127.0.0.1";
            this.port = 22;
            this.username = "me";
            this.password = "me";
        }

        //gather and save the user information and the port required to use this object and make a connection
        public ssh(string ip, int port, string username, string password) {
            this.ip = ip;
            this.port = port;
            this.username = username;
            this.password = password;

        }

        //gather and save the user information to use this object and make a connection
        public ssh(string ip, string username, string password)
        {
            this.ip = ip;
            this.port = 22; //assume if not specified that the port is 22
            this.username = username;
            this.password = password;

        }

        //Handle a self test on running a ssh connection
        public async Task sshStartTest()
        {
            //try to make a connection to the ssh server. if it fails send up a message
            try
            {
                await RunCommand("echo 'SSH Connection Test Successful'");
            }
            catch
            {   //if a failure does end up occuring. Notify the user and give up
                System.Windows.MessageBox.Show("SSH Server down or The connection settings are wrong");
                ConsoleWindow.WriteInfo("SSH Server down or The connection settings are wrong");
                throw new SSHConnectionFailed();
                //Note. I really think i should rethink this when im not half asleep.
            }
            ConsoleWindow.WriteInfo("SSH connected!");
        }

        //run a ssh command as specified.
        //if an error occurs this will throw 
        public async Task RunCommand(string command)
        {
            //TODO maybe add in more specific error handling to give more detailed feedback
            try //catch errors and throw a nicer looking exception forward.
            {
                //create the ssh client object which we are using.
                using (var client = new SshClient(this.ip, this.port, this.username, this.password))
                {
                    ConsoleWindow.WriteInfo("Starting command");
                    client.Connect();
                    
                    ConsoleWindow.WriteInfo(command); //debug write the command to the console window so we can find it easily if something goes wrong.
                    var cmd = client.CreateCommand(command);
                    
                    var result = cmd.BeginExecute();
                    ConsoleWindow.WriteInfo("Started command");
                    
                    //read all the stdout from the ssh client until it has stopped sending new lines/end of file/completed.
                    using (var reader =
                       new StreamReader(cmd.OutputStream, Encoding.UTF8, true, -1, true))
                    { 

                        while (!result.IsCompleted || !reader.EndOfStream)
                        {
                            string line = await reader.ReadLineAsync(); //read data from the reader
                            
                            if (line != null)
                            {
                                ConsoleWindow.WriteInfo("[SSH] " + line); //print the data to the ConsoleWindow if its important
                            }
                            await Task.Delay(10); //add a small delay to prevent this loop from going out of control
                        }
                    }
                    cmd.EndExecute(result);
                    ConsoleWindow.WriteInfo("Finished");
                }
            } catch (Exception ex)
            {
                ConsoleWindow.WriteInfo(ex.ToString());
                ConsoleWindow.WriteInfo(ex.StackTrace);
                throw new SSHConnectionFailed();
            }
        }
    }
    
    //ssh connection failed exception
    //this is ment to be an exception thrown if the connection failed for whatever reason. This should be caught later in the code to potentially bring this to the users attention
    public class SSHConnectionFailed : Exception
    {
        public SSHConnectionFailed()
            : base("SSH Server down or The connection settings are wrong") { }
    }
}
