using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using Commons;
using FFXIAggregateRoots;
using FFACETools;

namespace FFXICommands.Commands
{
    public class TradeItemToTargetCommand : BaseCommand
    {
        public ObjectAggregateRoot obj { get; set; }
        public List<FFACE.TRADEITEM> itemsToTrade { get; set; }
        public int gil { get; set;}
    }
}
