using ProcessAudiobooks_UI.CustomControls;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProcessAudiobooks_UI
{
    public class AudiobookProcesser
    {
        private TextBox tbCommand;
        private TextBox tbLocalPath;
        private TextBox tbRemotePath;
        private EnhancedListView eLvAudiobook;

        private Processing processing = Processing.Stopped;
        private Button btnStartCreateAudiobooks, btnStopCreateAudiobooks;

        public AudiobookProcesser(TextBox tbCommand, TextBox tbLocalPath, TextBox tbRemotePath, EnhancedListView eLvAudiobook, Button btnStartCreateAudiobooks , Button btnStopCreateAudiobooks)
        {
            this.tbCommand = tbCommand;
            this.tbLocalPath = tbLocalPath;
            this.tbRemotePath = tbRemotePath;
            this.eLvAudiobook = eLvAudiobook;
            this.btnStopCreateAudiobooks = btnStopCreateAudiobooks;
            this.btnStartCreateAudiobooks = btnStartCreateAudiobooks;
        } 

        public async Task StartProcess(ssh sshClient)
        {
            if (this.processing == Processing.Stopped) //if not started start.
            {
                this.processing = Processing.Started;
                this.btnStartCreateAudiobooks.IsEnabled = false;
                this.btnStopCreateAudiobooks.IsEnabled = true;
                foreach (DataObjects.Audiobook book in eLvAudiobook.Items)
                {
                    if (book.Status == DataObjects.AudiobookProcessingStatus.Ready)
                    {
                        if (this.processing == Processing.Stopped)
                            break;
                        book.Status = DataObjects.AudiobookProcessingStatus.Processing; //set audiobook status
                        this.eLvAudiobook.Items.Refresh();
                        ConsoleWindow.WriteInfo("Starting Audiobook: " + book.Name);
                        ConsoleWindow.WriteInfo("Copying Audiobook to processing path");
                        string destFolder = tbLocalPath.Text + "\\" + book.outputName;
                        Directory.CreateDirectory(destFolder);
                        string[] fileList = book.FileList.ToArray();
                        foreach (String file in fileList)
                        {
                            // get the file attributes for file or directory
                            FileAttributes attr = File.GetAttributes(file);
                            string[] fileArray = file.Split("\\");
                            string fileName = fileArray[fileArray.Length - 1];

                            //detect whether its a directory or file
                            if ((attr & FileAttributes.Directory) == FileAttributes.Directory) // if its a directory copy the directory and files individually
                            {
                                string trueDestFolder = destFolder + "\\" + fileName;
                                this.CopyDirectory(file, trueDestFolder);
                            }
                            else //if its a file just do a simple file copy
                            {
                                System.IO.File.Copy(file, file.Replace(file, destFolder + "\\" + fileName), true);
                            }
                        }
                        //Create Command

                        String command = tbCommand.Text;
                        string cmdOutputPath = "output/" + book.outputName;
                        command = command.Replace("%Name%", book.Name).Replace("%outputName%", cmdOutputPath).Replace("%Artist%",book.Artist).Replace("%Album%", book.Album)
                            .Replace("%Genre%", book.Genre).Replace("%Year%", book.Year).Replace("%Writer%", book.Writer);

                        string commandDestFolder = tbRemotePath.Text + "/" + book.outputName;

                        //Run Command
                        ConsoleWindow.WriteInfo("Copying book to its output directory");
                        await sshClient.RunCommand("cd \"" + commandDestFolder + "\" && " +command);

                        //copy files back

                        ConsoleWindow.WriteInfo("Copying book to its output directory");
                        this.CopyDirectory(tbLocalPath.Text + "\\" + book.outputName + "\\output\\", book.outputPath);

                        ConsoleWindow.WriteInfo("Cleaning up!");
                        Directory.Delete(tbLocalPath.Text + "\\" + book.outputName, true);
                        ConsoleWindow.WriteInfo("Finished Audiobook: " + book.Name);
                        book.Status = DataObjects.AudiobookProcessingStatus.Completed; //set audiobook status
                        this.eLvAudiobook.Items.Refresh();
                    }
                }
            } 
            this.processing = Processing.Stopped;
            this.btnStartCreateAudiobooks.IsEnabled = true;
            this.btnStopCreateAudiobooks.IsEnabled = false;
        }

        private void CopyDirectory(string file, string trueDestFolder)
        {
            Directory.CreateDirectory(trueDestFolder);
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(file, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(file, trueDestFolder));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(file, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(file, trueDestFolder), true);
        }

        public void StopProcess()
        {
            this.processing = Processing.Stopped;
        }

    }

    public enum Processing
    {
        Stopped = 0,
        Started = 1
    }
}
