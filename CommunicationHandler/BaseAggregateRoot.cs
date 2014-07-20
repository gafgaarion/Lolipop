using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CommunicationHandler
{
    public abstract class BaseAggregateRoot
    {
        private Dictionary<Type, MethodInfo> internalHandlers;
        private IBus bus;

        public BaseAggregateRoot()
        {
            internalHandlers = new Dictionary<Type,MethodInfo>();
        }

        public void RegisterBus(IBus bus)
        {
            this.bus = bus;
        }

        public void Register<T>()
        {
            try
            {
                var methodName = "On" + typeof(T).Name;
                var methodInfo = this.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                internalHandlers[typeof(T)] = methodInfo;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), ex.ToString());
                throw ex;
            }
        }

        public void Apply<T>(T domainEvent) where T : DomainEvent
        {
            internalHandlers[typeof(T)].Invoke(this, new object[] { domainEvent });
            bus.Publish(domainEvent);

        }
    }
}
