using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static RaxOS_Neo.Kernel;
using IOP = System.IO.Path;
using Sys = Cosmos.System;


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
        private static void GraphicalSetup()
        {
            Canvas canvas;
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
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

                Write("Creating 0:\\RaxOS\\SYSTEM...", 10, 30);
                canvas.Display();
                Directory.CreateDirectory(getPath(Path.SystemFolder));
                
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
                "}");

                Write("You will be redirected to the registration screen.", 10, 110);
                canvas.Display();
                System.Threading.Thread.Sleep(2000);
                canvas.Disable();
                RaxOS_Neo.GUI.Screens.Register.Display();
            }
        }
        public static void Setup(CosmosVFS fs, bool graphic = false)
        {
            if (graphic) { GraphicalSetup(); return; }

            if (!File.Exists("0:\\RaxOS\\SYSTEM\\System.cs"))
            {
                Console.WriteLine("Welcome to RAXOS Neo Installer");

                Console.WriteLine("Creating 0:\\RaxOS\\SYSTEM...");
                fs.CreateDirectory("0:\\RaxOS\\SYSTEM\\");

                Console.WriteLine("Creating System.cs...");
                File.WriteAllText(getPath(Path.SystemFolder) + "\\System.cs", "");

                Console.WriteLine("Creating Kernel.dll...");
                File.WriteAllText(getPath(Path.SystemFolder) + "\\Kernel.dll", "");
                
                Console.WriteLine("Creating sysinfo.inf...");
                File.WriteAllText(getPath(Path.SystemFolder) + "\\sysinfo.inf",
                    "{\n" +
                    "  \"installed\": true,\n" +
                    "  \"channel\": \"Neo\",\n" +
                    "  \"version\": \"0.1\",\n" +
                    "  \"build\": 1,\n" +
                    "  \"mode\": \"text\"\n" +
                "}");
                
                Console.Write("Please enter username: ");
                string usr = Console.ReadLine();

                Console.Write("Please enter password: ");
                string pss = Console.ReadLine();

                Console.Clear();

                string cleanUser = "default";
                if (!string.IsNullOrWhiteSpace(usr))
                    cleanUser = LimpiarNombre(usr);

                string hash = HashPassword(pss ?? "");

                var usersDbPath = IOP.Combine(getPath(Path.SystemFolder), "users.db");

                string[] usersDb =
                {
                    $"username={cleanUser}",
                    $"password_hash={hash}",
                    $"hash_algo=SHA256",
                    $"created={DateTime.Now:yyyy-MM-dd}"
                };

                
                File.WriteAllLines(usersDbPath, usersDb);

                CheckAndCreateDirectory(getPath(Path.UserDir));
                CheckAndCreateDirectory(IOP.Combine(getPath(Path.UserDir), cleanUser));
                CheckAndCreateDirectory(
                    IOP.Combine(getPath(Path.UserDir), cleanUser, "Documents\\")
                );

                Console.WriteLine("Cleaning setup leftovers...");
                CheckAndDeleteDirectory("0:\\Dir Testing\\");
                CheckAndDeleteDirectory("0:\\TEST\\");
                CheckAndDeleteFile("0:\\Kudzu.txt");
                CheckAndDeleteFile("0:\\Root.txt");

                Console.WriteLine("Setup complete.");
                Console.WriteLine("Press any key to reboot");
                Console.ReadKey();
                Sys.Power.Reboot();
            }
        }
        /*
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
                 }

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
                    "[SYSINFO]\n" +/*0
                    "Installed = true\n" +/*1
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
                string cleanUser = "default";
                if (usr != null){cleanUser = LimpiarNombre(usr);}
                CheckAndCreateDirectory(getPath(Path.UserDir));
                CheckAndCreateDirectory(IOP.Combine(getPath(Path.UserDir), cleanUser));
                Console.WriteLine("Deleting setup data...");
                CheckAndDeleteDirectory("0:\\Dir Testing\\");
                CheckAndDeleteDirectory("0:\\TEST\\");
                CheckAndDeleteFile("0:\\Kudzu.txt");
                CheckAndDeleteFile("0:\\Root.txt");
                Console.WriteLine("Deleting raxos_setup");
                Console.WriteLine("Creating Documents folder...");
                CheckAndCreateDirectory(IOP.Combine(getPath(Path.UserDir), $"{cleanUser}", "Documents\\"));
                Console.WriteLine("Press any key to reboot");
                Console.ReadKey();
                Sys.Power.Reboot();
            }
        }*/
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
            using var sha = SHA256.Create();
            return Convert.ToHexString(
                sha.ComputeHash(Encoding.UTF8.GetBytes(input))
            );
        }

    }
}