global using Path = RaxOS_Neo.Kernel.Path;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static void Setup()
        {

            CosmosVFS fs = new CosmosVFS();

            if (!File.Exists("0:\\RaxOS\\SYSTEM\\System.cs"))
            {
                //Custom Installer (Now you see why

                Console.WriteLine("Welcome to RAXOS Installer");
                Directory.CreateDirectory(getPath(Path.SystemFolder));
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
