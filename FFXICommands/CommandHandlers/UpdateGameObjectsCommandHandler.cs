using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXIAggregateRoots;
using FFXICommands.Commands;
using FFACETools;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace FFXICommands.CommandHandlers
{
    class UpdateGameObjectsCommandHandler : IHandleCommand<UpdateGameObjectsCommand>
    {
        private IAggregateRootRepository aggregateRootRepository;

        public UpdateGameObjectsCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            this.aggregateRootRepository = aggregateRootRepository;
        }


        T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
        }

        public void Handle(UpdateGameObjectsCommand command)
        {
                FFACE.MEntity entity = new FFACE.MEntity();
                for (int i = 0; i < 2048; i++)
                {
                    entity = ByteArrayToStructure<FFACE.MEntity>(command.instance.NPC.GetRawNPCData(i, 0, Marshal.SizeOf(entity)));
                    if (command.instance.NPC.IsActive(i))
                    {

                        ObjectAggregateRoot aggregate = aggregateRootRepository.getAggregateRootById<ObjectAggregateRoot>((int)entity.ID);

                        if (aggregate == null)
                        {
                            aggregate = aggregateRootRepository.createAggregateRoot<ObjectAggregateRoot>((int)entity.ID);

                            aggregate.InitializeObject((int)entity.Index,
                                                       command.instance.NPC.Name(i),
                                                       (int)entity.ID,
                                                       command.instance.NPC.Status(i),
                                                       command.instance.NPC.NPCType(i),
                                                       command.instance.NPC.ClaimedID(i),
                                                       command.instance.NPC.HPPCurrent(i),
                                                       command.instance.NPC.PosX(i),
                                                       command.instance.NPC.PosY(i),
                                                       command.instance.NPC.PosZ(i),
                                                       command.instance.NPC.PosH(i),
                                                       command.instance.NPC.IsActive(i),
                                                       (int)entity.NPCTalking,
                                                       command.instance);
                        }

                        aggregate.UpdateObjectFromClient(i,
                                                         command.instance.NPC.Name(i),
                                                         command.instance.NPC.Status(i),
                                                         command.instance.NPC.NPCType(i),
                                                         command.instance.NPC.ClaimedID(i),
                                                         (int)entity.ID,
                                                         command.instance.NPC.HPPCurrent(i),
                                                         command.instance.NPC.PosX(i),
                                                         command.instance.NPC.PosY(i),
                                                         command.instance.NPC.PosZ(i),
                                                         command.instance.NPC.PosH(i),
                                                         command.instance.NPC.IsActive(i),
                                                         (int)entity.NPCTalking);
                    }
                }

                List<ObjectAggregateRoot> objects = aggregateRootRepository.getAggregateList<ObjectAggregateRoot>();
                List<ObjectAggregateRoot> aggroeds = new List<ObjectAggregateRoot>();
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].pStatus == Status.Fighting && objects[i].pType == NPCType.Mob &&
                        !objects[i].isClaimedByOthers && objects[i].pStatus != Status.Dead1 && objects[i].pStatus != Status.Dead2)
                    {
                        aggroeds.Add(objects[i]);
                    }
                }

                for (int i = 0; i < objects.Count; i++)
                {
                    if ((objects[i].pType == NPCType.PC || objects[i].pType == NPCType.Self) && objects[i].pStatus != Status.Dead1 && objects[i].pStatus != Status.Dead2)
                    {
                        objects[i].hasAnyAggrodMe(aggroeds);
                    }
                }

        }
        
    }
}
