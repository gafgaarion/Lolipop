using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXIEvents.Events;
using System.IO;

namespace FFXIEvents.Impl
{
    public class FileBasedEventSerializer :IEventSerializer
    {
        private Stream logFileStream;
        private Dictionary<Type, int> serializationPrefixes;
        private Dictionary<int, Type> deserializationPrefixes;

        public FileBasedEventSerializer()
        {
            var mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var format = "MMM_ddd_d_HH_mm_yyyy";
            var logFileName = @"\FFXI_EventLog_" + DateTime.Now.ToString(format) + @".txt";

            this.logFileStream = File.Create(mydocpath + logFileName);
            this.serializationPrefixes = new Dictionary<Type, int>
            {
                {typeof(CharacterHasMovedEvent),100},
                {typeof(CharacterAvailableForActionEvent),101},
                {typeof(CharacterIsBusyWithActionEvent),102},
                {typeof(CharacterHPHasChangedEvent),103},
                {typeof(CharacterHasBeenInitializedEvent),104}
            };

            this.deserializationPrefixes = new Dictionary<int, Type>
            {
                {100, typeof(CharacterHasMovedEvent)},
                {101, typeof(CharacterAvailableForActionEvent)},
                {102, typeof(CharacterIsBusyWithActionEvent)},
                {103, typeof(CharacterHPHasChangedEvent)},
                {104, typeof(CharacterHasBeenInitializedEvent)}
            };
        }
        public Dictionary<int, Type> GetDeserializationPrefixEquivalenceDictionnary()
        {
            return this.deserializationPrefixes;
        }

        public Dictionary<Type, int> GetSerializationPrefixes()
        {
            return this.serializationPrefixes;
        }
        
        public void SerializeEvent<T>(T domainEvent) where T : DomainEvent
        {
            //Serializer.NonGeneric.SerializeWithLengthPrefix(logFileStream, domainEvent, PrefixStyle.Base128, this.serializationPrefixes[typeof(T)]);
        }
    }
}
