using ProcessAudiobooks_UI.CustomControls;
using System;
using System.Collections.Generic;
using System.Text;
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

            }
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
