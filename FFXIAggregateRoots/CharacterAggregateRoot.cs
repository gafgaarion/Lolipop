using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXIEvents.Events;
using FFACETools;
using Commons;
using System.Threading;
using System.IO;

namespace FFXIAggregateRoots
{
    public partial class CharacterAggregateRoot : BaseAggregateRoot
    {


        private bool initialized;
        private FFACE instance;
        private Controller controller;

        // Character Properties
        #region Character Properties

        public bool isLoading = true;
        public bool isEnabled = false;

        // Private accessors for events
        private int hpp { get; set; }
        private int hp { get; set; }
        private int mp { get; set; }
        private int tp { get; set; }
        private float positionX { get; set; }
        private float positionY { get; set; }
        private float positionZ { get; set; }
        private float facing { get; set; }
        private float percentCast { get; set; }
        private LoginStatus loginStatus { get; set; }
        private ViewMode viewMode { get; set; }
        private Status status { get; set; }
        private StatusEffect[] statusEffects { get; set; }
        private Zone mapId { get; set; }
        private TargetStruct target { get; set; }
        private byte hour;

        // Public accessors for realtime data
        public string characterName { get; set; }
        public bool isPartyLeader { get; set; }
        public bool isFighting { get { return this.controller.isFighting; } }
        public bool isRunning { get { return this.controller.isRunning; } }

        public int pHpp { get { return this.instance.Player.HPPCurrent; } }
        public int mHp { get { return this.instance.Player.HPCurrent; } }
        public int pMp { get { return this.instance.Player.MPCurrent; } }
        public int pTp { get { return this.instance.Player.TPCurrent; } }
        public float pX { get { return this.instance.Player.PosX; } }
        public float pY { get { return this.instance.Player.PosY; } }
        public float pZ { get { return this.instance.Player.PosZ; } }
        public float pH { get { return this.instance.Player.PosH; } }
        public float pPercentCast { get { return this.instance.Player.CastPercent; } }
        public LoginStatus pLoginStatus { get { return this.instance.Player.GetLoginStatus; } }
        public ViewMode pViewMode { get { return this.instance.Player.ViewMode; } }
        public Status pStatus { get { return this.instance.Player.Status; } }
        public StatusEffect[] pStatusEffects { get { return this.instance.Player.StatusEffects; } }
        public TargetStruct pTarget { get { return new TargetStruct(this.instance.Target); } }
        public Zone pMapId { get { return this.instance.Player.Zone; } }
        public FFACE.TimerTools.VanaTime pVanaTime { get { return this.instance.Timer.GetVanaTime(); } }
        public FFACE pInstance { get { return this.instance; } }

        #endregion

        #region Action Properties
        // Action Properties
        private Type currentActionCallbackType;
        public bool busy { get; private set; }
        public bool lastActionSuccess { get; private set; }

        // Action callback
        public delegate void ControllerCommandCompletedCallback(bool isSuccess, Type forceType = null);
        private ControllerCommandCompletedCallback CommandCompletedCallbackHandle;

