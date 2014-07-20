using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CommunicationHandler.Impl
{
    public class InMemoryAggregateRootRepository : IAggregateRootRepository
    {
        private ConcurrentDictionary<object, BaseAggregateRoot> aggregates;
        private IBus bus;
        
        public InMemoryAggregateRootRepository(IBus bus)
        {
            aggregates = new ConcurrentDictionary<object, BaseAggregateRoot>();
            this.bus = bus;
        }

        public T getAggregateRootById<T>(object id) where T : BaseAggregateRoot
        {
            if (!aggregates.Keys.Contains(id))
                return null;

            return (T)aggregates[id];
        }

        //public IEnumerable<T> getAggregateEnumerable<T>() where T : BaseAggregateRoot
        //{
        //    lock (aggregates)
        //    {
        //        foreach (BaseAggregateRoot _ele in aggregates.Values)
        //        {
        //            if (_ele.GetType() == typeof(T))
        //            {
        //                yield return (T)_ele;
        //            }

        //        }
        //    }
        //}

        public List<T> getAggregateList<T>() where T : BaseAggregateRoot
        {
            List<T> lst = new List<T>();
            foreach(var agg in aggregates)
            {
                if (agg.Value.GetType() == typeof(T))
                {
                    lst.Add((T)agg.Value);
                }
            }
            return lst;
        }

        public void copyAggregateTo<T>(int destKey, T copySource) where T : BaseAggregateRoot
        {
            this.aggregates.TryAdd(destKey, copySource);
        }

        public T createAggregateRoot<T>(object id) where T : BaseAggregateRoot, new()
        {
            var aggregate = new T();
            aggregate.RegisterBus(bus);
            aggregates.TryAdd(id, aggregate);
            return aggregate;
        }

        public void removeAggregateRoot(object id)
        {
            BaseAggregateRoot ret;
            aggregates.TryRemove(id, out ret);
        }

        public bool aggregateExists(object id)
        {
            return aggregates.Keys.Contains(id);
        }
    }
}
