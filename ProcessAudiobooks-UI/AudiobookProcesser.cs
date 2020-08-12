using ProcessAudiobooks_UI.CustomControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public AudiobookProcesser(TextBox tbCommand, TextBox tbLocalPath, TextBox tbRemotePath, EnhancedListView eLvAudiobook)
        {
            this.tbCommand = tbCommand;
            this.tbLocalPath = tbLocalPath;
            this.tbRemotePath = tbRemotePath;
            this.eLvAudiobook = eLvAudiobook;
        } 

        public async void StartProcess()
        {
            if (this.processing == Processing.Stopped) //if not started start.
            {
                this.processing = Processing.Started;
                foreach (DataObjects.Audiobook book in eLvAudiobook.Items)
                {
                    if (book.Status == DataObjects.AudiobookProcessingStatus.Ready)
                    {
                        string destFolder = tbLocalPath.Text + "\\" + book.Name;
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


                            

                            /**/


                            /*int startPos = file.CompareTo(tbLocalPath.Text);
                            startPos = startPos + tbLocalPath.Text.Length-1;
                            string newFile = file.Substring(startPos);
                            newFile = tbRemotePath.Text + newFile.Replace('\\', '/');
                            MessageBox.Show(newFile);*/
                        }
                    }
                }
            }
            this.processing = Processing.Stopped;
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
