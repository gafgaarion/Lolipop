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
using FFXIStrategies.Impl.fDemoStrategy.SubClasses;

namespace FFXIStrategies.Impl
{
    public class DemoStrategy : BaseStrategy, IStrategy, IHandleEvent<WorldHourHasChangedEvent>,
                                                         IHandleEvent<WorldAggroListChangedEvent>
    {

        class TimeExtention
        {
            public string TEName { get; set; }
            public int TEId { get; set; }
            public bool obtained { get; set; }
        }

        List<TimeExtention> TEList;

        public DemoStrategy(ITruthRepository repository, IBus bus)
            : base(repository, bus)
        {
           
            TEList = new List<TimeExtention>();
            TEList.Add(new TimeExtention() { TEName = "Adamanking Image", TEId = 0, obtained = false });
            TEList.Add(new TimeExtention() { TEName = "Goblin Statue", TEId = 0, obtained = false });
            TEList.Add(new TimeExtention() { TEName = "Goblin Statue", TEId = 0, obtained = false });
            TEList.Add(new TimeExtention() { TEName = "Goblin Statue", TEId = 0, obtained = false });
            TEList.Add(new TimeExtention() { TEName = null, TEId = 0, obtained = false });
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

        private ObjectAggregateRoot getPriorityTarget()
        {
            bool isClaimedAlready = false;
            float distance = 90000;
            ObjectAggregateRoot mostPrioritaryObject = null;
            CharacterAggregateRoot leader = this.worldTruth.getBotLeader();
            foreach (ObjectAggregateRoot npc in this.worldTruth.getObjectsByName("Damselfly"))
            {
                if (npc != null
                    && npc.pStatus != Status.Dead1
                    && npc.pStatus != Status.Dead2
                    && npc.pIsActive
                    && npc.pIsRendered)
                {
                    float npcDistance = (float)Math.Sqrt(Math.Pow(npc.pX - leader.pX,2) + Math.Pow(npc.pZ - leader.pZ,2));
                    if (npc.isClaimedByParty)
                    {
                        if (npcDistance < distance)
                        {
                            isClaimedAlready = true;
                            distance = npcDistance;
                            mostPrioritaryObject = npc;
                        }
                    }
                    
                    if (!isClaimedAlready)
                    {
                        if (Math.Sqrt(Math.Pow(npc.pX - leader.pX,2) + Math.Pow(npc.pZ - leader.pZ,2)) < distance)
                        {
                            distance = npcDistance;
                            mostPrioritaryObject = npc;
                        }
                    }
                }

            }
            return mostPrioritaryObject;
        }


        // this will loop until Terminate is called
        protected override void Strategy()
        {

            ObjectAggregateRoot target;
            target = this.worldTruth.getObjectByName("Sattsuh Ahkanpari");

            if (target != null)
            {
                List<FFACE.TRADEITEM> list = new List<FFACE.TRADEITEM>();
                int index = (byte)this.worldTruth.getBotLeader().pInstance.Item.GetFirstIndexByItemID(642, InventoryType.Inventory);
                list.Add(new FFACE.TRADEITEM() { Count = 2, ItemID = 642, Index = (byte)index } );

                index = (byte)this.worldTruth.getBotLeader().pInstance.Item.GetFirstIndexByItemID(922, InventoryType.Inventory);
                list.Add(new FFACE.TRADEITEM() { Count = 12, ItemID = 922, Index = (byte)index });


                this.sendAndWaitCompletionCommand(new TradeItemToTargetCommand
                {
                    characterName = this.worldTruth.getBotLeader().characterName,
                    obj = target,
                    itemsToTrade = list
                });
            }

            //while ((target = getPriorityTarget()) != null)
            //{
            //    if (target != null
            //        && target.pStatus != Status.Dead1
            //        && target.pStatus != Status.Dead2
            //        && target.pIsActive
            //        && target.pIsRendered)
            //    {
            //        this.broadcastAndWaitCompletionCommand(new MoveToObjectCommand
            //        {
            //            characterName = 0, // doesn't matter here since it's a broadcast
            //            obj = target,
            //            distanceFromDestination = 10
            //        });

            //        //this.broadcastAndWaitCompletionCommand(new UseRangedAttackCommand
            //        //{
            //        //    characterName = this.worldTruth.getBotLeader().characterName,
            //        //    obj = target,
            //        //    timeout = 20000
            //        //});

            //        this.broadcastAndWaitCompletionCommand(new BeginFightingTargetCommand
            //        {
            //            characterName = 0,
            //            obj = target
            //        });

            //        while (areAnyCharacterFighting())
            //        {
            //            //if (this.worldTruth.getCharacterByName("Queenelsa").pTp >= 1000)
            //            //{
            //            //    this.broadcastAndWaitCompletionCommand(new ReadyWeaponskillCommand
            //            //    {
            //            //        characterName = this.worldTruth.getBotLeader().characterName,
            //            //        ws = WeaponSkillList.Heavy_Swing,
            //            //        timeout = 5000,
            //            //        obj = target
            //            //    });
            //            //}

            //            //if (!hasStatusEffect(this.worldTruth.getCharacterByName("Queenelsa").pStatusEffects, StatusEffect.Haste))
            //            //{
            //            //    this.broadcastAndWaitCompletionCommand(new CastSpellCommand
            //            //    {
            //            //        characterName = this.worldTruth.getCharacterByName("Queenelsa").characterName,
            //            //        spell = SpellList.Haste,
            //            //        timeout = 5000
            //            //    });
            //            //}


            //            //if (!hasStatusEffect(this.worldTruth.getCharacterByName("Queenelsa").pStatusEffects, StatusEffect.Afflatus_Misery))
            //            //{
            //            //    this.broadcastAndWaitCompletionCommand(new UseAbilityCommand
            //            //    {
            //            //        characterName = this.worldTruth.getCharacterByName("Queenelsa").characterName,
            //            //        ability = AbilityList.Afflatus_Misery,
            //            //        timeout = 5000
            //            //    });
            //            //}

            //        }
            //    }
            //}



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

        #region Handlers

        public void Handle(WorldHourHasChangedEvent domainEvent)
        {
            // Stuff;
        }

        public void Handle(WorldAggroListChangedEvent domainEvent)
        {
            List<AggroShard> aggros = this.worldTruth.getAllAggros();

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
