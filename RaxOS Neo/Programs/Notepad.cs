using RaxOS_BETA.Programs.ProgramHelper;
using System;
using System.ComponentModel.DataAnnotations;
using c = System.Console;


namespace RaxOS_BETA.Programs
{
    internal class notepad : Program
    { 
        public static void Launch()
        {
            IsStable = Boolean.Parse("false");
             mv = 0;
            sv = 1;
            cv = 0;
            rv = 1;
            AppName = "Notepad";
            AppDescription = "Notepad for notes :)";
            MainLoop();

        }

        private static void MainLoop()
        {

            c.WriteLine($"{AppName} v{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}");
            c.ReadKey();
            c.Write("Press any key to continue");
            c.ReadKey();
            Close();
            c.Write("Type doc path [0:\\myfolder\\myfile.txt:");
#nullable enable
            string? docname = c.ReadLine();
#nullable disable
            c.WriteLine($"Document created as {docname}");
            c.WriteLine($"Start to type {docname} content. When finish, press enter.");
            ConsoleKeyInfo k = c.ReadKey();
            if (k.Key == ConsoleKey.Enter)
            {
                Close();
            }
        }

        private static void Close()
        {

            c.Clear();

        }
    }
}
