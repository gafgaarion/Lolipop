using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using Commons;
using FFXIAggregateRoots;

namespace FFXICommands.Commands
{
    public class BeginFightingTargetCommand : BaseCommand
    {
        public ObjectAggregateRoot obj { get; set; }
    }
}
