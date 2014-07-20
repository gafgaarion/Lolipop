using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFACETools;

namespace FFXICommands.Commands
{
    public class UpdateCharacterStatusCommand : BaseCommand
    {
        public FFACE.PlayerTools player { get; set; }
        public FFACE.TargetTools target { get; set; }
        public FFACE.PartyTools party { get; set; }
        public List<FFACE.ChatTools.ChatLine> chat { get; set; }
    }
}
