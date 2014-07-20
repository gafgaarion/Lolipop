using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace FFACETools
{
    public static class Voidwatch
    {
        public class VoidwatchSkillCollectionType
        {
            public VoidwatchProcType type { get; private set; }
            public Elements element { get; private set; }
            public VoidwatchSkillCollectionType(VoidwatchProcType _type, Elements _element)
            {
                element = _element;
                type = _type;
            }
        }

        private static Dictionary<VoidwatchSkillCollectionType, object> Weaknesses = null;

        public static VoidwatchSkillCollectionType getSkillType(object Skill)
        {

            if (Weaknesses == null)
            {
                Weaknesses = new Dictionary<VoidwatchSkillCollectionType, object>();

                foreach (VoidwatchProcType type in Enum.GetValues(typeof(VoidwatchProcType)))
                {
                    foreach (Elements element in Enum.GetValues(typeof(Elements)))
                    {
                        if (type != VoidwatchProcType.BlackMagic &&
                            type != VoidwatchProcType.WhiteMagic &&
                            type != VoidwatchProcType.BlueMagic &&
                            type != VoidwatchProcType.Ninjutsu &&
                            type != VoidwatchProcType.Song)
                        {

                            Weaknesses.Add(new VoidwatchSkillCollectionType(type, Elements.Neutral), getWeaknessSkills(type, element));
                        }
                        else
                            Weaknesses.Add(new VoidwatchSkillCollectionType(type, element), getWeaknessSkills(type, element));
                    }
                }
            }

            lock (Weaknesses)
            {
                if (Skill is WeaponSkillList)
                {
                    foreach (KeyValuePair<VoidwatchSkillCollectionType, object> skill in Weaknesses)
                    {
                        if (skill.Value != null && skill.Value is List<WeaponSkillList>)
                        {
                            foreach (WeaponSkillList ws in (List<WeaponSkillList>)skill.Value)
                            {
                                if (ws == (WeaponSkillList)Skill)
                                    return skill.Key;
                            }
                        }
                    }
                }

                if (Skill is AbilityList)
                {
                    foreach (KeyValuePair<VoidwatchSkillCollectionType, object> skill in Weaknesses)
                    {
                        if (skill.Value != null && skill.Value is List<AbilityList>)
                        {
                            foreach (AbilityList ws in (List<AbilityList>)skill.Value)
                            {
                                if (ws == (AbilityList)Skill)
                                    return skill.Key;
                            }
                        }
                    }
                }

                if (Skill is SpellList)
                {
                    foreach (KeyValuePair<VoidwatchSkillCollectionType, object> skill in Weaknesses)
                    {
                        if (skill.Value != null && skill.Value is List<SpellList>)
                        {
                            foreach (SpellList ws in (List<SpellList>)skill.Value)
                            {
                                if (ws == (SpellList)Skill)
                                    return skill.Key;
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns a list of skills corresponding to the weakness
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_element"></param>
        /// <returns></returns>
        public static object getWeaknessSkills(VoidwatchProcType _type, Elements _element)
        {
            List<WeaponSkillList> WeaponSkill = null;
            List<AbilityList> JobSkill = null;
            List<SpellList> MagicSkill = null;

            #region Weapon skills

            #region Archery
            if (_type == VoidwatchProcType.Archery)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Flaming_Arrow);
                WeaponSkill.Add(WeaponSkillList.Piercing_Arrow);
                WeaponSkill.Add(WeaponSkillList.Dulling_Arrow);
                WeaponSkill.Add(WeaponSkillList.Sidewinder);
                WeaponSkill.Add(WeaponSkillList.Blast_Arrow);
                WeaponSkill.Add(WeaponSkillList.Arching_Arrow);
                WeaponSkill.Add(WeaponSkillList.Empyreal_Arrow);
                WeaponSkill.Add(WeaponSkillList.Refulgent_Arrow);
            }
            #endregion

            #region Axe
            if (_type == VoidwatchProcType.Axe)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Raging_Axe);
                WeaponSkill.Add(WeaponSkillList.Smash_Axe);
                WeaponSkill.Add(WeaponSkillList.Gale_Axe);
                WeaponSkill.Add(WeaponSkillList.Avalanche_Axe);
                WeaponSkill.Add(WeaponSkillList.Spinning_Axe);
                WeaponSkill.Add(WeaponSkillList.Rampage);
                WeaponSkill.Add(WeaponSkillList.Calamity);
                WeaponSkill.Add(WeaponSkillList.Mistral_Axe);
                WeaponSkill.Add(WeaponSkillList.Decimation);
                WeaponSkill.Add(WeaponSkillList.Bora_Axe);
            }
            #endregion

            #region Club
            if (_type == VoidwatchProcType.Club)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Shining_Strike);
                WeaponSkill.Add(WeaponSkillList.Seraph_Strike);
                WeaponSkill.Add(WeaponSkillList.Brainshaker);
                WeaponSkill.Add(WeaponSkillList.Skullbreaker);
                WeaponSkill.Add(WeaponSkillList.True_Strike);
                WeaponSkill.Add(WeaponSkillList.Judgment);
                WeaponSkill.Add(WeaponSkillList.Hexa_Strike);
                WeaponSkill.Add(WeaponSkillList.Black_Halo);
                WeaponSkill.Add(WeaponSkillList.Flash_Nova);
            }
            #endregion

            #region Dagger
            if (_type == VoidwatchProcType.Dagger)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Wasp_Sting);
                WeaponSkill.Add(WeaponSkillList.Gust_Slash);
                WeaponSkill.Add(WeaponSkillList.Shadowstitch);
                WeaponSkill.Add(WeaponSkillList.Viper_Bite);
                WeaponSkill.Add(WeaponSkillList.Cyclone);
                WeaponSkill.Add(WeaponSkillList.Energy_Steal);
                WeaponSkill.Add(WeaponSkillList.Energy_Drain);
                WeaponSkill.Add(WeaponSkillList.Dancing_Edge);
                WeaponSkill.Add(WeaponSkillList.Shark_Bite);
                WeaponSkill.Add(WeaponSkillList.Evisceration);
                WeaponSkill.Add(WeaponSkillList.Aeolian_Edge);
            }
            #endregion

            #region Great Axe
            if (_type == VoidwatchProcType.GreatAxe)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Shield_Break);
                WeaponSkill.Add(WeaponSkillList.Iron_Tempest);
                WeaponSkill.Add(WeaponSkillList.Sturmwind);
                WeaponSkill.Add(WeaponSkillList.Armor_Break);
                WeaponSkill.Add(WeaponSkillList.Keen_Edge);
                WeaponSkill.Add(WeaponSkillList.Weapon_Break);
                WeaponSkill.Add(WeaponSkillList.Raging_Rush);
                WeaponSkill.Add(WeaponSkillList.Full_Break);
                WeaponSkill.Add(WeaponSkillList.Steel_Cyclone);
                WeaponSkill.Add(WeaponSkillList.Fell_Cleave);
            }
            #endregion

            #region Great Katana
            if (_type == VoidwatchProcType.GreatKatana)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Tachi_Enpi);
                WeaponSkill.Add(WeaponSkillList.Tachi_Hobaku);
                WeaponSkill.Add(WeaponSkillList.Tachi_Goten);
                WeaponSkill.Add(WeaponSkillList.Tachi_Kagero);
                WeaponSkill.Add(WeaponSkillList.Tachi_Jinpu);
                WeaponSkill.Add(WeaponSkillList.Tachi_Koki);
                WeaponSkill.Add(WeaponSkillList.Tachi_Yukikaze);
                WeaponSkill.Add(WeaponSkillList.Tachi_Gekko);
                WeaponSkill.Add(WeaponSkillList.Tachi_Kasha);
                WeaponSkill.Add(WeaponSkillList.Tachi_Ageha);
            }
            #endregion

            #region Great Sword
            if (_type == VoidwatchProcType.GreatSword)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Hard_Slash);
                WeaponSkill.Add(WeaponSkillList.Power_Slash);
                WeaponSkill.Add(WeaponSkillList.Frostbite);
                WeaponSkill.Add(WeaponSkillList.Freezebite);
                WeaponSkill.Add(WeaponSkillList.Shockwave);
                WeaponSkill.Add(WeaponSkillList.Crescent_Moon);
                WeaponSkill.Add(WeaponSkillList.Sickle_Moon);
                WeaponSkill.Add(WeaponSkillList.Spinning_Slash);
                WeaponSkill.Add(WeaponSkillList.Ground_Strike);
                WeaponSkill.Add(WeaponSkillList.Herculean_Slash);
            }
            #endregion

            #region Hand to Hand
            if (_type == VoidwatchProcType.Hand2Hand)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Combo);
                WeaponSkill.Add(WeaponSkillList.Shoulder_Tackle);
                WeaponSkill.Add(WeaponSkillList.One_Inch_Punch);
                WeaponSkill.Add(WeaponSkillList.Backhand_Blow);
                WeaponSkill.Add(WeaponSkillList.Raging_Fists);
                WeaponSkill.Add(WeaponSkillList.Spinning_Attack);
                WeaponSkill.Add(WeaponSkillList.Howling_Fist);
                WeaponSkill.Add(WeaponSkillList.Dragon_Kick);
                WeaponSkill.Add(WeaponSkillList.Asuran_Fists);
                WeaponSkill.Add(WeaponSkillList.Tornado_Kick);
            }
            #endregion

            #region Katana
            if (_type == VoidwatchProcType.Katana)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Blade_Rin);
                WeaponSkill.Add(WeaponSkillList.Blade_Retsu);
                WeaponSkill.Add(WeaponSkillList.Blade_Teki);
                WeaponSkill.Add(WeaponSkillList.Blade_To);
                WeaponSkill.Add(WeaponSkillList.Blade_Chi);
                WeaponSkill.Add(WeaponSkillList.Blade_Ei);
                WeaponSkill.Add(WeaponSkillList.Blade_Jin);
                WeaponSkill.Add(WeaponSkillList.Blade_Ten);
                WeaponSkill.Add(WeaponSkillList.Blade_Ku);
                WeaponSkill.Add(WeaponSkillList.Blade_Yu);
            }
            #endregion

            #region Marksmanship
            if (_type == VoidwatchProcType.Marksmanship)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Hot_Shot);
                WeaponSkill.Add(WeaponSkillList.Split_Shot);
                WeaponSkill.Add(WeaponSkillList.Sniper_Shot);
                WeaponSkill.Add(WeaponSkillList.Slug_Shot);
                WeaponSkill.Add(WeaponSkillList.Blast_Shot);
                WeaponSkill.Add(WeaponSkillList.Heavy_Shot);
                WeaponSkill.Add(WeaponSkillList.Detonator);
                WeaponSkill.Add(WeaponSkillList.Numbing_Shot);
            }
            #endregion

            #region Polearm
            if (_type == VoidwatchProcType.Polearm)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Double_Thrust);
                WeaponSkill.Add(WeaponSkillList.Thunder_Thrust);
                WeaponSkill.Add(WeaponSkillList.Raiden_Thrust);
                WeaponSkill.Add(WeaponSkillList.Leg_Sweep);
                WeaponSkill.Add(WeaponSkillList.Penta_Thrust);
                WeaponSkill.Add(WeaponSkillList.Vorpal_Thrust);
                WeaponSkill.Add(WeaponSkillList.Skewer);
                WeaponSkill.Add(WeaponSkillList.Wheeling_Thrust);
                WeaponSkill.Add(WeaponSkillList.Impulse_Drive);
                WeaponSkill.Add(WeaponSkillList.Sonic_Thrust);
            }
            #endregion

            #region Scythe
            if (_type == VoidwatchProcType.Scythe)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Slice);
                WeaponSkill.Add(WeaponSkillList.Dark_Harvest);
                WeaponSkill.Add(WeaponSkillList.Shadow_of_Death);
                WeaponSkill.Add(WeaponSkillList.Nightmare_Scythe);
                WeaponSkill.Add(WeaponSkillList.Spinning_Scythe);
                WeaponSkill.Add(WeaponSkillList.Vorpal_Scythe);
                WeaponSkill.Add(WeaponSkillList.Guillotine);
                WeaponSkill.Add(WeaponSkillList.Cross_Reaper);
                WeaponSkill.Add(WeaponSkillList.Spiral_Hell);
                WeaponSkill.Add(WeaponSkillList.Infernal_Scythe);
            }
            #endregion

            #region Staff
            if (_type == VoidwatchProcType.Staff)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Heavy_Swing);
                WeaponSkill.Add(WeaponSkillList.Rock_Crusher);
                WeaponSkill.Add(WeaponSkillList.Earth_Crusher);
                WeaponSkill.Add(WeaponSkillList.Starburst);
                WeaponSkill.Add(WeaponSkillList.Sunburst);
                WeaponSkill.Add(WeaponSkillList.Shell_Crusher);
                WeaponSkill.Add(WeaponSkillList.Full_Swing);
                WeaponSkill.Add(WeaponSkillList.Spirit_Taker);
                WeaponSkill.Add(WeaponSkillList.Retribution);
                WeaponSkill.Add(WeaponSkillList.Cataclysm);
            }
            #endregion

            #region Sword
            if (_type == VoidwatchProcType.Sword)
            {
                WeaponSkill = new List<WeaponSkillList>();

                WeaponSkill.Add(WeaponSkillList.Fast_Blade);
                WeaponSkill.Add(WeaponSkillList.Burning_Blade);
                WeaponSkill.Add(WeaponSkillList.Red_Lotus_Blade);
                WeaponSkill.Add(WeaponSkillList.Flat_Blade);
                WeaponSkill.Add(WeaponSkillList.Shining_Blade);
                WeaponSkill.Add(WeaponSkillList.Seraph_Blade);
                WeaponSkill.Add(WeaponSkillList.Circle_Blade);
                WeaponSkill.Add(WeaponSkillList.Spirits_Within);
                WeaponSkill.Add(WeaponSkillList.Vorpal_Blade);
                WeaponSkill.Add(WeaponSkillList.Swift_Blade);
                WeaponSkill.Add(WeaponSkillList.Savage_Blade);
                WeaponSkill.Add(WeaponSkillList.Sanguine_Blade);
            }
            #endregion

            #endregion

            #region Job abilities

            #region Black Mage
            if (_type == VoidwatchProcType.BlackMageJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Enmity_Douse);
            }
            #endregion

            #region Corsair
            if (_type == VoidwatchProcType.CorsairJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Fire_Shot);
                JobSkill.Add(AbilityList.Ice_Shot);
                JobSkill.Add(AbilityList.Wind_Shot);
                JobSkill.Add(AbilityList.Earth_Shot);
                JobSkill.Add(AbilityList.Thunder_Shot);
                JobSkill.Add(AbilityList.Water_Shot);
            }
            #endregion

            #region Dancer
            if (_type == VoidwatchProcType.DancerJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Quickstep);
                JobSkill.Add(AbilityList.Stutter_Step);
                JobSkill.Add(AbilityList.Box_Step);
                JobSkill.Add(AbilityList.Feather_Step);
                JobSkill.Add(AbilityList.Violent_Flourish);
                JobSkill.Add(AbilityList.Animated_Flourish);
                JobSkill.Add(AbilityList.Desperate_Flourish);
                JobSkill.Add(AbilityList.Wild_Flourish);
            }
            #endregion

            #region Dark Knight
            if (_type == VoidwatchProcType.DarkKnightJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Weapon_Bash);
            }
            #endregion

            #region Dragoon
            if (_type == VoidwatchProcType.DragoonJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Jump);
                JobSkill.Add(AbilityList.High_Jump);
            }
            #endregion

            #region Monk
            if (_type == VoidwatchProcType.MonkJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Chi_Blast);
            }
            #endregion

            #region Paladin
            if (_type == VoidwatchProcType.PaladinJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Shield_Bash);
            }
            #endregion

            #region Ranger
            if (_type == VoidwatchProcType.RangerJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Shadowbind);
            }
            #endregion

            #region Scholar
            if (_type == VoidwatchProcType.ScholarJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Libra);
            }
            #endregion

            #region Thief
            if (_type == VoidwatchProcType.ThiefJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Bully);
            }
            #endregion

            #region Warrior
            if (_type == VoidwatchProcType.WarriorJA)
            {
                JobSkill = new List<AbilityList>();

                JobSkill.Add(AbilityList.Provoke);
            }

            #endregion
            #endregion

            #region Magic

            #region Blue Magic
            if (_type == VoidwatchProcType.BlueMagic)
            {
                MagicSkill = new List<SpellList>();

                #region Fire
                if (_element == Elements.Fire)
                {
                    MagicSkill.Add(SpellList.Sandspin);
                    MagicSkill.Add(SpellList.Magnetite_Cloud);
                    MagicSkill.Add(SpellList.Cimicine_Discharge);
                    MagicSkill.Add(SpellList.Bad_Breath);
                }
                #endregion

                #region Earth
                if (_element == Elements.Earth)
                {
                    MagicSkill.Add(SpellList.Firespit);
                    MagicSkill.Add(SpellList.Heat_Breath);
                    MagicSkill.Add(SpellList.Thermal_Pulse);
                    MagicSkill.Add(SpellList.Blastbomb);
                }
                #endregion

                #region Water
                if (_element == Elements.Water)
                {
                    MagicSkill.Add(SpellList.Acrid_Stream);
                    MagicSkill.Add(SpellList.Maelstrom);
                    MagicSkill.Add(SpellList.Corrosive_Ooze);
                    MagicSkill.Add(SpellList.Cursed_Sphere);
                }
                #endregion

                #region Wind
                if (_element == Elements.Wind)
                {
                    MagicSkill.Add(SpellList.Hecatomb_Wave);
                    MagicSkill.Add(SpellList.Mysterious_Light);
                    MagicSkill.Add(SpellList.Leafstorm);
                    MagicSkill.Add(SpellList.Reaving_Wind);
                }
                #endregion

                #region Ice
                if (_element == Elements.Ice)
                {
                    MagicSkill.Add(SpellList.Infrasonics);
                    MagicSkill.Add(SpellList.Ice_Break);
                    MagicSkill.Add(SpellList.Cold_Wave);
                    MagicSkill.Add(SpellList.Frost_Breath);
                }
                #endregion

                #region Lightning
                if (_element == Elements.Lightning)
                {
                    MagicSkill.Add(SpellList.Temporal_Shift);
                    MagicSkill.Add(SpellList.Mind_Blast);
                    MagicSkill.Add(SpellList.Charged_Whisker);
                    MagicSkill.Add(SpellList.Blitzstrahl);
                }
                #endregion

                #region Light
                if (_element == Elements.Light)
                {
                    MagicSkill.Add(SpellList.Actinic_Burst);
                    MagicSkill.Add(SpellList.Radiant_Breath);
                    MagicSkill.Add(SpellList.Blank_Gaze);
                    MagicSkill.Add(SpellList.Light_of_Penance);
                }
                #endregion

                #region Dark
                if (_element == Elements.Dark)
                {
                    MagicSkill.Add(SpellList.Death_Ray);
                    MagicSkill.Add(SpellList.Eyes_On_Me);
                    MagicSkill.Add(SpellList.Sandspray);
                }
                #endregion
            }
            #endregion

            #region White Magic
            if (_type == VoidwatchProcType.WhiteMagic)
            {
                MagicSkill = new List<SpellList>();

                #region Fire
                if (_element == Elements.Fire)
                {
                    MagicSkill.Add(SpellList.Addle);
                }
                #endregion

                #region Earth
                if (_element == Elements.Earth)
                {
                    MagicSkill.Add(SpellList.Slow);
                }

                #region Water
                if (_element == Elements.Water)
                {
                }
                #endregion

                #region Wind
                if (_element == Elements.Wind)
                {
                }
                #endregion

                #region Ice
                if (_element == Elements.Ice)
                {
                    MagicSkill.Add(SpellList.Paralyze);
                }
                #endregion

                #region Lightning
                if (_element == Elements.Lightning)
                {
                }
                #endregion

                #region Light
                if (_element == Elements.Light)
                {
                    MagicSkill.Add(SpellList.Banish_II);
                    MagicSkill.Add(SpellList.Banish_III);
                    MagicSkill.Add(SpellList.Banishga);
                    MagicSkill.Add(SpellList.Banishga_II);
                    MagicSkill.Add(SpellList.Holy);
                    MagicSkill.Add(SpellList.Flash);
                    MagicSkill.Add(SpellList.Dia_II);
                    MagicSkill.Add(SpellList.Diaga);
                }
                #endregion

                #region Dark
                if (_element == Elements.Dark)
                {
                }
                #endregion
                #endregion
            }
            #endregion

            #region Black Magic

            if (_type == VoidwatchProcType.BlackMagic)
            {
                MagicSkill = new List<SpellList>();

                #region Fire
                if (_element == Elements.Fire)
                {
                    MagicSkill.Add(SpellList.Pyrohelix);
                    MagicSkill.Add(SpellList.Fire_II);
                    MagicSkill.Add(SpellList.Fire_III);
                    MagicSkill.Add(SpellList.Fire_IV);
                    MagicSkill.Add(SpellList.Firaga);
                    MagicSkill.Add(SpellList.Firaga_II);
                    MagicSkill.Add(SpellList.Firaga_III);
                    MagicSkill.Add(SpellList.Firaja);
                    MagicSkill.Add(SpellList.Flare);
                    MagicSkill.Add(SpellList.Burn);
                }
                #endregion

                #region Earth
                if (_element == Elements.Earth)
                {
                    MagicSkill.Add(SpellList.Geohelix);
                    MagicSkill.Add(SpellList.Stone_II);
                    MagicSkill.Add(SpellList.Stone_III);
                    MagicSkill.Add(SpellList.Stone_IV);
                    MagicSkill.Add(SpellList.Stonega);
                    MagicSkill.Add(SpellList.Stonega_II);
                    MagicSkill.Add(SpellList.Stonega_III);
                    MagicSkill.Add(SpellList.Stoneja);
                    MagicSkill.Add(SpellList.Quake);
                    MagicSkill.Add(SpellList.Rasp);
                }
                #endregion

                #region Water
                if (_element == Elements.Water)
                {
                    MagicSkill.Add(SpellList.Hydrohelix);
                    MagicSkill.Add(SpellList.Water_II);
                    MagicSkill.Add(SpellList.Water_III);
                    MagicSkill.Add(SpellList.Water_IV);
                    MagicSkill.Add(SpellList.Waterga);
                    MagicSkill.Add(SpellList.Waterga_II);
                    MagicSkill.Add(SpellList.Waterga_III);
                    MagicSkill.Add(SpellList.Waterja);
                    MagicSkill.Add(SpellList.Flood);
                    MagicSkill.Add(SpellList.Drown);
                    MagicSkill.Add(SpellList.Poison);
                    MagicSkill.Add(SpellList.Poison_II);
                    MagicSkill.Add(SpellList.Poisonga);
                }
                #endregion

                #region Wind
                if (_element == Elements.Wind)
                {
                    MagicSkill.Add(SpellList.Anemohelix);
                    MagicSkill.Add(SpellList.Aero_II);
                    MagicSkill.Add(SpellList.Aero_III);
                    MagicSkill.Add(SpellList.Aero_IV);
                    MagicSkill.Add(SpellList.Aeroga);
                    MagicSkill.Add(SpellList.Aeroga_II);
                    MagicSkill.Add(SpellList.Aeroga_III);
                    MagicSkill.Add(SpellList.Aeroja);
                    MagicSkill.Add(SpellList.Tornado);
                    MagicSkill.Add(SpellList.Choke);
                }
                #endregion

                #region Ice
                if (_element == Elements.Ice)
                {
                    MagicSkill.Add(SpellList.Anemohelix);
                    MagicSkill.Add(SpellList.Blizzard_II);
                    MagicSkill.Add(SpellList.Blizzard_III);
                    MagicSkill.Add(SpellList.Blizzard_IV);
                    MagicSkill.Add(SpellList.Blizzaga);
                    MagicSkill.Add(SpellList.Blizzaga_II);
                    MagicSkill.Add(SpellList.Blizzaga_III);
                    MagicSkill.Add(SpellList.Blizzaja);
                    MagicSkill.Add(SpellList.Freeze);
                    MagicSkill.Add(SpellList.Frost);
                }
                #endregion

                #region Lightning
                if (_element == Elements.Lightning)
                {
                    MagicSkill.Add(SpellList.Ionohelix);
                    MagicSkill.Add(SpellList.Thunder_II);
                    MagicSkill.Add(SpellList.Thunder_III);
                    MagicSkill.Add(SpellList.Thunder_IV);
                    MagicSkill.Add(SpellList.Thundaga);
                    MagicSkill.Add(SpellList.Thundaga_II);
                    MagicSkill.Add(SpellList.Thundaga_III);
                    MagicSkill.Add(SpellList.Thundaja);
                    MagicSkill.Add(SpellList.Burst);
                    MagicSkill.Add(SpellList.Shock);
                    MagicSkill.Add(SpellList.Stun);
                }
                #endregion

                #region Light
                if (_element == Elements.Light)
                {
                    MagicSkill.Add(SpellList.Luminohelix);
                }
                #endregion

                #region Dark
                if (_element == Elements.Dark)
                {
                    MagicSkill.Add(SpellList.Noctohelix);
                    MagicSkill.Add(SpellList.Drain);
                    MagicSkill.Add(SpellList.Aspir);
                    MagicSkill.Add(SpellList.Absorb_TP);
                    MagicSkill.Add(SpellList.Absorb_ACC);
                    MagicSkill.Add(SpellList.Absorb_STR);
                    MagicSkill.Add(SpellList.Absorb_DEX);
                    MagicSkill.Add(SpellList.Absorb_AGI);
                    MagicSkill.Add(SpellList.Absorb_VIT);
                    MagicSkill.Add(SpellList.Absorb_INT);
                    MagicSkill.Add(SpellList.Absorb_MND);
                    MagicSkill.Add(SpellList.Absorb_CHR);
                    MagicSkill.Add(SpellList.Bio_II);
                    MagicSkill.Add(SpellList.Blind);
                    MagicSkill.Add(SpellList.Dispel);
                }
                #endregion
            }
            #endregion

            #region Song
            if (_type == VoidwatchProcType.Song)
            {
                MagicSkill = new List<SpellList>();

                #region Fire
                if (_element == Elements.Fire)
                {
                    MagicSkill.Add(SpellList.Ice_Threnody);
                }
                #endregion

                #region Earth
                if (_element == Elements.Earth)
                {
                    MagicSkill.Add(SpellList.Lightning_Threnody);
                    MagicSkill.Add(SpellList.Battlefield_Elegy);
                    MagicSkill.Add(SpellList.Carnage_Elegy);
                }
                #endregion

                #region Water
                if (_element == Elements.Water)
                {
                    MagicSkill.Add(SpellList.Fire_Threnody);
                }
                #endregion

                #region Wind
                if (_element == Elements.Wind)
                {
                    MagicSkill.Add(SpellList.Earth_Threnody);
                }
                #endregion

                #region Ice
                if (_element == Elements.Ice)
                {
                    MagicSkill.Add(SpellList.Wind_Threnody);
                }
                #endregion

                #region Lightning
                if (_element == Elements.Lightning)
                {
                    MagicSkill.Add(SpellList.Water_Threnody);
                }
                #endregion

                #region Light
                if (_element == Elements.Light)
                {
                    MagicSkill.Add(SpellList.Dark_Threnody);
                    MagicSkill.Add(SpellList.Magic_Finale);
                }
                #endregion

                #region Dark
                if (_element == Elements.Dark)
                {
                    MagicSkill.Add(SpellList.Light_Threnody);
                }
                #endregion
            }
            #endregion

            #region Ninjutsu
            if (_type == VoidwatchProcType.Ninjutsu)
            {
                MagicSkill = new List<SpellList>();

                #region Fire
                if (_element == Elements.Fire)
                {
                    MagicSkill.Add(SpellList.Katon_Ichi);
                    MagicSkill.Add(SpellList.Katon_Ni);
                }
                #endregion

                #region Earth
                if (_element == Elements.Earth)
                {
                    MagicSkill.Add(SpellList.Doton_Ichi);
                    MagicSkill.Add(SpellList.Doton_Ni);
                    MagicSkill.Add(SpellList.Hojo_Ichi);
                    MagicSkill.Add(SpellList.Hojo_Ni);
                }
                #endregion

                #region Water
                if (_element == Elements.Water)
                {
                    MagicSkill.Add(SpellList.Suiton_Ichi);
                    MagicSkill.Add(SpellList.Suiton_Ni);
                    MagicSkill.Add(SpellList.Dokumori_Ichi);
                    MagicSkill.Add(SpellList.Aisha_Ichi);
                }
                #endregion

                #region Wind
                if (_element == Elements.Wind)
                {
                    MagicSkill.Add(SpellList.Huton_Ichi);
                    MagicSkill.Add(SpellList.Huton_Ni);
                }
                #endregion

                #region Ice
                if (_element == Elements.Ice)
                {
                    MagicSkill.Add(SpellList.Hyoton_Ichi);
                    MagicSkill.Add(SpellList.Hyoton_Ni);
                    MagicSkill.Add(SpellList.Jubaku_Ichi);
                }
                #endregion

                #region Lightning
                if (_element == Elements.Lightning)
                {
                    MagicSkill.Add(SpellList.Raiton_Ichi);
                    MagicSkill.Add(SpellList.Raiton_Ni);
                }
                #endregion

                #region Light
                if (_element == Elements.Light)
                {
                }
                #endregion

                #region Dark
                if (_element == Elements.Dark)
                {
                    MagicSkill.Add(SpellList.Kurayami_Ichi);
                    MagicSkill.Add(SpellList.Kurayami_Ni);
                    MagicSkill.Add(SpellList.Yurin_Ichi);
                }
                #endregion
            }
            #endregion

            #endregion

            if (JobSkill != null)
                return JobSkill;
            if (MagicSkill != null)
                return MagicSkill;
            return WeaponSkill;
        }

    }
}
