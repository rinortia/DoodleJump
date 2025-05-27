using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doodle_Jump.Modals
{
    public class NormalPlatform : PlatformBase
    {
        public NormalPlatform(float x, float y) : base(x, y) { }

        protected override Brush PlatformBrush => Brushes.SkyBlue;
    }
}
