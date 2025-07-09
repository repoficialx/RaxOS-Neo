using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_BETA.ExceptionHelper
{
    internal class ExceptionHandler
    {
        static Canvas canvas;
        public ExceptionHandler() 
        { 

        }
        public static void BSoD_Handler(Exception ex)
        {
            Console.Clear();
            canvas.Clear(Color.DarkBlue);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(":( Your PC has a problem and it needs to restart.");
            Console.WriteLine($"Error code: {ex.HResult}. Visit https://cricuni.x10.mx/empresas/iNS/raxos/stopcode for more info.");
        }
        internal class GraphicalHandler
        {
            public GraphicalHandler() { }
            public static void BSOD_GHandler(Exception ex)
            {
                byte[] buffer = new byte[1024];
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));
                canvas.Clear(Color.AliceBlue);
            }
        }
    }
}
