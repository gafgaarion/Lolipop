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
    public class PurgeCharacterCommandHandler : IHandleCommand<PurgeCharacterCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public PurgeCharacterCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }
        
        public void Handle(PurgeCharacterCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.PurgeCharacter();
            aggregateRootRepository.removeAggregateRoot(command.characterName);
        }
    }
}
