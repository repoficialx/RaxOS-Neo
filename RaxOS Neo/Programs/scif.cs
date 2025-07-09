using Cosmos.System;
using RaxOS_BETA.Programs.ProgramHelper;
using System.IO;
using System.Runtime.CompilerServices;
using c = System.Console;

namespace RaxOS_BETA.Programs
{
    internal class scif:Program
    {
        public static void Launch()
        {
            mv = 43;
            AppName = "scif";
            sv = 184; 
            cv = 22965; 
            rv = 1000;
            AppDescription = "Simple Content Information of Files (SCIF).";
            IsStable = true;
            MainLoop();
        }

        private static void MainLoop()
        {
            c.WriteLine($"{AppName} v{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}");
            c.ReadKey();
            c.Write("Press any key to continue");
            c.ReadKey();
            FILEPATH:
            c.Clear();
            c.Write("Type file path:");
#nullable enable
            string? path = c.ReadLine();
#nullable disable
            if (path==""||path==null)
            {
                c.WriteLine("Please type a file.");
                goto FILEPATH;
            }
            string[] path_contents1 = File.ReadAllLines(path:path);
            string path_contents = path_contents1.ToString();
            c.WriteLine(path_contents);
            c.WriteLine("Press any key to exit.");
            c.ReadKey();
            Close();
        }
        private static void Close()
        {
            c.Clear();
           
        }

    }
}
