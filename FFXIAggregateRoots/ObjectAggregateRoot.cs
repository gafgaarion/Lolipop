using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using FFXIEvents.Events;
using FFACETools;
using System.Collections.Concurrent;
using Commons;
using System.Threading;

namespace FFXIAggregateRoots
{
    public class ObjectAggregateRoot : BaseAggregateRoot
    {
        public int objectId { get; set; }

        private string objectName { get; set; }
        private Status status { get; set; }
        private NPCType type { get; set; }
        private int hpp { get; set; }
        private float x { get; set; }
        private float y { get; set; }
        private float z { get; set; }
        private float h { get; set; }
        private int claimId { get; set; }
        private bool initialized { get; set; }
        private bool isActive { get; set; }
        private bool rendered { get; set; }
        private int talking { get; set; }



        // Mob array index
        unsafe private int* arrayIndex;
        public int pIndex;
        public int Index
        {
            get 
            { 
                return pIndex;
            }
        }
        private FFACE leaderInstance;

        // Variable accessible from anywhere
        public string pObjectName { get { if (this.leaderInstance == null) return String.Empty; else return this.leaderInstance.NPC.Name(this.Index); } }
        public Status pStatus { get { if (this.leaderInstance == null) return Status.Dead2; else return this.leaderInstance.NPC.Status(this.Index); } }
        public NPCType pType { get { if (this.leaderInstance == null) return NPCType.NPC; else return this.leaderInstance.NPC.NPCType(this.Index); } }
        public int pHpp { get { if (this.leaderInstance == null) return 0; else return this.leaderInstance.NPC.HPPCurrent(this.Index); } }
        public float pX { get { if (this.leaderInstance == null) return 0; else return this.leaderInstance.NPC.PosX(this.Index); } }
        public float pY { get { if (this.leaderInstance == null) return 0; else return this.leaderInstance.NPC.PosY(this.Index); } }
        public float pZ { get { if (this.leaderInstance == null) return 0; else return this.leaderInstance.NPC.PosZ(this.Index); } }
        public float pH { get { if (this.leaderInstance == null) return 0; else return this.leaderInstance.NPC.PosH(this.Index); } }
        public int pClaimId { get { if (this.leaderInstance == null) return 0; else return this.leaderInstance.NPC.ClaimedID(this.Index); } }
        public bool pInitialized { get { return this.initialized; } }
        public bool pIsActive { get { if (this.leaderInstance == null) return false; else return this.leaderInstance.NPC.IsActive(this.Index); } }
        public bool pIsRendered { get { if (this.leaderInstance == null) return false; else return this.leaderInstance.NPC.IsRendered(this.Index); } }
        public double pDistance { get { if (this.leaderInstance == null) return 60; else return this.leaderInstance.NPC.Distance(this.Index); } }
        public FFACE.Position Position { get { if (this.leaderInstance == null) return new FFACE.Position(); else  return this.leaderInstance.NPC.GetPosition(this.Index); } }
        public int pTalking { get { return this.talking; } }
        public bool isStaggered { get; private set; }
        public bool isClaimedByOthers
        {
            get
            {
                // Unclaimed
                if (this.leaderInstance.NPC.ClaimedID(this.Index) == 0)
                    return false;

                // Claimed by someone in alliance ?
                foreach (var member in leaderInstance.PartyMember)
                {
                    if (member.Value.ServerID == this.leaderInstance.NPC.ClaimedID(this.Index))
                        return false;
                }
                return true;
            }
        }
        public bool isClaimedByParty
        {
            get
            {
                // Unclaimed
                if (this.leaderInstance.NPC.ClaimedID(this.Index) == 0)
                    return false;
                if (this.isClaimedByOthers)
                    return false;
                return true;
            }
        }

        #region Aggrolist

        private ConcurrentDictionary<int, AggroShard> aggroDic;
        public List<AggroShard> aggroList
        {
            get
            {
                lock (aggroDic)
                {
                    return aggroDic.Values.ToList();
                }
            }
        }
        #endregion

