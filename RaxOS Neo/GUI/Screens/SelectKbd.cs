using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.ScanMaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using K = Cosmos.System.KeyboardManager;
using System.Threading.Tasks;
using Console = System.Console;
using Sys = Cosmos.System;

namespace RaxOS_Neo.GUI.Screens
{
    public static class SelectKbd
    {
        static Canvas canvas;
        static bool keyboardSelected = false;
        static bool DisplayButton(string text, int x, int y, int width, int height)
        {
            // Dibuja el botón
            canvas.DrawFilledRectangle(Color.Gray, x, y, width, height);
            canvas.DrawString(text, PCScreenFont.Default, Color.White, x + 20, y + 5);
            // Verifica si el mouse está sobre el botón
            bool mouseOverButton =
                Sys.MouseManager.X > x &&
                Sys.MouseManager.X < x + width &&
                Sys.MouseManager.Y > y &&
                Sys.MouseManager.Y < y + height;
            if (mouseOverButton)
            {
                canvas.DrawRectangle(Color.White, x - 1, y - 1, width + 2, height + 2); // borde de hover
            }
            // Verifica si se hace clic
            if (Sys.MouseManager.MouseState == MouseState.Left && mouseOverButton)
            {
                return true; // El botón fue presionado
            }
            return false; // El botón no fue presionado
        }
        public static bool Display()
        {
            // Inicializa mouse y canvas
            Sys.MouseManager.ScreenWidth = 800;
            Sys.MouseManager.ScreenHeight = 600;
            Sys.MouseManager.X = 100;
            Sys.MouseManager.Y = 100;

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));

            while (!keyboardSelected)
            {
                canvas.Clear(Color.DarkGreen);

                // Dibuja el título
                canvas.DrawString("RaxOS Keyboard Selection", PCScreenFont.Default, Color.White, 50, 20);
                canvas.DrawString("Please select your keyboard layout:", PCScreenFont.Default, Color.White, 50, 50);

                // Dibuja los botones para cada layout
                int buttonY = 100;
                if (DisplayButton("Spanish (ES)", 50, buttonY, 200, 30))
                {
                    K.SetKeyLayout(new ESStandardLayout());
                    keyboardSelected = true;
                    break;
                }
                buttonY += 40;
                if (DisplayButton("English (EN)", 50, buttonY, 200, 30))
                {
                    K.SetKeyLayout(new GBStandardLayout());
                    keyboardSelected = true;
                    break;
                }
                buttonY += 40;
                if (DisplayButton("Turkish (TR)", 50, buttonY, 200, 30))
                {
                    K.SetKeyLayout(new TRStandardLayout());
                    keyboardSelected = true;
                    break;
                }
                buttonY += 40;
                if (DisplayButton("German (DE)", 50, buttonY, 200, 30))
                {
                    K.SetKeyLayout(new DEStandardLayout());
                    keyboardSelected = true;
                    break;
                }
                buttonY += 40;
                if (DisplayButton("French (FR)", 50, buttonY, 200, 30))
                {
                    K.SetKeyLayout(new FRStandardLayout());
                    keyboardSelected = true;
                    break;
                }
                buttonY += 40;
                if (DisplayButton("English (US)", 50, buttonY, 200, 30))
                {
                    K.SetKeyLayout(new USStandardLayout());
                    keyboardSelected = true;
                    break;
                }
                // Cursor del mouse
                int mouseX = (int)Sys.MouseManager.X;
                int mouseY = (int)Sys.MouseManager.Y;
                canvas.DrawLine(Color.White, mouseX - 5, mouseY, mouseX + 5, mouseY);
                canvas.DrawLine(Color.White, mouseX, mouseY - 5, mouseX, mouseY + 5);
                canvas.Display();
            }
            // Al loguear exitosamente
            canvas.Clear(Color.Black);
            canvas.DrawString("¡Eleccion de teclado exitosa!", PCScreenFont.Default, Color.White, 50, 50);
            canvas.Display();
            Thread.Sleep(100);
            canvas.Disable();
            return true;
        }
    }
}
