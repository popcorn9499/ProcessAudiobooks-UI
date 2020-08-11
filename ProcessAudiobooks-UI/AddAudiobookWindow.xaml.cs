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

namespace ProcessAudiobooks_UI
{
    /// <summary>
    /// Interaction logic for AddAudiobookWindow.xaml
    /// </summary>
    public partial class AddAudiobookWindow : Window
    {
        public AddAudiobookWindow()
        {
            InitializeComponent();
        }

        private void listFiles_Drop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                droppedFiles = e.Data.GetData(DataFormats.FileDrop, true) as string[];
            }

            if ((null == droppedFiles) || (!droppedFiles.Any())) { return; }

            listFiles.Items.Clear();

            foreach (string s in droppedFiles)
            {
                listFiles.Items.Add(s);
            }
        }
    }
}
