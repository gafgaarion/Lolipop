using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public interface IEventSerializer
    {
        Dictionary<int, Type> GetDeserializationPrefixEquivalenceDictionnary();
        Dictionary<Type, int> GetSerializationPrefixes();
        void SerializeEvent<T>(T domainEvent) where T : DomainEvent;
    }
}
