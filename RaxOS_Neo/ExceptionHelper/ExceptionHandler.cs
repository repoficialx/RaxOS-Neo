using Cosmos.Kernel.System;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = System.Console;
using Sys = Cosmos.Kernel.System;
 
namespace RaxOS_Neo.ExceptionHelper
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
            Cosmos.Kernel.HAL.PlatformHAL.PowerOps.Reboot();
            //canvas.Display();
        }
        internal class GraphicalHandler
        {
            public GraphicalHandler() { }
            public static void BSOD_GHandler(Exception ex)
            {
                // Start mouse
                Sys.Mouse.MouseManager.ScreenWidth = 800;
                Sys.Mouse.MouseManager.ScreenHeight = 600;
                //Sys.Mouse.MouseManager.X = 0;
                //Sys.Mouse.MouseManager.Y = 0;

                Redraw(ex);

                canvas.Display();
                var start = DateTime.Now;

                while (true)
                {
                    canvas.Clear();
                    Redraw(ex);
                    int mouseX = (int)Sys.Mouse.MouseManager.X;
                    int mouseY = (int)Sys.Mouse.MouseManager.Y;

                    // Dibuja una cruz como cursor
                    canvas.DrawLine(Color.White, mouseX - 5, mouseY, mouseX + 5, mouseY); // línea horizontal
                    canvas.DrawLine(Color.White, mouseX, mouseY - 5, mouseX, mouseY + 5); // línea vertical

                    if (Sys.Mouse.MouseManager.LeftButton)
                    {
                        if (Sys.Mouse.MouseManager.X > 50 && Sys.Mouse.MouseManager.X < 175)
                        {
                            if (Sys.Mouse.MouseManager.Y > 150 && Sys.Mouse.MouseManager.Y < 175)
                            {
                                Cosmos.Kernel.HAL.PlatformHAL.PowerOps.Reboot();
                            }
                        }
                    }

                    canvas.Display();
                    var elapsed = (DateTime.Now - start).TotalSeconds;
                    if (elapsed >= 25)
                    {
                        Cosmos.Kernel.HAL.PlatformHAL.PowerOps.Reboot();
                    }

                }
            }
            static void Redraw(Exception ex)
            {
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
                canvas.Clear(Color.Blue);
                canvas.DrawString("RaxOS has crashed :(", PCScreenFont.DefaultFont, Color.White, 50, 50);
                canvas.DrawString("Error code: "+ex.Message+" / "+ex.Code, PCScreenFont.DefaultFont, Color.White, 50, 70);
                canvas.DrawString("Source: "+ex.Source, PCScreenFont.DefaultFont, Color.White, 50, 90);
                canvas.DrawString("Visit repoficialx.xyz/raxos/stopcode", PCScreenFont.DefaultFont, Color.White, 50, 110);
                canvas.DrawString("Press Reboot to reboot or wait 25 seconds", PCScreenFont.DefaultFont, Color.White, 50, 130);
                int x = 50;
                int y = 150;
                int width = 125;
                int height = 25;
                

                // Restart button
                canvas.DrawFilledRectangle(Color.White, x, y, width, height); // fondo del botón
                canvas.DrawString("Reboot", PCScreenFont.DefaultFont, Color.Black, x + 5, y + 5); // texto
            }
        }
    }
}
