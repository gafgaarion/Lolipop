using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIWorldKnowledge.Shards;
using CommunicationHandler;
using FFXIEvents.Events;
using System.Windows.Forms;
using Commons;
using FFXIAggregateRoots;
using FFACETools;
using System.Collections.Concurrent;

namespace FFXIWorldKnowledge.Impl
{                                                           
    public class InMemoryTruthRepository : ITruthRepository, IHandleEvent<ObjectHasBeenDefeatedEvent>
    {

        private ConcurrentDictionary<int,AggroShard> aggroDic;
        private IAggregateRootRepository aggrepository;
        public List<AggroShard> aggroList
        {
            get
            {
                lock (this.aggroDic)
                {
                    return this.aggroDic.Values.ToList();
                }
            }
        }

        public InMemoryTruthRepository(IAggregateRootRepository aggrepository)
        {
            this.aggrepository = aggrepository;
            this.aggroDic = new ConcurrentDictionary<int, AggroShard>();
        }

        #region get Methods

        #region Aggregates get methods

        public T getAggregateById<T>(object id) where T : BaseAggregateRoot
        {
            return aggrepository.getAggregateRootById<T>(id);
        }

        public bool charactersAvailable()
        {
            foreach (CharacterAggregateRoot character in this.getCharacters())
            {
                if (character.isLoading)
                    return false;
            }
            return true;
        }

        public ObjectAggregateRoot getObjectByName(string name)
        {
            foreach (ObjectAggregateRoot _object in aggrepository.getAggregateList<ObjectAggregateRoot>())
            {
                if (_object.pObjectName == name)
                {
                    return _object;
                }
            }
            return null;
        }

        public IEnumerable<ObjectAggregateRoot> getObjectsByName(string name)
        {
            foreach (ObjectAggregateRoot _object in aggrepository.getAggregateList<ObjectAggregateRoot>())
            {
                if (_object.pObjectName == name)
                {
                    yield return _object;
                }
            }
        }


        public CharacterAggregateRoot getCharacterByName(string name)
        {
            foreach (CharacterAggregateRoot character in aggrepository.getAggregateList<CharacterAggregateRoot>())
            {
                if (character.characterName == name)
                {
                    return character;
                }
            }
            return null;
        }

        public CharacterAggregateRoot getBotLeader()
        {
            foreach (CharacterAggregateRoot character in aggrepository.getAggregateList<CharacterAggregateRoot>())
            {
                if (character.isBotLeader)
                {
                    return character;
                }
            }
            return null; // shouldn't happen!
        }

        public IEnumerable<CharacterAggregateRoot> getCharacters()
        {
            foreach (CharacterAggregateRoot character in aggrepository.getAggregateList<CharacterAggregateRoot>())
            {
                if (character.isEnabled)
                {
                    yield return character;
                }
            }
        }

        public IEnumerable<CharacterAggregateRoot> getCharactersButLeader()
        {
            foreach (CharacterAggregateRoot character in aggrepository.getAggregateList<CharacterAggregateRoot>())
            {
                if (character.isBotLeader)
                {
                    if (character.isEnabled)
                    {
                        yield return character;
                    }
                }
            }
        }

        public IEnumerable<ObjectAggregateRoot> getPCSelfObjects()
        {
            foreach (ObjectAggregateRoot character in aggrepository.getAggregateList<ObjectAggregateRoot>())
            {
                if (character.pType == NPCType.Self || character.pType == NPCType.PC)
                {
                    yield return character;
                }
            }
        }

        public IEnumerable<ObjectAggregateRoot> getMonsters()
        {
            foreach (ObjectAggregateRoot _object in aggrepository.getAggregateList<ObjectAggregateRoot>())
            {
                if (_object.pType == NPCType.Mob && 
                    _object.pStatus != Status.Dead1 && 
                    _object.pStatus != Status.Dead2)
                    yield return _object;
            }
        }

        public List<AggroShard> getAggros(int characterName)
        {
            string name = this.getAggregateById<CharacterAggregateRoot>(characterName).characterName;
            foreach (ObjectAggregateRoot _object in aggrepository.getAggregateList<ObjectAggregateRoot>())
            {
                if ((_object.pType == NPCType.PC || _object.pType == NPCType.Self) && _object.pObjectName == name)
                {
                    List<AggroShard> aggros = _object.aggroList;
                    for (int i = 0; i < aggros.Count; i++)
                    {
                        aggros[i].aggroedcharacterName = characterName;
                    }
                    return aggros;
                }
            }
            return null;
        }

        public List<AggroShard> getAggrosByObj(int objectId)
        {
            List<AggroShard> aggros = this.getAggregateById<ObjectAggregateRoot>(objectId).aggroList;
            List<int> toRemove = new List<int>();
            for(int i = 0; i < aggros.Count; i++)
            {
                if (this.getAggregateById<ObjectAggregateRoot>(aggros[i].ID).pStatus == Status.Dead2 ||
                    this.getAggregateById<ObjectAggregateRoot>(aggros[i].ID).pStatus == Status.Dead1)
                {
                    toRemove.Add(aggros[i].ID);
                }
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                this.getAggregateById<ObjectAggregateRoot>(objectId).removeAggroById(toRemove[i]);
            }
            if (toRemove.Count > 0)
                aggros = this.getAggregateById<ObjectAggregateRoot>(objectId).aggroList;

            return aggros;
        }

        public IEnumerable<ObjectAggregateRoot> getAllianceMembersObjects()
        {
            FFACE leader = this.getBotLeader().pInstance;
            foreach (FFACE.PartyMemberTools member in leader.PartyMember.Values)
            {
                ObjectAggregateRoot memObj = this.getAggregateById<ObjectAggregateRoot>(member.ServerID);
                if (memObj != null)
                    yield return memObj;
            }
        }

        public List<AggroShard> getAllAggros()
        {
            List<AggroShard> allAggros = new List<AggroShard>();
            foreach (ObjectAggregateRoot character in this.getAllianceMembersObjects())
            {
                List<AggroShard> toAdd = this.getAggrosByObj(character.objectId);
                if (toAdd != null)
                    allAggros.AddRange(toAdd);
            }
            return allAggros;
        }

        #endregion

        #region Shards get methods
        //public IEnumerable<CharacterShard> getCharacterShards()
        //{

        //    return characters.Select(kv => kv.Value);
        //}

        //public CharacterShard GetCharacterShardByName(string name)
        //{
        //    try
        //    {
        //        if (characters.Any(x => x.Value.name == name))
        //        {
        //            return characters.Where(x => x.Value.name == name).First().Value;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }

        //    return null;
        //}
            #endregion

        #endregion

        #region Handlers

        public void Handle(ObjectHasBeenDefeatedEvent domainEvent)
        {
            foreach (ObjectAggregateRoot obj in this.getPCSelfObjects())
            {
                obj.removeAggroById(domainEvent.objectId);
            }
        }


        #endregion

    }
}
