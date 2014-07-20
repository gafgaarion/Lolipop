using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using Commons;

namespace FFXICommands.Commands
{
    public class MoveToObjectCommand : BaseCommand
    {
        public BaseAggregateRoot obj { get; set; }
        public float distanceFromDestination { get; set; }
    }
}
