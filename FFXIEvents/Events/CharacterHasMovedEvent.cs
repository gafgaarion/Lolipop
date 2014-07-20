using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    [Serializable]
    public class CharacterHasMovedEvent : DomainEvent
    {
        public float oldX { get; set; }
        public float oldY { get; set; }
        public float oldZ { get; set; }
        public float newX { get; set; }
        public float newY { get; set; }
        public float newZ { get; set; }
        public float oldFacing { get; set; }
        public float newFacing { get; set; }
    }
}
