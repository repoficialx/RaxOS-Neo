global using Path = RaxOS_Neo.Kernel.Path;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Cosmos.Core.INTs;
using static RaxOS_Neo.Kernel;
using IOP = System.IO.Path;
using Sys = Cosmos.System;
using Console = System.Console;
using RaxOS_Neo.ExceptionHelper;
using Exception = RaxOS_Neo.ExceptionHelper.Exception;

namespace RaxOS_Neo.GUI.Screens
{
    internal class Register
    {
        public static bool logged = false;
        static Canvas canvas;
        static string username = "";
        static string password = "";
        static bool isTypingUsername = true;

        public static void Display()
        {
            // Inicializa mouse y canvas
            Sys.MouseManager.ScreenWidth = 800;
            Sys.MouseManager.ScreenHeight = 600;
            Sys.MouseManager.X = 100;
            Sys.MouseManager.Y = 100;

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));

            while (!logged)
            {
                canvas.Clear(Color.DarkGreen);

                // Dibuja título
                canvas.DrawString("RaxOS Login", PCScreenFont.Default, Color.White, 50, 20);

                // Username
                canvas.DrawString("Username:", PCScreenFont.Default, Color.White, 50, 50);
                canvas.DrawFilledRectangle(Color.White, 50, 65, 200, 20);
                canvas.DrawString(username + (isTypingUsername ? "_" : ""), PCScreenFont.Default, Color.Black, 55, 67);

                // Password (simulado visualmente con asteriscos)
                canvas.DrawString("Password:", PCScreenFont.Default, Color.White, 50, 100);
                canvas.DrawFilledRectangle(Color.White, 50, 115, 200, 20);
                string masked = new string('*', password.Length);
                canvas.DrawString(masked + (!isTypingUsername ? "_" : ""), PCScreenFont.Default, Color.Black, 55, 117);

                // Botón de login (rectángulo + texto)
                int bx = 50, by = 150, bw = 100, bh = 25;
                canvas.DrawFilledRectangle(Color.Gray, bx, by, bw, bh);
                canvas.DrawString("Register", PCScreenFont.Default, Color.White, bx + 20, by + 5);

                // Cursor del mouse
                int mouseX = (int)Sys.MouseManager.X;
                int mouseY = (int)Sys.MouseManager.Y;
                canvas.DrawLine(Color.White, mouseX - 5, mouseY, mouseX + 5, mouseY);
                canvas.DrawLine(Color.White, mouseX, mouseY - 5, mouseX, mouseY + 5);

                // Verifica si el cursor está sobre el botón
                bool mouseOverButton =
                    Sys.MouseManager.X > bx &&
                    Sys.MouseManager.X < bx + bw &&
                    Sys.MouseManager.Y > by &&
                    Sys.MouseManager.Y < by + bh;

                if (mouseOverButton)
                {
                    canvas.DrawRectangle(Color.White, bx - 1, by - 1, bw + 2, bh + 2); // borde de hover
                }

                // Verifica si se hizo clic izquierdo
                if (mouseOverButton && Sys.MouseManager.MouseState == MouseState.Left)
                {
                    Register();
                }
                /*void Register()
                {
                    canvas.Disable();
                    Thread.Sleep(50);

                    Console.WriteLine("A: antes de cualquier IO");
                    
                    string dbUser;
                    string dbPass;
                    dbUser = username;
                    dbPass = password;
                    string cleanUser = "default";
                    if (!string.IsNullOrWhiteSpace(username))
                        cleanUser = exCode.LimpiarNombre(username);

                    string hash = exCode.HashPassword(password ?? "");
                    Console.WriteLine("C: hash calculado");
                    
                    var usersDbPath = IOP.Combine(getPath(Path.SystemFolder), "users.db");
                    string[] usersDb =
                    {
                            $"username={cleanUser}",
                            $"password_hash={hash}",
                            $"hash_algo=SHA256",
                            $"created={DateTime.Now:yyyy-MM-dd}"
                        };
                    Console.WriteLine("D: array usersDb preparado");
                    
                    //canvas.Disable();
                    Thread.Sleep(50);
                    try
                    {
                        Console.WriteLine("Creating users.db...");
                        Console.WriteLine("E: antes de File.WriteAllLines");
                        File.WriteAllLines(usersDbPath, usersDb);
                        Console.WriteLine("F: después de File.WriteAllLines");
                        //Thread.Sleep(50);
                        logged = true;
                        Console.WriteLine("G: logged = true");
                    }
                    catch {
                        Exception e = new("COULDNT_CREATE_USER");
                        e.Source = "REGISTER_SCREEN";
                        e.Code = 0x70;
                        ExceptionHelper.ExceptionHandler.
                            GraphicalHandler.BSOD_GHandler(e);
                    };
                    exCode.CheckAndCreateDirectory(getPath(Path.UserDir));
                    exCode.CheckAndCreateDirectory(IOP.Combine(getPath(Path.UserDir), cleanUser));
                    exCode.CheckAndCreateDirectory(
                        IOP.Combine(getPath(Path.UserDir), cleanUser, "Documents\\")
                    );

                    Console.WriteLine("Cleaning setup leftovers...");
                    exCode.CheckAndDeleteDirectory("0:\\Dir Testing\\");
                    exCode.CheckAndDeleteDirectory("0:\\TEST\\");
                    exCode.CheckAndDeleteFile("0:\\Kudzu.txt");
                    exCode.CheckAndDeleteFile("0:\\Root.txt");

                    Console.WriteLine("Setup complete.");
                    Console.WriteLine("Press any key to reboot");
                    Console.ReadKey();
                    Thread.Sleep(500);
                    //Sys.Power.Reboot();
                    Console.WriteLine("B: fin de Register (sin IO)");
                    Thread.Sleep(500);
                }*/
                void Register()
                {
                    canvas.Disable();
                    Thread.Sleep(50);

                    Console.WriteLine("A: dentro de Register");

                    string cleanUser = username;
                    if (cleanUser == null || cleanUser == "")
                        cleanUser = "default";

                    string hash = exCode.HashPassword(password ?? "");
                    Console.WriteLine("B: hash calculado");

                    string created = "";//GetCreatedDate();
                    Console.WriteLine("C: created=" + created);

                    var usersDbPath = IOP.Combine(getPath(Path.SystemFolder), "users.db");
                    string[] usersDb =
                    {
        "username=" + cleanUser,
        "password_hash=" + hash,
        "hash_algo=SHA256",
        "created=" + created
    };
                    Console.WriteLine("D: array usersDb creado");

                    Console.WriteLine("SYSFOLDER = " + getPath(Path.SystemFolder));
                    Console.WriteLine("usersDbPath = " + usersDbPath);
                    Console.WriteLine("Dir exists? " + (Directory.Exists(getPath(Path.SystemFolder)) ? "YES" : "NO"));
                    Console.WriteLine("File exists before? " + (File.Exists(usersDbPath) ? "YES" : "NO"));
                    try
                    {
                        Console.WriteLine("E: antes de File.WriteAllLines");
                        File.WriteAllLines(usersDbPath, usersDb);
                        Console.WriteLine("F: después de File.WriteAllLines");
                        logged = true;
                    }
                    catch
                    {
                        Exception e = new("COULDNT_CREATE_USER");
                        e.Source = "REGISTER_SCREEN";
                        e.Code = 0x70;
                        Thread.Sleep(5000);
                        ExceptionHelper.ExceptionHandler.GraphicalHandler.BSOD_GHandler(e);
                    }

                    Console.WriteLine("G: antes de crear dirs");
                    // si da problemas, comenta estos uno a uno para probar
                    exCode.CheckAndCreateDirectory(getPath(Path.UserDir));
                    exCode.CheckAndCreateDirectory(IOP.Combine(getPath(Path.UserDir), cleanUser));
                    exCode.CheckAndCreateDirectory(IOP.Combine(getPath(Path.UserDir), cleanUser, "Documents\\"));

                    Console.WriteLine("H: cleaning leftovers");
                    exCode.CheckAndDeleteDirectory("0:\\Dir Testing\\");
                    exCode.CheckAndDeleteDirectory("0:\\TEST\\");
                    exCode.CheckAndDeleteFile("0:\\Kudzu.txt");
                    exCode.CheckAndDeleteFile("0:\\Root.txt");

                    Console.WriteLine("I: Setup complete. Press any key to reboot");
                    Console.ReadKey();
                    Thread.Sleep(500);
                    Sys.Power.Reboot();
                }
                // Entrada no bloqueante
                if (KeyboardManager.KeyAvailable)
                {
                    var key = KeyboardManager.ReadKey();
                    if (key.Key == ConsoleKeyEx.Enter)
                    {
                        Register();
                    }
                    else if (key.Key == ConsoleKeyEx.Tab)
                    {
                        isTypingUsername = !isTypingUsername; // alterna entre campos
                    }
                    else if (key.Key == ConsoleKeyEx.Backspace)
                    {
                        if (isTypingUsername && username.Length > 0)
                            username = username.Substring(0, username.Length - 1);
                        else if (!isTypingUsername && password.Length > 0)
                            password = password.Substring(0, password.Length - 1);
                    }
                    else if (key.KeyChar != '\0')
                    {
                        if (isTypingUsername)
                            username += key.KeyChar;
                        else
                            password += key.KeyChar;
                    }
                }

                canvas.Display();
            }

            // Al loguear exitosamente
            canvas.Clear(Color.Black);
            canvas.DrawString("¡Registro exitoso!", PCScreenFont.Default, Color.White, 50, 50);
            canvas.Display();
            Thread.Sleep(100);
            canvas.Disable();
            return;
        }
    }
}
