using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public interface IHandleCommand<T> where T : BaseCommand
    {
        void Handle(T command);
    }
}
