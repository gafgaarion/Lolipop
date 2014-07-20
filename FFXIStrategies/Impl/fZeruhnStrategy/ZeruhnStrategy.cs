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
    public class ZeruhnStrategy : BaseStrategy, IStrategy, IHandleEvent<WorldHourHasChangedEvent>,
                                                         IHandleEvent<WorldAggroListChangedEvent>
    {

        private const int BAT_MISSION_REQ = 5;
        private const int WORM_MISSION_REQ = 2;
        private const string BAT_NAME = "Colliery Bat";
        private const string WORM_NAME = "Burrower Worm";
        private int bats;
        private int worms;
        private bool resting = false;

        public ZeruhnStrategy(ITruthRepository repository, IBus bus)
            : base(repository, bus)
        {
            this.bats = 0;
            this.worms = 0;
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
            string target;
            if (bats == BAT_MISSION_REQ && worms == WORM_MISSION_REQ)
                bats = 0; worms = 0; // New page
            if (bats == BAT_MISSION_REQ) target = WORM_NAME;
            else if (worms == WORM_MISSION_REQ) target = BAT_NAME;
            else target = String.Empty;


            bool isClaimedAlready = false;
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
                {
                    if (target == String.Empty)
                    {
                        if (npc.pObjectName != BAT_NAME && npc.pObjectName != WORM_NAME)
                            continue;
                    }
                    else
                    {
                        if (npc.pObjectName != target)
                            continue;
                    }

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

            while ((target = getPriorityTarget()) != null)
            {
                if (resting)
                {
                    //while (this.worldTruth.getCharacterByName("Queenelsa").pStatus != Status.Healing)
                    //{
                    //    this.worldTruth.getCharacterByName("Queenelsa").pInstance.Windower.SendString("/heal");
                    //    Thread.Sleep(250);
                    //}
                    //if (this.worldTruth.getCharacterByName("Queenelsa").pMp >= 500)
                    //    resting = false;
                }
                else
                {
                    if (target != null
                        && target.pStatus != Status.Dead1
                        && target.pStatus != Status.Dead2
                        && target.pIsActive
                        && target.pIsRendered)
                    {

                        if (!GoToMonster(target))
                            continue;
                        
                        if (!PullMonster(target))
                            continue;


                        while (areAnyCharacterFighting())
                        {
                            ApplyLeaderFightingSequence(target);
                            ApplyElsaFightingSequence();
                        }

                        //if (this.worldTruth.getCharacterByName("Queenelsa").pMp < 40)
                        //    resting = true;

                    }
                }
            }

            // no target found, go home
            this.broadcastAndWaitCompletionCommand(new MoveToLocationCommand
            {
                distanceFromDestination = 2,
                destinationX = -33,
                destinationZ = -133
            });

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


        #region Wrapped commands
        /// <summary>
        /// Go to specified monster with all chars
        /// </summary>
        /// <param name="monsterTarget"></param>
        /// <returns></returns>
        private bool GoToMonster(ObjectAggregateRoot monsterTarget)
        {
            //this.sendAsyncCommandNoOverride(new MoveToObjectCommand
            //{
            //    characterName = this.worldTruth.getCharacterByName("Queenelsa").characterName,
            //    distanceFromDestination = 15,
            //    obj = monsterTarget
            //});

            //this.sendAsyncCommandNoOverride(new MoveToObjectCommand
            //{
            //    characterName = this.worldTruth.getCharacterByName("Kingofthenorth").characterName,
            //    distanceFromDestination = 15,
            //    obj = monsterTarget
            //});

            this.sendAsyncCommandNoOverride(new MoveToObjectCommand
            {
                characterName = this.worldTruth.getBotLeader().characterName, // doesn't matter here since it's a broadcast
                obj = monsterTarget,
                distanceFromDestination = 5
            });

            if (!this.waitForCharacter(this.worldTruth.getBotLeader().characterName, 25000))
            {
                this.broadcastAndWaitCompletionCommand(new MoveToLocationCommand
                {
                    distanceFromDestination = 2,
                    destinationX = -33,
                    destinationZ = -133
                });
                return false;
            }
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
                if (!this.sendAndWaitCompletionCommand(new UseAbilityCommand
                {
                    characterName = this.worldTruth.getBotLeader().characterName,
                    obj = monsterTarget,
                    ability = AbilityList.Provoke,
                    timeout = 7000
                }))
                {
                    this.sendAndWaitCompletionCommand(new UseAbilityCommand
                    {
                        characterName = this.worldTruth.getBotLeader().characterName,
                        obj = monsterTarget,
                        ability = AbilityList.Chi_Blast,
                        timeout = 7000
                    });
                }
            }

            this.sendAsyncCommandNoOverride(new BeginFightingTargetCommand
            {
                characterName = this.worldTruth.getBotLeader().characterName,
                obj = monsterTarget
            });

            Thread.Sleep(1000);

            //// Follow with elsa
            //this.sendAsyncCommandNoOverride(new MoveToObjectCommand
            //{
            //    characterName = this.worldTruth.getCharacterByName("Queenelsa").characterName,
            //    distanceFromDestination = 9,
            //    obj = this.worldTruth.getBotLeader()
            //});

            //// Follow with elsa
            //this.sendAsyncCommandNoOverride(new MoveToObjectCommand
            //{
            //    characterName = this.worldTruth.getCharacterByName("Kingofthenorth").characterName,
            //    distanceFromDestination = 9,
            //    obj = this.worldTruth.getBotLeader()
            //});

            List<string> fails = waitForCharacters(this.worldTruth.getCharacters(), 20000);

            if (fails.Count > 0)
            {
                this.broadcastAndWaitCompletionCommand(new MoveToLocationCommand
                {
                    distanceFromDestination = 2,
                    destinationX = -33,
                    destinationZ = -133
                });
                return false;
            }
            return true;
        }


        private void ApplyElsaFightingSequence()
        {
            //if (!hasStatusEffect(this.worldTruth.getBotLeader().pStatusEffects, StatusEffect.Haste))
            //{
            //    this.sendAsyncCommandNoOverride(new CastSpellCommand
            //    {
            //        characterName = this.worldTruth.getCharacterByName("Queenelsa").characterName,
            //        spell = SpellList.Haste,
            //        timeout = 7000,
            //        objectName = this.worldTruth.getBotLeader().characterName
            //    });
            //}

            //if (this.worldTruth.getBotLeader().pHpp <= 50)
            //{
            //    this.sendAsyncCommandNoOverride(new CastSpellCommand
            //    {
            //        characterName = this.worldTruth.getCharacterByName("Queenelsa").characterName,
            //        spell = SpellList.Cure_V,
            //        timeout = 7000,
            //        objectName = this.worldTruth.getBotLeader().characterName
            //    });
            //}
        }

        private void ApplyLeaderFightingSequence(ObjectAggregateRoot monsterTarget)
        {
            if (this.worldTruth.getBotLeader().pTp >= 1000 && monsterTarget.pHpp > 50)
            {
                this.sendAsyncCommandNoOverride(new ReadyWeaponskillCommand
                {
                    characterName = this.worldTruth.getBotLeader().characterName,
                    ws = WeaponSkillList.Victory_Smite,
                    timeout = 7000,
                    obj = monsterTarget
                });
            }

            if (this.worldTruth.getBotLeader().pHpp <= 75)
            {
                this.sendAsyncCommandNoOverride(new UseAbilityCommand
                {
                    characterName = this.worldTruth.getBotLeader().characterName,
                    ability = AbilityList.Chakra,
                    timeout = 7000
                });
            }
        }




        #endregion
    }
}
