global using Path = RaxOS_Neo.Kernel.Path;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using static RaxOS_Neo.Kernel;
using IOP = System.IO.Path;
using Sys = Cosmos.System;


namespace RaxOS_BETA
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
        private static void GraphicalSetup()         {
            // Aquí iría el código para la instalación gráfica
            // Por ahora, solo se muestra un mensaje de ejemplo
            Console.WriteLine("Instalador gráfico no implementado aún.");
            return;
            // Instalador gráfico
            Sys.Graphics.Canvas canvas = Sys.Graphics.FullScreenCanvas.GetFullScreenCanvas(new Sys.Graphics.Mode(800, 600, Sys.Graphics.ColorDepth.ColorDepth32));
            CosmosVFS fs = new CosmosVFS();
            canvas.Clear(System.Drawing.Color.Green);
            
            string System_cs = @"0:\RaxOS\System\System.cs";
            string Kernel_dll = @"0:\RaxOS\System\Kernel.dll";
            string Sysinfo_inf = @"0:\RaxOS\System\sysinfo.inf";
            string Users_db = @"0:\RaxOS\System\users.db";
            void Write(string text, int x, int y)
            {
                canvas.DrawString(text, Sys.Graphics.Fonts.PCScreenFont.Default, System.Drawing.Color.White, x, y);
            }
            if (!File.Exists(System_cs))
            {
                Write("Welcome to RaxOS Neo Installer!", 10, 10);
                canvas.Display();
                Directory.CreateDirectory(getPath(Path.SystemFolder));
                Write("Creating 0:\\RaxOS\\SYSTEM...", 10, 30);
                canvas.Display();
                File.WriteAllText(getPath(Path.SystemFolder) + "\\System.cs", "");
                Write("Creating 0:\\RaxOS\\SYSTEM\\System.cs...", 10, 50);
                canvas.Display();
                File.WriteAllText(getPath(Path.SystemFolder) + "\\Kernel.dll", "");
                Write("Creating 0:\\RaxOS\\SYSTEM\\Kernel.dll...", 10, 70);
                canvas.Display();
                File.WriteAllText(getPath(Path.SystemFolder) + "\\sysinfo.inf", "" +
                    "[SYSINFO]\n" +/*0*/
                    "Installed = true\n" +/*1*/
                    "Userspecified = true\n" +//2
                    "Passwordspecified = true\n" +//3
                    "RaxOS_Channel = {\n" +//4
                    "Neo\n" +//5
                    "}\n" +//6
                    "RaxOS_Version = {\n" +//7
                    "0.1\n" +//8
                    "}");//9
                Write("Creating 0:\\RaxOS\\SYSTEM\\sysinfo.inf...", 10, 90);
                canvas.Display();
                Write("You will be redirected to the registration screen.", 10, 110);
                canvas.Display();
                System.Threading.Thread.Sleep(2000);
            }
        }
        public static void Setup(CosmosVFS fs, bool graphic = false)
        {
            if (graphic) { GraphicalSetup(); return; }

            if (!File.Exists("0:\\RaxOS\\SYSTEM\\System.cs"))
            {
                // PRÓXIMO: Crear el instalador pero gráfico
                // FUTURO: Crear un instalador con GRUB


                Console.WriteLine("Welcome to RAXOS Neo Installer");

                /* var volumes = fs.GetVolumes();

                 Console.WriteLine("Volúmenes detectados: " + volumes.Count);
                 foreach (var vol in volumes)
                 {
                     Console.WriteLine("Nombre: " + vol.mName);
                 }*/

                try
                {
                    File.WriteAllText("0:\\test.txt", "Hola");
                    Console.WriteLine("Archivo creado.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error escribiendo archivo: " + ex.Message);
                }

                fs.CreateDirectory("0:\\RaxOS\\SYSTEM\\");
                Console.WriteLine("Creating 0:\\RaxOS\\SYSTEM...");
                File.WriteAllText(getPath(Path.SystemFolder) + "\\System.cs", "");
                Console.WriteLine("Creating 0:\\RaxOS\\SYSTEM\\System.cs...");
                File.WriteAllText(getPath(Path.SystemFolder) + "\\Kernel.dll", "");
                Console.WriteLine("Creating 0:\\RaxOS\\SYSTEM\\Kernel.dll...");
                File.WriteAllText(getPath(Path.SystemFolder) + "\\sysinfo.inf", "" +
                    "[SYSINFO]\n" +/*0*/
                    "Installed = true\n" +/*1*/
                    "Userspecified = true\n" +//2
                    "Passwordspecified = true\n" +//3
                    "RaxOS_Channel = {\n" +//4
                    "Neo\n" +//5
                    "}\n" +//6
                    "RaxOS_Version = {\n" +//7
                    "0.1\n" +//8
                    "}");//9
                Console.WriteLine("Creating 0:\\RaxOS\\SYSTEM\\sysinfo.inf...");
                Console.Write("Please enter username:");
                string usr = Console.ReadLine();
                Console.Write("Please enter password:");
                string pss = Console.ReadLine();
                Console.Clear();
                string[] usrpss = { usr, pss };
                Console.WriteLine("Users.db is going to be created");

                var ruta = IOP.Combine(getPath(Path.SystemFolder), "users.db");
                Console.WriteLine("Path to be created: " + ruta);

                try
                {
                    File.WriteAllLines(ruta, usrpss);
                    Console.WriteLine("Users.db created successfully.");
                }
                catch
                {
                    Console.WriteLine("ERROR writing file");
                }

                //Console.WriteLine("Leido: " + (usr == null ? "NULL" : usr));
                string cleanUser = "default";
                if (usr == null)
                {
                    //Console.WriteLine("usr es NULL");
                }
                else
                {
                    cleanUser = LimpiarNombre(usr);
                    //Console.WriteLine("cleanUser: " + cleanUser);
                }
                
                var systemPath = getPath(Path.SystemFolder);
                if (!File.Exists(systemPath))
                {
                    //Console.WriteLine("System folder no existe: " + systemPath);
                }

                if (!Directory.Exists(getPath(Path.UserDir)))
                {
                    Directory.CreateDirectory(getPath(Path.UserDir));
                }

                if (!Directory.Exists(IOP.Combine(getPath(Path.UserDir), cleanUser)))
                {
                    Directory.CreateDirectory(IOP.Combine(getPath(Path.UserDir), cleanUser));
                }
                
                //Console.WriteLine(2);

                if (Directory.Exists("0:\\Dir Testing\\"))
                {
                    Directory.Delete("0:\\Dir Testing\\", true);
                }

                Console.WriteLine("Deleting cache...");
                if (Directory.Exists("0:\\TEST\\"))
                {
                    Directory.Delete("0:\\TEST\\", true);
                }
                Console.WriteLine("Deleting Setup Data...");
                if (File.Exists("0:\\Kudzu.txt"))
                {
                    File.Delete("0:\\Kudzu.txt");
                }
                Console.WriteLine("Deleting logs...");
                if (File.Exists("0:\\Root.txt"))
                {
                    File.Delete("0:\\Root.txt");
                }
                Console.WriteLine("Deleting raxos_setup");
                if (!Directory.Exists(IOP.Combine(getPath(Path.UserDir), $"{cleanUser}", "Documents\\")))
                {
                    Directory.CreateDirectory(IOP.Combine(getPath(Path.UserDir), $"{cleanUser}", "Documents\\"));
                }
                
                Console.WriteLine("Creating DOCS Folder...");
                Console.WriteLine("Press any key to reboot");
                Console.ReadKey();
                Sys.Power.Reboot();
            }
            else {}
        }
    }
}