        public ObjectAggregateRoot() : base()
        {
            this.aggroDic = new ConcurrentDictionary<int, AggroShard>();
            this.initialized = false;
            this.isStaggered = false;

            unsafe
            {
                this.arrayIndex = null;
            }

            Register<ObjectHasAggroedCharacterEvent>();
            Register<ObjectHasBeenDefeatedEvent>();
            Register<ObjectHasBeenDiscoveredEvent>();
            Register<ObjectHasMovedEvent>();
            Register<ObjectHPHasChangedEvent>();
            Register<ObjectHasBeenClaimedEvent>();
            Register<ObjectHasBeenStaggeredEvent>();
            Register<WorldAggroListChangedEvent>();
            Register<ObjectHasDisappearedEvent>();
            
            
        }


        #region Aggroed
        //Compute the dot product AB . AC
        private double DotProduct(double[] pointA, double[] pointB, double[] pointC)
        {
            double[] AB = new double[2];
            double[] BC = new double[2];
            AB[0] = pointB[0] - pointA[0];
            AB[1] = pointB[1] - pointA[1];
            BC[0] = pointC[0] - pointB[0];
            BC[1] = pointC[1] - pointB[1];
            double dot = AB[0] * BC[0] + AB[1] * BC[1];

            return dot;
        }

        //Compute the cross product AB x AC
        private double CrossProduct(double[] pointA, double[] pointB, double[] pointC)
        {
            double[] AB = new double[2];
            double[] AC = new double[2];
            AB[0] = pointB[0] - pointA[0];
            AB[1] = pointB[1] - pointA[1];
            AC[0] = pointC[0] - pointA[0];
            AC[1] = pointC[1] - pointA[1];
            double cross = AB[0] * AC[1] - AB[1] * AC[0];

            return cross;
        }

        //Compute the distance from A to B
        double Distance(double[] pointA, double[] pointB)
        {
            double d1 = pointA[0] - pointB[0];
            double d2 = pointA[1] - pointB[1];

            return Math.Sqrt(d1 * d1 + d2 * d2);
        }

        //Compute the distance from AB to C
        //if isSegment is true, AB is a segment, not a line.
        double LineToPointDistance2D(double[] pointA, double[] pointB, double[] pointC,
            bool isSegment)
        {
            double dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);
            if (isSegment)
            {
                double dot1 = DotProduct(pointA, pointB, pointC);
                if (dot1 > 0)
                    return Distance(pointB, pointC);

                double dot2 = DotProduct(pointB, pointA, pointC);
                if (dot2 > 0)
                    return Distance(pointA, pointC);
            }
            return Math.Abs(dist);
        }

        public void hasAnyAggrodMe(List<ObjectAggregateRoot> _list)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                double facing = Math.Abs(_list[i].pH - 2*Math.PI);
               // if (facing > 2 * Math.PI)
                  //  facing = 0 + (2 * Math.PI - facing);

                double[] fakePt = new double[2];
                double[] mobPos = new double[2];
                double[] charPos = new double[2];
                mobPos[0] = _list[i].pX;
                mobPos[1] = _list[i].pZ;
                fakePt[0] = Math.Cos(facing) + _list[i].pX;
                fakePt[1] = Math.Sin(facing) + _list[i].pZ;
                charPos[0] = this.pX;
                charPos[1] = this.pZ;

                double distance = LineToPointDistance2D(mobPos, charPos, fakePt, false);

