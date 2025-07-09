using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.IO;
using Sys = Cosmos.System;
using static RaxOS_Neo.Kernel;

namespace RaxOS_BETA
{
    public static class exCode
    {
        public static void Setup()
        {
            CosmosVFS fs = new CosmosVFS();
            VFSManager.RegisterVFS(fs);

            if (!File.Exists("0:\\SYSTEM\\System.cs"))
            {
                //Custom Installer (Now you see why

                Console.WriteLine("Welcome to RAXOS Installer");
                Directory.CreateDirectory(Paths[Pth.SystemFolder]);
                Console.WriteLine("Creating 0:\\SYSTEM...");
                File.WriteAllText(Paths[Pth.SystemFolder] + "\\System.cs", "");
                Console.WriteLine("Creating 0:\\SYSTEM\\System.cs...");
                File.WriteAllText(Paths[Pth.SystemFolder] + "\\Kernel.dll", "");
                Console.WriteLine("Creating 0:\\SYSTEM\\Kernel.dll...");
                File.WriteAllText(Paths[Pth.SystemFolder] + "\\sysinfo.inf", "" +
                    "[SYSINFO]\n" +/*0*/
                    "Installed = true\n" +/*1*/
                    "Userspecified = true\n" +//2
                    "Passwordspecified = true\n" +//3
                    "RaxOS_Channel = {\n" +//4
                    "Beta\n" +//5
                    "}\n" +//6
                    "RaxOS_Version = {\n" +//7
                    "0.0.5\n" +//8
                    "}");//9
                Console.WriteLine("Creating 0:\\RaxOS\\SYSTEM\\sysinfo.inf...");
                Console.Write("Please enter username:");
                string usr = Console.ReadLine();
                Console.Write("Please enter password:");
                string pss = Console.ReadLine();
                Console.Clear();
                string[] usrpss = { usr, pss };
                File.WriteAllLines(Paths[Pth.SystemFolder] + "\\users.db", usrpss);
                Console.WriteLine("Creating users.db...");
                Directory.CreateDirectory(Paths[Pth.UserDir] + $@"\{usr}");
                Directory.Delete("0:\\Dir Testing\\", true);
                Console.WriteLine("Deleting cache...");
                Directory.Delete("0:\\TEST\\", true);
                Console.WriteLine("Deleting Setup Data...");
                File.Delete("0:\\Kudzu.txt");
                Console.WriteLine("Deleting logs...");
                File.Delete("0:\\Root.txt");
                Console.WriteLine("Deleting raxos_setup");
                fs.CreateDirectory(Paths[Pth.UserDir] + $"\\{usr}\\Documents\\");
                Console.WriteLine("Creating DOCS Folder...");
                Console.WriteLine("Press any key to reboot");
                Console.ReadKey();
                Sys.Power.Reboot();
            }
            else {}
        }
    }
}
