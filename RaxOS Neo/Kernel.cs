using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.ScanMaps;
using RaxOS_BETA.Programs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using K = Cosmos.System.KeyboardManager;

namespace RaxOS_Neo
{
    public class Kernel : Sys.Kernel
    {
        public enum Path
        {
            SystemFolder,
            UserDir
        }
        public string getPath(Path path) => path switch
        {
            Path.SystemFolder => "0:\\USER",
            Path.UserDir => "0:\\SYSTEM\\",
            _ => throw new ArgumentOutOfRangeException(nameof(path), $"Not expected direction value: {path}")
        };
        protected override void BeforeRun()
        {
            Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            if (!File.Exists("0:\\SYSTEM\\System.cs"))
            {
                //Custom Installer (Now you see why

                Console.WriteLine("Welcome to RAXOS Installer");
                Directory.CreateDirectory("0:\\SYSTEM\\");
                Console.WriteLine("Creating 0:\\SYSTEM...");
                File.WriteAllText("0:\\SYSTEM\\System.cs", "");
                Console.WriteLine("Creating 0:\\SYSTEM\\System.cs...");
                File.WriteAllText("0:\\SYSTEM\\Kernel.dll", "");
                Console.WriteLine("Creating 0:\\SYSTEM\\Kernel.dll...");
                File.WriteAllText("0:\\SYSTEM\\sysinfo.inf", "" +
                    "[SYSINFO]\n" +/*0*/
                    "Installed = true\n" +/*1*/
                    "Userspecified = true\n" +//2
                    "Passwordspecified = true\n" +//3
                    "RaxOS_Channel = Neo\n" +//4
                    "RaxOS_Version = {\n" +
                    "0.0.0.2\n" +
                    "}");//5
                Console.WriteLine("Creating 0:\\SYSTEM\\sysinfo.inf...");
                Console.Write("Please enter username:");
                string usr = Console.ReadLine();
            Z:
                Console.WriteLine("Please press enter and after enter password");
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Press Enter.");
                    goto Z;
                }

                string pss = Console.ReadLine();
                Console.Clear();
                string[] usrpss = { usr, pss };
                File.WriteAllLines("0:\\SYSTEM\\users.db", usrpss);
                Console.WriteLine("Creating users.db...");
                Directory.CreateDirectory($"0:\\Users\\{usr}");
                Directory.Delete("0:\\Dir Testing\\", true);
                Console.WriteLine("Deleting cache...");
                Directory.Delete("0:\\TEST\\", true);
                Console.WriteLine("Deleting Setup Data...");
                File.Delete("0:\\Kudzu.txt");
                Console.WriteLine("Deleting logs...");
                File.Delete("0:\\Root.txt");
                Console.WriteLine("Deleting raxos_setup");
                fs.CreateDirectory($"0:\\{usr}\\Documents\\");
                Console.WriteLine("Creating 0:\\SYSTEM...");
                Console.WriteLine("Press any key to reboot");
                Console.ReadKey();
                Sys.Power.Reboot();
                return; 
            }
            Console.WriteLine("Loading Filesystem...");
            Console.Clear();
            string loading_text = "Booting";
            Console.WriteLine(loading_text);
            if (!File.Exists(Paths()[(int)Path.SystemFolder] + "\\resol.conf"))
            {
                File.WriteAllText("0:\\RaxOS\\SYSTEM\\resol.conf", "1");
            }
            if (!File.Exists("0:\\RaxOS\\SYSTEM\\color.conf"))
            {
                File.WriteAllText("0:\\RaxOS\\SYSTEM\\color.conf", "1");
            }
            string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
            Console.WriteLine("Reading user data...");
            Console.WriteLine("Reading configuration...");
            string[] SYSINFO = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");
            string currver = SYSINFO[6];
            Console.WriteLine("Reading SysInfo...");
            if (SYSINFO[8] != LatestVersion)
            {
                Console.WriteLine("Please update RaxOS Now, if you don't update, some files and system will be broken!");
                Console.WriteLine("To update open raxupd (run -a raxupd) in command line and put \"--check\".");
            }
            Console.ReadLine();
            Console.Clear();

