using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFACETools;
using System.Collections.Concurrent;

namespace FFXIEvents.Events
{
    public class CharacterAggroListChangedEvent : DomainEvent
    {
        public List<AggroShard> aggroList { get; set; }
    }
}
