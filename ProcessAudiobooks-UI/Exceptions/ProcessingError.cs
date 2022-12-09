using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAudiobooks_UI.Exceptions
{
    public class ProcessingError : Exception
    {
        public ProcessingError() { }

        public String toString()
        {
            return "Processing error occured";
        }
    }
}
