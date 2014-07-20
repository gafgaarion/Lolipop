using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFACETools;

namespace FFXICommands.Commands
{
    public class UpdateChatCommand : BaseCommand
    {
        public List<FFACE.ChatTools.ChatLine> lines { get; set; }
    }
}
