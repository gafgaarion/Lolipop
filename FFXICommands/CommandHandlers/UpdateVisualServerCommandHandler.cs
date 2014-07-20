using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using CommunicationHandler;
using CommunicationHandler.Impl;
using System.Reflection;
using FFXICommands.Commands;
using FFXIAggregateRoots;

namespace BotServer
{
    /*
    class UpdateVisualServerCommandHandler : IHandleCommand<UpdateVisualServerCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public UpdateVisualServerCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(UpdateCharacterStatusCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.UpdateCharacterFromClient(command.positionX, command.positionY, command.positionZ, command.facing,
                                                command.hpp, command.hp, command.tp, command.mp, command.botCurrentAction);
        }
    }
     */
}
