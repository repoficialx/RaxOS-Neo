using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
                canvas.Clear(Color.Blue);
                canvas.DrawString("RaxOS has crashed :(", PCScreenFont.Default, Color.White, 50, 50);
                canvas.DrawString("Error code: 0x00F", PCScreenFont.Default, Color.White, 50, 70);
                canvas.DrawString("Visit repoficialx.xyz/raxos/stopcode", PCScreenFont.Default, Color.White, 50, 90);
                canvas.DrawString("Press any key to reboot", PCScreenFont.Default, Color.White, 50, 110);
                canvas.Display();
                Console.ReadKey();
                Cosmos.HAL.Power.CPUReboot();
            }
        }
    }
}
