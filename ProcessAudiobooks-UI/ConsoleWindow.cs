using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessAudiobooks_UI
{
    class ConsoleWindow
    {
        public enum DEBUG_LEVELS
        {
            INFO = 10,
            WARNING = 20,
            ERROR = 30,
            CRITICAL = 40,
            DEBUG = 50,
        }

        private int debugLevel = (int)DEBUG_LEVELS.DEBUG;

        private static string PREFIX = "[ProcessAudioBooks-UI]: ";

        public ConsoleWindow()
        {
            AllocConsole();
            this.WriteDebug(" Started");
        }


        private void _Write(String msg)
        {
            Console.WriteLine(ConsoleWindow.PREFIX + msg);
        }

        public void WriteInfo(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.INFO)
            {
                this._Write("[INFO] " + msg);
            }
        }

        public void WriteWarning(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.WARNING)
            {
                this._Write("[WARNING] " + msg);
            }
        }

        public void WriteError(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.ERROR)
            {
                this._Write("[ERROR] " + msg);
            }
        }

        public void WriteCritical(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.CRITICAL)
            {
                this._Write("[CRITICAL] " + msg);
            }
        }

        public void WriteDebug(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.DEBUG)
            {
                this._Write("[DEBUG] " + msg);
            }
        }

        //these 2 imports allow us to create a console window in a wpf application.
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
    }


}