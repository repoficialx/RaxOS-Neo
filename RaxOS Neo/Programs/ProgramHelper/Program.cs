using System;
using System.Resources;
using c = System.Console;

namespace RaxOS_BETA.Programs.ProgramHelper
{
    internal partial class Program
    {
        public static string AppName;


        public static int mv;
        public static int sv;
        public static int cv;
        public static int rv;
        public static readonly string MainVersion = mv.ToString();
        public static readonly string SecondaryVersion = sv.ToString();
        public static readonly string CompilationVersion = cv.ToString();
        public static readonly string Revision = rv.ToString();
        public static readonly int[] Version =
        {
            int.Parse(MainVersion),
            int.Parse(SecondaryVersion),
            int.Parse(CompilationVersion),
            int.Parse(Revision)
        };


        public static string AppDescription;
        public static bool IsStable; //Change this to false when in Alpha or Beta 

        private static void Launch()
        {

            MainLoop();

        }

        private static void MainLoop()
        {

            c.WriteLine($"{0} v{1}.{2}.{3}.{4}", AppName, Version[0], Version[1], Version[2], Version[3]);
            c.ReadKey();
            c.Write("Press any key to continue");
            c.ReadKey();
            Close();
            
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
