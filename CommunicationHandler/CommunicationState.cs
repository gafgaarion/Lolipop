using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public class CommunicationState
    {
        public int characterName { get; set; }
        public int port { get; set; }
        public bool keepConnectionAlive { get; set; }
        
    }
}
