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
    public class TradeItemToTargetCommandHandler : IHandleCommand<TradeItemToTargetCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public TradeItemToTargetCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(TradeItemToTargetCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.TradeItemToObject(command.obj, command.itemsToTrade, command.gil);

        }
    }
}
