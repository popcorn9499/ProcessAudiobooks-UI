using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAudiobooks_UI.DataObjects
{
    public enum AudiobookProcessingStatus
    { //An easy way to keep track of my audiobook status
        Ready = 0,
        Processing = 1,
        Completed = 2,
        Error = 3,
        CleanupError = 4
    }
}
