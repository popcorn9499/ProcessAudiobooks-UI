using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Interop;
using System.Reflection;
using ProcessAudiobooks_UI.DataObjects;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace ProcessAudiobooks_UI
{
    /// <summary>
    /// Interaction logic for AddAudiobookWindow.xaml
    /// </summary>
    public partial class AddAudiobookWindow : Window
    {

        public bool outputPathManuallySet = false;
        public DataObjects.Audiobook book = null;

        public AddAudiobookWindow()
        {
            InitializeComponent();
            
        }

        public AddAudiobookWindow(DataObjects.Audiobook audiobook)
        {
            InitializeComponent();
            tbName.Text = audiobook.Name;
            tbOutputName.Text = audiobook.outputName;
            tbArtist.Text = audiobook.Artist;
            tbAlbum.Text = audiobook.Album;
            tbGenre.Text = audiobook.Genre;
            tbYear.Text = audiobook.Year;
            tbWriter.Text = audiobook.Writer;
            tbOutputPath.Text = audiobook.outputPath;
            foreach (String data in audiobook.FileList)
            {
                lvListFiles.Items.Add(data);
            }
        }

            private void listFiles_Drop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) //checks if we have any dropped files and adds them.
            {
                droppedFiles = e.Data.GetData(DataFormats.FileDrop, true) as string[];
            }

            if ((null == droppedFiles) || (!droppedFiles.Any())) { return; } //if we have no dropped files then stop this function

            foreach (string s in droppedFiles) //if we have dropped files add them to the listview ListFiles
            {
                lvListFiles.Items.Add(s);
            }
            //get output path directory automatically
            if (!this.outputPathManuallySet)
            {
                string file = lvListFiles.Items[0].ToString();
                FileAttributes attr = File.GetAttributes(file);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                { //handling for a directory
                    string[] fileSplit = file.Split('\\');
                    string outputDirPath;
                    if (fileSplit.Length > 1)
                    {
                        List<string> fileSplitList = fileSplit.ToList();
                        fileSplitList.RemoveAt(fileSplit.Length - 1);
                        char x = '\\';
                        outputDirPath = string.Join(x, fileSplitList);
                    }
                    else
                    { //if its in the root folder of a drive
                        outputDirPath = fileSplit[0];
                    }
                    tbOutputPath.Text = outputDirPath;
                }
                else
                { //handling for a file
                    string[] fileSplit = file.Split('\\');
                    string outputDirPath;
                    List<string> fileSplitList = fileSplit.ToList();
                    fileSplitList.RemoveAt(fileSplit.Length - 1);
                    char x = '\\';
                    outputDirPath = string.Join(x, fileSplitList);
                    tbOutputPath.Text = outputDirPath;
                }
            }

        }

        private void btnClearSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            //removes items selected in filesList
            var selected = lvListFiles.SelectedItems.Cast<Object>().ToArray();
            foreach (var eachItem in selected)
            {
                lvListFiles.Items.Remove(eachItem);
            }
        }

        private void btnClearAllFiles_Click(object sender, RoutedEventArgs e)
        {
                lvListFiles.Items.Clear();
        }

        private void btnFindLocalPathDirectory_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == true)
            {
                tbOutputPath.Text = openFolderDialog.SelectedPath;
                outputPathManuallySet = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void tbSave_Click(object sender, RoutedEventArgs e)
        {
            List<String> fileList = lvListFiles.Items.Cast<String>().ToList();
            book = new DataObjects.Audiobook(tbName.Text, tbOutputName.Text, tbArtist.Text, tbAlbum.Text, tbGenre.Text, tbYear.Text, tbWriter.Text, fileList, tbOutputPath.Text);
            this.Close();
        }
    }
}
