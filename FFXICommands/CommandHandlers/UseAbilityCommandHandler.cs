using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXICommands.Commands;
using FFXIAggregateRoots;

namespace FFXICommands.CommandHandlers
{
    public class UseAbilityCommandHandler : IHandleCommand<UseAbilityCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public UseAbilityCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(UseAbilityCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.UseAbility(command.obj, command.objectName, command.ability, command.timeout);

        }
    }
}
