using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using Commons;

namespace FFXICommands.Commands
{
    public class MoveToLocationCommand : BaseCommand
    {
        public float destinationX { get; set; }
        public float destinationZ { get; set; }
        public float distanceFromDestination { get; set; }
    }
}
