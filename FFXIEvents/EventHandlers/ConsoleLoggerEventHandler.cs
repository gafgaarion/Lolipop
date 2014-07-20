using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXIEvents.Events;
using Commons;

namespace FFXIEvents.EventHandlers
{
    class ConsoleLoggerEventHandler : IHandleEvent<CharacterHasBeenInitializedEvent>,
                                                IHandleEvent<CharacterHasDisconnectedEvent>,
                                                IHandleEvent<CharacterHasMovedEvent>
              
    {
        public void Handle(CharacterHasBeenInitializedEvent domainEvent)
        {
            
            Console.WriteLine(domainEvent.characterName + " has been registered as character #" + domainEvent.characterName);
        }

        public void Handle(CharacterHasDisconnectedEvent domainEvent)
        {
            Console.WriteLine(domainEvent.characterName + " has disconnected.");
        }

        public void Handle(CharacterHasMovedEvent domainEvent)
        {
            Console.WriteLine("Character " + domainEvent.characterName + " moved from ("+domainEvent.oldX+","+domainEvent.oldY
                    +"," + domainEvent.oldZ+") to ("+domainEvent.newX+","+domainEvent.newY+","+domainEvent.newZ+"). Facing " + domainEvent.oldFacing + " -> " + domainEvent.newFacing  );
        }

        public void Handle(CharacterHPHasChangedEvent domainEvent)
        {
            Console.WriteLine("Character " + domainEvent.characterName + " HP changed from " + domainEvent.oldHp + "(" + 
                domainEvent.oldHpp+") to " + domainEvent.hp+ "(" + domainEvent.hpp+") .");
        }

         public void Handle(CharacterTPHasChangedEvent domainEvent)
        {
            Console.WriteLine("Character " + domainEvent.characterName + " TP changed to " + domainEvent.tp +" .");
        }

         public void Handle(CharacterMPHasChangedEvent domainEvent)
         {
             Console.WriteLine("Character " + domainEvent.characterName + " MP changed from " + domainEvent.oldMp + " to " + domainEvent.mp +" .");
         }

         public void Handle(CharacterAvailableForActionEvent domainEvent)
         {
             Console.WriteLine("Character " + domainEvent.characterName + " is no longer busy.");
         }

         public void Handle(CharacterIsBusyWithActionEvent domainEvent)
         {
             Console.WriteLine("Character " + domainEvent.characterName + " is busy.");
         }



         private bool NodeIsSpawned(Byte spawnFlag)
         {
             return (spawnFlag & (1 << 0)) != 0;
         }


    }
}
