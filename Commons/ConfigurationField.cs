using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons
{
    public class ConfigurationField
    {
        public string name { get; set; }
        public  List<string> values { get; set; }
        public  Type fieldType { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}
