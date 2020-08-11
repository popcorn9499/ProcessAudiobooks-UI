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

namespace ProcessAudiobooks_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        public int x;
        public MainWindow()
        {
            InitializeComponent();
            ConsoleWindow consoleObs = new ConsoleWindow();
        }

        private void btnFindLocalPathDirectory_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == true)
                tbLocalPath.Text = openFolderDialog.SelectedPath;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            AddAudiobookWindow x = new AddAudiobookWindow();

            x.Show();
        }

        private void btnEditBook_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteBook_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
