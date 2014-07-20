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
    public class CharacterLoginStatusChangedEvent : DomainEvent
    {
        public FFACETools.LoginStatus loginStatus { get; set; }
        public FFACETools.LoginStatus oldLoginStatus { get; set; }
    }
}
