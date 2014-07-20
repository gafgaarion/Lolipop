using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    public class ObjectHasBeenStaggeredEvent : DomainEvent
    {
        public int objectId { get; set; }
        public string staggeredBy { get; set; }
        public int staggeredById { get; set; }
    }
}