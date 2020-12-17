using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using ProcessAudiobooks_UI.Properties;
using System.Windows.Forms;
using Ookii.Dialogs.Wpf;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;

namespace ProcessAudiobooks_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        public int x;
        public ssh sshClient = null;

        public AudiobookProcesser processor;
        public MainWindow()
        {
            InitializeComponent();
            ConsoleWindow consoleObs = new ConsoleWindow();
            try
            {
                if (!Settings.Default.sshConnectDetails.Equals(""))
                {
                    sshClient = JsonConvert.DeserializeObject<ssh>(Settings.Default.sshConnectDetails);
                    
                    tbSshIP.Text = sshClient.ip;
                    tbSshPort.Text = sshClient.port.ToString();
                    tbSshUsername.Text = sshClient.username;
                    pbSshPassword.Password = sshClient.password;
                }
            } catch (System.NullReferenceException)
            {
                MessageBox.Show("Invalid ssh information");
            }
            tbCommand.Text = Settings.Default.remoteCommand;
            tbLocalPath.Text = Settings.Default.localPath;
            tbRemotePath.Text = Settings.Default.remotePath;

            try //catch any errors for this new setting in case it doesnt exist yet
            {
                tbOutputDirOverride.Text = Settings.Default.overrideOutputDirectory;
                cbOverrideOutputDir.IsChecked = Settings.Default.overrideOutputDirectoryBool;
            }
            catch { }
            processor = new AudiobookProcesser(tbCommand, tbLocalPath, tbRemotePath, eLvAudiobook, btnStartCreateAudiobooks, btnStopCreateAudiobooks);
        }

        private void btnFindLocalPathDirectory_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == true)
                tbLocalPath.Text = openFolderDialog.SelectedPath;
        }

        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            DataObjects.Audiobook audiobook = null;
            AddAudiobookWindow audiobookWindow;
            bool looped = false;
            string outputDirOverride = "";
            if ((bool)cbOverrideOutputDir.IsChecked)
            {
                outputDirOverride = tbOutputDirOverride.Text;
            }
            do
            {
                if (!looped)
                {
                    audiobookWindow = new AddAudiobookWindow(outputDirOverride);
                }
                else
                {
                    audiobookWindow = new AddAudiobookWindow(audiobook, outputDirOverride);
                }
                audiobookWindow.ShowDialog(); //used show dialog to keep the window open for an extended period of time
                if (audiobookWindow.book == null) //if we never set a audiobook we know it was never configured therefore just stop trying to add it to the listview
                    return;
                audiobook = audiobookWindow.book;

                //Handle empty windows
                if (audiobook.Name.Equals(""))
                {
                    System.Windows.MessageBox.Show("Please set name to something");
                }
                if (audiobook.outputName.Equals(""))
                {
                    System.Windows.MessageBox.Show("Please set Output Name to something");
                }
                looped = true;
            } while (audiobook.Name.Equals("") || audiobook.outputName.Equals(""));
            eLvAudiobook.Items.Add(audiobook);
        }

        private void btnEditBook_Click(object sender, RoutedEventArgs e)
        {
            DataObjects.Audiobook audiobook = (DataObjects.Audiobook)eLvAudiobook.SelectedItem; ;
            AddAudiobookWindow audiobookWindow;
            if (audiobook == null) //fixes crash if we try to edit without selecting something
                return;
            do
            {
                audiobookWindow = new AddAudiobookWindow(audiobook);
                audiobookWindow.ShowDialog(); //used show dialog to keep the window open for an extended perikod of time
                eLvAudiobook.Items.Remove(audiobook);
                audiobook = audiobookWindow.book;

                //Handle empty windows
                if (audiobook.Name.Equals(""))
                {
                    System.Windows.MessageBox.Show("Please set name to something");
                }
                if (audiobook.outputName.Equals(""))
                {
                    System.Windows.MessageBox.Show("Please set Output Name to something");
                }
            } while (audiobook.Name.Equals("") || audiobook.outputName.Equals(""));
            
            eLvAudiobook.Items.Add(audiobook);
        }

        private void btnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            //removes items selected in Audiobook List
            var selected = eLvAudiobook.SelectedItems.Cast<Object>().ToArray();
            foreach (var eachItem in selected)
            {
                eLvAudiobook.Items.Remove(eachItem);
            }
        }

        private async void btnSshTestConnection_Click(object sender, RoutedEventArgs e)
        {
            string ip, username, password;
            int port;

            if (!int.TryParse(tbSshPort.Text, out port))
            {
                System.Windows.MessageBox.Show("Please Specify a integer for the port!");
                return;
            }

            ip = tbSshIP.Text;
            username = tbSshUsername.Text;
            password = pbSshPassword.Password;
            try
            {
                sshClient = new ssh(ip, port, username, password);
                await sshClient.sshStartTest();
            }
            catch
            {
                sshClient = null;
                return;
            }
            System.Windows.MessageBox.Show("Connection Sucessful");

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.sshConnectDetails = JsonConvert.SerializeObject(sshClient);
            Settings.Default.remoteCommand = tbCommand.Text;
            Settings.Default.localPath = tbLocalPath.Text;
            Settings.Default.remotePath = tbRemotePath.Text;
            Settings.Default.overrideOutputDirectory = tbOutputDirOverride.Text;
            Settings.Default.overrideOutputDirectoryBool = (bool)cbOverrideOutputDir.IsChecked;
            Settings.Default.Save();
        }

        private void btnStartCreateAudiobooks_Click(object sender, RoutedEventArgs e)
        {
            var tsk =  processor.StartProcess(sshClient);
        }



        private void btnStopCreateAudiobooks_Click(object sender, RoutedEventArgs e)
        {
            processor.StopProcess();
        }

        private void btnOutputDirPath_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == true)
                tbOutputDirOverride.Text = openFolderDialog.SelectedPath;
        }
    }
}