                if (distance < 0.1 &&
                    Math.Sqrt(Math.Pow(this.leaderInstance.NPC.GetPosition(this.Index).X - this.leaderInstance.NPC.GetPosition(_list[i].Index).X, 2) +
                    Math.Pow(this.leaderInstance.NPC.GetPosition(this.Index).Z - this.leaderInstance.NPC.GetPosition(_list[i].Index).Z, 2)) < 25)
                {

                    if (this.aggroDic.Count > 40)
                        this.aggroDic.Clear();

                    if (!this.aggroDic.ContainsKey(_list[i].objectId))
                        this.aggroDic.TryAdd(_list[i].objectId, new AggroShard() { ID = _list[i].objectId, lastUpdated = DateTime.Now.AddSeconds(-2), aggroedObjectId = this.objectId });

                    // Limiter for updates within time
                    if ((DateTime.Now - this.aggroDic[_list[i].objectId].lastUpdated).TotalMilliseconds < 500)
                        continue;

                    if (aggroDic[_list[i].objectId].H != _list[i].pH ||
                        aggroDic[_list[i].objectId].X != _list[i].pX ||
                        aggroDic[_list[i].objectId].Y != _list[i].pY ||
                        aggroDic[_list[i].objectId].Z != _list[i].pZ ||
                        aggroDic[_list[i].objectId].claimedID != _list[i].pClaimId)
                    {

                        aggroDic[_list[i].objectId].H = _list[i].pH;
                        aggroDic[_list[i].objectId].X = _list[i].pX;
                        aggroDic[_list[i].objectId].Y = _list[i].pY;
                        aggroDic[_list[i].objectId].Z = _list[i].pZ;
                        aggroDic[_list[i].objectId].claimedID = _list[i].pClaimId;
                        aggroDic[_list[i].objectId].lastUpdated = DateTime.Now;

                        Apply<WorldAggroListChangedEvent>(new WorldAggroListChangedEvent());
                    }
                }
            }
        }
        #endregion

        public void InitializeObject(int index, string name, int objectId, Status status, NPCType type, int claimId,
                                     int hpp, float x, float y, float z, float h, bool active, int talking, FFACE instance)
        {
            this.leaderInstance = instance;
            unsafe
            {
                if (this.initialized)
                    throw new InvalidOperationException("Object already exists");

                // Saves the mob array index in a pointer, to be able to access it from anywhere the moment it changes
                lock (this)
                {
                    pIndex = index;
                    fixed (int* ptr = &pIndex) { arrayIndex = ptr; }
                }

                this.isActive = active;
                this.objectId = objectId;
                this.objectName = name;
                this.initialized = true;
                this.hpp = hpp;
                this.h = h;
                this.x = x;
                this.y = y;
                this.z = z;
                this.h = h;
                this.status = status;
                this.type = type;
                this.talking = talking;
                this.rendered = this.leaderInstance.NPC.IsRendered(index);

                Apply<ObjectHasBeenDiscoveredEvent>(new ObjectHasBeenDiscoveredEvent
                {
                    objectId = objectId,
                    objectName = name
                });
            }
        }

        public void UpdateObjectFromClient(int index, string name, Status status, NPCType type, int claimId,
                                           int objectId, int hpp, float x, float y, float z, float h, bool active, int talking)
        {
            unsafe
            {
                // Saves the mob array index in a pointer, to be able to access it from anywhere the moment it changes
                lock (this)
                {
                    pIndex = index;
                    fixed (int* ptr = &pIndex) { arrayIndex = ptr; }
                }

                this.isActive = active;

                if (!this.initialized)
                    throw new InvalidOperationException("Tried to update non-initialized object");

                if (this.status != status && (status == Status.Dead2 || status == Status.Dead1))
                {
                    this.status = status;
                    Apply<ObjectHasBeenDefeatedEvent>(new ObjectHasBeenDefeatedEvent { objectId = objectId });
                }

                if (this.leaderInstance.NPC.IsRendered(index) != this.rendered)
                {
                    this.rendered = this.leaderInstance.NPC.IsRendered(index);
                    if (this.rendered)
                    {
                        Apply<ObjectHasBeenDiscoveredEvent>(new ObjectHasBeenDiscoveredEvent
                        {
                            objectId = objectId,
                            objectName = name
                        });
                    }
                    else
                    {
                        Apply<ObjectHasDisappearedEvent>(new ObjectHasDisappearedEvent
                        {
                            objectId = objectId,
                            objectName = name
                        });
                    }
                }


                if (this.status != status && (this.status == Status.Dead2 || this.status == Status.Dead1))
                {
                    this.status = status;
                    Apply<ObjectHasBeenDiscoveredEvent>(new ObjectHasBeenDiscoveredEvent
                    {
                        objectId = objectId,
                        objectName = name
                    });
                }


                if (hpp != this.hpp)
                    Apply<ObjectHPHasChangedEvent>(new ObjectHPHasChangedEvent { objectId = objectId, hpp = hpp, oldHpp = this.hpp });



                if (x != this.x || y != this.y || z != this.z || h != this.h)
                    Apply<ObjectHasMovedEvent>(new ObjectHasMovedEvent
                    {
                        objectId = this.objectId,
                        oldX = this.x,
                        oldY = this.y,
                        oldZ = this.z,
                        newX = x,
                        newY = y,
                        newZ = z,
                        oldFacing = this.h,
                        newFacing = h
                    });


                if (pStatus == Status.Fighting && claimId != this.claimId && claimId != 0)
                {
                    Apply<ObjectHasBeenClaimedEvent>(new ObjectHasBeenClaimedEvent { objectId = this.objectId, claimedId = claimId });
                }
            }
        }


        public void NewChatLine(FFACE.ChatTools.ChatLine line)
        {
            if (this.pStatus == Status.Fighting)
            {
                if (line.Text.Contains("attack staggers the fiend"))
                {
                    // Check if it belongs to alliance
                    foreach (var member in this.leaderInstance.PartyMember)
                    {
                        if (line.Text.Contains(member.Value.Name))
                        {
                            new Thread(
                                 delegate()
                                 {
                                     Thread.Sleep(1500);
                                     // Staggered!!
                                     Apply<ObjectHasBeenStaggeredEvent>(new ObjectHasBeenStaggeredEvent { objectId = this.objectId, staggeredBy = member.Value.Name, staggeredById = member.Value.ServerID });
                                 }).Start();
                        }
                    }
                    if (line.Text.Contains("Your "))
                    {
                        new Thread(
                             delegate()
                             {
                                 Thread.Sleep(1500);
                                 // Staggered!!
                                 Apply<ObjectHasBeenStaggeredEvent>(new ObjectHasBeenStaggeredEvent { objectId = this.objectId, staggeredBy = this.leaderInstance.Player.Name, staggeredById = this.leaderInstance.Player.PlayerServerID });
                             }).Start();
                    }
                }
            }
        }

        public void removeAggroById(int objectId)
        {
            if (aggroDic.ContainsKey(objectId))
            {
                AggroShard removed;
                this.aggroDic.TryRemove(objectId, out removed);
                Apply<WorldAggroListChangedEvent>(new WorldAggroListChangedEvent());
            }
        }


        private void OnObjectHasBeenStaggeredEvent(ObjectHasBeenStaggeredEvent domainEvent)
        {
            this.isStaggered = true;
        }

        private void OnWorldAggroListChangedEvent(WorldAggroListChangedEvent domainEvent)
        {
            GlobalDelegates.onWorldAggroListChanged(domainEvent);
        }

        private void OnObjectHasAggroedCharacterEvent(ObjectHasAggroedCharacterEvent domainEvent)
        {
        }

        private void OnObjectHasBeenDefeatedEvent(ObjectHasBeenDefeatedEvent domainEvent)
        {
            this.status = Status.Dead2;
            this.hpp = 0;
        }

        private void OnObjectHasBeenDiscoveredEvent(ObjectHasBeenDiscoveredEvent domainEvent)
        {
        }

        private void OnObjectHasDisappearedEvent(ObjectHasDisappearedEvent domainEvent)
        {
        }

        private void OnObjectHasMovedEvent(ObjectHasMovedEvent domainEvent)
        {
            unsafe
            {
                this.x = domainEvent.newX;
                this.y = domainEvent.newY;
                this.z = domainEvent.newZ;
                this.h = domainEvent.newFacing;
            }
        }

        private void OnObjectHPHasChangedEvent(ObjectHPHasChangedEvent domainEvent)
        {
            unsafe
            {
                this.hpp = domainEvent.hpp;

                if (this.hpp <= 0)
                    this.status = Status.Dead2;
            }
        }

        private void OnObjectHasBeenClaimedEvent(ObjectHasBeenClaimedEvent domainEvent)
        {
            this.claimId = domainEvent.claimedId;
            
        }


    }
}

