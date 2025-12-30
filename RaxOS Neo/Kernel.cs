using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Network;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.ScanMaps;
using RaxOS_BETA.Programs;
using RaxOS_Neo.GUI.Screens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using K = Cosmos.System.KeyboardManager;
using Sys = Cosmos.System;

namespace RaxOS_Neo
{
    public class Kernel : Sys.Kernel
    {
        private static int startSecond;
        private static int startMinute;
        private static int startHour;

        private static int secondsUptime { get; set; } = 0;
        private static Timer timer;
        public static string user = "";
        public static string password = "";

        public static string[] CommandExecs = new string[100];
        public static int CommandExecsCount = 0;
        public enum Path
        {
            SystemFolder,
            UserDir
        }
        public static string getPath(Path path) => path switch
        {
            Path.SystemFolder => "0:\\RaxOS\\SYSTEM\\",
            Path.UserDir => "0:\\USER",
            _ => throw new ArgumentOutOfRangeException(nameof(path), $"Not expected direction value: {path}")
        };
        protected override void BeforeRun()
        {
            startSecond = RTC.Second;
            startMinute = RTC.Minute;
            startHour = RTC.Hour;


            Console.Clear();
            Sys.FileSystem.CosmosVFS fs = new();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            fs.Initialize(false);
            
            if (!File.Exists("0:\\RaxOS\\SYSTEM\\System.cs"))
            {
                RaxOS_BETA.exCode.Setup(fs);
            }
            GUI.Screens.BootScreen.Display();
            string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
            Console.WriteLine("Reading user data...");
            string[] SYSINFO = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");
            string currver = SYSINFO[6];
            Console.WriteLine("Reading Sysinfo...");
            Console.WriteLine("Sysinfo length: " + SYSINFO.Length);
            string version = null;

            for (int i = 0; i < SYSINFO.Length - 1; i++)
            {
                if (SYSINFO[i].StartsWith("RaxOS_Version"))
                {
                    version = SYSINFO[i + 1]; // asumimos que la siguiente línea es la versión
                    break;
                }
            }

            if (version != null)
            {
                Console.WriteLine($"RaxOS Version " + version);

                if (version != LatestVersion)
                {
                    Console.WriteLine("Please update RaxOS NEO!");
                    Console.WriteLine("To update open raxupd (run -a raxupd) in command line and put \"--check\".");
                }
                else
                {
                    Console.WriteLine("Latest update installed. You're safe!");
                }
            }
            
            else
            {
                Console.WriteLine("Version info not found in sysinfo.");
            }
            Console.WriteLine($"Boot config status: {File.Exists(@"0:\RaxOS\System\boot.conf")}");
            if (File.Exists(@"0:\RaxOS\System\boot.conf"))
            {
                string[] contents = File.ReadAllLines(@"0:\RaxOS\System\boot.conf");
                foreach (string line in contents)
                {
                    if (line.Contains("1"))
                    {
                        InitCLI.Init();
                        Console.Clear();
                        Console.WriteLine("Welcome to RaxOS Neo, " + userData[0] + "!");
                        Console.WriteLine("Type 'header' to see the header.");
                        Console.WriteLine("Type 'help' for help.");
                        Console.WriteLine("Type 'info' for system info.");
                        Console.WriteLine("Type 'uptime' to see the uptime.");
                        Console.WriteLine("Type 'dir' to see the current directory contents.");
                        Console.WriteLine("Type 'scif' to run SCIF (Simple Content Information of Files).");
                        Console.WriteLine("Type 'raxget' to install applications from RaxGET Store.");
                        return;
                    }
                }}
            Login:
                RaxOS_Neo.GUI.Screens.Login.Display();
                if (!GUI.Screens.Login.logged)
                {
                    goto Login;
                }
                var _ = GUI.Screens.SelectKbd.Display();
                if (_)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to RaxOS Neo, " + userData[0] + "!");
                    Console.WriteLine("Type 'header' to see the header.");
                    Console.WriteLine("Type 'help' for help.");
                    Console.WriteLine("Type 'info' for system info.");
                    Console.WriteLine("Type 'uptime' to see the uptime.");
                    Console.WriteLine("Type 'dir' to see the current directory contents.");
                    Console.WriteLine("Type 'scif' to run SCIF (Simple Content Information of Files).");
                    Console.WriteLine("Type 'raxget' to install applications from RaxGET Store.");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Keyboard not selected. Please select a keyboard layout.");
                }
            

        }
        public static string current_directory { get; set; } = "0:\\";
        public static string[] apps =
                    {
                        "cli.scif",
                        "core.notepad",
                        "RaxOS.Settings",
                        "RaxOS.RaxGET",
                        "Utils.RaxUPD"
                    };
        public static string current_version { get; set; } = "0.1";
        public static string LatestVersion { get; set; } = "0.1";
        public static string LastVersion { get; internal set; } = LatestVersion;
        public static void AddCommand(string input)
        {
            if (CommandExecsCount < CommandExecs.Length)
            {
                CommandExecs[CommandExecsCount] = input;
                CommandExecsCount++;
            }
            else
            {
                // Podrías implementar desplazamiento, pero cuidado con rendimiento
            }
        }
        protected override void Run()
        {
            string[] dirs = GetDirFadr(current_directory);
            string[] fils = GetFilFadr(current_directory);
            Console.Write(current_directory + "> ");
            string input = Console.ReadLine();
            AddCommand(input);
            if (input.StartsWith("echo "))
            {
                Console.WriteLine(input.Remove(0, 5));
            }
            else if (input.StartsWith("shutdown"))
            {
                var x = input.Length > 9 ? input[9..] : "";

                if (x.StartsWith("-s"))
                {
                    if (x.Length == 2)
                    {
                        Sys.Power.Shutdown();
                    }
                    else
                    {
                        var y = x.Length > 3 ? x[3..] : "";
                        if (y.StartsWith("-f"))
                        {
                            Cosmos.HAL.Power.ACPIShutdown();
                        }
                    }
                }
                else if (x.StartsWith("-r"))
                {
                    if (x.Length == 2)
                    {
                        Sys.Power.Reboot();
                    }
                    else
                    {
                        var y = x.Length > 3 ? x[3..] : "";
                        if (y.StartsWith("-f"))
                        {
                            Cosmos.HAL.Power.CPUReboot();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("syntax: shutdown <-s|-r> [-f] -- -s: shutdown; -r: reboot; -f: force");
                }
                return;
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
                    
                }
                if (пц.ToLower() == "list --SAFE")
                {
                    Console.WriteLine($"raxget function: Applications:\n" +
                        $"cli.scif | FUNCTION | 1.47KB \n" +
                        $"core.notepad | APP | 1.31KB\n" +
                        $"RaxOS.Settings | SYSTEM APP | 19.93KB\n" +
                        $"RaxOS.RaxGET | FUNCTION | 0.07KB\n" +
                        $"Utils.RaxUPD | SYSTEM FUNCTION | 2.17KB");
                    
                }

                else if (пц.ToLower() == "install cli.scif")
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
                    
                }
                else if (пц.ToLower() == "install core.notepad")
                {

                }
                else if (пц.ToLower() == "install RaxOS.Settings")
                {
                    Directory.CreateDirectory(@"0:\Programs\RaxOS\Settings");
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
                        "\r\nusing Cosmos.System.FileSystem.VFS;" +
                        "\r\nusing RaxOS_BETA .Programs.ProgramHelper;" +
                        "\r\nusing System;\r\nusing System.IO;" +
                        "\r\nusing System.Runtime.Serialization.Formatters;" +
                        "\r\nusing System.Threading;\r\nusing c = System.Console;" +
                        "\r\n" +
                        "\r\nnamespace RaxOS_BETA.Programs" +
                        "\r\n{" +
                        "\r\n    internal partial class Settings" +
                        "\r\n    {" +
                        "\r\n        internal class SettingsMenu:Program" +
                        "\r\n        {" +
                        "\r\n            public static void Launch()" +
                        "\r\n            {" +
                        "\r\n                mv = 1;" +
                        "\r\n                AppName = \"Settings\";" +
                        "\r\n                sv = 0;" +
                        "\r\n                cv = 0;" +
                        "\r\n                rv = 0;" +
                        "\r\n                AppDescription = \"Settings Menu to configure your RaxOS Computer.\";" +
                        "\r\n                IsStable = true;" +
                        "\r\n                MainLoop();" +
                        "\r\n            }" +
                        "\r\n" +
                        "\r\n            private static void MainLoop()" +
                        "\r\n            {" +
                        "\r\n                c.WriteLine($\"{AppName} v{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}\");" +
                        "\r\n                c.Write(\"Press any key to continue\");" +
                        "\r\n                c.ReadKey();" +
                        "\r\n                c.Clear();" +
                        "\r\n            SETTING:" +
                        "\r\n                c.Write(\"Type setting rxlt [help:help]:\");" +
                        "\r\n#nullable enable" +
                        "\r\n                string? rxlt = c.ReadLine();" +
                        "\r\n#nullable disable" +
                        "\r\n                if (rxlt == \"\" || rxlt == null)" +
                        "\r\n                {" +
                        "\r\n                    c.WriteLine(\"Please type a file.\");" +
                        "\r\n                    goto SETTING;" +
                        "\r\n                }" +
                        "\r\n                else if (rxlt == \"help\")" +
                        "\r\n                {" +
                        "\r\n                    c.WriteLine(\"rsl [--Px320Bt4, --Px320Bt8, --Px640Bt4, --Px720Bt16]\\n\" +" +
                        "\r\n                        \"net [--init]\\n\" +" +
                        "\r\n                        \"factory [--true]\");" +
                        "\r\n                    goto SETTING;" +
                        "\r\n                }" +
                        "\r\n                else if (rxlt == \"rsl\")" +
                        "\r\n                {" +
                        "\r\n                    {" +
                        "\r\n                        c.WriteLine(\"rsl --Px[resol+bitcolor]\");" +
                        "\r\n                        c.WriteLine(\"resol+bitcolor: 320Bt4, 320Bt8, 640Bt4, 720Bt16\");" +
                        "\r\n                    }" +
                        "\r\n                    {" +
                        "\r\n                        goto SETTING;" +
                        "\r\n                    }" +
                        "\r\n                }" +
                        "\r\n                else if (rxlt.StartsWith(\"rsl \"))" +
                        "\r\n                {string tmp = rxlt.Substring(4);" +
                        "\r\n                    {" +
                        "\r\n                        if (tmp == \"--Px320Bt4\") {" +
                        "\r\n                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px320Bt4);" +
                        "\r\n                        }" +
                        "\r\n                        else if (tmp == \"--Px320Bt8\")" +
                        "\r\n                        {" +
                        "\r\n                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px320Bt8);" +
                        "\r\n                        }" +
                        "\r\n                        else if (tmp == \"--Px640Bt4\")" +
                        "\r\n                        {" +
                        "\r\n                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px640Bt4);" +
                        "\r\n                        }" +
                        "\r\n                        else if (tmp == \"--Px720Bt16\")" +
                        "\r\n                        {" +
                        "\r\n                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px720Bt16);" +
                        "\r\n                        }" +
                        "\r\n                        else" +
                        "\r\n                        {" +
                        "\r\n                            c.WriteLine(\"rsl --Px[resol+bitcolor]\");" +
                        "\r\n                            c.WriteLine(\"resol+bitcolor: 320Bt4, 320Bt8, 640Bt4, 720Bt16\");" +
                        "\r\n                            c.WriteLine(\"EXAMPLES: rsl --Px320Bt4, rsl --Px720Bt16\");" +
                        "\r\n                        }" +
                        "\r\n                    }" +
                        "\r\n                    " +
                        "\r\n                }" +
                        "\r\n                else if (rxlt == \"net --init\")" +
                        "\r\n                {" +
                        "\r\n                    Cosmos.HAL.Network.NetworkInit.Init();" +
                        "\r\n                    goto SETTING;" +
                        "\r\n                }" +
                        "\r\n                else if (rxlt == \"net\")" +
                        "\r\n                {" +
                        "\r\n                    c.WriteLine(\"net --init: Calls HAL.Network.NetworkInit.Init(); method\");" +
                        "\r\n                    goto SETTING;" +
                        "\r\n                }" +
                        "\r\n                else if (rxlt == \"factory\")" +
                        "\r\n                {" +
                        "\r\n                    c.WriteLine(\"WARNING!: The \\\"factory --true\\\" deletes the OS! USE ONLY IF YOU WILL REINSTALL RAXOS OR INSTALL ANOTHER OS.\");" +
                        "\r\n                    goto SETTING;" +
                        "\r\n                }" +
                        "\r\n                else if (rxlt == \"factory --true\")" +
                        "\r\n                {" +
                        "\r\n                    c.WriteLine(\"WARNING!: The \\\"factory --true\\\" deletes the OS! USE ONLY IF YOU WILL REINSTALL RAXOS OR INSTALL ANOTHER OS.\");" +
                        "\r\n                    c.WriteLine(\"ARE YOU SURE TO DELETE THE OS?? [Y/N]\");" +
                        "\r\n#nullable enable" +
                        "\r\n                    string? ind = c.ReadLine().ToUpper();" +
                        "\r\n#nullable disable" +
                        "\r\n                    if (ind == \"Y\")" +
                        "\r\n                    {" +
                        "\r\n                        SystemReserved.DONOTENTER.Format();" +
                        "\r\n                    }" +
                        "\r\n                    else" +
                        "\r\n                    {" +
                        "\r\n                        c.WriteLine(\"Good!\");" +
                        "\r\n                    }" +
                        "\r\n                    goto SETTING;" +
                        "\r\n                }" +
                        "\r\n" +
                        "\r\n                c.WriteLine(\"Press any key to exit.\");" +
                        "\r\n                c.ReadKey();" +
                        "\r\n                Close();" +
                        "\r\n            }" +
                        "\r\n            private static void Close()" +
                        "\r\n            {" +
                        "\r\n                c.Clear();" +
                        "\r\n" +
                        "\r\n            }" +
                        "\r\n            public static void BSoD()" +
                        "\r\n            {" +
                        "\r\n                c.BackgroundColor = ConsoleColor.Blue;" +
                        "\r\n                c.ForegroundColor = ConsoleColor.White;" +
                        "\r\n                c.Clear();" +
                        "\r\n                c.WriteLine(\":(          .\");" +
                        "\r\n            }" +
                        "\r\n        }" +
                        "\r\n    }" +
                        "\r\n}" +
                        "\r\nnamespace SystemReserved" +
                        "\r\n{" +
                        "\r\n    public class DONOTENTER" +
                        "\r\n    {" +
                        "\r\n        public static void Format()" +
                        "\r\n        {" +
                        "\r\n            try" +
                        "\r\n            {" +
                        "\r\n                c.WriteLine(\"DELETING RaxOS... [Your computer will be unbootable!]\");" +
                        "\r\n                #region Uninstaller" +
                        "\r\n                //Custom Installer (Now you see why" +
                        "\r\n" +
                        "\r\n                c.WriteLine(\"Welcome to RAXOS Uninstaller.\");" +
                        "\r\n                Cosmos.System.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();" +
                        "\r\n                c.Clear();" +
                        "\r\n                if (Directory.Exists(@\"0:\\RaxOS\\\"))" +
                        "\r\n                {" +
                        "\r\n                    Directory.Delete(\"0:\\\\RaxOS\", true);" +
                        "\r\n                }" +
                        "\r\n" +
                        "\r\n                c.WriteLine(\"Deleting users.db...\");" +
                        "\r\n                if (Directory.Exists(@\"0:\\Dir Testing\\\"))" +
                        "\r\n                {" +
                        "\r\n                    Directory.Delete(\"0:\\\\Dir Testing\\\\\", true);" +
                        "\r\n                }" +
                        "\r\n                " +
                        "\r\n                c.WriteLine(\"Deleting cache...\");" +
                        "\r\n" +
                        "\r\n                if (Directory.Exists(@\"0:\\TEST\\\"))" +
                        "\r\n                {" +
                        "\r\n                    Directory.Delete(\"0:\\\\TEST\\\\\", true);" +
                        "\r\n                }" +
                        "\r\n" +
                        "\r\n                c.WriteLine(\"Deleting Uninstaller ...\");" +
                        "\r\n" +
                        "\r\n                if (File.Exists(@\"0:\\Kudzu.txt\")) {" +
                        "\r\n                    File.Delete(\"0:\\\\Kudzu.txt\");" +
                        "\r\n                }" +
                        "\r\n" +
                        "\r\n                c.WriteLine(\"  ...\");" +
                        "\r\n" +
                        "\r\n                if (File.Exists(@\"0:\\Root.txt\"))" +
                        "\r\n                {" +
                        "\r\n                    File.Delete(\"0:\\\\Root.txt\");" +
                        "\r\n                }" +
                        "\r\n" +
                        "\r\n                c.WriteLine(\" ...\");" +
                        "\r\n" +
                        "\r\n                if (Directory.Exists(@\"0:\\Documents\\\"))" +
                        "\r\n                {" +
                        "\r\n                    Directory.Delete(\"0:\\\\Documents\\\\\");" +
                        "\r\n                }" +
                        "\r\n                var x = VFSManager.GetDisks();" +
                        "\r\n                foreach (var disk in x)" +
                        "\r\n                {" +
                        "\r\n                    var y = disk.Partitions;" +
                        "\r\n                    foreach (var partition in y)" +
                        "\r\n                    {" +
                        "\r\n                        var index = disk.Partitions.IndexOf(partition);" +
                        "\r\n                        if (index != -1)" +
                        "\r\n                        {" +
                        "\r\n                            disk.FormatPartition(index, \"FAT32\");" +
                        "\r\n                            //Console.WriteLine($\"Formatted partition {index} on disk {disk}\");" +
                        "\r\n                        }" +
                        "\r\n                        else" +
                        "\r\n                        {" +
                        "\r\n                            //Console.WriteLine(\"Partition index not found!\");" +
                        "\r\n                        }" +
                        "\r\n                    }" +
                        "\r\n                }" +
                        "\r\n" +
                        "\r\n" +
                        "\r\n                c.WriteLine(\" ...\");" +
                        "\r\n                c.WriteLine(\"    \");" +
                        "\r\n                Thread.Sleep(2000);" +
                        "\r\n                //Cosmos.System.Power.Reboot();" +
                        "\r\n                RaxOS_BETA.Programs.Settings.SettingsMenu.BSoD();" +
                        "\r\n                Thread.Sleep(3000);" +
                        "\r\n                Cosmos.HAL.Power.CPUReboot();" +
                        "\r\n                // There is no SYSTEM directory yet, so we just shut the computer down there" +
                        "\r\n                #endregion" +
                        "\r\n            }" +
                        "\r\n            catch" +
                        "\r\n            {" +
                        "\r\n                RaxOS_BETA.ExceptionHelper.Exception _ex = new(\"UNINSTALL_ERROR\");" +
                        "\r\n                _ex.Source = \"uninsaller\";" +
                        "\r\n                _ex.Code = 0x100;" +
                        "\r\n                _ex.Message = \"UNINSTALL_ERROR\";" +
                        "\r\n                RaxOS_BETA.ExceptionHelper.ExceptionHandler.BSoD_Handler(_ex);" +
                        "\r\n            }" +
                        "\r\n        }" +
                        "\r\n    }";
                    File.WriteAllText(@"0:\Progrms\RaxOS\Settings\app.code", scifCode);
                    
                }
                else if (пц == "install RaxOS.RaxGET")
                {

                }
                else if (пц == "install Utils.RaxUPD")
                {

                }
            }
            else if (input.ToLower().StartsWith("theme "))
            {
                string theme = input[6..];
                switch (theme)
                {
                    case "light":
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case "dark":
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    default:
                        Console.WriteLine("Out of index.");
                        break;
                }
                Console.Clear();
            }
            else if (input.ToLower().StartsWith("invoke "))
            {
                string x = input.Remove(0, 7);
                if (x.StartsWith("exception "))
                {
                    string y = x.Remove(0, 10);
                    RaxOS_BETA.ExceptionHelper.Exception @new = new("INVOKED_EXCEPTION");
                    @new.Code = 0x00F;
                    @new.Source = "INVOKE EXCEPTION <X>";
                    RaxOS_BETA.ExceptionHelper.ExceptionHandler.BSoD_Handler(@new);
                }
            }
            else if (input.ToLower().StartsWith("cd "))
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
            else if (input.ToLower().StartsWith("mkdir "))
            {
                string вь = input.Remove(0, 6);
                Directory.CreateDirectory("0:\\" + вь/*[вь = dm]*/);
            }
            else if (input.ToLower().StartsWith("run -a "))
            {
                string app = input.Remove(0, 7);
                if (app == "notepad")
                {
                    if (!File.Exists(@"0:\Programs\Notepad\app.code"))
                    {
                        Console.WriteLine("core.notepad not found. Install it on raxget.");
                        return;
                    }
                    notepad.Launch();
                }
                if (app == "settings")
                {
                    if (!File.Exists(@"0:\Programs\RaxOS\Settings\app.code"))
                    {
                        return;
                        
                    }
                    Settings.SettingsMenu.Launch();
                }
                if (app == "list")
                {
                    foreach (var _app in apps)
                    {
                        Console.WriteLine(_app);
                    }
                }
            }
            else if (input.ToLower().StartsWith("run -s "))
            {

                string settings = input.Remove(0, 7);
                if (settings == "sc40016pc")
                {
                    File.Delete("0:\\RaxOS\\SYSTEM\\resol.conf");
                    File.Delete("0:\\RaxOS\\SYSTEM\\color.conf");
                    File.WriteAllText("0:\\RaxOS\\SYSTEM\\resol.conf", "1");
                    File.WriteAllText("0:\\RaxOS\\SYSTEM\\color.conf", "1");
                    Sys.Power.Reboot();
                }
                if (settings == "SetDisplayResolution-1080p")
                {
                    try
                    {
                        Sys.Graphics.VGAScreen.SetGraphicsMode(Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size720x480, Sys.Graphics.ColorDepth.ColorDepth32);
                    }
                    catch (Exception ex)
                    {
                        //VGAScreen.SetGraphicsMode(Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size640x480, ColorDepth.ColorDepth4);
                        Console.WriteLine("ERROR. " + ex.Message);
                    }

                    Sys.MouseManager.ScreenWidth = 1920;
                    Sys.MouseManager.ScreenHeight = 1080;
                }
                else if (settings == "SDR-200p")
                {
                    File.Delete("0:\\RaxOS\\SYSTEM\\resol.conf");
                    File.Delete("0:\\RaxOS\\SYSTEM\\color.conf");
                    File.WriteAllText("0:\\RaxOS\\SYSTEM\\resol.conf", "1");
                    File.WriteAllText("0:\\RaxOS\\SYSTEM\\color.conf", "1");
                }
                else if (settings == "SDR-480p")
                {
                    File.WriteAllText("0:\\RaxOS\\SYSTEM\\resol.conf", "2");
                    File.WriteAllText("0:\\RaxOS\\SYSTEM\\color.conf", "1");
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
            else if (input.ToLower().StartsWith("color "))
            {
                var color = input[6..];
                switch (color[0].ToString().ToLower())
                {
                    case "1":
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case "2":
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case "3":
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case "4":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "5":
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case "6":
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case "7":
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case "8":
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case "9":
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case "A":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "B":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case "C":
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case "D":
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case "E":
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case "F":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                }
                switch (color[1].ToString().ToLower())
                {
                    case "1":
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case "2":
                        //Console.WriteLine("Background Color: Blue");
                        Console.BackgroundColor = ConsoleColor.Blue;
                        break;
                    case "3":
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        break;
                    case "4":
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "5":
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        break;
                    case "6":
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;
                    case "7":
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case "8":
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        break;
                    case "9":
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        break;
                    case "A":
                        Console.BackgroundColor = ConsoleColor.Gray;
                        break;
                    case "B":
                        Console.BackgroundColor = ConsoleColor.Green;
                        break;
                    case "C":
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        break;
                    case "D":
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    case "E":
                        Console.BackgroundColor = ConsoleColor.White;
                        break;
                    case "F":
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        break;
                }
                return;
            }
            else if (input.ToLower() == "clear")
            {
                Console.Clear();
            }
            else if (input.ToLower() == "scif")
            {
                if (!File.Exists(@"0:\Programs\RaxOS\SCIF"))
                {
                    Console.WriteLine("cli.scif NOT INSTALLED!!! - Please install it on RaxGET Store");
                    return;
                }
                Console.WriteLine("Loadi" +
                    "ng SCIF...");
                scif.Launch();
            }
            else if (input.ToLower() == "uptime")
            {
                int nowSecond = RTC.Second;
                int nowMinute = RTC.Minute;
                int nowHour = RTC.Hour;

                // Calcular uptime
                int totalSecs = (nowHour - startHour) * 3600 + (nowMinute - startMinute) * 60 + (nowSecond - startSecond);

                // Puedes formatear así:
                int hours = totalSecs / 3600;
                int minutes = (totalSecs % 3600) / 60;
                int seconds = totalSecs % 60;

                Console.WriteLine($"Uptime: {hours}h {minutes}m {seconds}s");

            }
            else if (input.ToLower() == "dir")
            {
                foreach (var item in dirs)
                {
                    Console.WriteLine(item);
                }
                foreach (var item in fils)
                {
                    Console.WriteLine(item);
                }
            }
            else if (input.ToLower() == "sysinfo")
            {
                string[] sysinfo = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");
                foreach (var item in sysinfo)
                {
                    Console.WriteLine(item);
                }
                return;
            }
            
            else if (input == "header")
            {
                Console.Clear();
                Console.WriteLine($"RaxOS CLI v{sysinfo.versionString}-{sysinfo.getChannel()}");
                Console.WriteLine($"Type help for help.");
            }
            else if (input == "info")
            {
                Cosmos.HAL.PCSpeaker.Beep(30000);
                //Cosmos.HAL.PCSpeaker.Beep(37000);
                Console.WriteLine("System info:");
                Console.WriteLine($"RaxOS v{sysinfo.versionString} {sysinfo.getChannel()}");
                Console.WriteLine($"CPU Brand : {CPU.GetCPUBrandString()}");
                Console.WriteLine($"CPU Vendor: {CPU.GetCPUVendorName()}");
                Console.WriteLine($"RAM       : {CPU.GetAmountOfRAM()}");
                Console.WriteLine($"Is VM     ? {GetVM()}");
            }
            else if (input == "net test")
            {
                Net();
            }
            else if (input == "help")
            {
                Console.WriteLine("help -- show list");
                Console.WriteLine("info -- Show system information");
                Console.WriteLine("shutdown <-s|-r> [-f] -- Shutdown or reboot");
                Console.WriteLine("test [component] - -audio, -network, -graphics...");
                Console.WriteLine("ls -- ContentList with files & folders in 0:\\ ");
                Console.WriteLine("dir -- Shows all files and folders in the location specified in app");
                Console.WriteLine("scif <file> -- Read content of <file>");
                Console.WriteLine("run -a <program> -- Open application");
                Console.WriteLine("mkdir <name> -- Make a folder named <name>");
                Console.WriteLine("echo <text> -- Display <text> on screen");
                Console.WriteLine("invoke exception <details> -- Crashes the system");
                Console.WriteLine("net test -- Tests the network connection [!]");
            }
            else if (input == "ls")
            {
                foreach (var item in dirs)
                {
                    /*Sys.FileSystem.CosmosVFS fs = new();
                    var dirAttribs = fs.GetFileAttributes(item);*/

                    Console.WriteLine("<DIRECTORY> " + /*Directory.GetCreationTime(
                        System.IO.Path.Combine(current_directory, item)) +*/ item);
                }
                foreach (var item in fils)
                {
                    Console.WriteLine("<FILE>      " + /*File.GetCreationTime(
                        System.IO.Path.Combine(current_directory, item)) +*/ item);
                }
            }
            else if (input == "whoami")
            {
                string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
                string username = userData[0];
                Console.WriteLine(username);
            }
            else if (input == "gui")
            {
                Desktop.Show(); 
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
        }
        public static void Net()
        {
            try
            {
                RaxOS_Neo.Net.Network.ShowNetworkDevices();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DNS Error: " + ex.Message);
            }

            System.Threading.Thread.Sleep(5000); // Pausa para no saturar la consola
        }
        public static string GetVM() => GetCheckVM() switch
        {
            "VirtualBox" => "Yes, VirtualBox",
            "VMware" => "Yes, VMware",
            "QEMU" => "Yes, QEMU",
            "NotVM" => "No. Real PC",
            _ => "Unknown"
        };
        static string GetCheckVM()
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

        }
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
