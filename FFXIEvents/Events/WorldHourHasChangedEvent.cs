using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    public class WorldHourHasChangedEvent : DomainEvent
    {
        public byte hour { get; set; }
        public byte oldHour { get; set; }
    }
}
