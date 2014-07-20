using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFACETools
{
    public class AggroShard
    {
        public int ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float H { get; set; }
        public int claimedID { get; set; }
        public int aggroedObjectId { get; set; }
        public int aggroedcharacterName { get; set; }
        public DateTime lastUpdated { get; set; }
    }
}
