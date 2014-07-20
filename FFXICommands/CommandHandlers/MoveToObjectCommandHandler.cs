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
    public class MoveToObjectCommandHandler : IHandleCommand<MoveToObjectCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public MoveToObjectCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(MoveToObjectCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            if (command.obj.GetType() == typeof(ObjectAggregateRoot))
                aggregate.MoveToObject((ObjectAggregateRoot)command.obj, command.distanceFromDestination);
            if (command.obj.GetType() == typeof(CharacterAggregateRoot))
                aggregate.MoveToCharacter((CharacterAggregateRoot)command.obj, command.distanceFromDestination);
            

        }
    }
}
