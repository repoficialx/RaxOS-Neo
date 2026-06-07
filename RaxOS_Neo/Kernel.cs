using System;
using Sys = Cosmos.Kernel.System;
// using Cosmos.Kernel.System.Vfs;  // FIXME: Commented out until CosmosVFS is available

namespace RaxOS_Neo;

/// <summary>
/// Main kernel class - inherits from Cosmos.Kernel.System.Kernel.
/// </summary>
public class Kernel : Sys.Kernel
{
    private static int startSecond;
    private static int startMinute;
    private static int startHour;

    private static int secondsUptime {get;set;} = 0;
    private static Timer timer;
    public static string user = "";
    public static string password = "";

    public static string[] CommandExecs = new string[100];
    public static int CommandExecsCount = 0;

    public enum Path
    {
        SystemDir,
        UserDir
    }
    public static string getPath(Path path) => path switch
    {
        Path.SystemDir => "0:\\RaxOS\\SYSTEM\\",
        Path.UserDir => $"0:\\USER\\",
        _ => throw new ArgumentException(nameof(path), $"Not expected direction value: {path}")
    };
    // public static VFS fs;  // FIXME: CosmosVFS not available in Cosmos gen3

    DateTime? getDTfromRTC()
    {
        if (Cosmos.Kernel.HAL.Devices.Clock.EfiRtc.TryGetTime(out long ticks))
        {
            DateTime fechaActual = new DateTime(ticks);
            return fechaActual;
        }
        return null;
    }
    int getSecondsFromTicks()
    {
        if (getDTfromRTC() == null) {return -1;}
        return getDTfromRTC()!.Value.Second;
    }
    int getMinutesFromTicks()
    {
        if (getDTfromRTC() == null) {return -1;}
        return getDTfromRTC()!.Value.Minute;
    }
    int getHoursFromTicks()
    {
        if (getDTfromRTC() == null) {return -1;}
        return getDTfromRTC()!.Value.Hour;
    }
    
