using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_Neo.GUI
{
    internal class MouseManagement
    {
        public static void Display(uint X, uint Y, Canvas canvas)
        {
            int mouseX = (int)X;
            int mouseY = (int)Y;
            canvas.DrawLine(Color.White, mouseX - 5, mouseY, mouseX + 5, mouseY);
            canvas.DrawLine(Color.White, mouseX, mouseY - 5, mouseX, mouseY + 5);
        }
    }
}
