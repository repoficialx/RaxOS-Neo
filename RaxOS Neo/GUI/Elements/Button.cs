using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_Neo.GUI.Elements
{
    internal struct Location
    {
        public int x;
        public int y;
    }
    internal class Button
    {
        public Button() { }
        public Button(string text) { }
        public Button(string text, Bitmap icon) { }
        public string Text { get; set; }
#nullable enable
        public Bitmap? Icon { get; set; }
        public System.Drawing.Size Size { get; set; }
        public System.Drawing.Color Color { get; set; }
        public Location Location { get; set; }
        protected void Show(Canvas parentCanvas)
        {
            parentCanvas.DrawRectangle(Color, Location.x, Location.y, Size.Width, Size.Height);
            if (Icon != null) {
                parentCanvas.CroppedDrawImage(
                        image:Icon, 
                        x:Location.x+2, 
                        y:Location.y-2, 
                        maxWidth:Size.Width-2, 
                        maxHeight:Size.Height-2
                    );
                parentCanvas.DrawString(
                        str:Text, 
                        font:PCScreenFont.Default, 
                        color:System.Drawing.Color.FromArgb(255,255,255), 
                        x:Location.x+(Size.Width-2), 
                        y:Location.y-2+Size.Height
                    );
            }
            else
            {
                parentCanvas.DrawString(Text, PCScreenFont.Default, Color, Location.x + 2, Location.y - 2);
            }
        }
    }
}
