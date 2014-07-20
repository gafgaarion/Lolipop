using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public interface IAggregateRootRepository
    {
        bool aggregateExists(object id);
        T getAggregateRootById<T>(object id) where T : BaseAggregateRoot;
        T createAggregateRoot<T>(object id) where T : BaseAggregateRoot, new();
        void removeAggregateRoot(object id);
        //IEnumerable<T> getAggregateEnumerable<T>() where T : BaseAggregateRoot;
        List<T> getAggregateList<T>() where T : BaseAggregateRoot;
        void copyAggregateTo<T>(int destKey, T copySource) where T : BaseAggregateRoot;
    }
}
