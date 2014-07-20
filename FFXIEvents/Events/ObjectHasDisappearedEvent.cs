using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{

    public class ObjectHasDisappearedEvent : DomainEvent
    {
        public int objectId { get; set; }
        public string objectName { get; set; }
        public FFACETools.Status status { get; set; }
        public FFACETools.NPCType type { get; set; }
    }
}
