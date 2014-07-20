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
    public class UseRangedAttackCommandHandler : IHandleCommand<UseRangedAttackCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public UseRangedAttackCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(UseRangedAttackCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.UseRangedAttack(command.obj, command.timeout);

        }
    }
}
