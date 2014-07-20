using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
namespace Commons
{
    public class GlobalFunctions
    {
        private static CompareLogic compareLogic = null;

        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        public static bool ObjectsAreEqual<T>(T obj1, T obj2)
        {
            if (compareLogic == null)
                compareLogic = new CompareLogic();

            ComparisonResult result = compareLogic.Compare(obj1, obj2);
            
            return result.AreEqual;
        }

        public static string SkillRemoveEnumDecorations(string skill)
        {
            skill = skill.Replace("Tachi_", "Tachi: ");
            skill = skill.Replace("Blade_", "Blade: ");
            skill = skill.Replace("Absorb_", "Absorb-");
            skill = skill.Replace("_", " ");
            return skill;
        }

        public static string SkillAddEnumDecorations(string skill)
        {
            skill = skill.Replace("Tachi: ", "Tachi_");
            skill = skill.Replace("Blade: ", "Blade_");
            skill = skill.Replace("Absorb-", "Absorb_");
            skill = skill.Replace(" ", "_");
            return skill;
        }

    }
}