        LOGIN:
            string username = userData[0];
            string password = userData[1];
            Console.Write("Please, log in with Password:");
            string input = Console.ReadLine();
            if (input == password)
            {
                Console.Clear();
                Console.WriteLine($"Hello, {username}!");
                Console.WriteLine($"Welcome to RaxOS {SYSINFO[5]} {SYSINFO[8]}");
                //Run();
            }
            else
            {
                Console.WriteLine("WRONG PASSWORD. TRY AGAIN!");
                goto LOGIN;
            }
        TAYTEJMFCKDIKED:
            Console.Write("Keyboard layout: [ES/EN/US/TR/DE/FR]");
            string keylay = Console.ReadLine();
            switch (keylay)
            {
                default:
                    Console.WriteLine("Don't Exist.");
                    goto TAYTEJMFCKDIKED;
                case "ES":
                    K.SetKeyLayout(new ESStandardLayout());
                    break;
                case "EN":
                    K.SetKeyLayout(new GBStandardLayout());
                    break;
                case "US":
                    K.SetKeyLayout(new USStandardLayout());
                    break;
                case "TR":
                    K.SetKeyLayout(new TRStandardLayout());
                    break;
                case "DE":
                    K.SetKeyLayout(new DEStandardLayout());
                    break;
                case "FR":
                    K.SetKeyLayout(new FRStandardLayout());
                    break;
            }
        }
        public static string current_directory { get; set; } = "0:\\";
        public static string[] apps =
                    {
                        "cli.scif",
                        "core.notepàd",
                        "RaxOS.Settings",
                        "RaxOS.RaxGET",
                        "Utils.RaxUPD"
                    };
        public static string current_version { get; set; } = "";
        public string LatestVersion { get; private set; }

