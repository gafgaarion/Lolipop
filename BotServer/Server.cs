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
using FFXIWebObserver;
using FFXIEvents.Events;
using FFXICommands;
using FFXICommands.Commands;
using FFXIWorldKnowledge;
using FFXIWorldKnowledge.Impl;
using FFXIMemoryObserver;
using FFXIMemoryObserver.Impl;
using FFACETools;
using Commons;
using FFXIStrategies;
using FFXIStrategies.Impl;
namespace BotServer
{
    public class Server
    {

        static private IBus bus;
        WindsorContainer container;

        public Server()
        {
            this.Initialize();
            bindCommandHandlers(container);
            bindEventHandlers(container);
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        private void Initialize()
        {
            container = new WindsorContainer();

            //register communications
            container.Register(Component.For<IBus>().ImplementedBy<SimpleBus>());
            container.Register(Component.For<IMemoryInspector>().ImplementedBy<ProcessScannerMemoryInspector>());
            container.Register(Component.For<IAggregateRootRepository>().ImplementedBy<InMemoryAggregateRootRepository>());
            container.Register(Component.For<ITruthRepository>().ImplementedBy<InMemoryTruthRepository>());
            container.Register(Component.For<IObserver>().ImplementedBy<WebObserverHttpServer>());
            container.Register(Component.For<IGateway>().ImplementedBy<Gateway>());
        }

        public void Dispose()
        {
            container.Dispose();
        }

        public IStrategy setStrategy(string strategy)
        {

            container.Dispose();
            this.Initialize();
            bindCommandHandlers(container);
            bindEventHandlers(container);


            if (strategy == "DynamisStrategy")
                container.Register(Component.For<IStrategy>().ImplementedBy<DynamisStrategy>());
            if (strategy == "DemoStrategy")
                container.Register(Component.For<IStrategy>().ImplementedBy<DemoStrategy>());
            if (strategy == "ZeruhnStrategy")
                container.Register(Component.For<IStrategy>().ImplementedBy<ZeruhnStrategy>());
            if (strategy == "VoidwatchStrategy")
                container.Register(Component.For<IStrategy>().ImplementedBy<VoidwatchStrategy>());
            if (strategy == "SpellSkillUpStrategy")
                container.Register(Component.For<IStrategy>().ImplementedBy<SpellSkillUpStrategy>());
            if (strategy == "RangedSkillUpStrategy")
                container.Register(Component.For<IStrategy>().ImplementedBy<RangedSkillUpStrategy>());

            bindStrategy(container);



            return container.Resolve<IStrategy>();
        }

        private static void bindCommandHandlers(WindsorContainer container)
        {
            bus = container.Resolve<IBus>();

            var ffxiCommandsNamespace = @"FFXICommands.Commands";
            var ffxiCommandsTypes = typeof(InitializeCharacterCommand).Assembly.GetTypes();

            foreach (var t in ffxiCommandsTypes.Where(x => x.IsClass && x.Namespace == ffxiCommandsNamespace))
            {
                var registerMethod = bus.GetType().GetMethod("RegisterCommandHandler").MakeGenericMethod(t);

                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var handlerType = typeof(IHandleCommand<>).MakeGenericType(t);
                    var builderTypes = a.GetTypes().Where(x => x.IsClass && x.GetInterfaces().Any(i => i == handlerType));

                    if (builderTypes.Count() == 0)
                        continue;

                    var builderType = builderTypes.First();

                    object instance = null;

                    try
                    {
                        var firstInterfaceType = builderType.GetInterfaces().First();
                        instance = container.Resolve(firstInterfaceType);
                    }
                    catch (Exception e)
                    {
                        var ctorInfo = builderType.GetConstructors().First();

                        var paramList = new List<object>();

                        foreach (var ctorParamInfo in ctorInfo.GetParameters())
                        {
                            paramList.Add(container.Resolve(ctorParamInfo.ParameterType));
                        }

                        instance = ctorInfo.Invoke(paramList.ToArray());
                    }

    
                    var invokeMethod = builderType.GetMethod("Handle");

                    if (invokeMethod == null)
                        throw new Exception("Cannot find Handle<" + t + "> for strategy object " + builderType + " .");

                    Func<object, object, object> action = (i, p) => { object[] arr = { p }; return invokeMethod.Invoke(i, arr); };

                    object[] objInvPrms = new object[] { instance, action };

                    registerMethod.Invoke(bus, objInvPrms);
                }
            }
        }


