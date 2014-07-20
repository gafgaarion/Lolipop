using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXIAggregateRoots;
using FFXICommands.Commands;
using FFACETools;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace FFXICommands.CommandHandlers
{
    class UpdateChatCommandHandler : IHandleCommand<UpdateChatCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public UpdateChatCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }

        public void Handle(UpdateChatCommand command)
        {
            foreach (ObjectAggregateRoot agg in aggregateRootRepository.getAggregateList<ObjectAggregateRoot>())
            {
                for (int i = 0; i < command.lines.Count; i++)
                {
                    agg.NewChatLine(command.lines[i]);
                }
            }
        }
    }
}
