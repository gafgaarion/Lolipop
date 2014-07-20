using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using Castle.Windsor;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Concurrent;
namespace CommunicationHandler.Impl
{
    public class DomainHandlerTuple
    {
        public object instance { get; set; }
        public Func<object, object, object> action { get; set; }
    }

    public class SimpleBus : IBus
    {
        private Thread commandWorkerThread;
        private Thread eventHandlingWorkerThread;
        private ConcurrentQueue<object> commandStack;
        private ConcurrentQueue<object> eventStack;
        private Dictionary<Type, DomainHandlerTuple> commandHandlers;
        private Dictionary<Type, MethodInfo> handleCommandsMethodInfos;
        private Dictionary<Type, List<DomainHandlerTuple>> domainEventHandlers;
        private Dictionary<Type, MethodInfo> handleEventsMethodInfos;
        private bool loopThreads;

        public SimpleBus()
        {
            handleCommandsMethodInfos = new Dictionary<Type, MethodInfo>();
            handleEventsMethodInfos = new Dictionary<Type, MethodInfo>();
            commandHandlers = new Dictionary<Type, DomainHandlerTuple>();
            commandStack = new ConcurrentQueue<object>();
            eventStack = new ConcurrentQueue<object>();
            domainEventHandlers = new Dictionary<Type, List<DomainHandlerTuple>>();
            commandWorkerThread = new Thread(new ThreadStart(CommandWork));
            eventHandlingWorkerThread = new Thread(new ThreadStart(EventWork));
        }
        
        public void Send(BaseCommand command)
        {
            commandStack.Enqueue(command);
        }

        public void Publish(DomainEvent domainEvent)
        {
            try
            {
                eventStack.Enqueue(domainEvent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        public void RegisterCommandHandler<T>(object instance, Func<object, object, object> handler)
        {
            commandHandlers[typeof(T)] = new DomainHandlerTuple { instance = instance, action = handler };
        }

        public void RegisterEventHandler<T>(object instance, Func<object, object, object> handler)
        {
            if (!domainEventHandlers.Keys.Contains(typeof(T)))
                domainEventHandlers.Add(typeof(T), new List<DomainHandlerTuple>());

            domainEventHandlers[typeof(T)].Add(new DomainHandlerTuple { instance = instance, action = handler });
        }

        public void Start()
        {
            loopThreads = true;
            commandWorkerThread.Start();
            eventHandlingWorkerThread.Start();
        }

        public void Terminate()
        {
            loopThreads = false;
            // Have to abort these, because they usually hang as new events come in during terminaison
            commandWorkerThread.Abort();
            eventHandlingWorkerThread.Abort();
        }
        
        private void EventWork()
        {
            //Console.WriteLine("Bus event worker thread initalized and started");

            while (loopThreads)
            {
                if (eventStack.Count > 0)
                {
                    object domainEvent;
                    eventStack.TryDequeue(out domainEvent);

                    if (domainEvent == null)
                        continue;

                    var eventType = domainEvent.GetType();

                    if (!handleEventsMethodInfos.Keys.Contains(eventType))
                        BuildEventInvokerForType(eventType);
                  
                    object[] parameters = new object[]{domainEvent};

                    handleEventsMethodInfos[eventType].Invoke(this, parameters);
                }
                else
                    Thread.Yield();
            }

            //Console.WriteLine("Bus event worker thread shutting down");
        }

        private void CommandWork()
        {
            //Console.WriteLine("Bus command worker thread initalized and started");

            while (loopThreads)
            {
                if (commandStack.Count > 0)
                {
                    object cmd;
                    commandStack.TryDequeue(out cmd);

                    if (cmd == null)
                        continue;

                    var commandType = cmd.GetType();

                    if (!handleCommandsMethodInfos.Keys.Contains(commandType))
                        BuildCommandInvokerForType(commandType);

                    object[] parameters = new object[] { cmd };
                    handleCommandsMethodInfos[commandType].Invoke(this, parameters);
                }
                else
                    Thread.Yield();
            }

            //Console.WriteLine("Bus command worker thread shutting down");
        }
        
        private void BuildEventInvokerForType(Type domainType)
        {
            Type[] typeArr = new Type[]{domainType};

            var invokeMethod = typeof(SimpleBus).GetMethod("HandleEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            var invokeGeneric = invokeMethod.MakeGenericMethod(typeArr);

            handleEventsMethodInfos[domainType] = invokeGeneric;
        }

        private void BuildCommandInvokerForType(Type commandType)
        {
            Type[] typeArr = new Type[] { commandType };

            var invokeMethod = typeof(SimpleBus).GetMethod("HandleCommand", BindingFlags.NonPublic | BindingFlags.Instance);
            var invokeGeneric = invokeMethod.MakeGenericMethod(typeArr);

            handleCommandsMethodInfos[commandType] = invokeGeneric;
        }

        
        private void HandleCommand<T>(T command)
        {
            if (!commandHandlers.Keys.Contains(typeof(T)))
                return;

            commandHandlers[typeof(T)].action.Invoke(commandHandlers[typeof(T)].instance, command);
        }

        private void HandleEvent<T>(T domainEvent)
        {
            if (!domainEventHandlers.Keys.Contains(typeof(T)))
                return;

            foreach (var handlerTuple in domainEventHandlers[typeof(T)])
            {
                handlerTuple.action.Invoke(handlerTuple.instance, domainEvent);
            }
        }



    }
}