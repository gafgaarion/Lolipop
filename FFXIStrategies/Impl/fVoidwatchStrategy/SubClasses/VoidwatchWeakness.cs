using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;
using System.Collections.Concurrent;

namespace FFXIStrategies.Impl.fVoidwatchStrategy.Subclasses
{
    public class VoidwatchWeakness
    {
        public bool procced;
        public VoidwatchProcType ProcType;
        public Elements Elemental;
        public ConcurrentDictionary<AbilityList, bool> JobSkill;
        public ConcurrentDictionary<SpellList, bool> MagicSkill;
        public ConcurrentDictionary<WeaponSkillList, bool> WeaponSkill;
        public VoidwatchProcStrength strength;

        public VoidwatchWeakness(VoidwatchProcStrength _strength, VoidwatchProcType _type, Elements _element)
        {
            ProcType = _type;
            Elemental = _element;
            strength = _strength;
            JobSkill = new ConcurrentDictionary<AbilityList, bool>();
            MagicSkill = new ConcurrentDictionary<SpellList, bool>();
            WeaponSkill = new ConcurrentDictionary<WeaponSkillList, bool>();
            procced = false;

        }

        public bool haveToTry(SpellList _spell)
        {
            if (MagicSkill.ContainsKey(_spell))
            {
                return MagicSkill[_spell];
            }
            else
                return false;
        }
        public bool haveToTry(AbilityList _ability)
        {
            if (JobSkill.ContainsKey(_ability))
            {
                return JobSkill[_ability];
            }
            else
                return false;
        }

        public bool haveToTry(WeaponSkillList _ws)
        {
            if (WeaponSkill.ContainsKey(_ws))
            {
                return WeaponSkill[_ws];
            }
            else
                return false;
        }

    }
}