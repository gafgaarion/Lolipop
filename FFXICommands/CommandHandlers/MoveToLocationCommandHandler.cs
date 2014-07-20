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
    public class MoveToLocationCommandHandler : IHandleCommand<MoveToLocationCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public MoveToLocationCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }
        
        public void Handle(MoveToLocationCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.MoveToLocation(command.destinationX, command.destinationZ, command.distanceFromDestination);

        }
    }
}
