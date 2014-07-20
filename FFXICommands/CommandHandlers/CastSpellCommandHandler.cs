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
    public class CastSpellCommandHandler : IHandleCommand<CastSpellCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public CastSpellCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(CastSpellCommand command)
        {
            var aggregate = aggregateRootRepository.getAggregateRootById<CharacterAggregateRoot>(command.characterName);

            if (aggregate == null)
                return;

            aggregate.CastSpell(command.obj, command.objectName, command.spell, command.timeout);

        }
    }
}
