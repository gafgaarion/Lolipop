using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{

    public class ObjectHasBeenDiscoveredEvent : DomainEvent
    {
        public int objectId { get; set; }
        public string objectName { get; set; }
        public int hpp { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float h { get; set; }
        public int NPCTalking { get; set; }
        public int PetIndex { get; set; }
        public int ClaimID { get; set; }
        public FFACETools.Status status { get; set; }
        public FFACETools.NPCType type { get; set; }
    }
}
