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
    public class SwitchTargetCommandHandler : IHandleCommand<SwitchTargetCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public SwitchTargetCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(SwitchTargetCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.SwitchTarget(command.obj.objectId, command.obj, command.lockTarget);

        }
    }
}
