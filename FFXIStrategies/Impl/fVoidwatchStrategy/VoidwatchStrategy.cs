using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons;
using FFXIWorldKnowledge;
using FFXIWorldKnowledge.Impl;
using FFXIWorldKnowledge.Shards;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using CommunicationHandler;
using FFXIEvents.Events;
using FFXIAggregateRoots;
using FFXICommands.Commands;
using FFACETools;
using System.Collections.Concurrent;
using FFXIStrategies.Impl.fVoidwatchStrategy.Subclasses;

namespace FFXIStrategies.Impl
{
    public class VoidwatchStrategy : BaseStrategy, IStrategy, IHandleEvent<ObjectHasBeenDiscoveredEvent>,
                                                              IHandleEvent<ObjectHasDisappearedEvent>,
                                                              IHandleEvent<AllianceActionUsedEvent>,
                                                              IHandleEvent<ObjectHasBeenStaggeredEvent>
    {
        private Dictionary<string, CharacterIntel> Intels;
        private List<VoidwatchWeakness> WeaknessList;
        private ObjectAggregateRoot VoidwatchBoss;
        private Dictionary<int, object> PartyMembersLastActionUsed;
        private ObjectAggregateRoot PlanarRift;

        public VoidwatchStrategy(ITruthRepository repository, IBus bus)
            : base(repository, bus)
        {
            this.PartyMembersLastActionUsed = new Dictionary<int, object>();
            this.Intels = new Dictionary<string, CharacterIntel>();
            this.WeaknessList = new List<VoidwatchWeakness>();
            this.VoidwatchBoss = null;
            this.PlanarRift = null;
            WeaknessList.Add(new VoidwatchWeakness(VoidwatchProcStrength.White, VoidwatchProcType.BlackMagic, Elements.Dark));
        }


        private void initIntel()
        {
            foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
            {
                Intels.Add(character.characterName, new CharacterIntel(character.characterName,
                                                                       character.pInstance.Player.MainJob,
                                                                       character.pInstance.Player.SubJob,
                                                                       character.pInstance));
            }
        }

        private void TryProcWeakness()
        {
            if (this.VoidwatchBoss != null)
            {
                foreach (CharacterIntel intel in this.Intels.Values)
                {

                    CharacterAggregateRoot character = this.worldTruth.getAggregateById<CharacterAggregateRoot>(intel.mcharacterName);

                    if (character != null && !character.busy)
                    {
                        object skill = intel.getBestSkillProcAttempt(WeaknessList);

                        if (skill == null)
                            continue;

                        float maxRange = intel.CurrentSkillMaxDistance;

                        // Approach close enough to be able to execute the skill.
                        this.sendAndWaitCompletionCommand(new MoveToObjectCommand
                        {
                            characterName = character.characterName,
                            obj = this.VoidwatchBoss,
                            distanceFromDestination = maxRange - 1
                        });

                        if (skill is AbilityList)
                        {
                            this.sendAndWaitCompletionCommand(new UseAbilityCommand
                            {
                                characterName = character.characterName,
                                obj = this.VoidwatchBoss,
                                ability = (AbilityList)skill,
                                timeout = 15000
                            });
                        }
                        else if (skill is SpellList)
                        {
                            this.sendAndWaitCompletionCommand(new CastSpellCommand
                            {
                                characterName = character.characterName,
                                obj = this.VoidwatchBoss,
                                spell = (SpellList)skill,
                                timeout = 15000
                            });
                        }
                        else if (skill is WeaponSkillList)
                        {
                            this.sendAndWaitCompletionCommand(new ReadyWeaponskillCommand
                            {
                                characterName = character.characterName,
                                obj = this.VoidwatchBoss,
                                ws = (WeaponSkillList)skill,
                                timeout = 15000
                            });
                        }
                    }
                }
            }
        }


        private bool areAnyCharacterFighting()
        {
            if (this.stop)
                return false;

            foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
            {
                if (character.isFighting)
                    return true;
            }
            return false;
        }

        private bool hasStatusEffect(StatusEffect[] list, StatusEffect _effect)
        {
            foreach (StatusEffect effect in list)
            {
                if (effect == _effect)
                    return true;
            }
            return false;
        }

        private bool characterInRange(string characterName)
        {
            foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
            {
                if (character.characterName == characterName)
                {
                    if (character.pInstance.NPC.Distance(this.PlanarRift.pIndex) > 20)
                        return false;
                }
            }
            return true;
        }

        private void addItemToList(FFACE instance, List<FFACE.TRADEITEM> list, ushort itemID, byte count)
        {
             // Cobalt cells
            int index = (byte)instance.Item.GetFirstIndexByItemID(itemID, InventoryType.Inventory);
            byte icount = (byte)instance.Item.GetInventoryItemCountByIndex(index);

            if (icount < count)
                count = icount;

            if (index != 0)
                list.Add(new FFACE.TRADEITEM() { Count = count, ItemID = itemID, Index = (byte)index });
        }

        private void TradeCells()
        {
            lock (this.Intels)
            {
                foreach (CharacterIntel character in this.Intels.Values)
                {
                    if (!character.IsBusy)
                    {
                        if (characterInRange(character.mcharacterName))
                        {
                            character.IsBusy = true;
                            new Thread(() => TradeCellsWorker(character.mcharacterName)).Start();
                        }
                    }
                }
            }
        }

        // this will loop until Terminate is called
        protected override void Strategy()
        {

            // Trade cells
            if (this.PlanarRift != null)
            {
                this.TradeCells();
            }

            Thread.Sleep(100);
        }

        public Type getType()
        {
            return this.GetType();
        }

        public List<ConfigurationField> getConfigurationFields()
        {
            return this.configurations;
        }

        private void TradeCellsWorker(string _characterName)
        {
            //Dictionary<int, FFACE.Position> positionbuffer = new Dictionary<int, FFACE.Position>();
            //foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
            //{

            //    float xbuffer = character.pX;
            //    float zbuffer = character.pZ;

            //    positionbuffer.Add(character.characterName, new FFACE.Position() { X = xbuffer, Z = zbuffer });

            //    List<FFACE.TRADEITEM> list = new List<FFACE.TRADEITEM>();

            //    this.addItemToList(character.pInstance, list, 3434, 3); // Cobalt cells
            //    this.addItemToList(character.pInstance, list, 3435, 3); // Rubicund cells
            //    this.addItemToList(character.pInstance, list, 3436, 3); // Xanthous cells

            //    if (list.Count > 0)
            //    {
            //        this.sendAsyncCommandNoOverride(new TradeItemToTargetCommand
            //        {
            //            characterName = character.characterName,
            //            obj = TradeCellsTarget,
            //            itemsToTrade = list
            //        });

            //    }

            //}

            //this.waitForCharacters(this.worldTruth.getCharacters());

            //foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
            //{
            //    character.usePathFinder = false;
            //    this.sendAsyncCommandNoOverride(new MoveToLocationCommand
            //    {
            //        characterName = character.characterName,
            //        destinationX = positionbuffer[character.characterName].X,
            //        destinationZ = positionbuffer[character.characterName].Z,
            //        distanceFromDestination = 1.5f
            //    });

            //}

            //this.waitForCharacters(this.worldTruth.getCharacters());

            //foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
            //{
            //    character.pInstance.Navigator.FaceHeading(TradeCellsTarget.Position);
            //    character.pInstance.Windower.SendKeyPress(KeyCode.NP_Number5);
            //}

            //this.TradeCellsTarget = null;
            //this.TradeCellsWorkerThread = null;
        }

        #region Handlers

        public void Handle(AllianceActionUsedEvent domainEvent)
        {
            lock (this.PartyMembersLastActionUsed)
            {
                if (!this.PartyMembersLastActionUsed.ContainsKey(domainEvent.objectId))
                    this.PartyMembersLastActionUsed.Add(domainEvent.objectId, domainEvent.Skill);
                else
                    this.PartyMembersLastActionUsed[domainEvent.objectId] = domainEvent.Skill;
            }
        }

        public void Handle(ObjectHasBeenStaggeredEvent domainEvent)
        {
            object action = this.PartyMembersLastActionUsed[domainEvent.staggeredById];

            Voidwatch.VoidwatchSkillCollectionType type = Voidwatch.getSkillType(action);

            for ( int i = 0; i < this.WeaknessList.Count; i++)
            {
                if (this.WeaknessList[i].Elemental == type.element &&
                    this.WeaknessList[i].ProcType == type.type)
                {
                    lock (this.WeaknessList)
                    {
                        this.WeaknessList.RemoveAt(i);
                    }
                    break;
                }
            }

        }

        public void Handle(ObjectHasBeenDiscoveredEvent domainEvent)
        {
            if (domainEvent.objectName == "Planar Rift")
            {
                ObjectAggregateRoot target;
                target = this.worldTruth.getAggregateById<ObjectAggregateRoot>(domainEvent.objectId);

                if (target != null)
                {
                    this.PlanarRift = target;

                    lock (this.Intels)
                    {
                        foreach (CharacterIntel character in this.Intels.Values)
                        {
                            character.TradedCells = false;
                        }
                    }
                }
            }

            if (domainEvent.objectName != "Planar Rift" &&
                this.VoidwatchBoss == null)
            {
                ObjectAggregateRoot target;
                target = this.worldTruth.getAggregateById<ObjectAggregateRoot>(domainEvent.objectId);

                if (target.pIsRendered && target.isClaimedByParty)
                {
                    this.VoidwatchBoss = target;
                }
            }

        }


        public void Handle(ObjectHasDisappearedEvent domainEvent)
        {
            if (domainEvent.objectName == "Planar Rift")
            {
                this.PlanarRift = null;
            }
            else
            {
                if (this.VoidwatchBoss != null)
                {
                    if (this.VoidwatchBoss.objectId == domainEvent.objectId)
                        this.VoidwatchBoss = null;
                }
            }

        }

        public override void Handle(CharacterAvailableForActionEvent domainEvent)
        {
            base.Handle(domainEvent); // this line is important for this event

        }

        public override void Handle(CharacterIsBusyWithActionEvent domainEvent)
        {
            base.Handle(domainEvent); // this line is important for this event

        }

        #endregion


    }
}
