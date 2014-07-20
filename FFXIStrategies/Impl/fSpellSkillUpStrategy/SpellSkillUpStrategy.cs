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

namespace FFXIStrategies.Impl
{
    public class SpellSkillUpStrategy : BaseStrategy, IStrategy
    {
        public SpellSkillUpStrategy(ITruthRepository repository, IBus bus)
            : base(repository, bus)
        {
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


        // this will loop until Terminate is called
        protected override void Strategy()
        {

            //ObjectAggregateRoot target;
            //target = this.worldTruth.getObjectByName("Sattsuh Ahkanpari");

            if (this.worldTruth.getBotLeader().isEnabled)
            {
                this.worldTruth.getBotLeader().usePathFinder = false;


                this.worldTruth.getBotLeader().pInstance.Navigator.Goto(86, -142, false, 3500);

                while (this.worldTruth.getObjectByName("Moogle") == null) ;

                Thread.Sleep(8000);

                this.worldTruth.getBotLeader().pInstance.Navigator.FaceHeading(new FFACE.Position() { X = 0, Z = -7.12f });
                Thread.Sleep(500);
                this.worldTruth.getBotLeader().pInstance.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(500);
                this.worldTruth.getBotLeader().pInstance.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(500);
                this.worldTruth.getBotLeader().pInstance.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(300);
                this.worldTruth.getBotLeader().pInstance.Windower.SendKeyPress(KeyCode.EnterKey);

                while (this.worldTruth.getObjectByName("Phersula") == null) ;

                while (this.worldTruth.getBotLeader().pMp > 12)
                {
                    this.sendAndWaitCompletionCommand(new CastSpellCommand
                    {
                        characterName = this.worldTruth.getBotLeader().characterName,
                        objectName = this.worldTruth.getBotLeader().characterName,
                        spell = SpellList.Barfira,
                        timeout = 7000
                    });

                    this.sendAndWaitCompletionCommand(new CastSpellCommand
                    {
                        characterName = this.worldTruth.getBotLeader().characterName,
                        objectName = this.worldTruth.getBotLeader().characterName,
                        spell = SpellList.Barthundra,
                        timeout = 7000
                    });

                    this.sendAndWaitCompletionCommand(new CastSpellCommand
                    {
                        characterName = this.worldTruth.getBotLeader().characterName,
                        objectName = this.worldTruth.getBotLeader().characterName,
                        spell = SpellList.Barstonra,
                        timeout = 7000
                    });

                    this.sendAndWaitCompletionCommand(new CastSpellCommand
                    {
                        characterName = this.worldTruth.getBotLeader().characterName,
                        objectName = this.worldTruth.getBotLeader().characterName,
                        spell = SpellList.Barwatera,
                        timeout = 7000
                    });

                    this.sendAndWaitCompletionCommand(new CastSpellCommand
                    {
                        characterName = this.worldTruth.getBotLeader().characterName,
                        objectName = this.worldTruth.getBotLeader().characterName,
                        spell = SpellList.Barblizzara,
                        timeout = 7000
                    });
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
