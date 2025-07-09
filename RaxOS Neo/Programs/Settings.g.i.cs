using Cosmos.System;
using RaxOS_BETA .Programs.ProgramHelper;
using System;
using System.IO;
using System.Threading;
using c = System.Console;

namespace RaxOS_BETA.Programs
{
    internal partial class Settings
    {
        internal class SettingsMenu:Program
        {
            public static void Launch()
            {
                mv = 1;
                AppName = "Settings";
                sv = 0;
                cv = 0;
                rv = 0;
                AppDescription = "Settings Menu to configure your RaxOS Computer.";
                IsStable = true;
                MainLoop();
            }

            private static void MainLoop()
            {
                c.WriteLine($"{AppName} v{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}");
                c.Write("Press any key to continue");
                c.ReadKey();
                c.Clear();
            SETTING:
                c.Write("Type setting rxlt [help:help]:");
#nullable enable
                string? rxlt = c.ReadLine();
#nullable disable
                if (rxlt == "" || rxlt == null)
                {
                    c.WriteLine("Please type a file.");
                    goto SETTING;
                }
                else if (rxlt == "help")
                {
                    c.WriteLine("rsl [--Px320Bt4, --Px320Bt8, --Px640Bt4, --Px720Bt16]\n" +
                        "net [--init]\n" +
                        "factory [--true]");
                    goto SETTING;
                }
                else if (rxlt == "rsl")
                {
                    {
                        c.WriteLine("rsl --Px[resol+bitcolor]");
                        c.WriteLine("resol+bitcolor: 320Bt4, 320Bt8, 640Bt4, 720Bt16");
                    }
                    {
                        goto SETTING;
                    }
                }
                else if (rxlt.StartsWith("rsl "))
                {string tmp = rxlt.Substring(4);
                    {
                        if (tmp == "--Px320Bt4") {
                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px320Bt4);
                        }
                        else if (tmp == "--Px320Bt8")
                        {
                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px320Bt8);
                        }
                        else if (tmp == "--Px640Bt4")
                        {
                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px640Bt4);
                        }
                        else if (tmp == "--Px720Bt16")
                        {
                            ResolutionSettings.SetResolution(ResolutionSettings.Mode.Px720Bt16);
                        }
                        else
                        {
                            c.WriteLine("rsl --Px[resol+bitcolor]");
                            c.WriteLine("resol+bitcolor: 320Bt4, 320Bt8, 640Bt4, 720Bt16");
                            c.WriteLine("EXAMPLES: rsl --Px320Bt4, rsl --Px720Bt16");
                        }
                    }
                    
                }
                else if (rxlt == "net --init")
                {
                    Cosmos.HAL.Network.NetworkInit.Init();
                    goto SETTING;
                }
                else if (rxlt == "net")
                {
                    c.WriteLine("net --init: Calls HAL.Network.NetworkInit.Init(); method");
                    goto SETTING;
                }
                else if (rxlt == "factory")
                {
                    c.WriteLine("WARNING!: The \"factory --true\" deletes the OS! USE ONLY IF YOU WILL REINSTALL RAXOS OR INSTALL ANOTHER OS.");
                    goto SETTING;
                }
                else if (rxlt == "factory --true")
                {
                    c.WriteLine("WARNING!: The \"factory --true\" deletes the OS! USE ONLY IF YOU WILL REINSTALL RAXOS OR INSTALL ANOTHER OS.");
                    c.WriteLine("ARE YOU SURE TO DELETE THE OS?? [Y/N]");
#nullable enable
                    string? ind = c.ReadLine().ToUpper();
#nullable disable
                    if (ind == "Y")
                    {
                        SystemReserved.DONOTENTER.Format();
                    }
                    else
                    {
                        c.WriteLine("Good!");
                    }
                    goto SETTING;
                }

                c.WriteLine("Press any key to exit.");
                c.ReadKey();
                Close();
            }
            private static void Close()
            {
                c.Clear();

            }
            public static void BSoD()
            {
                c.Clear();
                c.WriteLine(":(          .");
            }
        }
    }
}
namespace SystemReserved
{
    public class DONOTENTER
    {
        public static void Format()
        {
            try
            {
                c.WriteLine("DELETING RaxOS... [Your computer will be unbootable!]");
                #region Uninstaller
                //Custom Installer (Now you see why

                c.WriteLine("Welcome to RAXOS Uninstaller.");
                Cosmos.System.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();
                //Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(fs);
                c.Clear();
                Directory.Delete("0:\\SYSTEM", true);
                c.WriteLine("Deleting users.db...");
                Directory.Delete("0:\\Dir Testing\\", true);
                c.WriteLine("Deleting cache...");
                Directory.Delete("0:\\TEST\\", true);
                c.WriteLine("Deleting Uninstaller ...");
                File.Delete("0:\\Kudzu.txt");
                c.WriteLine("  ...");
                File.Delete("0:\\Root.txt");
                c.WriteLine(" ...");
                Directory.Delete("0:\\Documents\\");
                c.WriteLine(" ...");
                c.WriteLine("    ");
                c.ReadKey();
                //Cosmos.System.Power.Reboot();
                RaxOS_BETA.Programs.Settings.SettingsMenu.BSoD();
                Thread.Sleep(3000);
                Cosmos.HAL.Power.CPUReboot();
                // There is no SYSTEM directory yet, so we just shut the computer down there
                #endregion
            }
            catch (Exception ex)
            {
                RaxOS_BETA.ExceptionHelper.ExceptionHandler.BSoD_Handler(ex);
            }
        }
    }
}