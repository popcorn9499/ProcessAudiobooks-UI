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
        public DataObjects.Audiobook book = null; //Create a space to store the book information we are gathering

        //create a empty audiobook window for the user to add information to
        public AddAudiobookWindow(string overrideDir = "")
        {
            InitializeComponent();
            if (!overrideDir.Equals(""))
            {
                tbOutputPath.Text = overrideDir;
            }
        }


        //create a prefilled audiobook window for the user to add more information to/fix any mistakes
        public AddAudiobookWindow(DataObjects.Audiobook audiobook, string overrideDir = "")
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
            if (!overrideDir.Equals(""))
            {
                tbOutputPath.Text = overrideDir;
            }

            //go through the list adding all the items to the listview object
            foreach (String data in audiobook.FileList)
            {
                lvListFiles.Items.Add(data);
            }
        }


        //handle dragging files to the window to add to the list of files to process.
        //add by drag and dropping a file support
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
            if (tbOutputPath.Text.Equals(""))
            {   //declaring the variables used in this conditional statement
                string[] fileSplit;
                string outputDirPath;
                List<string> fileSplitList;
                char dirSeperator = '\\';

                string file = lvListFiles.Items[0].ToString();

                fileSplit = file.Split('\\');
                if (fileSplit.Length > 1)
                {
                    fileSplitList = fileSplit.ToList();//convert the array to a list
                    fileSplitList.RemoveAt(fileSplit.Length - 1);//remove the last element from the list... Yes I could do this without a list however I was incredibly lazy and this seemed quicker to write physically.
                    outputDirPath = string.Join(dirSeperator, fileSplitList); 
                }
                else
                { //if its in the root folder of a drive
                    outputDirPath = fileSplit[0];
                }
                tbOutputPath.Text = outputDirPath;
            }

        }

        //removes items selected in filesList
        private void btnClearSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvListFiles.SelectedItems.Cast<Object>().ToArray();
            foreach (var eachItem in selected)
            {
                lvListFiles.Items.Remove(eachItem);
            }
        }

        //remove all items in the fileList  
        private void btnClearAllFiles_Click(object sender, RoutedEventArgs e)
        {
            lvListFiles.Items.Clear();
        }

        //bring up a dialog box so the user can manually select the directory to output to.
        private void btnFindLocalPathDirectory_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == true)
            {
                tbOutputPath.Text = openFolderDialog.SelectedPath;
            }
        }


        //On window close give the user a chance to realize his or her mistake and cancel the closing operation
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to cancel?", "ProcessAudioBook Prompt", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                e.Cancel = true;
        }

        //Create the new audiobook object and close the window.
        //The audiobook object is to be collected later on by the program
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            List<String> fileList = lvListFiles.Items.Cast<String>().ToList();
            String outputName = tbOutputName.Text;
            if (!outputName.Contains(".m4b")) //add the file extension if its missing
                outputName = outputName + ".m4b";
            book = new DataObjects.Audiobook(tbName.Text, outputName , tbArtist.Text, tbAlbum.Text, tbGenre.Text, tbYear.Text, tbWriter.Text, fileList, tbOutputPath.Text);
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {   
            //this function is to cancel out of the window and do something else in the program
            //On window close give the user a chance to realize his or her mistake and cancel the closing operation
            MessageBoxResult result = MessageBox.Show("Do you really want to cancel?", "ProcessAudioBook Prompt", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                this.Close();
        }
    }
}
