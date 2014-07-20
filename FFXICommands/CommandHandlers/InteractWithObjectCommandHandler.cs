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
    public class InteractWithObjectCommandHandler : IHandleCommand<InteractWithObjectCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public InteractWithObjectCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(InteractWithObjectCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.InteractWithObject(command.obj, command.DialogTextToChooseOptional, command.timeout);
            

        }
    }
}
