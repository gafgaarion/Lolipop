using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace FFACETools
{
    public class TargetStruct
    {
        public TargetStruct(FFACE.TargetTools _target)
        {
            this.ID = _target.ID;
            this.SubID = _target.SubID;
            this.ServerID = _target.ServerID;
            this.SubServerID = _target.SubServerID;
            this.Mask = _target.Mask;
            this.SubMask = _target.SubMask;
            this.IsLocked = _target.IsLocked;
            this.IsSub = _target.IsSub;
            this.HPPCurrent = _target.HPPCurrent;
            this.Name = _target.Name;
            this.PosX = _target.PosX;
            this.PosY = _target.PosY;
            this.PosZ = _target.PosZ;
            this.PosH = _target.PosH;
            this.Type = _target.Type;
        }

        public TargetStruct()
        {}

        public int ID {get; set; }
        public int SubID {get; set; }
        public int ServerID {get; set; }
        public int SubServerID {get; set; }
        public ushort Mask {get; set; }
        public ushort SubMask {get; set; }
        public bool IsLocked {get; set; }
        public bool IsSub {get; set; }
        public short HPPCurrent {get; set; }

        public string Name {get; set; }
        public float PosX {get; set; }

        public float PosY {get; set; }
        public float PosZ {get; set; }
        public float PosH {get; set; }
        public NPCType Type {get; set; }
    }
}
