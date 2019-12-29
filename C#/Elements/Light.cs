using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    public class Light : Element
    {
        public static Light Main { get; set; }

        public Colour Colour { get; set; } = new Colour(255, 255, 255);
    }
}
