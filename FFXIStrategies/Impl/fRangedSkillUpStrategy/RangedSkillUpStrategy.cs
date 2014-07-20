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
//using FFXIStrategies.Impl.fZeruhnStrategy.SubClasses;

namespace FFXIStrategies.Impl
{
    public class RangedSkillUpStrategy : BaseStrategy, IStrategy, IHandleEvent<ObjectHasBeenDiscoveredEvent>
    {

        public RangedSkillUpStrategy(ITruthRepository repository, IBus bus)
            : base(repository, bus)
        {
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

            float distance = 90000;
            ObjectAggregateRoot mostPrioritaryObject = null;
            CharacterAggregateRoot leader = this.worldTruth.getBotLeader();


            foreach (ObjectAggregateRoot npc in this.worldTruth.getMonsters())
            {
                if (npc != null
                    && npc.pStatus != Status.Dead1
                    && npc.pStatus != Status.Dead2
                    && !npc.isClaimedByOthers
                    && npc.pIsActive
                    && npc.pIsRendered)
                   // && npc.pObjectName.Contains("bird"))
                {

                    float npcDistance = (float)Math.Sqrt(Math.Pow(npc.pX - leader.pX,2) + Math.Pow(npc.pZ - leader.pZ,2));
                    if (!npc.isClaimedByOthers)
                    {
                        if (npcDistance < distance)
                        {
                            distance = npcDistance;
                            mostPrioritaryObject = npc;
                        }
                    }

                }

            }
            return mostPrioritaryObject;
        }

        private void checkAmmo()
        {
            uint count = this.worldTruth.getBotLeader().pInstance.Item.GetEquippedItemCount(EquipSlot.Ammo);
            if (count == 0)
            {
                this.worldTruth.getBotLeader().pInstance.Windower.SendString("/equip ammo \"Crossbow bolt\"");
            }
        }

        private const int CONST_GO_DISTANCE = 5;
        // this will loop until Terminate is called
        protected override void Strategy()
        {

            ObjectAggregateRoot target;
            int mustGoDistance = CONST_GO_DISTANCE;
            while ((target = getPriorityTarget()) != null)
            {

                    if (target != null
                        && target.pStatus != Status.Dead1
                        && target.pStatus != Status.Dead2
                        && target.pIsActive
                        && target.pIsRendered)
                    {
                        checkAmmo();

                        if (!GoToMonster(target, mustGoDistance))
                            continue;

                        if (PullMonster(target))
                            mustGoDistance = CONST_GO_DISTANCE;
                        else
                        {
                            mustGoDistance -= 2;
                            continue;
                        }

                        while (target != null
                            && target.pStatus != Status.Dead1
                            && target.pStatus != Status.Dead2
                            && target.pIsActive
                            && target.pIsRendered)
                        {
                            checkAmmo();

                            if (this.worldTruth.getBotLeader().pHpp < 75)
                            {
                                Thread.Sleep(2000);
                                this.sendAndWaitCompletionCommand(new UseAbilityCommand
                                {
                                    characterName = this.worldTruth.getBotLeader().characterName,
                                    ability = AbilityList.Curing_Waltz_III,
                                    timeout = 6000
                                });
                            }

                            this.sendAndWaitCompletionCommand(new UseRangedAttackCommand
                            {
                                characterName = this.worldTruth.getBotLeader().characterName,
                                obj = target,
                                timeout = 6000
                            });
                        }


                    }
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

        #region Handlers

        public void Handle(WorldHourHasChangedEvent domainEvent)
        {
            // Stuff;
        }


        public void Handle(ObjectHasBeenDiscoveredEvent domainEvent)
        {
            ObjectAggregateRoot obj = this.worldTruth.getAggregateById<ObjectAggregateRoot>(domainEvent.objectId);
            if (domainEvent.objectName.Contains("Treasure Casket") && obj.pIsRendered)
            {
                this.worldTruth.getBotLeader().pInstance.Windower.SendString("/p <call>");
            }
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


        #region Wrapped commands
        /// <summary>
        /// Go to specified monster with all chars
        /// </summary>
        /// <param name="monsterTarget"></param>
        /// <returns></returns>
        private bool GoToMonster(ObjectAggregateRoot monsterTarget, int distance)
        {

            this.sendAsyncCommandNoOverride(new MoveToObjectCommand
            {
                characterName = this.worldTruth.getBotLeader().characterName, // doesn't matter here since it's a broadcast
                obj = monsterTarget,
                distanceFromDestination = distance
            });

            return true;
        }

        /// <summary>
        /// Pull the specified monster
        /// </summary>
        /// <param name="monsterTarget"></param>
        /// <returns></returns>
        private bool PullMonster(ObjectAggregateRoot monsterTarget)
        {

            if (monsterTarget.pClaimId == 0)
            {
                if (!this.sendAndWaitCompletionCommand(new UseRangedAttackCommand
                {
                    characterName = this.worldTruth.getBotLeader().characterName,
                    obj = monsterTarget,
                    timeout = 7000
                }))
                {
                    return false;
                }

            }

            return true;
        }





        #endregion
    }
}
