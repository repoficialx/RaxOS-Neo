using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = System.Console;
using Sys = Cosmos.System;

namespace RaxOS_BETA.ExceptionHelper
{
    public class Exception
    {
        public string Message;
        public int Code;
        public string Source;

        public Exception(string message)
        {
            Message = message;
        }
    }

    internal class ExceptionHandler
    {
        static Canvas canvas;
        public ExceptionHandler() 
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas();

        }
        public static void BSoD_Handler(Exception ex, bool gui = false)
        {
            //debug:
            gui = true;

            if (gui) { GraphicalHandler.BSOD_GHandler(ex); return; }
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            //canvas.Clear(Color.DarkBlue);
            
            Console.WriteLine(":( Your PC has a problem and it needs to restart.");
            Console.WriteLine($"Error code: {ex.Message} / {ex.Code}");
            Console.WriteLine($"Source: {ex.Source}");
            Console.WriteLine("More info: https://repoficialx.xyz/raxos/stopcode");
            Console.ReadKey();
            Cosmos.HAL.Power.CPUReboot();
            //canvas.Display();
        }
        internal class GraphicalHandler
        {
            public GraphicalHandler() { }
            public static void BSOD_GHandler(Exception ex)
            {
                // Start mouse
                Sys.MouseManager.ScreenWidth = 800;
                Sys.MouseManager.ScreenHeight = 600;
                Sys.MouseManager.X = 0;
                Sys.MouseManager.Y = 0;

                Redraw();

                canvas.Display();
                var start = DateTime.Now;

                while (true)
                {
                    canvas.Clear();
                    Redraw();
                    //var mouse = Sys.MouseManager;
                    int mouseX = (int)Sys.MouseManager.X;
                    int mouseY = (int)Sys.MouseManager.Y;

                    // Dibuja una cruz como cursor
                    canvas.DrawLine(Color.White, mouseX - 5, mouseY, mouseX + 5, mouseY); // línea horizontal
                    canvas.DrawLine(Color.White, mouseX, mouseY - 5, mouseX, mouseY + 5); // línea vertical

                    if (Sys.MouseManager.MouseState == MouseState.Left)
                    {
                        // Acción cuando se hace clic izquierdo
                        //canvas.DrawString("¡Click izquierdo!", PCScreenFont.Default, Color.White, 10, 10);
                        if (MouseManager.X > 50 && MouseManager.X < 175)
                        {
                            if (MouseManager.Y > 150 && MouseManager.Y < 175)
                            {
                                Cosmos.HAL.Power.CPUReboot();
                            }
                        }
                    }

                    if (Sys.MouseManager.MouseState == MouseState.Right)
                    {
                        // Acción cuando se hace clic derecho
                        //canvas.DrawString("¡Click derecho!", PCScreenFont.Default, Color.White, 10, 30);
                    }

                    canvas.Display(); // muestra los cambios si usas doble buffer
                    var elapsed = (DateTime.Now - start).TotalSeconds;
                    if (elapsed >= 25)
                    {
                        Cosmos.HAL.Power.CPUReboot();
                    }

                }
            }
            static void Redraw()
            {
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
                canvas.Clear(Color.Blue);
                canvas.DrawString("RaxOS has crashed :(", PCScreenFont.Default, Color.White, 50, 50);
                canvas.DrawString("Error code: 0x00F", PCScreenFont.Default, Color.White, 50, 70);
                canvas.DrawString("Visit repoficialx.xyz/raxos/stopcode", PCScreenFont.Default, Color.White, 50, 90);
                canvas.DrawString("Press any key to reboot", PCScreenFont.Default, Color.White, 50, 110);
                int x = 50;
                int y = 150;
                int width = 125;
                int height = 25;
                

                // Restart button
                canvas.DrawFilledRectangle(Color.White, x, y, width, height); // fondo del botón
                canvas.DrawString("Restart now", PCScreenFont.Default, Color.Black, x + 5, y + 5); // texto
            }
        }
    }
}
