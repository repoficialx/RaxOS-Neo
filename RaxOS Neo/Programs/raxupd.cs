using RaxOS_BETA.Programs.ProgramHelper;
using System.IO;
using c = System.Console;

namespace RaxOS_BETA.Programs
{
    internal class RaxUPD : Program
    {
        public static void Launch()
        {
            mv = 1;
            AppName = "RaxUPD";
            sv = 4;
            cv = 0;
            rv = 0;
            AppDescription = "RaxOS Updater (RaxUPD)";
            IsStable = true;
            MainLoop();
        }

        private static void MainLoop()
        {
            c.WriteLine($"{AppName} v{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}");
            c.Write("Press any key to continue");
            c.ReadKey();
            c.Clear();
        CLI:c.Write("Type raxupd clicommand: ");
#nullable enable
            string? cli = c.ReadLine();
#nullable disable
            if (cli == "" || cli == null)
            {
                c.WriteLine("Please type a file.");
                goto CLI;
            }
            else if (cli == "--check")
            {
                CheckUpdates();
            }
            else
            {
                    
            }
            c.WriteLine("Press any key to exit.");
            c.ReadKey();
            Close();
        }
        private static void Close()
        {
            c.Clear();
        }
        private static void CheckUpdates()
        {
            string[] SYSINFO = File.ReadAllLines("0:\\SYSTEM\\sysinfo.inf");
            string currver = SYSINFO[6];
            string lastver = Kernel.LastVersion;
            string KRNLLastver = Kernel.LastVersion;
            c.WriteLine("Reading SysInfo...");
            if (SYSINFO[6] == lastver)
            {
                c.WriteLine("Your PC is Updated!");
                Close();
            }
            else
            {
                c.Write("Please update now! Type Y to update (THIS WILL ERASE ALL YOUR DATA!!).");
                System.ConsoleKeyInfo key = c.ReadKey();
                char letter = key.KeyChar;
                if (letter == 'Y')
                {
                    SystemReserved.DONOTENTER.Format();
                }
            }
        }

    }
}
