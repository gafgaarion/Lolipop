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
    public class StopMoveCommandHandler : IHandleCommand<StopMoveCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public StopMoveCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }
        
        public void Handle(StopMoveCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.StopMove();

        }
    }
}
