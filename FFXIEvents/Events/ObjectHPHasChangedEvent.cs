using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    public class ObjectHPHasChangedEvent : DomainEvent
    {
        public int objectId { get; set; }
        public int hpp { get; set; }
        public int oldHpp { get; set; }
    }
}
