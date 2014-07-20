using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXICommands.Commands;
using System.Threading;
using FFACETools;

namespace FFXICommands
{
    public interface IGateway
    {
        void Send(BaseCommand command);
        void Terminate();
    }
}
