using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Network;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.ScanMaps;
using RaxOS_Neo.Programs;
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
                exCode.Setup(fs);
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
                        $"{apps[0]}      | FUNCTION        | 1.47KB \n" +
                        $"{apps[1]}      | APP             | 1.31KB\n" +
                        $"{apps[2]}      | SYSTEM APP      | 19.93KB\n" +
                        $"{apps[3]}      | FUNCTION        | 0.07KB\n" +
                        $"{apps[4]}      | SYSTEM FUNCTION | 2.17KB");
                    
                }
                if (пц.ToLower() == "list --SAFE")
                {
                    Console.WriteLine($"raxget function: Applications:\n" +
                        $"cli.scif       | FUNCTION        | 1.47KB \n" +
                        $"core.notepad   | APP             | 1.31KB\n" +
                        $"RaxOS.Settings | SYSTEM APP      | 19.93KB\n" +
                        $"RaxOS.RaxGET   | FUNCTION        | 0.07KB\n" +
                        $"Utils.RaxUPD   | SYSTEM FUNCTION | 2.17KB");
                    
                }

                else if (пц.ToLower() == "install cli.scif")
                {
                    string scifPath = "0:\\Programs\\SCIF\\scif.rxlt";
                    if (!File.Exists(scifPath))
                    {
                        string scif_rxlt = @"command ""scif""
print ""SCIF - Simple Content Information of Files""
print ""Type any key to continue""
call wait_key

label FILEPATH
print ""Type file path:""
call read_input path

if_empty path FILEPATH

call read_file path
call print_file path

print ""Press any key to exit""
call wait_key
call clear_screen
end
";
                        File.WriteAllText(scifPath, scif_rxlt);
                    }
                }
                else if (пц.ToLower() == "install core.notepad")
                {
                    string simpleNanoPath = "0:\\Programs\\Nano\\simplenano.rxlt";
                    if (!File.Exists(simpleNanoPath))
                    {
                        string scif_rxlt = @"command ""simplenano""
print ""SimpleNano v1.0""
print ""Type your document. Press ESC to finish and save.""

print ""File path [0:\\myfolder\\newfile.txt]:""
call read_input path

label EDIT
print ""Type text (ESC to finish):""
call read_multiline path
print ""Saved "" path
call clear_screen
end
";
                        File.WriteAllText(simpleNanoPath, scif_rxlt);
                    }
                }
                else if (пц.ToLower() == "install RaxOS.Settings")
                {
                    
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
                        Console.WriteLine("Only light and dark supported.");
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
                    ExceptionHelper.Exception @new = new("INVOKED_EXCEPTION");
                    @new.Code = 0x0F;
                    @new.Source = current_directory;
                    ExceptionHelper.ExceptionHandler.GraphicalHandler.BSOD_GHandler(@new);
                }
            }
            else if (input.ToLower().StartsWith("cd "))
            {
                string jk = input.Remove(0, 3);
                if (Directory.Exists(current_directory + jk))
                {
                    current_directory += jk;
                }
                else
                {
                    Console.WriteLine($"The directory {jk} does not exist!");
                }

            }
            else if (input.ToLower().StartsWith("mkdir "))
            {
                string dm = input.Remove(0, 6);
                Directory.CreateDirectory(System.IO.Path.Combine(current_directory,dm));
            }
            else if (input.ToLower().StartsWith("run -a "))
            {
                string app = input.Remove(0, 7);
                if (app.ToLower() == "scif")
                if (app.ToLower() == "notepad")
                {
                    if (!File.Exists(@"0:\Programs\Nano\simplenano.rxlt"))
                    {
                        Console.WriteLine("core.notepad not found. Install it on raxget.");
                        return;
                    }
                    RXLTRun.ExecuteFile(@"0:\Programs\Nano\simplenano.rxlt");
                }
                if (app.ToLower() == "settings")
                {
                    if (!File.Exists(@"0:\Programs\\Settings\settings.rxlt"))
                    {
                        Console.WriteLine("RaxOS.Settings not found. Install it on raxget.");
                        return;
                    }
                    RXLTRun.ExecuteFile(@"0:\Programs\Settings\settings.rxlt");
                }
                if (app.ToLower() == "list")
                {
                    foreach (var _app in AppsRegistry.ListApps())
                    {
                        Console.WriteLine(_app);
                    }
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
                if (!File.Exists(@"0:\Programs\SCIF\scif.rxlt"))
                {
                    Console.WriteLine("cli.scif not found - Please install it on RaxGET Store");
                    return;
                }
                Console.WriteLine("Loading SCIF...");
                RXLTRun.ExecuteFile(@"0:\Programs\SCIF\scif.rxlt");
            }
            else if (input.ToLower() == "uptime")
            {
                int nowSecond = RTC.Second;
                int nowMinute = RTC.Minute;
                int nowHour = RTC.Hour;

                int totalSecs = (nowHour - startHour) * 3600 + (nowMinute - startMinute) * 60 + (nowSecond - startSecond);

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
                Console.WriteLine($"RaxOS v{SysInfo.Version}-{SysInfo.Channel}");
                Console.WriteLine($"Type help for help.");
            }
            else if (input == "info")
            {
                Cosmos.HAL.PCSpeaker.Beep(30000);
                Console.WriteLine("System info:");
                Console.WriteLine($"RaxOS {SysInfo.Channel} v{SysInfo.Version}");
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

                    Console.WriteLine("<DIRECTORY> " + item);
                }
                foreach (var item in fils)
                {
                    Console.WriteLine("<FILE>      " + item);
                }
            }
            else if (input == "whoami")
            {
                Console.WriteLine(UserInfo.Username);
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
    }
}
