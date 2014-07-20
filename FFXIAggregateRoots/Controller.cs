using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;
using System.Threading;
using FFXIEvents.Events;
using Commons;
using CommunicationHandler;
using System.Drawing;

namespace FFXIAggregateRoots
{
    public class Controller
    {
        private float X_Adjustment = 0.0f;
        private float Z_Adjustment = 0.0f;
        private const float normalLengthMult = 3f;
        private const float DISTANCE_TOLERANCE = 1.5f;

        private float positionX;
        private float positionY;
        private float positionZ;

        private D3DMap mapLib;
        private FFACE m_instance;
        public CharacterAggregateRoot.ControllerCommandCompletedCallback commandCompletedCallback;

        // Pathing variables
        List<float[]> myPath; // pathfinder nodes
        float minDistanceToLoca;

        public bool isRunning { get; private set; }
        public bool isDestinationReached { get; private set; }
        public bool usePathFinder;

        // Threading
        private Thread memoryThread = null;
        private bool stop;

       // Fighting
        private bool fightingStop = false;
        private Thread fightingMemoryThread = null;
        public bool isFighting { get; private set; }

        public Controller(FFACE _instance,
                          CharacterAggregateRoot.ControllerCommandCompletedCallback callback,
                          bool usePathFinder = true)
        {
            this.commandCompletedCallback = callback;
            this.usePathFinder = usePathFinder;
            isRunning = false;
            isFighting = false;
            positionX = 0;
            positionY = 0;
            positionZ = 0;
            m_instance = _instance;
            stop = false;
            mapLib = new D3DMap(X_Adjustment, Z_Adjustment);
        }

        public void Terminate()
        {
            this.stop = true;
            this.fightingStop = true;

            if (this.fightingMemoryThread != null)
                this.fightingMemoryThread.Abort();
            
            if (this.memoryThread != null)
                this.memoryThread.Abort();
        }

        public void LoadMap(Zone mapId, bool _notifyUI)
        {
            //if (!mapLib.LoadMap((short)mapId))
              //  throw new Exceptions.UnableToLoadMapException();

     
            int lowerPriorityLayers = 1;

            if (mapId == Zone.Dynamis_Valkurm)
            {
                lowerPriorityLayers = 2;
            }
            else if (mapId == Zone.Valkurm_Dunes)
            {
                lowerPriorityLayers = 2;
            }
            else if (mapId == Zone.Port_Windurst)
            {
                lowerPriorityLayers = 1;
            }

            mapLib.LoadMap((short)mapId, lowerPriorityLayers, this.X_Adjustment, this.Z_Adjustment);

            if (_notifyUI) NotifyUI();
            
        }

        private void OverrideLastCommand(bool runningFlag = true)
        {
            stop = true;
            if (this.memoryThread != null)
                this.memoryThread.Abort();
            stop = false;

            if (runningFlag)
                this.isRunning = false;

            this.m_instance.Navigator.Reset();
            this.m_instance.Navigator.StopRunning();
            this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
            this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);
            Thread.Sleep(20);
            //this.removeSubTarget();
            //this.CloseMenus();
        }



        public void NotifyUI()
        {
            GlobalDelegates.onMapHasBeenLoaded(mapLib);
        }

        #region Movement controls

        private CharacterAggregateRoot characterPoint;
        private ObjectAggregateRoot objectPoint;

        public void setPosition(float _x, float _y, float _z)
        {
            positionX = _x;
            positionY = _y;
            positionZ = _z;
        }


