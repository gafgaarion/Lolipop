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
    public class CharacterHasBeenInitializedEvent : DomainEvent
    {
        public FFACE instance { get; set; }
    }
}
