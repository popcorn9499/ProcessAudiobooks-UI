using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAudiobooks_UI.Exceptions
{
    public class ProcessingCleanupError : Exception
    {
        public ProcessingCleanupError() { }

        public String toString()
        {
            return "Processing cleanup error occured";
        }
    }
}
