using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Commons
{
    public class GlobalDelegates
    {
        public delegate void onCharacterAvailableForAction(Object _event);
        public delegate void onCharacterHasBeenInitialized(Object _event);
        public delegate void onCharacterHasDisconnected(Object _event);
        public delegate void onCharacterHasMoved(Object _event);
        public delegate void onCharacterHPHasChanged(Object _event);
        public delegate void onCharacterIsBusyWithAction(Object _event);
        public delegate void onCharacterMPHasChanged(Object _event);
        public delegate void onCharacterTPHasChanged(Object _event);
        public delegate void onCharacterMapHasChanged(Object _event);

        public delegate void onCharacterHasChangedTarget(Object _event);
        public delegate void onCharacterCastProgressChanged(Object _event);
        public delegate void onCharacterLoginStatusChanged(Object _event);
        public delegate void onCharacterNameChanged(Object _event);
        public delegate void onCharacterViewModeChanged(Object _event);
        public delegate void onCharacterStatusHasChanged(Object _event);
        public delegate void onCharacterStatusEffectsHaveChanged(Object _event);
        public delegate void onCharacterUniqueIDHasChanged(Object _event);
        public delegate void onWorldAggroListHasChanged(Object _event);

        public static GlobalDelegates.onCharacterAvailableForAction onAvailable;
        public static GlobalDelegates.onCharacterHasBeenInitialized onHasBeenInitialized;
        public static GlobalDelegates.onCharacterHasDisconnected onHasDisconnected;
        public static GlobalDelegates.onCharacterHasMoved onHasMoved;
        public static GlobalDelegates.onCharacterHPHasChanged onHPHasChanged;
        public static GlobalDelegates.onCharacterIsBusyWithAction onIsBusyWithAction;
        public static GlobalDelegates.onCharacterMPHasChanged onMPHasChanged;
        public static GlobalDelegates.onCharacterTPHasChanged onTPHasChanged;
        public static GlobalDelegates.onCharacterMapHasChanged onMapHasChanged;
        public static GlobalDelegates.onCharacterHasChangedTarget onChangedTarget;
        public static GlobalDelegates.onCharacterCastProgressChanged onCastProgressChanged;
        public static GlobalDelegates.onCharacterLoginStatusChanged onLoginStatusChanged;
        public static GlobalDelegates.onCharacterNameChanged onNameChanged;
        public static GlobalDelegates.onCharacterViewModeChanged onViewModeChanged;
        public static GlobalDelegates.onCharacterStatusHasChanged onStatusHasChanged;
        public static GlobalDelegates.onCharacterStatusEffectsHaveChanged onStatusEffectsHaveChanged;
        public static GlobalDelegates.onCharacterUniqueIDHasChanged onUniqueIDHasChanged;
        public static GlobalDelegates.onCharacterMapHasBeenLoaded onMapHasBeenLoaded;
        public static GlobalDelegates.onWorldAggroListHasChanged onWorldAggroListChanged;

        // special delegates (comes from ffxiworldtruth)
        public delegate void onCharacterMapHasBeenLoaded(Object _event);

                                               
    }
}