        private static void bindEventHandlers(WindsorContainer container)
        {
            var bus = container.Resolve<IBus>();

            var ffxiEventsNamespace = @"FFXIEvents.Events";
            var ffxiEventsTypes = typeof(CharacterHasBeenInitializedEvent).Assembly.GetTypes();

            foreach (var t in ffxiEventsTypes.Where(x => x.IsClass && x.Namespace == ffxiEventsNamespace))
            {
                var registerMethod = bus.GetType().GetMethod("RegisterEventHandler").MakeGenericMethod(t);

                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var handlerType = typeof(IHandleEvent<>).MakeGenericType(t);
                    var builderTypes = a.GetTypes().Where(x => x.IsClass && x.GetInterfaces().Any(i => i == handlerType) && !x.GetInterfaces().Any(i => i == typeof(IStrategy)) && x.Name != "BaseStrategy"); //

                    foreach (var builderType in builderTypes)
                    {

                        object instance = null;
                        
                        try
                        {
                            var firstInterfaceType = builderType.GetInterfaces().First();
                            instance = container.Resolve(firstInterfaceType);
                        }
                        catch (Exception e)
                        {
                            var ctorInfo = builderType.GetConstructors().First();

                            var paramList = new List<object>();

                            foreach (var ctorParamInfo in ctorInfo.GetParameters())
                            {
                                paramList.Add(container.Resolve(ctorParamInfo.ParameterType));
                            }

                            instance = ctorInfo.Invoke(paramList.ToArray());
                        }


                        var invokeMethod = builderType.GetMethod("Handle", new Type[]{t});

                        if (invokeMethod == null)
                            throw new Exception("Cannot find Handle<" + t + "> for strategy object " + builderType + " .");

                        Func<object, object, object> action = (i, p) => { object[] arr = { p }; return invokeMethod.Invoke(i, arr); };

                        object[] objInvPrms = new object[] { instance, action };

                        registerMethod.Invoke(bus, objInvPrms);
                    }
                }
            }

          
        }


        private static Type bindStrategy(WindsorContainer container)
        {
            var bus = container.Resolve<IBus>();
            var strategyInstance = container.Resolve<IStrategy>();

            if (strategyInstance == null)
                throw new Exception("Cannot build strategy object!");

            var ffxiEventsNamespace = @"FFXIEvents.Events";
            var ffxiEventsTypes = typeof(CharacterHasBeenInitializedEvent).Assembly.GetTypes();

            foreach (var t in ffxiEventsTypes.Where(x => x.IsClass && x.Namespace == ffxiEventsNamespace))
            {
                var registerMethod = bus.GetType().GetMethod("RegisterEventHandler").MakeGenericMethod(t);
                var handlerType = typeof(IHandleEvent<>).MakeGenericType(t);

                var builderType = strategyInstance.GetType();

                if (!builderType.GetInterfaces().Any(i => i == handlerType))
                    continue;

                var invokeMethod = builderType.GetMethod("Handle", new Type[] { t });

                if (invokeMethod == null)
                    throw new Exception("Cannot find Handle<" + t + "> for strategy object " + builderType + " .");

                Func<object, object, object> action = (i, p) => { object[] arr = { p }; return invokeMethod.Invoke(i, arr); };
                object[] objInvPrms = new object[] { strategyInstance, action };
                registerMethod.Invoke(bus, objInvPrms);
            }

            return strategyInstance.GetType();
        }
    }
}
