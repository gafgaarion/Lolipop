using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;
using Commons;
using FFXIWorldKnowledge;
using FFXICommands;

namespace FFXIWorldKnowledge.Shards
{
    public class CharacterShard
    {
        public int characterName { get; set; }
        public string name { get; set; }
        public Position position { get; set; }
        public int hpp { get; set; }
        public int hp { get; set; }
        public int mp { get; set; }
        public int tp { get; set; }

        public float facing { get; set; }

        public int uniqueGameID { get; set; }
        public float percentCast { get; set; }
        public LoginStatus loginStatus { get; set; }
        public ViewMode viewMode { get; set; }
        public Status status { get; set; }
        public StatusEffect[] statusEffects { get; set; }

        public TargetStruct target { get; set; }
        public Zone mapId { get; set; }

        // Bot status
        public int currentAction { get; set; }
        public MachineState MS_movement { get; set; }
        public MachineState MS_action { get; set; }
        public MachineState MS_tabbing { get; set; }
    }
}
