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
    class UpdateVisualServerEventHandler : IHandleEvent<CharacterHasBeenInitializedEvent>,
                                           IHandleEvent<CharacterHasDisconnectedEvent>,
                                           IHandleEvent<CharacterHasMovedEvent>,
                                           IHandleEvent<CharacterHPHasChangedEvent>,
                                           IHandleEvent<CharacterTPHasChangedEvent>,
                                           IHandleEvent<CharacterMPHasChangedEvent>,
                                           IHandleEvent<CharacterAvailableForActionEvent>,
                                           IHandleEvent<CharacterIsBusyWithActionEvent>,
                                           IHandleEvent<CharacterHasChangedMapEvent>,
                                           IHandleEvent<CharacterHasChangedTargetEvent>,
                                           IHandleEvent<CharacterCastProgressChangedEvent>,
                                           IHandleEvent<CharacterLoginStatusChangedEvent>,
                                           IHandleEvent<CharacterNameChangedEvent>,
                                           IHandleEvent<CharacterViewModeChangedEvent>,
                                           IHandleEvent<CharacterStatusHasChangedEvent>,
                                           IHandleEvent<CharacterStatusEffectsHaveChangedEvent>,
                                           IHandleEvent<CharacterUniqueIDHasChanged>
    //          IHandleEvent<MonsterHasAggroedCharacterEvent>,
    //             IHandleEvent<MonsterHasBeenClaimedEvent>,
    //           IHandleEvent<MonsterHasBeenDefeatedEvent>,
    //          IHandleEvent<MonsterHasBeenDiscoveredEvent>
    //           IHandleEvent<MonsterHasMovedEvent>,
    //                 IHandleEvent<MonsterHPHasChangedEvent>,
    //        IHandleEvent<CharacterJobAbilityIsNoLongerOnCooldownEvent>,
    //       IHandleEvent<CharacterJobAbilityIsNowOnCooldownEvent>//,
    //   IHandleEvent<CharacterHasSwitchedTargetsEvent>
    {
        public void Handle(CharacterHasBeenInitializedEvent domainEvent)
        {
            GlobalDelegates.onHasBeenInitialized(domainEvent);
        }

        public void Handle(CharacterHasChangedMapEvent domainEvent)
        {
            GlobalDelegates.onMapHasChanged(domainEvent);
        }

        public void Handle(CharacterHasDisconnectedEvent domainEvent)
        {
            GlobalDelegates.onHasDisconnected(domainEvent);
        }

        public void Handle(CharacterHasMovedEvent domainEvent)
        {
            GlobalDelegates.onHasMoved(domainEvent);
        }

        public void Handle(CharacterHPHasChangedEvent domainEvent)
        {
            GlobalDelegates.onHPHasChanged(domainEvent);
        }

        public void Handle(CharacterTPHasChangedEvent domainEvent)
        {
            GlobalDelegates.onTPHasChanged(domainEvent);
        }

        public void Handle(CharacterMPHasChangedEvent domainEvent)
        {
            GlobalDelegates.onMPHasChanged(domainEvent);
        }

        public void Handle(CharacterAvailableForActionEvent domainEvent)
        {
            GlobalDelegates.onAvailable(domainEvent);
        }

        public void Handle(CharacterIsBusyWithActionEvent domainEvent)
        {
            GlobalDelegates.onIsBusyWithAction(domainEvent);
        }



   
        public void Handle(CharacterHasChangedTargetEvent domainEvent)
        {
            GlobalDelegates.onChangedTarget(domainEvent);
        }
        public void Handle(CharacterCastProgressChangedEvent domainEvent)
        {
            GlobalDelegates.onCastProgressChanged(domainEvent);
        }
        public void Handle(CharacterLoginStatusChangedEvent domainEvent)
        {
            GlobalDelegates.onLoginStatusChanged(domainEvent);
        }
        public void Handle(CharacterNameChangedEvent domainEvent)
        {
            GlobalDelegates.onNameChanged(domainEvent);
        }
        public void Handle(CharacterViewModeChangedEvent domainEvent)
        {
            GlobalDelegates.onViewModeChanged(domainEvent);
        }
        public void Handle(CharacterStatusHasChangedEvent domainEvent)
        {
            GlobalDelegates.onStatusHasChanged(domainEvent);
        }
        public void Handle(CharacterStatusEffectsHaveChangedEvent domainEvent)
        {
            GlobalDelegates.onStatusEffectsHaveChanged(domainEvent);
        }
        public void Handle(CharacterUniqueIDHasChanged domainEvent)
        {
            GlobalDelegates.onUniqueIDHasChanged(domainEvent);
        }

    }
}