        protected override void Run()
        {
            string[] dirs = GetDirFadr(current_directory);
            string[] fils = GetFilFadr(current_directory);
            Console.Write(current_directory + "> ");
            string input = Console.ReadLine();
            if (input.StartsWith("echo "))
            {
                Console.WriteLine(input.Remove(0, 5));
            }
            else if (input == "shutdown")
            {
                Sys.Power.Shutdown();
            }
            else if (input == "cd..")
            {
                DirectoryInfo currdir = new DirectoryInfo(current_directory);
                current_directory = currdir.Parent.ToString();
            }
            else if (input.StartsWith("raxget "))
            {
                string пц/*gw*/ = input.Remove(0, 7);
                if (пц == "list")
                {

                    Console.WriteLine($"raxget function: Applications:\n" +
                        $"{apps[0]} | FUNCTION | 1.47KB \n" +
                        $"{apps[1]} | APP | 1.31KB\n" +
                        $"{apps[2]} | SYSTEM APP | 19.93KB\n" +
                        $"{apps[3]} | FUNCTION | 0.07KB\n" +
                        $"{apps[4]} | SYSTEM FUNCTION | 2.17KB");
                    Run();
                }
                if (пц == "list --SAFE")
                {
                    Console.WriteLine($"raxget function: Applications:\n" +
                        $"cli.scif | FUNCTION | 1.47KB \n" +
                        $"core.notepad | APP | 1.31KB\n" +
                        $"RaxOS.Settings | SYSTEM APP | 19.93KB\n" +
                        $"RaxOS.RaxGET | FUNCTION | 0.07KB\n" +
                        $"Utils.RaxUPD | SYSTEM FUNCTION | 2.17KB");
                    Run();
                }
                else if (пц == "install cli.scif")
                {
                    Directory.CreateDirectory(@"0:\Programs\SCIF");
                    string rxpdCode =
                        "using RaxOS_BETA.Programs.ProgramHelper;\r\n" +
                        "using System.IO;\r\n" +
                        "using c = System.Console;\r\n" +
                        "\r\n" +
                        "namespace RaxOS_BETA.Programs\r\n" +
                        "{\r\n" +
                        "    internal class RaxUPD : Program\r\n" +
                        "    {\r\n" +
                        "        public static void Launch()\r\n" +
                        "        {\r\n" +
                        "            mv = 1;\r\n" +
                        "            AppName = \"RaxUPD\";\r\n" +
                        "            sv = 4;\r\n" +
                        "            cv = 0;\r\n" +
                        "            rv = 0;\r\n" +
                        "            AppDescription = \"RaxOS Updater (RaxUPD)\";\r\n" +
                        "            IsStable = true;\r\n" +
                        "            MainLoop();\r\n" +
                        "        }\r\n" +
                        "\r\n" +
                        "        private static void MainLoop()\r\n" +
                        "        {\r\n" +
                        "            c.WriteLine($\"{AppName} v{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}\");\r\n" +
                        "            c.Write(\"Press any key to continue\");\r\n" +
                        "            c.ReadKey();\r\n" +
                        "            c.Clear();\r\n" +
                        "        CLI:c.Write(\"Type raxupd clicommand: \");\r\n" +
                        "#nullable enable\r\n" +
                        "            string? cli = c.ReadLine();\r\n" +
                        "#nullable disable\r\n" +
                        "            if (cli == \"\" || cli == null)\r\n" +
                        "            {\r\n" +
                        "                c.WriteLine(\"Please type a file.\");\r\n" +
                        "                goto CLI;\r\n" +
                        "            }\r\n" +
                        "            else if (cli == \"--check\")\r\n" +
                        "            {\r\n" +
                        "                CheckUpdates();\r\n" +
                        "            }\r\n" +
                        "            else\r\n" +
                        "            {\r\n" +
                        "                    \r\n" +
                        "            }\r\n" +
                        "            c.WriteLine(\"Press any key to exit.\");\r\n" +
                        "            c.ReadKey();\r\n" +
                        "            Close();\r\n" +
                        "        }\r\n" +
                        "        private static void Close()\r\n" +
                        "        {\r\n" +
                        "            c.Clear();\r\n" +
                        "        }\r\n" +
                        "        private static void CheckUpdates()\r\n" +
                        "        {\r\n" +
                        "            string[] SYSINFO = File.ReadAllLines(\"0:\\\\SYSTEM\\\\sysinfo.inf\");\r\n" +
                        "            string currver = SYSINFO[6];\r\n" +
                        "            string lastver = Kernel.LastVersion;\r\n" +
                        "            string KRNLLastver = Kernel.LastVersion;\r\n" +
                        "            c.WriteLine(\"Reading SysInfo...\");\r\n" +
                        "            if (SYSINFO[6] == lastver)\r\n" +
                        "            {\r\n" +
                        "                c.WriteLine(\"Your PC is Updated!\");\r\n" +
                        "                Close();\r\n" +
                        "            }\r\n" +
                        "            else\r\n" +
                        "            {\r\n" +
                        "                c.Write(\"Please update now! Type Y to update (THIS WILL ERASE ALL YOUR DATA!!).\");\r\n" +
                        "                System.ConsoleKeyInfo key = c.ReadKey();\r\n" +
                        "                char letter = key.KeyChar;\r\n" +
                        "                if (letter == 'Y')\r\n" +
                        "                {\r\n" +
                        "                    SystemReserved.DONOTENTER.Format();\r\n" +
                        "                }\r\n" +
                        "            }\r\n" +
                        "        }\r\n" +
                        "\r\n" +
                        "    }\r\n" +
                        "}\r\n" +
                        "";
                    string scifCode =
                        "using Cosmos.System;" +
                        "\r\nusing RaxOS_BETA.Programs.ProgramHelper;" +
                        "\r\nusing System.IO;" +
                        "\r\nusing System.Runtime.CompilerServices;" +
                        "\r\nusing c = System.Console;" +
                        "\r\n" +
                        "\r\nnamespace RaxOS_BETA.Programs" +
                        "\r\n" +
                        "{" +
                        "\r\n    internal class scif:Program" +
                        "\r\n    {" +
                        "\r\n        public static void Launch()" +
                        "\r\n        {" +
                        "\r\n            mv = 43;" +
                        "\r\n            AppName = \"scif\";" +
                        "\r\n            sv = 184; " +
                        "\r\n            cv = 22965; " +
                        "\r\n            rv = 1000;" +
                        "\r\n            AppDescription = \"Simple Content Information of Files (SCIF).\";" +
                        "\r\n            IsStable = true;" +
                        "\r\n            MainLoop();" +
                        "\r\n        }" +
                        "\r\n" +
                        "\r\n        private static void MainLoop()" +
                        "\r\n        {" +
                        "\r\n            c.WriteLine($\"{AppName} v{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}\");" +
                        "\r\n            c.ReadKey();" +
                        "\r\n            c.Write(\"Press any key to continue\");" +
                        "\r\n            c.ReadKey();" +
                        "\r\n            FILEPATH:" +
                        "\r\n            c.Clear();" +
                        "\r\n            c.Write(\"Type file path:\");" +
                        "\r\n#nullable enable" +
                        "\r\n            string? path = c.ReadLine();" +
                        "\r\n#nullable disable" +
                        "\r\n            if (path==\"\"||path==null)" +
                        "\r\n            {" +
                        "\r\n                c.WriteLine(\"Please type a file.\");" +
                        "\r\n                goto FILEPATH;" +
                        "\r\n            }" +
                        "\r\n            string[] path_contents1 = File.ReadAllLines(path:path);" +
                        "\r\n            string path_contents = path_contents1.ToString();" +
                        "\r\n            c.WriteLine(path_contents);" +
                        "\r\n            c.WriteLine(\"Press any key to exit.\");" +
                        "\r\n            c.ReadKey();" +
                        "\r\n            Close();" +
                        "\r\n        }" +
                        "\r\n        private static void Close()" +
                        "\r\n        {" +
                        "\r\n            c.Clear();" +
                        "\r\n           " +
                        "\r\n        }" +
                        "\r\n" +
                        "\r\n    }" +
                        "\r\n}";
                    File.WriteAllText(@"0:\Progrms\SCIF\app.code", scifCode);
                    Run();
                }
                else if (пц == "install core.notepad")
                {

                }
                else if (пц == "install RaxOS.Settings")
                {

                }
                else if (пц == "install RaxOS.RaxGET")
                {

                }
                else if (пц == "install Utils.RaxUPD")
                {

                }
            }
            else if (input.StartsWith("cd "))
            {
                string oл/*jk*/ = input.Remove(0, 3);
                if (Directory.Exists(current_directory + oл))
                {
                    current_directory += oл;
                }
                else
                {
                    Console.WriteLine($"The directory {oл} does not exist!");
                }

            }
            else if (input.StartsWith("mkdir "))
            {
                string вь = input.Remove(0, 6);
                Directory.CreateDirectory("0:\\" + вь/*[вь = dm]*/);
            }
            else if (input.StartsWith("run -a "))
            {

                string app = input.Remove(0, 7);
                if (app == "notepad")
                {
                    if (!File.Exists(@"0:\Programs\Notepad\app.code"))
                    {
                        Console.WriteLine("core.notepad NOT INSTALLED!!! - Please install it on RaxGET");
                        Run();
                    }
                    notepad.Launch();
                }
                if (app == "settings")
                {
                    if (!File.Exists(@"0:\Programs\RaxOS\Settings\app.code"))
                    {
                        Console.WriteLine("RaxOS.Settings NOT INSTALLED!!! - Please install it on RaxGET");
                        Run();
                    }
                    Settings.SettingsMenu.Launch();
                }

            }
            else if (input.StartsWith("run -s "))
            {

                string settings = input.Remove(0, 7);
                if (settings == "sc40016pc")
                {
                    File.Delete("0:\\SYSTEM\\resol.conf");
                    File.Delete("0:\\SYSTEM\\color.conf");
                    File.WriteAllText("0:\\SYSTEM\\resol.conf", "1");
                    File.WriteAllText("0:\\SYSTEM\\color.conf", "1");
                    Sys.Power.Reboot();
                }
                if (settings == "SetDisplayResolution-1080p")
                {
                    try
                    {
                        Sys.Graphics.VGAScreen.SetGraphicsMode(Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size720x480, Sys.Graphics.ColorDepth.ColorDepth32);
                    }
                    catch (Exception)
                    {
                        //VGAScreen.SetGraphicsMode(Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size640x480, ColorDepth.ColorDepth4);
                        Console.WriteLine("ERROR.");
                        throw;
                    }

                    Sys.MouseManager.ScreenWidth = 1920;
                    Sys.MouseManager.ScreenHeight = 1080;
                }
                else if (settings == "SDR-200p")
                {
                    File.Delete("0:\\SYSTEM\\resol.conf");
                    File.Delete("0:\\SYSTEM\\color.conf");
                    File.WriteAllText("0:\\SYSTEM\\resol.conf", "1");
                    File.WriteAllText("0:\\SYSTEM\\color.conf", "1");
                }
                else if (settings == "SDR-480p")
                {
                    File.WriteAllText("0:\\SYSTEM\\resol.conf", "2");
                    File.WriteAllText("0:\\SYSTEM\\color.conf", "1");
                }
                else if (settings == "MouseState-Enable")
                {
                    Sys.MouseManager.HandleMouse(1920 / 2, 1080 / 2, 0, 0);
                }
                else if (settings == "help")
                {
                    Console.WriteLine("Syntax: run -s <setting>");
                    Console.WriteLine(
                        "Examples:\n" +
                        "run -s MouseState-Enable\n" +
                        "run -s SDR-480p" +
                        "run -s SDR-200p"
                        );
                }

            }
            else if (input == "scif")
            {
                if (!File.Exists(@"0:\Programs\RaxOS\SCIF"))
                {
                    Console.WriteLine("cli.scif NOT INSTALLED!!! - Please install it on RaxGET Store");
                    Run();
                }
                Console.WriteLine("Loadi" +
                    "ng SCIF...");
                scif.Launch();
            }
            else if (input == "dir")
            {
                foreach (var item in dirs)
                {
                    Console.WriteLine(item);
                }
                foreach (var item in fils)
                {
                    Console.WriteLine(item);
                }
                Run();
            }
            else if (input == "info")
            {
                Cosmos.HAL.PCSpeaker.Beep(37000);
                Console.WriteLine("" +
                    "System INFO:\n" +
                    $"CPU Brand: {CPU.GetCPUBrandString}\n" +
                    $"CPU Vendor: {CPU.GetCPUVendorName}\n" +
                    $"RAM: {CPU.GetAmountOfRAM}\n");
            }
            else
            {

            }

            string[] GetDirFadr(string adr) // Get Directories From Address
            {
                var dirs = Directory.GetDirectories(adr);
                return dirs;
            }
            string[] GetFilFadr(string adr)
            {
                var fils = Directory.GetFiles(adr);
                return fils;
            }

            switch (input)
            {
                case "help":
                    Console.WriteLine("help -- show list\n");
                    Console.WriteLine("info -- Show system information\n");
                    Console.WriteLine("shutdown -- Shutdown ACPI\n");
                    Console.WriteLine("reboot -- Reboot system\n");
                    Console.WriteLine("test [component] - -audio, -network, -graphics...\n");
                    Console.WriteLine("ls -- ContentList with files & folders in 0:\\ \n");
                    Console.WriteLine("dir -- DIR v2: Shows all files and folders in the location specified in app.\n");
                    Console.WriteLine("scif <file> -- Read content of <file>\n");
                    Console.WriteLine("run -a <program> -- Open application\n");
                    Console.WriteLine("mkdir <name> -- Make a folder with <name>\n");
                    Console.WriteLine("echo <text> -- Display <text> on screen\n");
                    Run();
                    break;
                case "reboot":
                    Sys.Power.Reboot();
                    break;
            }
        }
        public static void Net()
        {
            Console.WriteLine("In develop.");
        }
        string GetCheckVM()
        {
            for (int I = 0; I < PCI.Count; I++)
            {
                if (PCI.Devices[I].VendorID == 0x80EE)
                {
                    return "VirtualBox";
                }
            }
            for (int I = 0; I < PCI.Count; I++)
            {
                if (PCI.Devices[I].VendorID == 0x15ad)
                {
                    return "VMware";
                }
            }
            for (int I = 0; I < PCI.Count; I++)
            {
                if (PCI.Devices[I].VendorID == 0x1af4)
                {
                    return "QEMU";
                }
            }
            return "NotVM";
        }
        /*public static void* TEST(string A)
        {
            return TEST2;
        }
        void* TEST2()
        {

        }*//*
        static Task<string> MyTask()
        {
            return ReturnTask();
        }

        static Task<string> ReturnTask(string A = "aaaaaaa")
        {
            if (A == "AeIoU")
            {
                return MyTask();
            }
            return MyTask();
        }
        static Task ReturnTask2()
        {
        }
    }*/
    }
}
