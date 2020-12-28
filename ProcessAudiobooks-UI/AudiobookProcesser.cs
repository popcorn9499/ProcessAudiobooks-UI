using ProcessAudiobooks_UI.CustomControls;
using ProcessAudiobooks_UI.DataObjects;
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
            bool failed = false; //if any error occurs during this loop proceed to make it to the bottom and the the user it failed
            if (this.processing == Processing.Stopped) //if not started start.
            {
                this.processing = Processing.Started;
                this.btnStartCreateAudiobooks.IsEnabled = false;
                this.btnStopCreateAudiobooks.IsEnabled = true;
                foreach (DataObjects.Audiobook book in eLvAudiobook.Items)
                {
                    //if the book is ready to be processed and Processing hasnt been set to stop indicating we should stop mid execution of this loop. 
                    if (book.Status == DataObjects.AudiobookProcessingStatus.Ready && this.processing != Processing.Stopped) 
                    {
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
                        command = this.addVariables(command, cmdOutputPath, book);

                        string commandDestFolder = tbRemotePath.Text + "/" + book.outputName;

                        //Run Command
                        ConsoleWindow.WriteInfo("Processing the audiobook");
                        MessageBoxResult retryConnection = MessageBoxResult.No; //handle retrying to connect if for whatever reason the ssh server refuses to respond.
                        do //loop over this code until either the user gives up or 
                        {
                            try
                            {
                                //handle running a ssh command with the sshClient
                                await sshClient.RunCommand("cd \"" + commandDestFolder + "\" && " + command);
                            }
                            catch (SSHConnectionFailed)
                            {
                                retryConnection = MessageBox.Show("Do you wish to retry connecting to the ssh server?",
                                                                    "ProcessAudioBook Prompt", MessageBoxButton.YesNo, MessageBoxImage.Error);
                                if (retryConnection == MessageBoxResult.No) //indicate that this loop has failed and that we should give up processing audiobooks.
                                {
                                    failed = true;
                                }
                            }
                        } while (retryConnection == MessageBoxResult.Yes);

                        if (failed)
                        { //we must exit this existing foreach loop and proceed to reset some of the program
                            book.Status = DataObjects.AudiobookProcessingStatus.Ready;
                            break;
                        }

                        //copy files back
                        string finalOutputPath = this.addVariables(book.outputPath, cmdOutputPath, book);
                        ConsoleWindow.WriteInfo("Copying book to its output directory");
                        this.CopyDirectory(tbLocalPath.Text + "\\" + book.outputName + "\\output\\", finalOutputPath);

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
            if (!failed)
            {
                MessageBox.Show("Completed the audiobooks!");
            } else {
                MessageBox.Show("Failure occured. some audiobooks never finished being processed");
            }
        }

        private void CopyDirectory(string file, string trueDestFolder)
        {
            Directory.CreateDirectory(trueDestFolder);
            //Now Create all of the directories
            string destPath;
            foreach (string dirPath in Directory.GetDirectories(file, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(file, trueDestFolder));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(file, "*.*",
                SearchOption.AllDirectories))
            {
                destPath = newPath.Replace(file, trueDestFolder + "\\");
                File.Copy(newPath, destPath, true);
            }
        }

        public string addVariables(string data,string cmdOutputPath, Audiobook book)
        {
            return data.Replace("%Name%", book.Name).Replace("%outputName%", cmdOutputPath).Replace("%Artist%", book.Artist).Replace("%Album%", book.Album)
                            .Replace("%Genre%", book.Genre).Replace("%Year%", book.Year).Replace("%Writer%", book.Writer);
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
