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
    public class Gateway : IGateway
    {
        IBus bus;

        public Gateway(IBus bus)
        {
            this.bus = bus;
        }

        public void Send(BaseCommand command)
        {
            bus.Send(command);
        }

        public void Terminate()
        {
            bus.Terminate();
        }
    }
}
