using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    [Serializable]
    public class CharacterTPHasChangedEvent : DomainEvent
    {
        public int tp { get; set; }
    }
}
