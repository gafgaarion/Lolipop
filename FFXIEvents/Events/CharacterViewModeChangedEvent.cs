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
    public class CharacterViewModeChangedEvent : DomainEvent
    {
        public FFACETools.ViewMode viewMode { get; set; }
        public FFACETools.ViewMode oldViewMode { get; set; }
    }
}
