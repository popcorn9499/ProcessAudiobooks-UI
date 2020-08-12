using Renci.SshNet;
using System;
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

        public ssh(string ip, int port, string username, string password) {
            this.ip = ip;
            this.port = port;
            this.username = username;
            this.password = password;
            //Console.WriteLine("Starting Audiobook Processor");
            var tsk = TestMethod();
            for (int i = 0; i< 5; i++)
            {
                Console.WriteLine("TEST123");
                Thread.Sleep(1000);
            }
            tsk.Wait();

        }

        public static async Task TestMethod()
        {
            using (var client = new SshClient("ip", "username", "test"))
            {
                client.Connect();
                var cmd = client.CreateCommand("for i in {1..5}; do echo 'hi' ; sleep 1 ; done");
                var result = cmd.BeginExecute();
               // Console.WriteLine(cmd.Result);

                using (var reader =
                   new StreamReader(cmd.OutputStream, Encoding.UTF8, true, -1, true))
                {
                    while (!result.IsCompleted || !reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            Console.WriteLine(line);
                            await Task.Delay(50);
                        }
                    }
                }

                cmd.EndExecute(result);


            }
        }
    }
}