        private void CommandCompletedCallback(bool isSuccess, Type forceType = null)
        {
            lastActionSuccess = isSuccess;

            Type buffer;

            if (forceType != null)
                buffer = forceType;
            else
                buffer = currentActionCallbackType;

            if (buffer == typeof(CmdMoveCompletedEvent))
                Apply<CmdMoveCompletedEvent>(new CmdMoveCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdStopMoveCompletedEvent))
                Apply<CmdStopMoveCompletedEvent>(new CmdStopMoveCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdSwitchTargetCompletedEvent))
                Apply<CmdSwitchTargetCompletedEvent>(new CmdSwitchTargetCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdBeginFightingCompletedEvent))
                Apply<CmdBeginFightingCompletedEvent>(new CmdBeginFightingCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdEndFightingCompletedEvent))
                Apply<CmdEndFightingCompletedEvent>(new CmdEndFightingCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdAbilityCompletedEvent))
                Apply<CmdAbilityCompletedEvent>(new CmdAbilityCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdSpellCompletedEvent))
                Apply<CmdSpellCompletedEvent>(new CmdSpellCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdWeaponskillCompletedEvent))
                Apply<CmdWeaponskillCompletedEvent>(new CmdWeaponskillCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdRaCompletedEvent))
                Apply<CmdRaCompletedEvent>(new CmdRaCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdInteractCompletedEvent))
                Apply<CmdInteractCompletedEvent>(new CmdInteractCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });
            else if (buffer == typeof(CmdTradeCompletedEvent))
                Apply<CmdTradeCompletedEvent>(new CmdTradeCompletedEvent { characterName = this.characterName, isSuccess = isSuccess });

            Thread.Sleep(10);
            if (forceType == null)
            {
                // Generic version of busy release
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = isSuccess });
            }
           
        }

        #endregion

        #region Character configurations
        // Character configurations
        public bool usePathFinder
        {
            get { return this.controller.usePathFinder; }
            set { this.controller.usePathFinder = value; }
        }
        private bool pbotLeader;
        public bool isBotLeader 
        { 
            get
            {
                return pbotLeader;
            }
            set
            {
                if (mapId != Zone.Unknown)
                    this.controller.NotifyUI();
                this.pbotLeader = value;
            }
        }
        #endregion

        /// <summary>
        ///  Raises events when commands are completed, so that strategy will be aware of it.
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="eventType"></param>

        public CharacterAggregateRoot() : base()
        {
            initialized = false;
            this.controller = null;
            this.target = new TargetStruct();

            Register<CharacterHPHasChangedEvent>();
            Register<CharacterTPHasChangedEvent>();
            Register<CharacterMPHasChangedEvent>();
            Register<CharacterHasChangedMapEvent>();
            Register<CharacterHasBeenInitializedEvent>();
            Register<CharacterAvailableForActionEvent>();
            Register<CharacterHasMovedEvent>();
            Register<CharacterHasDisconnectedEvent>();
            Register<CharacterIsBusyWithActionEvent>();
            Register<CharacterHasChangedTargetEvent>();
            Register<CharacterCastProgressChangedEvent>();
            Register<CharacterLoginStatusChangedEvent>();
            Register<CharacterNameChangedEvent>();
            Register<CharacterViewModeChangedEvent>();
            Register<CharacterStatusHasChangedEvent>();
            Register<CharacterStatusEffectsHaveChangedEvent>();
            Register<CharacterUniqueIDHasChanged>();
            Register<CharacterTPHasChangedEvent>();
            Register<WorldHourHasChangedEvent>();
            Register<AllianceActionUsedEvent>();

            // Commands completed events
            Register<CmdMoveCompletedEvent>();
            Register<CmdStopMoveCompletedEvent>();
            Register<CmdSwitchTargetCompletedEvent>();
            Register<CmdBeginFightingCompletedEvent>();
            Register<CmdEndFightingCompletedEvent>();
            Register<CmdAbilityCompletedEvent>();
            Register<CmdSpellCompletedEvent>();
            Register<CmdWeaponskillCompletedEvent>();
            Register<CmdRaCompletedEvent>();
            Register<CmdInteractCompletedEvent>();
            Register<CmdTradeCompletedEvent>();
            
            
        }



        public void Terminate()
        {
            if (this.controller != null)
                this.controller.Terminate();
        }


        public bool isOnCooldown(AbilityList _abiliy)
        {
            return this.instance.Timer.GetAbilityRecast(_abiliy) > 0;
        }

        public bool isOnCooldown(SpellList _spell)
        {
            return this.instance.Timer.GetSpellRecast(_spell) > 0;
        }


        #region Applies
        public void InitializeCharacter(FFACE _instance, string name)
        {
            if(initialized)
                throw new InvalidOperationException("Character is already active !");



            Apply<CharacterHasBeenInitializedEvent>(new CharacterHasBeenInitializedEvent { instance = _instance, characterName = name });
            Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = name });
        }


        public void UpdateCharacterFromClient(FFACE.PlayerTools player, FFACE.PartyTools party, FFACE.TargetTools target)
        {

            if (!initialized)
                throw new InvalidOperationException("Character hasn't been initialized !");

            if (this.isBotLeader)
            {
                if (this.pVanaTime.Hour != this.hour)
                {
                    Apply<WorldHourHasChangedEvent>(new WorldHourHasChangedEvent { hour = this.pVanaTime.Hour, oldHour = this.hour });
                }
            }

            if (player.HPPCurrent != this.hpp)
                Apply<CharacterHPHasChangedEvent>(new CharacterHPHasChangedEvent { characterName = characterName, hpp = player.HPPCurrent, oldHpp = this.hpp, hp = player.HPCurrent, oldHp = this.hp });

            if (player.TPCurrent != this.tp)
                Apply<CharacterTPHasChangedEvent>(new CharacterTPHasChangedEvent { characterName = characterName, tp = player.TPCurrent });

            if (player.MPCurrent != this.mp)
                Apply<CharacterMPHasChangedEvent>(new CharacterMPHasChangedEvent { characterName = characterName, mp = player.MPCurrent, oldMp = this.mp });

            if (player.PosX != this.positionX || player.PosY != this.positionY || player.PosZ != this.positionZ || player.PosH != this.facing)
                Apply<CharacterHasMovedEvent>(new CharacterHasMovedEvent
                {
                    characterName = characterName,
                    oldX = this.positionX,
                    oldY = this.positionY,
                    oldZ = this.positionZ,
                    newX = player.PosX,
                    newY = player.PosY,
                    newZ = player.PosZ,
                    oldFacing = this.facing,
                    newFacing = player.PosH
                });

            TargetStruct struc = new TargetStruct(target);

            // May generate more than 1 event, because name isn't loaded very quickly.
            if (struc.ServerID != this.target.ServerID || struc.Name != this.target.Name)
                Apply<CharacterHasChangedTargetEvent>(new CharacterHasChangedTargetEvent { characterName = characterName, target = struc, oldTarget = this.target });
      
            if (player.CastMax != this.percentCast)
                Apply<CharacterCastProgressChangedEvent>(new CharacterCastProgressChangedEvent { characterName = characterName, newCastProgress = player.CastCountDown, oldCastProgress = this.percentCast });

            if (player.GetLoginStatus != this.loginStatus)
                Apply<CharacterLoginStatusChangedEvent>(new CharacterLoginStatusChangedEvent { characterName = characterName, loginStatus = player.GetLoginStatus, oldLoginStatus = this.loginStatus });

            if (player.Name != this.characterName)
                Apply<CharacterNameChangedEvent>(new CharacterNameChangedEvent { characterName = characterName, oldCharacterName = this.characterName });

            if (player.ViewMode != this.viewMode)
                Apply<CharacterViewModeChangedEvent>(new CharacterViewModeChangedEvent { characterName = characterName, viewMode = player.ViewMode, oldViewMode = this.viewMode });

            if (player.Status != this.status)
                Apply<CharacterStatusHasChangedEvent>(new CharacterStatusHasChangedEvent { characterName = characterName, status = player.Status, oldStatus = this.status });

            if (!GlobalFunctions.ArraysEqual<StatusEffect>(player.StatusEffects, this.statusEffects))
                Apply<CharacterStatusEffectsHaveChangedEvent>(new CharacterStatusEffectsHaveChangedEvent { characterName = characterName, statusEffects = player.StatusEffects, oldStatusEffects = this.statusEffects });

            if (player.Zone != this.mapId)
                Apply<CharacterHasChangedMapEvent>(new CharacterHasChangedMapEvent { characterName = characterName, mapId = player.Zone });

            
        }


        public void PurgeCharacter()
        {
            if (!initialized)
                throw new InvalidOperationException("Character is already purged !");

            Apply<CharacterHasDisconnectedEvent>(new CharacterHasDisconnectedEvent { characterName = characterName });
        }
        #endregion

        #region Events
        private void OnCharacterHasBeenInitializedEvent(CharacterHasBeenInitializedEvent domainEvent)
        {
            initialized = true;
            busy = false;
            characterName = domainEvent.characterName;
            characterName = domainEvent.characterName;
            instance = domainEvent.instance;
            currentActionCallbackType = typeof(void);
            status = Status.Unknown;
            
            CommandCompletedCallbackHandle = new ControllerCommandCompletedCallback(CommandCompletedCallback);
            controller = new Controller(instance, CommandCompletedCallbackHandle, true);
            this.controller.StopMove();
            usePathFinder = true;

        }

        private void OnCharacterHPHasChangedEvent(CharacterHPHasChangedEvent domainEvent)
        {
            this.hpp = domainEvent.hpp;
            this.hp = domainEvent.hp;
        }

        private void OnWorldHourHasChangedEvent(WorldHourHasChangedEvent domainEvent)
        {
            this.hour = domainEvent.hour;
        }

        private void OnCharacterTPHasChangedEvent(CharacterTPHasChangedEvent domainEvent)
        {
            this.tp = domainEvent.tp;
        }

        private void OnCharacterHasMovedEvent(CharacterHasMovedEvent domainEvent)
        {
            this.positionX = domainEvent.newX;
            this.positionY = domainEvent.newY;
            this.positionZ = domainEvent.newZ;
            this.facing = domainEvent.newFacing;
            controller.setPosition(this.positionX, this.positionY, this.positionZ);
        }

        private void OnCharacterHasDisconnectedEvent(CharacterHasDisconnectedEvent domainEvent)
        {
            this.controller.StopMove();
            initialized = false;
        }

        private void OnCharacterMPHasChangedEvent(CharacterMPHasChangedEvent domainEvent)
        {
            this.mp = domainEvent.mp;
        }

        private void OnObjectHasAggroedCharacterEvent(ObjectHasAggroedCharacterEvent domainEvent)
        {
            
        }


        private void OnCharacterIsBusyWithActionEvent(CharacterIsBusyWithActionEvent domainEvent)
        {
        }

        private void OnCharacterAvailableForActionEvent(CharacterAvailableForActionEvent domainEvent)
        {
            this.busy = false;
            this.currentActionCallbackType = typeof(void);
        }

        private void OnCharacterHasChangedMapEvent(CharacterHasChangedMapEvent domainEvent)
        {
            isLoading = true;
            this.mapId = domainEvent.mapId;

            bool notifyUI = false;
            if (this.isBotLeader) notifyUI = true;
            controller.LoadMap(this.mapId, notifyUI);
            isLoading = false;
        }

        private void OnCharacterHasChangedTargetEvent(CharacterHasChangedTargetEvent domainEvent)
        {
            this.target = domainEvent.target;
        }

        private void OnCharacterCastProgressChangedEvent(CharacterCastProgressChangedEvent domainEvent)
        {
            this.percentCast = domainEvent.newCastProgress;
        }

        private void OnCharacterLoginStatusChangedEvent(CharacterLoginStatusChangedEvent domainEvent)
        {
            this.loginStatus = domainEvent.loginStatus;
        }

        private void OnCharacterNameChangedEvent(CharacterNameChangedEvent domainEvent)
        {
            this.characterName = domainEvent.characterName;
        }

        private void OnCharacterViewModeChangedEvent(CharacterViewModeChangedEvent domainEvent)
        {
            this.viewMode = domainEvent.viewMode;
        }

        private void OnCharacterStatusHasChangedEvent(CharacterStatusHasChangedEvent domainEvent)
        {
            this.status = domainEvent.status;
        }

        private void OnCharacterStatusEffectsHaveChangedEvent(CharacterStatusEffectsHaveChangedEvent domainEvent)
        {
            this.statusEffects = domainEvent.statusEffects;
        }

        private void OnAllianceActionUsedEvent(AllianceActionUsedEvent domainEvent)
        {

        }

        #endregion

        #region Events of commands completed
        private void OnCmdStopMoveCompletedEvent(CmdStopMoveCompletedEvent domainEvent)
        {
        }
        private void OnCmdMoveCompletedEvent(CmdMoveCompletedEvent domainEvent)
        {
        }
        private void OnCmdSwitchTargetCompletedEvent(CmdSwitchTargetCompletedEvent domainEvent)
        {
        }
        private void OnCmdBeginFightingCompletedEvent(CmdBeginFightingCompletedEvent domainEvent)
        {
        }
        private void OnCmdEndFightingCompletedEvent(CmdEndFightingCompletedEvent domainEvent)
        {
        }
        private void OnCmdAbilityCompletedEvent(CmdAbilityCompletedEvent domainEvent)
        {
        }
        private void OnCmdSpellCompletedEvent(CmdSpellCompletedEvent domainEvent)
        {
        }
        private void OnCmdWeaponskillCompletedEvent(CmdWeaponskillCompletedEvent domainEvent)
        {
        }
        private void OnCmdRaCompletedEvent(CmdRaCompletedEvent domainEvent)
        {
        }
        private void OnCmdInteractCompletedEvent(CmdInteractCompletedEvent domainEvent)
        {
        }
        private void OnCmdTradeCompletedEvent(CmdTradeCompletedEvent domainEvent)
        {
        }
        #endregion

        #region Chat

        // New chat line
        public void NewChatLine(FFACE.ChatTools.ChatLine line)
        {

            if (line.Type == ChatMode.PlayersBadCast)
            {
                if (line.Text.Contains("Unable to see"))
                    this.instance.Windower.SendKeyPress(KeyCode.NP_Number8);

                if (line.Text.Contains("too far away") ||
                    line.Text.Contains("Unable to see") ||
                    line.Text.Contains("Cannot see") ||
                    line.Text.Contains("Unable to use") ||
                    line.Text.Contains("You cannot see") ||
                    line.Text.Contains("have enough MP") ||
                    line.Text.Contains("You move and inter") ||
                    line.Text.Contains("command error occured") ||
                    line.Text.Contains("out of range"))
                {
                    this.controller.abilityCastFailed = true;
                    this.controller.spellCastFailed = true;
                    this.controller.weaponskillFailed = true;
                    this.controller.RaCastFailed = true;
                }


            }

            if ((int)line.Type > 12)
            {
                if (line.Text.Contains("You ") || line.Text.Contains(this.characterName))
                {
                    if (line.Text.Contains("casting is interrupted"))
                    {
                        if (line.Text.Contains("You ") || line.Text.Contains(this.characterName))
                            this.controller.spellCastFailed = true;
                    }

                    if (line.Text.Contains("You must specify a valid"))
                    {
                        this.controller.abilityCastFailed = true;
                        this.controller.spellCastFailed = true;
                        this.controller.weaponskillFailed = true;
                        this.controller.RaCastFailed = true;
                    }

                    if (line.Text.Contains(" casts ") || line.Text.Contains(" has no effect on the ")) // spell went off
                        this.controller.spellCasted = true;

                    if (line.Text.Contains(" uses ")) // Abi went off
                        this.controller.abilityUsed = true;

                    if (line.Text.Contains(" starts casting ")) // starting to cast spell
                        this.controller.spellCastedStart = true;

                    if (line.Text.Contains(" readies ")) // WS went off
                        this.controller.spellCasted = true;

                    if (line.Text.Contains(" ranged attack ") ||
                        line.Text.Contains("Ranged attack")) // starting to cast spell
                        this.controller.RaUsed = true;

                    // Register action used events for party
                    if (this.isBotLeader)
                    {
                        if (line.Text.Contains(" casts ") || line.Text.Contains(" has no effect on the ")  || 
                            line.Text.Contains(" uses ") ||
                            line.Text.Contains(" readies "))
                        {
                            foreach (FFACE.PartyMemberTools member in this.instance.PartyMember.Values)
                            {
                                if (line.Text.Contains(member.Name))
                                {
                                    int index = line.Text.IndexOf("casts ");
                                    if (index == -1)
                                    {
                                        index = line.Text.IndexOf("uses ");

                                        if (index == -1)
                                            index = line.Text.IndexOf("readies ") + 8;
                                        else
                                            index += 5;
                                    }
                                    else
                                        index += 6;

                                    string actionname = line.Text.Substring(index, line.Text.Length - index - 1);
                                    string decoratedactionname = GlobalFunctions.SkillAddEnumDecorations(actionname);
                                    object action = null;

                                    if (Enum.IsDefined(typeof(WeaponSkillList), decoratedactionname))
                                        action = System.Enum.Parse(typeof(WeaponSkillList), decoratedactionname);
                                    else if (Enum.IsDefined(typeof(AbilityList), decoratedactionname))
                                        action = System.Enum.Parse(typeof(AbilityList), decoratedactionname);
                                    else if (Enum.IsDefined(typeof(SpellList), decoratedactionname))
                                        action = System.Enum.Parse(typeof(SpellList), decoratedactionname);

                                    Apply<AllianceActionUsedEvent>(new AllianceActionUsedEvent() { objectId = member.ServerID, Skill = action });
                    
                                }
                            }
                        }

                    }
                }
            }



        }

        #endregion

        #region Handlers



        #endregion

        #region Commands

        public void StopMove()
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdStopMoveCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.StopMove(); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void MoveToObject(ObjectAggregateRoot obj, float distance)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdMoveCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.MoveToObject(obj, distance); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void MoveToCharacter(CharacterAggregateRoot charac, float distance)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdMoveCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.MoveToCharacter(charac, distance); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void MoveToLocation(float x, float z, float distance)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdMoveCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.MoveToLocation(x, z, distance); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void SwitchTarget(int objectId, ObjectAggregateRoot obj, bool _lock)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdSwitchTargetCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.switchTarget(objectId, obj, _lock); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void BeginFighting(ObjectAggregateRoot obj)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdBeginFightingCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.BeginFighting(obj); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void EndFighting()
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdEndFightingCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.EndFighting(); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void UseAbility(ObjectAggregateRoot obj, string objectName, AbilityList ability, int timeout)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdAbilityCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });


                new Thread(delegate() { this.controller.useAbility(obj, objectName, ability, timeout); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }

        }

        public void CastSpell(ObjectAggregateRoot obj, string objectName, SpellList spell, int timeout)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdSpellCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.castSpell(obj, objectName, spell, timeout); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void ReadyWeaponskill(ObjectAggregateRoot obj, string objectName, WeaponSkillList ws, int timeout)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdSpellCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.readyWeaponskill(obj, objectName, ws, timeout); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void UseRangedAttack(ObjectAggregateRoot obj, int timeout)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdRaCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.useRangedAttack(obj, timeout); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void InteractWithObject(ObjectAggregateRoot obj, string DialogTextToChoose, int timeout)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdRaCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.interactWithObject(obj, DialogTextToChoose, timeout); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        public void TradeItemToObject(ObjectAggregateRoot obj, List<FFACE.TRADEITEM> items, int gil = -1)
        {
            if (this.isEnabled)
            {
                this.currentActionCallbackType = typeof(CmdTradeCompletedEvent);
                this.busy = true;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });

                new Thread(delegate() { this.controller.TradeItemToTarget(obj, items, gil); }).Start();
            }
            else
            {
                this.busy = false;
                Apply<CharacterIsBusyWithActionEvent>(new CharacterIsBusyWithActionEvent { characterName = this.characterName });
                Apply<CharacterAvailableForActionEvent>(new CharacterAvailableForActionEvent { characterName = this.characterName, isSuccess = false });
            }
        }

        #endregion
    }
}
