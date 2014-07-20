using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFACETools;

namespace FFXIEvents.Events
{
    public class AllianceActionUsedEvent : DomainEvent
    {
        public int objectId { get; set; }
        public object Skill { get; set; }
    }
}
