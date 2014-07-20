using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFACETools;

namespace FFXIEvents.Events
{
    [Serializable]
    public class CharacterUniqueIDHasChanged : DomainEvent
    {
        public int uniqueID { get; set; }
        public int oldUniqueID { get; set; }
    }
}
