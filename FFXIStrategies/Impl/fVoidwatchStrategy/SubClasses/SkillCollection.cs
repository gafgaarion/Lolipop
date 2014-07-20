using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace FFXIStrategies.Impl.fVoidwatchStrategy.Subclasses
{
    public class SkillCollection
    {
        public VoidwatchProcType ProcType { get; private set; }
        public Elements Elemental { get; private set; }
        public List<AbilityList> JobSkill { get; private set; }
        public List<SpellList> MagicSkill { get; private set; }
        public List<WeaponSkillList> WeaponSkill { get; private set; }
        public int RequiredWeaponItemId { get; private set; }
        public float MaxRange { get; private set; }

        public SkillCollection(VoidwatchProcType _procType, Elements _elemental, float _maxRange = 4)
        {
            this.MaxRange = _maxRange;
            this.Elemental = _elemental;
            this.ProcType = _procType;
            this.JobSkill = new List<AbilityList>();
            this.MagicSkill = new List<SpellList>();
            this.WeaponSkill = new List<WeaponSkillList>();
        }

        public void AddSkill(AbilityList Skill) { JobSkill.Add(Skill); }
        public void AddSkill(SpellList Skill) { MagicSkill.Add(Skill); this.MaxRange = 20;  }
        public void AddSkill(WeaponSkillList Skill) { WeaponSkill.Add(Skill); }
    }
}
