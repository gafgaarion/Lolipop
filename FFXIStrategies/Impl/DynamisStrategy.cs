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

namespace FFXIStrategies.Impl
{
    public class DynamisStrategy : BaseStrategy, IStrategy
    {
        public DynamisStrategy(ITruthRepository repository, IBus bus)
            : base(repository, bus)
        {
            // Configuration for bestiary mobs to look for
            this.configurations.Add(new ConfigurationField { fieldType = typeof(ListView), name = "MonstersList0-8", values = null, height = 300, width = 200 });
            this.configurations.Add(new ConfigurationField { fieldType = typeof(ListView), name = "MonstersList8-16", values = null, height = 300, width = 200 });
            this.configurations.Add(new ConfigurationField { fieldType = typeof(ListView), name = "MonstersList16-24", values = null, height = 300, width = 200 });

        }


        // this will loop until Terminate is called
        protected override void Strategy()
        {

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
