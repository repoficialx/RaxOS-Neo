using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_Neo.GUI
{
    public partial class Elements
    {
        public abstract class Form
        {
            public bool IsOpen { get; set; }
            public virtual void InitializeComponent() { }
            public virtual void Draw(Canvas canvas) { }
            public virtual void Update() { }
        }
    }
}