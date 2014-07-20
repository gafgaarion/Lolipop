using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFACETools;

namespace FFXICommands.Commands
{
    public class InitializeCharacterCommand : BaseCommand
    {
        public string characterName { get; set; }
        public FFACE instance { get; set; }
    }
}
