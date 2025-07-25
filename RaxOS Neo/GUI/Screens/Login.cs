using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;

namespace RaxOS_Neo.GUI.Screens
{
    /*public static class Login
    {
        public static bool logged = false;
        static Canvas canvas;
        private static string u;
        private static string p;
        public static void Display()
        {
            //canvas.DrawString("TEST", PCScreenFont.Default, Color.White, 10, 10);
            //canvas.DrawFilledRectangle(Color.Red, 20, 20, 50, 25); // Rectángulo sencillo
            // Start mouse
            Sys.MouseManager.ScreenWidth = 800;
            Sys.MouseManager.ScreenHeight = 600;
            Sys.MouseManager.X = 0;
            Sys.MouseManager.Y = 0;

            InitCanvas();
            Redraw();

            canvas.Display();
            var start = DateTime.Now;

            while (false)
            {
                canvas.Clear();         // Limpia todo
                Redraw();               // Dibuja los elementos UI

                // 💡 Dibuja el cursor DESPUÉS de Redraw
                int mouseX = (int)Sys.MouseManager.X;
                int mouseY = (int)Sys.MouseManager.Y;
                canvas.DrawString("MouseX: " + mouseX, PCScreenFont.Default, Color.White, 10, 10);
                canvas.DrawString("MouseY: " + mouseY, PCScreenFont.Default, Color.White, 10, 30);

                canvas.DrawLine(Color.White, mouseX - 5, mouseY, mouseX + 5, mouseY);     // línea horizontal
                canvas.DrawLine(Color.White, mouseX, mouseY - 5, mouseX, mouseY + 5);

                if (MxUI.MxUIElements.Button(canvas, "Login", 50, 150, 100, 25))
                {
                    string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
                    string username = userData[0];
                    string password = userData[1];
                    string u=username; 
                    string p=password;

                    if (Login.u == u && Login.p == p)
                    {
                        logged = true;
                        return;
                    }
                }
                canvas.Display(); // muestra los cambios si usas doble buffer
            }

        }
        static void InitCanvas()
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
        }
        static void Redraw()
        {
            canvas.Clear(Color.Green);
            var username = ""; // MxUI.MxUIElements.TextBox(canvas, "Username:", 50, 50);
            var password = ""; //MxUI.MxUIElements.TextBox(canvas, "Password:", 50, 100, 200, isPassword: true);
            u = username;
            p = password;
        }
    }*/

    public static class Login
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
                canvas.DrawString("Login", PCScreenFont.Default, Color.White, bx + 20, by + 5);

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
                    string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
                    string dbUser = userData[0];
                    string dbPass = userData[1];

                    if (username == dbUser && password == dbPass)
                        logged = true;
                }

                // Entrada no bloqueante
                if (KeyboardManager.KeyAvailable)
                {
                    var key = KeyboardManager.ReadKey();
                    if (key.Key == ConsoleKeyEx.Enter)
                    {
                        // Validación básica
                        string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
                        string dbUser = userData[0];
                        string dbPass = userData[1];

                        if (username == dbUser && password == dbPass)
                            logged = true;
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
            canvas.DrawString("¡Login exitoso!", PCScreenFont.Default, Color.White, 50, 50);
            canvas.Display();
            Thread.Sleep(100);
            canvas.Disable();
            return;
        }
    }
}