        public void StopMove()
        {
            if (this.myPath != null)
            {
                lock (myPath)
                {
                    this.myPath.Clear();
                }
            }


            this.m_instance.Navigator.StopRunning();
            this.m_instance.Navigator.Reset();
            this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
            this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);
            this.commandCompletedCallback(true);
        }

        /// <summary>
        /// Starts the process of going to a given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>True if the process has begun properly</returns>
        public bool MoveToCharacter(CharacterAggregateRoot _object, float distanceFromLoca = 3.0f, bool doCallback = true)
        {
            characterPoint = _object;
            objectPoint = null;
            return MoveToLocation(_object.pX, _object.pZ, distanceFromLoca, true, doCallback);
        }

        /// <summary>
        /// Starts the process of going to a given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>True if the process has begun properly</returns>
        public bool MoveToObject(ObjectAggregateRoot _object, float distanceFromLoca = 3.0f, bool doCallback = true)
        {
            objectPoint = _object;
            characterPoint = null;
            return MoveToLocation(_object.pX, _object.pZ, distanceFromLoca, true, doCallback);
        }

        private void reroute()
        {
            if (characterPoint != null)
                MoveToCharacter(characterPoint, minDistanceToLoca);
            else if (objectPoint != null)
                MoveToObject(objectPoint, minDistanceToLoca);
        }


        DateTime PathStartTime; // Recalculate timer to preserve CPU
        /// <summary>
        /// Starts the process of going to a given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>True if the process has begun properly</returns>
        public bool MoveToLocation(float x, float z, float distanceFromLoca, bool followPoint = false, bool doCallback = true)
        {
            PathStartTime = DateTime.Now;
            this.isRunning = false;
            if (!followPoint)
            {
                characterPoint = null;
                objectPoint = null;
            }

            // Already there
            if (this.getDistance(x, z, this.positionX, this.positionZ) <= distanceFromLoca)
            {
                this.commandCompletedCallback(true);
                return true;
            }

            // As close as bot needs to reach before calling it.
            minDistanceToLoca = distanceFromLoca;

            this.UnlockTarget();
            

            if (this.usePathFinder && getDistance(x, z, this.positionX, this.positionZ) > 2)
            {
                List<float[]> nodes;
                bool closestFound;

                Point destination = this.mapLib.getClosestFromFFxiCoord(x, z, out closestFound);
                if (!closestFound)
                {
                    this.commandCompletedCallback(false);
                    return false;
                }

                Point start = this.mapLib.getClosestFromFFxiCoord(this.positionX, this.positionZ, out closestFound);
                if (!closestFound)
                {
                    this.commandCompletedCallback(false);
                    return false;
                }

                nodes = this.mapLib.getPathFromFFxiCoord(start, destination);

                if (nodes == null)
                {
                    this.commandCompletedCallback(false);
                    return false;
                }

                // Adjust last node
                nodes[0][0] = x;
                nodes[0][1] = z;

                this.MoveAlongPath(nodes, doCallback);
                this.isRunning = true;
            }
            else
            {
                this.MoveToNearbyPoint(x, z, false, doCallback);
                this.isRunning = true;
            }
            return true;
        }

        #region Private Methods

        private float getDistance(float Xpt1, float Zpt1,
                                  float Xpt2, float Zpt2)
        {
            return (float)Math.Sqrt(Math.Pow(Xpt2 - Xpt1, 2) + Math.Pow(Zpt2 - Zpt1, 2));
        }

        // will adjust the destination depending on what was the angle error on previous point
        private double[] adjustNextPoint(float originX, 
                                         float originZ,
                                         float nextX, 
                                         float nextZ)
        {
            if (!(originX == positionX && originZ == positionZ))
            {
                float headingTo = this.m_instance.Navigator.HeadingTo(originX, originZ, nextX, nextZ, FFACETools.HeadingType.Degrees);
                float headingTo2 = this.m_instance.Navigator.HeadingTo(originX, originZ, this.positionX, this.positionZ, HeadingType.Degrees);
                float result = (headingTo2 - headingTo) * -1;

                double distance = Math.Sqrt(Math.Pow(nextX - originX, 2) + Math.Pow(nextZ - originZ, 2));

                double newX = Math.Cos(headingTo2) * (nextX - this.positionX) - Math.Sin(headingTo2) * (nextZ - this.positionZ) + this.positionX;
                double newZ = Math.Sin(headingTo2) * (nextX - this.positionX) + Math.Cos(headingTo2) * (nextZ - this.positionZ) + this.positionZ;

                return new double[2] { newX, newZ };
            }
            return new double[2] { nextX, nextZ };
        }


        private void MoveToNextNoAdjustment()
        {
            lock (this)
            {
                float destX;
                float destZ;
                float originX;
                float originZ;




                while (!stop && myPath != null && myPath.Count > 0)
                {
                    // Recalculate if object has changed location
                    if (((DateTime.Now - this.PathStartTime).TotalMilliseconds) > 1800)
                    {
                        if (this.characterPoint != null)
                        {
                            if (Math.Abs(this.characterPoint.pX - this.myPath[0][0]) > 1 ||
                                Math.Abs(this.characterPoint.pZ - this.myPath[0][1]) > 1)
                            {
                                this.m_instance.Navigator.Reset();
                                this.m_instance.Navigator.StopRunning();
                                this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                                this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);
                                new Thread(reroute).Start();
                                return;
                            }
                        }

                        if (this.objectPoint != null)
                        {
                            if (Math.Abs(this.objectPoint.pX - this.myPath[0][0]) > 1 ||
                                Math.Abs(this.objectPoint.pZ - this.myPath[0][1]) > 1)
                            {
                                this.m_instance.Navigator.Reset();
                                this.m_instance.Navigator.StopRunning();
                                this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                                this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);
                                new Thread(reroute).Start();
                                return;
                            }
                        }
                    }

                    destX = myPath[myPath.Count - 1][0];
                    destZ = myPath[myPath.Count - 1][1];
                    originX = this.positionX;
                    originZ = this.positionZ;

                    // Minimum distance to destination has been reached, abort rest, mission is done!
                    if (this.getDistance(originX, originZ, this.myPath[0][0], this.myPath[0][1]) <= this.minDistanceToLoca)
                    {
                        lock (myPath)
                        {
                            myPath.Clear();
                            this.m_instance.Navigator.StopRunning();
                            this.m_instance.Navigator.Reset();
                            this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                            this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);
                            if (MoveDoCallback) this.commandCompletedCallback(true);
                            return;
                        }
                    }

                    // If it's the last point, adjust tolerance to fit the command
                    if (this.myPath.Count == 1)
                    {
                        this.m_instance.Navigator.DistanceTolerance = this.minDistanceToLoca;
                    }

                    if (!this.m_instance.Navigator.Goto(destX, destZ, true, 3700))
                    {
                        if (!stop)
                        {
                            // Attempts moving after moving through normals
                            double dz;
                            double dx;
                            double distanceN1dest;
                            double distanceN2dest;
                            int nbLoop = 0;

                            do
                            {
                                dz = destZ - this.positionZ;
                                dx = destX - this.positionX;


                                double aheadPointX = myPath[myPath.Count - 1][0] + X_Adjustment;
                                double aheadPointZ = myPath[myPath.Count - 1][1] + Z_Adjustment;

                                if (myPath.Count > 1)
                                {
                                    aheadPointX = myPath[myPath.Count - 2][0] + X_Adjustment;
                                    aheadPointZ = myPath[myPath.Count - 2][1] + Z_Adjustment;
                                }

                                distanceN1dest = Math.Sqrt(Math.Pow(destX - (-aheadPointZ + this.positionX), 2) + Math.Pow(destZ - (aheadPointX + this.positionZ - Z_Adjustment), 2));
                                distanceN2dest = Math.Sqrt(Math.Pow(destX - (aheadPointZ + this.positionX), 2) + Math.Pow(destZ - (-aheadPointX + this.positionZ - Z_Adjustment), 2));

                                this.m_instance.Navigator.StopRunning();
                                this.m_instance.Navigator.Reset();


                                if (distanceN1dest < distanceN2dest)
                                {
                                    this.m_instance.Windower.SendKey(KeyCode.NP_Number2, true);
                                    Thread.Sleep(500);
                                    this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                                    this.m_instance.Navigator.Goto((float)-dz * normalLengthMult + this.positionX, (float)dx * normalLengthMult + this.positionZ, false, 3000); // normal 1
                                    if (!this.m_instance.Navigator.Goto(destX, destZ, true, 3000))
                                    {
                                        this.m_instance.Windower.SendKey(KeyCode.NP_Number2, true);
                                        Thread.Sleep(500);
                                        this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                                        this.m_instance.Navigator.Goto((float)dz * normalLengthMult + this.positionX, (float)-dx * normalLengthMult + this.positionZ, false, 3000); // normal 2
                                    }
                                }
                                else
                                {
                                    this.m_instance.Windower.SendKey(KeyCode.NP_Number2, true);
                                    Thread.Sleep(500);
                                    this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                                    this.m_instance.Navigator.Goto((float)dz * normalLengthMult + this.positionX, (float)-dx * normalLengthMult + this.positionZ, false, 3000); // normal 2
                                    if (!this.m_instance.Navigator.Goto(destX, destZ, true, 3000))
                                    {
                                        this.m_instance.Windower.SendKey(KeyCode.NP_Number2, true);
                                        Thread.Sleep(500);
                                        this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                                        this.m_instance.Navigator.Goto((float)-dz * normalLengthMult + this.positionX, (float)dx * normalLengthMult + this.positionZ, false, 3000); // normal 1
                                    }
                                }
                                nbLoop++;
                            }
                            while (!this.m_instance.Navigator.Goto(destX, destZ, true, 3500) && !stop && nbLoop < 2);

                            if (nbLoop >= 2)
                            {
                                // failed to reach destination
                                this.m_instance.Navigator.StopRunning();
                                this.m_instance.Navigator.Reset();
                                this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                                this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);
                                this.stop = true;
                                if (MoveDoCallback) this.commandCompletedCallback(false);
                                return;
                            }
                        }

                    }

                    lock (myPath)
                    {
                        if (myPath.Count > 0)
                        {
                            myPath.RemoveAt(myPath.Count - 1);
                        }
                    }
                    System.Threading.Thread.Yield();

                }


                this.m_instance.Navigator.StopRunning();
                this.m_instance.Navigator.Reset();
                this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
                this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);

                // If hasn't been forced to stop
                if (!stop)
                {
                    if (MoveDoCallback) this.commandCompletedCallback(true);
                }

            }
        }

        private bool MoveDoCallback;
        private void MoveAlongPath(List<float[]> nodes, bool doCallback = true)
        {
            MoveDoCallback = doCallback;
            m_instance.Navigator.DistanceTolerance = DISTANCE_TOLERANCE;

            if (this.myPath != null)
            {
                lock (myPath)
                {
                    this.myPath.Clear();
                }
            }

            OverrideLastCommand();

            myPath = nodes;

            
            this.memoryThread = new Thread(MoveToNextNoAdjustment);
            this.memoryThread.Start();
        }

        private float nearbyX;
        private float nearbyZ;
        private bool continueToRun = false;
        /// <summary>
        /// When pathfinder is disabled
        /// </summary>
        private void MoveToNearbyPoint(float x, float z, bool _continueToRun, bool RunningFlag = true, bool doCallback = true)
        {
            continueToRun = _continueToRun;
            nearbyX = x;
            nearbyZ = z;

            OverrideLastCommand(RunningFlag);

            this.memoryThread = new Thread(MoveToNearbyWorker);
            this.memoryThread.Start();
        }

        private void MoveToNearbyWorker()
        {
            m_instance.Navigator.DistanceTolerance = this.minDistanceToLoca;
            if (this.m_instance.Navigator.Goto(nearbyX, nearbyZ, continueToRun, 10000))
            {
                this.commandCompletedCallback(true);
            }
            else
            {
                this.commandCompletedCallback(false);
            }
            m_instance.Navigator.DistanceTolerance = DISTANCE_TOLERANCE;
        }

        #endregion
    

        #endregion

        #region Fighting controls

        private ObjectAggregateRoot foughtObject;


        private void StopFightingWorker()
        {
            fightingStop = true;
            if (this.fightingMemoryThread != null)
                this.fightingMemoryThread.Join();
            fightingStop = false;

            this.m_instance.Navigator.Reset();
            this.m_instance.Navigator.StopRunning();
        }

        public void BeginFighting(ObjectAggregateRoot obj)
        {
            fightingStop = false;
            this.OverrideLastCommand();

            foughtObject = obj;

            this.fightingMemoryThread = new Thread(FightingWorker);
            this.fightingMemoryThread.Start();
        }

        public void EndFighting()
        {
            // Stops the fighting worker
            StopFightingWorker();

            // Unlock target
            while (this.m_instance.Target.IsLocked && this.m_instance.Target.SubServerID == 0)
            {
                this.m_instance.Windower.SendString("/lockon");
                Thread.Sleep(300);
            }

            if (this.m_instance.Target.SubServerID != 0)
            {
                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
            }

            // Disengage target
            while (this.m_instance.Player.Status == Status.Fighting)
            {
                this.m_instance.Windower.SendString("/attack off");
                Thread.Sleep(300);
            }

            isFighting = false;

        }

        private bool canFight()
        {
            if (!fightingStop &&
                foughtObject.pStatus != Status.Dead1 &&
                foughtObject.pStatus != Status.Dead2 &&
                !foughtObject.isClaimedByOthers)
            {
                return true;

            }
            return false;
        }


        private void Disengage()
        {
            lock(this)
            {
                // Engage/disengage target
                while (this.m_instance.Player.Status == Status.Fighting && !stop)
                {
                    this.m_instance.Windower.SendString("/attack off");
                    Thread.Sleep(500);
                }
            }
        }

        private void Engage()
        {
            lock (this)
            {
                // Engage/disengage target
                while (this.m_instance.Player.Status == Status.Fighting != true && !stop && !fightingStop
                       && doTargetting(foughtObject.objectId, foughtObject, true, 4000))
                {

                    this.m_instance.Windower.SendString("/attack on");
                    Thread.Sleep(500);
                }
            }
        }


        private void standBetweenDistanceRange(float min, float max)
        {

            DateTime Start = DateTime.Now;
            // We want to be close enough to attack
            while (this.getDistance(foughtObject.pX, foughtObject.pZ, this.m_instance.Player.PosX, this.m_instance.Player.PosZ) >= max
                && canFight()
                && ((DateTime.Now - Start).TotalMilliseconds) < 7000)
            {
                this.isRunning = true;

                this.LockTarget(foughtObject);
                // Move up
                this.m_instance.Windower.SendKeyPress(KeyCode.NP_Number8);


                this.isRunning = false;
            }

            Start = DateTime.Now;
            // We don't want to stand too close
            while (this.getDistance(foughtObject.pX, foughtObject.pZ, this.m_instance.Player.PosX, this.m_instance.Player.PosZ) <= min
                && canFight()
                && ((DateTime.Now - Start).TotalMilliseconds) < 3000)
            {
                this.isRunning = true;

                this.LockTarget(foughtObject);
                // Move up
                this.m_instance.Windower.SendKeyPress(KeyCode.NP_Number2);

                this.isRunning = false;
            }

            // We haven't reached the sweet spot, stuck ?
            if (this.getDistance(foughtObject.pX, foughtObject.pZ, this.m_instance.Player.PosX, this.m_instance.Player.PosZ) >= max)
            {
                if (this.m_instance.Target.SubServerID != 0)
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                }

                // Unlock target
                if (this.m_instance.Target.IsLocked)
                    this.m_instance.Windower.SendString("/lockon");

                this.MoveToObject(foughtObject, 3);
            }
        }

        private void FightingWorker()
        {

            isFighting = false;
            if (foughtObject != null)
            {
                // Not a mob... abort
                if (foughtObject.pType != NPCType.Mob)
                {
                    this.commandCompletedCallback(false);
                    return;
                }

                // It's a mob, and it's dead, abort
                if (foughtObject.pStatus == Status.Dead2)
                {
                    this.commandCompletedCallback(false);
                    return;
                }

                // Target the right dude
                while (canFight())
                {

                    if (canFight()
                        && !doTargetting(foughtObject.objectId, foughtObject, true, 4000))
                    {
                        this.fightingStop = true;
                        this.commandCompletedCallback(false);
                    }

                    // Third person for battles, way cooler!
                    if (canFight())
                    {
                        SetThirdPerson();
                    }

                                        // Third person for battles, way cooler!
                    if (canFight())
                    {
                        standBetweenDistanceRange(2, 3); // stand between 2 and 3 yalms
                    }

                    // We are close enough, let's engage
                    if (this.getDistance(foughtObject.pX, foughtObject.pZ, this.m_instance.Player.PosX, this.m_instance.Player.PosZ) <= 3
                        && canFight())
                    {
                        this.Engage();
                    }

                    if (canFight())
                    {
                        if (foughtObject.isClaimedByParty)
                        {
                            if (!isFighting) // Return true and delegate battle commanding to strategy
                            {
                                Thread.Sleep(1000); // wait a bit
                                this.commandCompletedCallback(true);
                                isFighting = true;
                            }
                        }
                        else
                        {
                            if (foughtObject.isClaimedByOthers) // Return false and quit the fight
                            {
                                this.commandCompletedCallback(false);
                                fightingStop = true;
                            }
                        }
                    }
                }

                if (this.m_instance.Target.SubServerID != 0)
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                }

                this.UnlockTarget();
                this.Disengage();

                if (!isFighting)
                {
                    this.commandCompletedCallback(false);
                }
                else
                {
                    this.commandCompletedCallback(false, typeof(CmdEndFightingCompletedEvent));
                }

                this.isFighting = false;
                this.isRunning = false;
                this.m_instance.Windower.SendKey(KeyCode.NP_Number8, false);
                this.m_instance.Windower.SendKey(KeyCode.NP_Number2, false);
            }
        }

        #endregion

        #region Target controls

        private int switchToTargetid;
        private bool switchTargetlock;
        private ObjectAggregateRoot targetObject;
        public void switchTarget(int id, ObjectAggregateRoot obj, bool _lock)
        {

            switchTargetlock = _lock;
            switchToTargetid = id;
            targetObject = obj;

            OverrideLastCommand();

            this.memoryThread = new Thread(switchTargetWorker);
            this.memoryThread.Start();

        }

        private void SetFirstPerson()
        {
            if (this.m_instance.Player.ViewMode == ViewMode.ThirdPerson)
            {
                this.m_instance.Windower.SendKeyPress(KeyCode.NP_Number5);
            }
        }

        private void SetThirdPerson()
        {
            if (this.m_instance.Player.ViewMode == ViewMode.FirstPerson)
            {
                this.m_instance.Windower.SendKeyPress(KeyCode.NP_Number5);
            }
        }

        private void UnlockTarget()
        {
            lock (this)
            {
                // Remove subtarget to be able to lock
                if (this.m_instance.Target.SubServerID != 0)
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                }

                while (this.m_instance.Target.IsLocked != false && !stop)
                {
                    this.m_instance.Windower.SendString("/lockon");
                    Thread.Sleep(500);
                }
            }
        }

        private void LockTarget(ObjectAggregateRoot target)
        {
            lock (this)
            {
                // Remove subtarget to be able to lock
                if (this.m_instance.Target.SubServerID != 0)
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                }

                while (this.m_instance.Target.IsLocked != true && !stop && target.objectId == this.m_instance.Target.ServerID)
                {
                    this.m_instance.Windower.SendString("/lockon");
                    Thread.Sleep(500);
                }
            }
        }

        private void removeSubTarget()
        {
            if (this.m_instance.Target.SubServerID != 0 || this.m_instance.Target.Mask != 0)
            {
                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
            }
        }

        private bool doTargetting(int id, ObjectAggregateRoot obj, bool _lock, int timeout, bool isStCast = false)
        {
            CloseMenus();
            DateTime Start = DateTime.Now;

            if (!isStCast)
            {
                // Already locked to another target
                if (this.m_instance.Target.ServerID != id && this.m_instance.Target.SubServerID != id)
                {
                    this.removeSubTarget();
                    this.UnlockTarget();

                    Thread.Sleep(100);


                }
            }

            Thread.Sleep(125);
            this.m_instance.Navigator.FaceHeading(obj.Position);
            Thread.Sleep(125);

            if (this.m_instance.Target.ServerID != id && this.m_instance.Target.SubServerID != id)
            {
                // Set view 
                this.SetFirstPerson();
            }
            Thread.Sleep(125);

            // Targets
            while ((this.m_instance.Target.ServerID != id && this.m_instance.Target.SubServerID != id) && !stop &&
                    ((DateTime.Now - Start).TotalMilliseconds) < timeout 
                    && obj.pStatus != Status.Dead2)
            {
                
                this.m_instance.Windower.SendKeyPress(KeyCode.TabKey);
                Thread.Sleep(175);
            }

            // If there's a subtarget, confirm it, unless 
            if (!isStCast)
            {
                while (this.m_instance.Target.SubServerID != 0 && !stop)
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                    Thread.Sleep(300);
                }
            }


            if (this.m_instance.Target.ServerID == id || (this.m_instance.Target.SubServerID == id && isStCast))
            {
                if (_lock) this.LockTarget(obj);
                return true;
            }


            return false;
        }

        private void switchTargetWorker()
        {
            if (this.doTargetting(switchToTargetid, targetObject, switchTargetlock, 6000))
                this.commandCompletedCallback(true);
            else
                this.commandCompletedCallback(false);
        }



        #endregion

        #region Use Ability controls

        private bool castIsSt;
        public bool abilityUsed { get; set; }
        public bool abilityCastFailed { get; set; }
        private int abilityCastTimeout;
        private void checkAbilityUsed()
        {
            lock (this)
            {
                this.abilityUsed = false; // Initialize to unused ;
                this.abilityCastFailed = false; // Initialize to not failed yet! XD
                DateTime Start = DateTime.Now;

                // Targets
                while (!abilityUsed &&
                       !stop &&
                       !abilityCastFailed &&
                       ((DateTime.Now - Start).TotalMilliseconds) < abilityCastTimeout)
                {

                    if (foughtObject != null &&
                        this.isFighting)
                    {
                        if (foughtObject.pStatus == Status.Dead2)
                            break;
                    }

                    this.m_instance.Windower.SendString(spellcmd);

                    if (castIsSt)
                    {
                        if (this.doTargetting(spellTarget.objectId, spellTarget, false, abilityCastTimeout, true)) // Target STNPC
                        {
                            Thread.Sleep(300);
                            if (this.m_instance.Target.SubServerID != 0)
                                this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                            else
                            {
                                this.m_instance.Windower.SendString(spellcmd);
                                Thread.Sleep(300);
                                this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                            }
                        }
                    }


                    DateTime Wait = DateTime.Now;
                    while (!abilityUsed &&
                           !stop &&
                           !abilityCastFailed &&
                           ((DateTime.Now - Wait).TotalMilliseconds) < 3000)
                    {
                        Thread.Yield();
                    }
                }



                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);

                if (abilityUsed)
                    this.commandCompletedCallback(true); // Success
                else
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                    this.commandCompletedCallback(false); // Failure
                }
            }
        }

        public void useAbility(ObjectAggregateRoot obj, string objectName, AbilityList ability, int timeout)
        {
            this.spellTarget = obj;
            this.castIsSt = false;
            this.abilityCastTimeout = timeout;
            bool wasRunning = isRunning; // If character is running before command override

            OverrideLastCommand();


            if (this.m_instance.Timer.GetAbilityRecast(ability) == 0)
            {
                if (wasRunning)
                    Thread.Sleep(1000); // Need to wait before using ability after stopping the running

                string spellname = GlobalFunctions.SkillRemoveEnumDecorations(ability.ToString());
                // Don't need to target to use ; target is a PC object
                if (obj == null && (objectName == null || objectName == String.Empty))
                    spellcmd = "/ma \"" + spellname + "\" <me>";
                else if (obj == null)
                {
                    spellcmd = "/ja \"" + spellname + "\" " + objectName;
                }
                else if (obj.objectId == this.m_instance.Target.ServerID)
                {
                    spellcmd = "/ja \"" + spellname + "\" <t>";
                }
                else if (obj.objectId == this.m_instance.PartyMember[0].ServerID)
                {
                    spellcmd = "/ja \"" + spellname + "\" <me>";
                }
                else
                { // for NPC/Mob targets
                    this.castIsSt = true;
                    spellcmd = "/ja \"" + spellname + "\" <stnpc>";
                }

                this.memoryThread = new Thread(checkAbilityUsed);
                this.memoryThread.Start();
            }
            else
            {
                this.commandCompletedCallback(false);
            }


        }

        #endregion

        #region Cast Spell controls

        public bool spellCasted { get; set; }
        public bool spellCastedStart { get; set; }
        public bool spellCastFailed { get; set; }
        private int spellCastTimeout;
        private string spellcmd;
        private ObjectAggregateRoot spellTarget;

        private void checkSpellCasted()
        {
            lock (this)
            {
                DateTime Start = DateTime.Now;
                this.spellCasted = false; // Initialize to unused ;
                this.spellCastFailed = false; // Initialize to not failed yet! XD
                this.spellCastedStart = false; // Begun to cast spell

                // Targets
                while (!spellCasted &&
                       !stop &&
                       !spellCastFailed &&
                       ((DateTime.Now - Start).TotalMilliseconds) < spellCastTimeout)
                {

                    if (!spellCastedStart)
                    {

                        if (foughtObject != null &&
                        this.isFighting)
                        {
                            if (foughtObject.pStatus == Status.Dead2)
                                break;
                        }

                        this.m_instance.Windower.SendString(spellcmd);

                        if (castIsSt)
                        {
                            Thread.Sleep(300);
                            if (this.doTargetting(spellTarget.objectId, spellTarget, false, spellCastTimeout, true))
                            {
                                if (this.m_instance.Target.SubServerID != 0)
                                    this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                                else
                                {
                                    this.m_instance.Windower.SendString(spellcmd);
                                    Thread.Sleep(300);
                                    this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                                }
                            }// Target STNPC 
                        }
                    }


                    DateTime Wait = DateTime.Now;
                    while (!spellCasted &&
                           !stop &&
                           !spellCastFailed &&
                           !spellCastedStart &&
                           ((DateTime.Now - Wait).TotalMilliseconds) < 3000)
                    {
                        Thread.Yield();
                    }
                }


                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);

                if (spellCasted)
                    this.commandCompletedCallback(true); // Success
                else
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                    this.commandCompletedCallback(false); // Failure
                }
            }
        }

        public void castSpell(ObjectAggregateRoot obj, string objectName, SpellList spell, int timeout)
        {
            this.spellTarget = obj;
            this.spellcmd = null;
            this.castIsSt = false;
            this.spellCastTimeout = timeout;
            bool wasRunning = isRunning; // If character is running before command override

            OverrideLastCommand();

            if (this.m_instance.Timer.GetSpellRecast(spell) == 0)
            {
                if (wasRunning)
                    Thread.Sleep(1000); // Need to wait before using ability after stopping the running


                string spellname = GlobalFunctions.SkillRemoveEnumDecorations(spell.ToString());
                // Don't need to target to use ; target is a PC object
                if (obj == null && (objectName == null || objectName == String.Empty))
                    spellcmd = "/ma \"" + spellname + "\" <me>";
                else if (obj == null)
                    spellcmd = "/ma \"" + spellname + "\" " + objectName;
                else if (obj.objectId == this.m_instance.Target.ServerID)
                    spellcmd = "/ma \"" + spellname + "\" <t>";
                else if (obj.objectId == this.m_instance.PartyMember[0].ServerID)
                    spellcmd = "/ma \"" + spellname + "\" <me>";
                else
                { // for NPC/Mob targets
                    this.castIsSt = true;
                    spellcmd = "/ma \"" + spellname + "\" <stnpc>";
                }

                this.memoryThread = new Thread(checkSpellCasted);
                this.memoryThread.Start();

            }
            else
            {
                this.commandCompletedCallback(false);
            }

        }

        #endregion

        #region Ready Weaponskill controls

        public bool weaponskillReadied { get; set; }
        public bool weaponskillFailed { get; set; }
        private int weaponskillTimeout;

        private void checkWeaponskillReady()
        {
            lock (this)
            {
                DateTime Start = DateTime.Now;
                this.weaponskillReadied = false; // Initialize to unused ;
                this.weaponskillFailed = false; // Initialize to not failed yet! XD

                // Targets
                while (!weaponskillReadied &&
                       !stop &&
                       !weaponskillFailed &&
                       ((DateTime.Now - Start).TotalMilliseconds) < weaponskillTimeout)
                {

                    if (foughtObject != null &&
                        this.isFighting)
                    {
                        if (foughtObject.pStatus == Status.Dead2)
                            break;
                    }

                    this.m_instance.Windower.SendString(spellcmd);

                    if (castIsSt)
                    {
                        Thread.Sleep(300);
                        if (this.doTargetting(spellTarget.objectId, spellTarget, false, weaponskillTimeout, true))// Target STNPC
                        {
                            if (this.m_instance.Target.SubServerID != 0)
                                this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                            else
                            {
                                this.m_instance.Windower.SendString(spellcmd);
                                Thread.Sleep(300);
                                this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                            }
                        }
                    }

                    DateTime Wait = DateTime.Now;
                    while (!weaponskillReadied &&
                           !stop &&
                           !weaponskillFailed &&
                           ((DateTime.Now - Wait).TotalMilliseconds) < 3000)
                    {
                        Thread.Yield();
                    }
                }


                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);

                if (weaponskillReadied)
                    this.commandCompletedCallback(true); // Success
                else
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                    this.commandCompletedCallback(false); // Failure
                }
            }
        }

        public void readyWeaponskill(ObjectAggregateRoot obj, string objectName, WeaponSkillList ws, int timeout)
        {
            this.spellTarget = obj;
            this.castIsSt = false;
            this.weaponskillTimeout = timeout;
            bool wasRunning = isRunning; // If character is running before command override

            OverrideLastCommand();

            if (this.m_instance.Player.TPCurrent > 1000)
            {
                if (wasRunning)
                    Thread.Sleep(1000); // Need to wait before using ability after stopping the running

                string spellname = GlobalFunctions.SkillRemoveEnumDecorations(ws.ToString());
                // Don't need to target to use ; target is a PC object
                if (obj == null && (objectName == null || objectName == String.Empty))
                    spellcmd = "/ma \"" + spellname + "\" <me>";
                else if (obj == null)
                {
                    spellcmd = "/ws \"" + spellname + "\" " + objectName;
                }
                else if (obj.objectId == this.m_instance.Target.ServerID)
                {
                    spellcmd = "/ws \"" + spellname + "\" <t>";
                }
                else if (obj.objectId == this.m_instance.PartyMember[0].ServerID)
                {
                    spellcmd = "/ws \"" + spellname + "\" <me>";
                }
                else
                { // for NPC/Mob targets
                    this.castIsSt = true;
                    spellcmd = "/ws \"" + spellname + "\" <stnpc>";
                }

                this.memoryThread = new Thread(checkWeaponskillReady);
                this.memoryThread.Start();
            }
            else
            {
                this.commandCompletedCallback(false);
            }

        }

        #endregion

        #region Use Rangedattack controls

        public bool RaUsed { get; set; }
        public bool RaCastFailed { get; set; }
        private int RaCastTimeout;
        private void checkRaUsed()
        {
            lock (this)
            {
                this.RaUsed = false; // Initialize to unused ;
                this.RaCastFailed = false; // Initialize to not failed yet! XD
                DateTime Start = DateTime.Now;

                // Targets
                while (!RaUsed &&
                       !stop &&
                       !RaCastFailed &&
                       ((DateTime.Now - Start).TotalMilliseconds) < RaCastTimeout)
                {

                    if (foughtObject != null &&
                        this.isFighting)
                    {
                        if (foughtObject.pStatus == Status.Dead2)
                            break;
                    }

                    this.m_instance.Navigator.FaceHeading(this.spellTarget.Position); // face for ranged attack
                    this.m_instance.Windower.SendString(spellcmd);
                    if (castIsSt)
                    {
                        Thread.Sleep(125);
                        if (this.doTargetting(spellTarget.objectId, spellTarget, false, RaCastTimeout, true)) // Target STNPC
                        {
                            if (this.m_instance.Target.SubServerID != 0)
                                this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                            else
                            {
                                this.m_instance.Windower.SendString(spellcmd);
                                Thread.Sleep(300);
                                this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                            }
                        }
                    }

                    DateTime Wait = DateTime.Now;
                    while (!RaUsed &&
                           !stop &&
                           !RaCastFailed &&
                           ((DateTime.Now - Wait).TotalMilliseconds) < 3000)
                    {
                        Thread.Yield();
                    }
                }


                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);

                if (RaUsed)
                    this.commandCompletedCallback(true); // Success
                else
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                    this.commandCompletedCallback(false); // Failure
                }
            }
        }

        public void useRangedAttack(ObjectAggregateRoot obj, int timeout)
        {
            this.spellTarget = obj;
            this.castIsSt = false;
            this.RaCastTimeout = timeout;
            bool wasRunning = isRunning; // If character is running before command override

           OverrideLastCommand();

            if (wasRunning)
                Thread.Sleep(1000); // Need to wait before using ability after stopping the running

            this.m_instance.Navigator.FaceHeading(obj.Position); // face for ranged attack

            if (obj.objectId == this.m_instance.Target.ServerID)
            {
                spellcmd = "/ra <t>";
            }
            else
            { // for NPC/Mob targets
                this.castIsSt = true;
                spellcmd = "/ra <stnpc>";
            }

            this.memoryThread = new Thread(checkRaUsed);
            this.memoryThread.Start();


        }

        #endregion

        #region Interact with NPC controls

        private void hitEnterUntilDialogOrEnd()
        {
            while (this.m_instance.Menu.DialogID == 0 && this.m_instance.Menu.IsOpen)
            {
                this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(2000);
            }
        }

        private void CloseMenus(int sleep = 2000)
        {
            lock (this)
            {
                while (this.m_instance.Menu.IsOpen && !stop && this.m_instance.Player.Status != Status.Fighting)
                {
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                    Thread.Sleep(sleep);
                }
            }
        }

        private void CloseMenusButDialogs()
        {
            while (this.m_instance.Menu.DialogID == 0 &&
                   this.m_instance.Menu.IsOpen && !stop)
            {
                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                Thread.Sleep(2000);
            }
        }

        private bool SelectDialogOption(int index)
        {
            int i = 0;
            while (this.m_instance.Menu.DialogOptionIndex != index
                   && i < 30)
            {
                if (index > this.m_instance.Menu.DialogOptionIndex)
                    this.m_instance.Windower.SendKeyPress(KeyCode.DownArrow);
                else
                    this.m_instance.Windower.SendKeyPress(KeyCode.UpArrow);

                i++;
                Thread.Sleep(125);
            }

            if (this.m_instance.Menu.DialogOptionIndex == index)
                return true;

            return false;
        }

        public void doInteractionWorker()
        {
            lock (this)
            {
                CloseMenusButDialogs();
                Thread.Sleep(800);
                if (this.m_instance.Menu.DialogID != 0 || this.doTargetting(InteractionObject.objectId, InteractionObject, true, InteractionTimeout, false))
                {

                    if (this.m_instance.Menu.DialogID == 0)
                    {
                        this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(2000);
                    }

                    if (this.m_instance.Menu.DialogID != 0
                        || this.m_instance.Menu.IsOpen)
                    {
                        this.hitEnterUntilDialogOrEnd();

                        // Have to choose an option
                        if (InteractionDialogTextToChoose != null && InteractionDialogTextToChoose != String.Empty)
                        {
                            if (this.m_instance.Menu.DialogID != 0 && this.m_instance.Menu.DialogOptionCount > 0)
                            {
                                FFACE.DialogText text = this.m_instance.Menu.DialogText;
                                int targetDialogOptNum = 0;
                                bool targetDialogOptFound = false;

                                for (int i = 0; i < text.Options.Count(); i++)
                                {
                                    if (InteractionDialogTextToChoose == text.Options[i])
                                    {
                                        targetDialogOptFound = true;
                                        targetDialogOptNum = i;
                                    }
                                }

                                if (targetDialogOptFound)
                                {

                                    this.SelectDialogOption(targetDialogOptNum);

                                    // Option chosen
                                    this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);

                                    this.hitEnterUntilDialogOrEnd();

                                    commandCompletedCallback(true);
                                }
                                else
                                    commandCompletedCallback(false); // Option we're looking for isn't among the choices
                            }
                            else
                                commandCompletedCallback(false); // the npc didn't have options to offer as previously expected
                        }
                        else
                        {
                            if (this.m_instance.Menu.DialogID != 0 && this.m_instance.Menu.DialogOptionCount > 0)
                            {
                                // Options are present, abort dialog
                                this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                                commandCompletedCallback(false); // didn't command to choose an option
                            }
                            else
                                commandCompletedCallback(true); // didn't command to choose an option
                        }
                    }
                    else
                        commandCompletedCallback(false);
                }

                this.InteractionThread = null;
            }
        }

        private Thread InteractionThread = null;
        private ObjectAggregateRoot InteractionObject;
        private string InteractionDialogTextToChoose;
        private int InteractionTimeout;

        public void interactWithObject(ObjectAggregateRoot obj, string DialogTextToChoose, int timeout)
        {
            this.StopMove();
            InteractionObject = obj;
            InteractionDialogTextToChoose = DialogTextToChoose;
            InteractionTimeout = timeout;

            if (InteractionThread == null)
            {
                InteractionThread = new Thread(doInteractionWorker);
                InteractionThread.Start();
            }
        }


        #endregion

        #region Trade controls

        private void TradeWorker()
        {

            FFACE.NPCTRADEINFO tradeinfo = new FFACE.NPCTRADEINFO();
            if (TradeItems != null)
                tradeinfo.items = TradeItems.ToArray();
            else if (TradeGils != -1)
                tradeinfo.Gil = (uint)TradeGils;



            DateTime Wait = DateTime.Now;

            if (this.doTargetting(TradeTarget.objectId, TradeTarget, true, 5000))
            {
                this.m_instance.Menu.OpenTradeMenu(this.m_instance.Target.ID);
                Thread.Sleep(100);
                while (this.m_instance.Menu.lastTradeMenuStatus == FFACE.MenuTools.ThreadStatus.Running &&
                       ((DateTime.Now - Wait).TotalMilliseconds) < 30000)
                {
                    Thread.Yield();
                }
                if (this.m_instance.Menu.lastTradeMenuStatus == FFACE.MenuTools.ThreadStatus.Succeeded)
                {
                    this.m_instance.Menu.SetNPCTradeInformation(tradeinfo);

                    Thread.Sleep(125);
                    this.m_instance.Windower.SendKeyPress(KeyCode.EscapeKey);
                    Thread.Sleep(125);
                    this.m_instance.Windower.SendKeyPress(KeyCode.UpArrow);
                    Thread.Sleep(125);
                    this.m_instance.Windower.SendKeyPress(KeyCode.EnterKey);

                    commandCompletedCallback(true);
                }
                else
                    commandCompletedCallback(false);
            }
            else
                commandCompletedCallback(false);
        }

        private List<FFACE.TRADEITEM> TradeItems;
        private ObjectAggregateRoot TradeTarget;
        private int TradeGils;
        public void TradeItemToTarget(ObjectAggregateRoot obj, List<FFACE.TRADEITEM> items, int gil = -1)
        {
            TradeItems = items;
            TradeTarget = obj;
            TradeGils = gil;

            new Thread(TradeWorker).Start();
        }

        #endregion
    }
}
