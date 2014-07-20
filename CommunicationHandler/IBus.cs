using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public interface IBus
    {
        void Start();
        void Send(BaseCommand command);
        void Publish(DomainEvent domainEvent);
        void RegisterEventHandler<T>(object instance, Func<object, object, object> handler);
        void RegisterCommandHandler<T>(object instance, Func<object, object, object> handler);
        void Terminate();
    }
}
