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
    public class ReadyWeaponskillCommandHandler : IHandleCommand<ReadyWeaponskillCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public ReadyWeaponskillCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(ReadyWeaponskillCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.ReadyWeaponskill(command.obj, command.objectName, command.ws, command.timeout);

        }
    }
}