    protected override void BeforeRun()
    {
        startSecond = getSecondsFromTicks();
        startMinute = getMinutesFromTicks();
        startHour = getHoursFromTicks();

        Console.Clear();
        // FIXME: VFS initialization commented out - waiting for Cosmos gen3 filesystem driver
        // fs = new CosmosVFS();
        // VFSManager.RegisterVFS(fs);
        // fs.Initialize(false);

        if (!File.Exists("0:\\RaxOS\\SYSTEM\\system.cs"))
        {
            // FIXME: Setup call commented - VFS not available yet
            // _ = exCode.Setup(fs,true);
        }
        BootScreen.Display();
        string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
        Console.WriteLine("Reading user data...");
        string[] SYSINFO = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");
        string currver = SYSINFO[6];
        Console.WriteLine("Reading system info...");
        Console.WriteLine("SYSINFO length: " + SYSINFO.Length);
        string version = null;

        for (int i = 0; i < SYSINFO.Length; i++)
        {
            if (SYSINFO[i].StartsWith("RaxOS_Version"))
            {
                version = SYSINFO[i+1];
                break;
            }
        }

        if (version != null)
        {
            Console.WriteLine($"RaxOS Version {version}");

            if (version != LatestVersion)
            {
                Console.WriteLine("Please update RaxOS Neo!");
                Console.WriteLine("To update open raxupd (run -a raxupd) in command line and pass the argument --check to check for updates.");
            }
            else
            {
                Console.WriteLine("You are running the latest version of RaxOS Neo!");
            }
        }
        else
        {
            Console.WriteLine("Failed to read RaxOS version from sysinfo.inf");
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
                    Console.WriteLine("Type 'help' to see the list of commands.");
                    Console.WriteLine("Type 'info' to see system info.");
                    Console.WriteLine("Type 'uptime' to see system uptime.");
                    Console.WriteLine("Type 'dir' to see the contents of the current directory.");
                    Console.WriteLine("Type 'scif' to run SCIF.");
                    Console.WriteLine("Type 'raxget' to install applications from the store.");
                    return;
                }
            }
        }

        Login:
        RaxOS_Neo.GUI.Screens.Login.Display();
        if (!GUI.Screens.Login.logged)
        {
            goto Login;
        }

        var kbdchosen = GUI.Screens.SelectKbd.Display();
        if (kbdchosen)
        {
            Console.Clear();
            Console.WriteLine("Welcome to RaxOS Neo, " + userData[0] + "!");
            Console.WriteLine("Type 'header' to see the header.");
            Console.WriteLine("Type 'help' to see the list of commands.");
            Console.WriteLine("Type 'info' to see system info.");
            Console.WriteLine("Type 'uptime' to see system uptime.");
            Console.WriteLine("Type 'dir' to see the contents of the current directory.");
            Console.WriteLine("Type 'scif' to run SCIF.");
            Console.WriteLine("Type 'raxget' to install applications from the store.");
        } else
        {
            Console.Clear();
            Console.WriteLine("Keyboard not selected. Please select a layout to continue.");
        }
    }

    private static string current_directory {get;set;} = "0:\\";
    public static string[] apps =
    {
        "cli.scif",
        "core.notepad",
        "system.settings",
        "system.raxget",
        "utils.raxupd"
    };

    public static string current_version {get; set;} = "0.8.1";
    public static string LatestVersion = "0.8.1";
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
            Console.WriteLine("Command history is full. Please restart.");
        }
    }

    protected override void Run()
    {
        string[] dirs = GetDirFadr(current_directory);
        string[] fils = GetFilFadr(current_directory);
        Console.Write(current_directory + "> ");
        var input = Console.ReadLine();
        AddCommand(input);
        if (input.StartsWith("echo "))
        {
            Console.WriteLine(input[5..]);
            return;
        }
        else if (input.StartsWith("shutdown"))
        {
            var x = input.Length > 9 ? input[9..].Trim() : "";

            if (x.StartsWith("-s"))
            {
                if (x.Length == 2)
                {
                    Sys.Power.Shutdown();
                } else
                {
                    var y = x.Length > 3 ? x[3..].Trim() : "";
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
                    var y = x.Length > 3 ? x[3..].Trim() : "";
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
            string gw = input[7..].Trim();
            if (gw == "list")
            {
                Console.WriteLine($"raxget function: Applications:\n" +
                $"{apps[0]}          | FUNCTION.       | 1.47 KB \n" +
                $"{apps[1]}          | APP             | 1.31 KB \n" +
                $"{apps[2]}          | SYSTEM APP      | 19.93 KB \n" +
                $"{apps[3]}          | FUNCTION        | 0.07 KB \n" +
                $"{apps[4]}          | SYSTEM FUNCTION | 2.17 KB \n");
            }
            if (gw.ToLower() == "list --SAFE")
            {
                Console.WriteLine($"raxget function: Applications:\n" +
                $"cli.scif           | FUNCTION        | 1.47 KB \n" +
                $"core.notepad       | APP             | 1.31 KB \n" +
                $"system.settings    | SYSTEM APP      | 19.93 KB \n" +
                $"system.raxget      | FUNCTION        | 0.07 KB \n" +
                $"utils.raxupd       | SYSTEM FUNCTION | 2.17 KB \n");
            }

            else if (gw.ToLower() == "install cli.scif")
            {
                string scifPath = "0:\\Programs\\SCIF\\scif.rxlt";
                if (!File.Exists(scifPath))
                {
                    string scifCode = @"command ""scif""
                    print ""SCIF - Simple Content Information of Files""
                    print ""Type any key to continue""
                    call wait_key

                    label FILEPATH
                    print ""Enter file path:""
                    call read_input path

                    if_empty path FILEPATH

                    call read_file path
                    call print_file path

                    print ""Press any key to exit SCIF.""
                    call wait_key
                    call clear_screen
                    end
                    ";
                    File.WriteAllText(scifPath, scifCode);
                    Console.WriteLine("cli.scif installed successfully!");
                }
            }
            else if (gw.ToLower() == "install core.notepad")
            {
                string simpleNanoPath = "0:\\Programs\\Nano\\simplenano.rxlt";
                if (!File.Exists(simpleNanoPath))
                {
                    string simpleNanoCode = @"command ""simplenano""
                    print ""SimpleNano v1.0""
                    print ""Type your docuent. Press ESC to finish and save.""

                    print ""File path (format: 0:\path\to\file.txt): ""
                    call read_input filepath

                    label EDIT
                    print ""Type text (ESC to finish): ""
                    call read_multiline_input path
                    print ""Saved "" path
                    call clear_screen
                    end
                    ";
                    File.WriteAllText(simpleNanoPath, simpleNanoCode);
                    Console.WriteLine("core.notepad installed successfully!");
                }
            }
            // añadir ajustes, raxget y raxupd
        }
        else if (input.ToLower().StartsWith("theme "))
        {
            string theme = input[6..];
            switch (theme.ToLower())
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
                Console.WriteLine($"Theme \"{theme}\" not found. Available themes: light, dark.");
                break;
            }
            Console.Clear();
        }
        else if (input.ToLower().StartsWith("invoke "))
        {
            string x = input[7..].Trim();
            if (x.StartsWith("exception "))
            {
                string y = x[10..].Trim();
                ExceptionHelper.Exception @new = new("INVOKED_EXCEPTION");
                @new.Code = 0x0F;
                @new.Source = current_directory;
                ExceptionHelper.ExceptionHelper.GraphicalHandler.BSOD_GHandler(@new);
            }
        }
        else if (input.ToLower().StartsWith("cd "))
        {
            string dir = input[3..].Trim();
            if (Directory.Exists(current_directory + dir))
            {
                current_directory += dir + "\\";
            }
            else
            {
                Console.WriteLine($"Directory \"{dir}\" not found.");
            }
        }
        else if (input.ToLower().StartsWith("mkdir "))
        {
            string dir = input[6..].Trim();
            if (!Directory.Exists(current_directory + dir))
            {
                Directory.CreateDirectory(current_directory + dir);
                Console.WriteLine($"Directory \"{dir}\" created.");
            }
            else
            {
                Console.WriteLine($"Directory \"{dir}\" already exists.");
            }
        }
        else if (input.ToLower().StartsWith("run -a "))
        {
            string app = input[7..].Trim();
            if (app.ToLower() == "scif")
            if (app.ToLower() == "notepad")
                {
                    if (!File.Exists(@"0:\Programs\Nano\simplenano.rxlt"))
                    {
                        Console.WriteLine("core.notepad not found. Please install it using raxget.");
                        return;
                    }
                    RXLTRun.ExecuteFile(@"0:\Programs\Nano\simplenano.rxlt");
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
            var color = input[6..].Trim();
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
                case "a":
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
                case "b":
                Console.ForegroundColor = ConsoleColor.Green;
                break;
                case "c":
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;
                case "d":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
                case "e":
                Console.ForegroundColor = ConsoleColor.White;
                break;
                case "f":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
                default:
                Console.WriteLine($"Color \"{color}\" not found. Available colors: 1-9, a-f.");
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
                case "a":
                Console.BackgroundColor = ConsoleColor.Gray;
                break;
                case "b":
                Console.BackgroundColor = ConsoleColor.Green;
                break;
                case "c":
                Console.BackgroundColor = ConsoleColor.Magenta;
                break;
                case "d":
                Console.BackgroundColor = ConsoleColor.Red;
                break;
                case "e":
                Console.BackgroundColor = ConsoleColor.White;
                break;
                case "f":
                Console.BackgroundColor = ConsoleColor.Yellow;
                break;
                default:
                Console.WriteLine($"Color \"{color}\" not found. Available colors: 1-9, a-f.");
                break;
            }
            return;
        }
        else if (input.ToLower() == "clear")
        {
            Console.Clear();
            return;
        }
        else if (input.ToLower() == "scif")
        {
            if (!File.Exists(@"0:\Programs\SCIF\scif.rxlt"))
            {
                Console.WriteLine("cli.scif not found. Please install it using raxget.");
                return;
            }
            Console.WriteLine("Running SCIF...");
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

            Console.WriteLine($"Uptime: {hours} hours, {minutes} minutes, {seconds} seconds");
        }
        else if (input.ToLower() == "dir")
        {
            Console.WriteLine("Directories:");
            foreach (var dir in dirs)
            {
                Console.WriteLine($"<DIR> {dir}");
            }
            Console.WriteLine("Files:");
            foreach (var fil in fils)
            {
                Console.WriteLine($"      {fil}");
            }
        }
        else if (input.ToLower() == "sysinfo")
        {
            string[] sysinfo = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");
            foreach (var line in sysinfo)
            {
                Console.WriteLine(line);
            }
        }
        else if (input.ToLower() == "header")
        {
            Console.WriteLine($"RaxOS Neo v{SysInfo.Version}-{SysInfo.Channel}");
            Console.WriteLine($"Type 'help' to see the list of commands.");
        }
        else if (input.ToLower() == "info")
        {
            Cosmos.HAL.PCSpeaker.Beep(30000);
            Console.WriteLine("System info:");
            Console.WriteLine($"RaxOS {SysInfo.Channel} v{SysInfo.Version}");
            Console.WriteLine($"CPU Brand : {CPU.GetCPUBrand()}");
            Console.WriteLine($"CPU Vendor: {CPU.GetCPUVendor()}");
            Console.WriteLine($"CPU Speed : {CPU.GetCPUSpeed()} MHz");
            Console.WriteLine($"RAM       : {RAM.TotalMB} MB");
            Console.WriteLine($"Storage   : {SysInfo.Storage} MB");
            Console.WriteLine($"Is VM?    : {GetIsVM()}\n");
        }
        else
        {
            Console.WriteLine($"Command \"{input}\" not found. Type 'help' to see the list of commands.");
        }
    }
}
