using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXIAggregateRoots;
using FFXICommands.Commands;
using FFACETools;
using System.Threading;
using Commons;

namespace FFXICommands.CommandHandlers
{
    class UpdateCharacterStatusCommandHandler : IHandleCommand<UpdateCharacterStatusCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public UpdateCharacterStatusCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(UpdateCharacterStatusCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            
            if (aggregate == null)
            {
                return;
            }


            aggregate.UpdateCharacterFromClient(command.player, command.party, command.target);

            for( int i = 0; i < command.chat.Count; i++)
            {
                aggregate.NewChatLine(command.chat[i]);
            }
        }
    }
}
