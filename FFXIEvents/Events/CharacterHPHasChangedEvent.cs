using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    [Serializable]
    public class CharacterHPHasChangedEvent : DomainEvent
    {
        public int hpp { get; set; }
        public int oldHpp { get; set; }
        public int hp { get; set; }
        public int oldHp { get; set; }
    }
}
