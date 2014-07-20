using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIWorldKnowledge.Shards;
using FFXIAggregateRoots;
using CommunicationHandler;
using FFACETools;

namespace FFXIWorldKnowledge
{
    public interface ITruthRepository
    {
        T getAggregateById<T>(object id) where T : BaseAggregateRoot;
        CharacterAggregateRoot getBotLeader();
        CharacterAggregateRoot getCharacterByName(string name);
        IEnumerable<CharacterAggregateRoot> getCharacters();
        IEnumerable<CharacterAggregateRoot> getCharactersButLeader();
        IEnumerable<ObjectAggregateRoot> getMonsters();
        ObjectAggregateRoot getObjectByName(string name);
        IEnumerable<ObjectAggregateRoot> getObjectsByName(string name);
        List<AggroShard> getAggros(int characterName);
        List<AggroShard> getAllAggros();
        bool charactersAvailable();
    }
}
