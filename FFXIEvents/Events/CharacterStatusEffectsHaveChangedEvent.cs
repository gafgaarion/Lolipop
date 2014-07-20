using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFACETools;

namespace FFXIEvents.Events
{
    [Serializable]
    public class CharacterStatusEffectsHaveChangedEvent : DomainEvent
    {
        public FFACETools.StatusEffect[] statusEffects { get; set; }
        public FFACETools.StatusEffect[] oldStatusEffects { get; set; }
    }
}
