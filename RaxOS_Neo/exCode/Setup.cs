using Cosmos.Kernel.System.IO;
using Cosmos.Kernel.System.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RaxOS_Neo.Crypt;
using System.Text;
using static RaxOS_Neo.Kernel;
using IOP = System.IO.Path;
using Sys = Cosmos.Kernel.System;
 

namespace RaxOS_Neo
{
    public static class exCode
    {
        public static string LimpiarNombre(string nombre)
        {
            // Obtiene los caracteres inválidos para nombres de archivo
            char[] charsInvalidos = IOP.GetInvalidFileNameChars();

            // Elimina todos esos caracteres del nombre
            var nombreLimpio = new string(nombre.Where(c => !charsInvalidos.Contains(c)).ToArray());

            return nombreLimpio;
        }

        private static int GraphicalSetup()
        {
            Canvas canvas;
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
            canvas.Clear(System.Drawing.Color.Green);

            string System_cs = @"0:\RaxOS\SYSTEM\System.cs";
            string Kernel_dll = @"0:\RaxOS\SYSTEM\Kernel.dll";
            string Sysinfo_inf = @"0:\RaxOS\SYSTEM\sysinfo.inf";
            void Write(string text, int x, int y)
            {
                canvas.DrawString(text, Sys.Graphics.Fonts.PCScreenFont.DefaultFont, System.Drawing.Color.White, x, y);
            }
            Write("Welcome to RaxOS Neo Installer!", 10, 10);
            canvas.Display();

            // FIXME: VFS operations commented out
            Write("Creating 0:\\RaxOS\\SYSTEM...", 10, 30);
            canvas.Display();
            
            Write("Creating System.cs...", 10, 50);
            canvas.Display();
            File.WriteAllText(System_cs, "");
                
            Write("Creating Kernel.dll...", 10, 70);
            canvas.Display();
            File.WriteAllText(Kernel_dll, "");
                
            Write("Creating 0:\\RaxOS\\SYSTEM\\sysinfo.inf...", 10, 90);
            canvas.Display();
            File.WriteAllText(Sysinfo_inf,
                "{\n" +
                "  \"installed\": true,\n" +
                "  \"channel\": \"Neo\",\n" +
                "  \"version\": \"0.1\",\n" +
                "  \"build\": 1,\n" +
                "  \"mode\": \"text\"\n" +
                "}"
            );

            Write("You will be redirected to the registration screen.", 10, 110);
            canvas.Display();
            System.Threading.Thread.Sleep(2000);
            canvas.Disable();
            RaxOS_Neo.GUI.Screens.Register.Display();
            return 0;
        }

        public static int Setup(bool graphic = false)
        {
            if (graphic) { return GraphicalSetup(); }

            if (!File.Exists("0:\\RaxOS\\SYSTEM\\System.cs"))
            {
                Console.WriteLine("Welcome to RAXOS Neo Installer");
                Console.WriteLine("VFS operations are commented out - waiting for driver implementation");
                return 0;
            }
            return 0;
        }

        public static void CheckAndDeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void CheckAndDeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static void CheckAndCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string HashPassword(string input)
        {
            return input;
        }
    }
}
