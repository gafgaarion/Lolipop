using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIWorldKnowledge
{
    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString()
        {
            return "X:" + X.ToString() + " Y:" + Y.ToString() + " Z:" + Z.ToString();
        }
    }
}
