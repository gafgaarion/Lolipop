using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    public class ObjectHasAggroedCharacterEvent : DomainEvent
    {
        public int objectId { get; set; }
    }
}
