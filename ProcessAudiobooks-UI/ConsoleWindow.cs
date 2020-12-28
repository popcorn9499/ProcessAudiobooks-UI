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

        private static int debugLevel = (int)DEBUG_LEVELS.INFO; //NOTE. potentially make this not hard coded.

        private static string PREFIX = "[ProcessAudioBooks-UI]: ";

        public ConsoleWindow()
        {
            AllocConsole(); //load the console window which we are writing to.
            ConsoleWindow.WriteDebug(" Started");
        }


        private static void _Write(String msg)
        {
            Console.WriteLine(ConsoleWindow.PREFIX + msg);
        }

        public static void WriteInfo(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.INFO)
            {
                ConsoleWindow._Write("[INFO] " + msg);
            }
        }

        public static void WriteWarning(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.WARNING)
            {
                ConsoleWindow._Write("[WARNING] " + msg);
            }
        }

        public static void WriteError(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.ERROR)
            {
                ConsoleWindow._Write("[ERROR] " + msg);
            }
        }

        public static void WriteCritical(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.CRITICAL)
            {
                ConsoleWindow._Write("[CRITICAL] " + msg);
            }
        }

        public static void WriteDebug(string msg)
        {
            if (debugLevel >= (int)ConsoleWindow.DEBUG_LEVELS.DEBUG)
            {
                ConsoleWindow._Write("[DEBUG] " + msg);
            }
        }

        //these 2 imports allow us to create a console window in a wpf application.
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
    }


}