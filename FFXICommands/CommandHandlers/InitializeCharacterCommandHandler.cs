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
    public class InitializeCharacterCommandHandler : IHandleCommand<InitializeCharacterCommand>
    {

        private IAggregateRootRepository aggregateRootRepository;

        public InitializeCharacterCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(InitializeCharacterCommand command)
        {
            var aggregate = aggregateRootRepository.createAggregateRoot<CharacterAggregateRoot>(command.characterName);
            aggregate.InitializeCharacter(command.instance, command.characterName);
        }
    }
}
