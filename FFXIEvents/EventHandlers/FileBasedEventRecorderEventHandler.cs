using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using System.IO;
using FFXIVARREvents.Events;
using ProtoBuf;

namespace FFXIVARREvents.EventHandlers
{
    public class FileBasedEventRecorderEventHandler : IHandleEvent<CharacterHasMovedEvent>,
                                                IHandleEvent<CharacterAvailableForActionEvent>,
                                            IHandleEvent<CharacterIsBusyWithActionEvent>,
                                            IHandleEvent<CharacterHPHasChangedEvent>,
                                        IHandleEvent<CharacterHasBeenInitializedEvent>
    {
        private IEventSerializer eventSerializer;
    
        public FileBasedEventRecorderEventHandler(IEventSerializer eventSerializer)
        {
            this.eventSerializer = eventSerializer;
        }

        public void Handle(CharacterHasMovedEvent domainEvent)
        {
            eventSerializer.SerializeEvent<CharacterHasMovedEvent>(domainEvent);
        }

        public void Handle(CharacterIsBusyWithActionEvent domainEvent)
        {
            eventSerializer.SerializeEvent<CharacterIsBusyWithActionEvent>(domainEvent);
        }

        public void Handle(CharacterAvailableForActionEvent domainEvent)
        {
            eventSerializer.SerializeEvent<CharacterAvailableForActionEvent>(domainEvent);
        }

        public void Handle(CharacterHPHasChangedEvent domainEvent)
        {
            eventSerializer.SerializeEvent<CharacterHPHasChangedEvent>(domainEvent);
        }

        public void Handle(CharacterHasBeenInitializedEvent domainEvent)
        {
            eventSerializer.SerializeEvent<CharacterHasBeenInitializedEvent>(domainEvent);
        }
    }
}
