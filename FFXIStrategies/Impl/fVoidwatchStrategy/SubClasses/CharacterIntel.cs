using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;
using FFXIAggregateRoots;
using FFXICommands;
using System.Collections.Concurrent;

namespace FFXIStrategies.Impl.fVoidwatchStrategy.Subclasses
{
    public class CharacterIntel
    {
        public string mcharacterName;
        private Job mJob;
        private Job mSubJob;
        private List<SkillCollection> mSkillCollections;
        private FFACE instance;
        private SkillCollection CurrentSkillCollectionUsed;
        public float CurrentSkillMaxDistance
        {
            get
            {
                return this.CurrentSkillCollectionUsed.MaxRange;
            }
        }
        public bool TradedCells { get; set; }
        public bool IsBusy { get; set; }

        public CharacterIntel(string _characterName, Job _Job, Job _SubJob, FFACE _instance)
        {
            this.mcharacterName = _characterName;
            this.mJob = _Job;
            this.mSubJob = _SubJob;
            this.mSkillCollections = new List<SkillCollection>();
            this.instance = _instance;
            this.CurrentSkillCollectionUsed = null;
            this.TradedCells = false;

            #region Black mage
            if (_Job == Job.BLM)
            {
                #region Fire
                SkillCollection fireskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Fire);

                fireskills.AddSkill(SpellList.Fire_II);
                fireskills.AddSkill(SpellList.Fire_III);
                fireskills.AddSkill(SpellList.Fire_IV);
                fireskills.AddSkill(SpellList.Firaga);
                fireskills.AddSkill(SpellList.Firaga_II);
                fireskills.AddSkill(SpellList.Firaga_III);
                fireskills.AddSkill(SpellList.Firaja);
                fireskills.AddSkill(SpellList.Flare);
                fireskills.AddSkill(SpellList.Burn);
                #endregion

                #region Earth
                SkillCollection earthskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Earth);

                earthskills.AddSkill(SpellList.Stone_II);
                earthskills.AddSkill(SpellList.Stone_III);
                earthskills.AddSkill(SpellList.Stone_IV);
                earthskills.AddSkill(SpellList.Stonega);
                earthskills.AddSkill(SpellList.Stonega_II);
                earthskills.AddSkill(SpellList.Stonega_III);
                earthskills.AddSkill(SpellList.Stoneja);
                earthskills.AddSkill(SpellList.Quake);
                earthskills.AddSkill(SpellList.Rasp);
                #endregion

                #region Water
                SkillCollection waterskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Water);

                waterskills.AddSkill(SpellList.Water_II);
                waterskills.AddSkill(SpellList.Water_III);
                waterskills.AddSkill(SpellList.Water_IV);
                waterskills.AddSkill(SpellList.Waterga);
                waterskills.AddSkill(SpellList.Waterga_II);
                waterskills.AddSkill(SpellList.Waterga_III);
                waterskills.AddSkill(SpellList.Waterja);
                waterskills.AddSkill(SpellList.Flood);
                waterskills.AddSkill(SpellList.Drown);
                waterskills.AddSkill(SpellList.Poison);
                waterskills.AddSkill(SpellList.Poison_II);
                waterskills.AddSkill(SpellList.Poisonga);
                #endregion

                #region Wind
                SkillCollection windskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Wind);

                windskills.AddSkill(SpellList.Aero_II);
                windskills.AddSkill(SpellList.Aero_III);
                windskills.AddSkill(SpellList.Aero_IV);
                windskills.AddSkill(SpellList.Aeroga);
                windskills.AddSkill(SpellList.Aeroga_II);
                windskills.AddSkill(SpellList.Aeroga_III);
                windskills.AddSkill(SpellList.Aeroja);
                windskills.AddSkill(SpellList.Choke);
                windskills.AddSkill(SpellList.Tornado);
                #endregion

                #region Ice
                SkillCollection iceskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Ice);

                iceskills.AddSkill(SpellList.Blizzard_II);
                iceskills.AddSkill(SpellList.Blizzard_III);
                iceskills.AddSkill(SpellList.Blizzard_IV);
                iceskills.AddSkill(SpellList.Blizzaga);
                iceskills.AddSkill(SpellList.Blizzaga_II);
                iceskills.AddSkill(SpellList.Blizzaga_III);
                iceskills.AddSkill(SpellList.Blizzaja);
                iceskills.AddSkill(SpellList.Frost);
                iceskills.AddSkill(SpellList.Freeze);
                #endregion

                #region Lightning
                SkillCollection lightningskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Lightning);

                lightningskills.AddSkill(SpellList.Thunder_II);
                lightningskills.AddSkill(SpellList.Thunder_III);
                lightningskills.AddSkill(SpellList.Thunder_IV);
                lightningskills.AddSkill(SpellList.Thundaga);
                lightningskills.AddSkill(SpellList.Thundaga_II);
                lightningskills.AddSkill(SpellList.Thundaga_III);
                lightningskills.AddSkill(SpellList.Thundaja);
                lightningskills.AddSkill(SpellList.Shock);
                lightningskills.AddSkill(SpellList.Stun);
                lightningskills.AddSkill(SpellList.Burst);
                #endregion

                #region Darkness
                SkillCollection darknessskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Dark);

                darknessskills.AddSkill(SpellList.Drain);
                darknessskills.AddSkill(SpellList.Aspir);
                darknessskills.AddSkill(SpellList.Bio_II);
                darknessskills.AddSkill(SpellList.Blind);
                #endregion

                #region Light
                #endregion

                #region FindProc
                SkillCollection findprocskills = new SkillCollection(VoidwatchProcType.FindProcSpell, Elements.Neutral);

                findprocskills.AddSkill(SpellList.Bio);
                findprocskills.AddSkill(SpellList.Bio_II);

                this.mSkillCollections.Add(findprocskills);
                #endregion

                this.mSkillCollections.Add(fireskills);
                this.mSkillCollections.Add(earthskills);
                this.mSkillCollections.Add(waterskills);
                this.mSkillCollections.Add(windskills);
                this.mSkillCollections.Add(iceskills);
                this.mSkillCollections.Add(lightningskills);
                this.mSkillCollections.Add(darknessskills);
            }
            #endregion

            #region Red mage
            if (_Job == Job.RDM)
            {
                #region Fire
                SkillCollection fireskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Fire);
                SkillCollection wfireskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Fire);

                fireskills.AddSkill(SpellList.Fire_II);
                fireskills.AddSkill(SpellList.Fire_III);
                fireskills.AddSkill(SpellList.Fire_IV);
                fireskills.AddSkill(SpellList.Addle);
                #endregion

                #region Earth
                SkillCollection earthskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Earth);
                SkillCollection wearthskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Earth);

                earthskills.AddSkill(SpellList.Stone_II);
                earthskills.AddSkill(SpellList.Stone_III);
                earthskills.AddSkill(SpellList.Stone_IV);
                earthskills.AddSkill(SpellList.Slow);
                #endregion

                #region Water
                SkillCollection waterskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Water);

                waterskills.AddSkill(SpellList.Water_II);
                waterskills.AddSkill(SpellList.Water_III);
                waterskills.AddSkill(SpellList.Water_IV);
                waterskills.AddSkill(SpellList.Poison);
                waterskills.AddSkill(SpellList.Poison_II);
                #endregion

                #region Wind
                SkillCollection windskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Wind);

                windskills.AddSkill(SpellList.Aero_II);
                windskills.AddSkill(SpellList.Aero_III);
                windskills.AddSkill(SpellList.Aero_IV);
                #endregion

                #region Ice
                SkillCollection iceskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Ice);
                SkillCollection wiceskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Ice);

                iceskills.AddSkill(SpellList.Blizzard_II);
                iceskills.AddSkill(SpellList.Blizzard_III);
                iceskills.AddSkill(SpellList.Blizzard_IV);
                iceskills.AddSkill(SpellList.Paralyze);
                #endregion

                #region Lightning
                SkillCollection lightningskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Lightning);

                lightningskills.AddSkill(SpellList.Thunder_II);
                lightningskills.AddSkill(SpellList.Thunder_III);
                lightningskills.AddSkill(SpellList.Thunder_IV);
                #endregion

                #region Darkness
                SkillCollection darknessskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Dark);

                darknessskills.AddSkill(SpellList.Bio_II);
                darknessskills.AddSkill(SpellList.Blind);
                darknessskills.AddSkill(SpellList.Dispel);
                #endregion

                #region Light
                SkillCollection wlightskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Light);

                wlightskills.AddSkill(SpellList.Dia_II);
                #endregion

                #region FindProc
                SkillCollection findprocskills = new SkillCollection(VoidwatchProcType.FindProcSpell, Elements.Neutral);

                findprocskills.AddSkill(SpellList.Bio);
                findprocskills.AddSkill(SpellList.Bio_II);

                this.mSkillCollections.Add(findprocskills);
                #endregion

                this.mSkillCollections.Add(fireskills);
                this.mSkillCollections.Add(earthskills);
                this.mSkillCollections.Add(waterskills);
                this.mSkillCollections.Add(windskills);
                this.mSkillCollections.Add(iceskills);
                this.mSkillCollections.Add(lightningskills);
                this.mSkillCollections.Add(darknessskills);

                this.mSkillCollections.Add(wfireskills);
                this.mSkillCollections.Add(wearthskills);
                this.mSkillCollections.Add(wiceskills);
                this.mSkillCollections.Add(wlightskills);
            }
            #endregion

            #region Scholar
            if (_Job == Job.SCH)
            {
                #region Fire
                SkillCollection fireskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Fire);

                fireskills.AddSkill(SpellList.Pyrohelix);
                fireskills.AddSkill(SpellList.Fire_II);
                fireskills.AddSkill(SpellList.Fire_III);
                fireskills.AddSkill(SpellList.Fire_IV);
                #endregion

                #region Earth
                SkillCollection earthskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Earth);
                SkillCollection wearthskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Earth);

                earthskills.AddSkill(SpellList.Geohelix);
                earthskills.AddSkill(SpellList.Stone_II);
                earthskills.AddSkill(SpellList.Stone_III);
                earthskills.AddSkill(SpellList.Stone_IV);
                wearthskills.AddSkill(SpellList.Slow);
                #endregion

                #region Water
                SkillCollection waterskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Water);

                waterskills.AddSkill(SpellList.Hydrohelix);
                waterskills.AddSkill(SpellList.Water_II);
                waterskills.AddSkill(SpellList.Water_III);
                waterskills.AddSkill(SpellList.Water_IV);
                waterskills.AddSkill(SpellList.Poison);
                waterskills.AddSkill(SpellList.Poison_II);
                #endregion

                #region Wind
                SkillCollection windskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Wind);

                windskills.AddSkill(SpellList.Anemohelix);
                windskills.AddSkill(SpellList.Aero_II);
                windskills.AddSkill(SpellList.Aero_III);
                windskills.AddSkill(SpellList.Aero_IV);
                #endregion

                #region Ice
                SkillCollection iceskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Ice);
                SkillCollection wiceskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Ice);

                iceskills.AddSkill(SpellList.Cryohelix);
                iceskills.AddSkill(SpellList.Blizzard_II);
                iceskills.AddSkill(SpellList.Blizzard_III);
                iceskills.AddSkill(SpellList.Blizzard_IV);
                wiceskills.AddSkill(SpellList.Paralyze);
                #endregion

                #region Lightning
                SkillCollection lightningskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Lightning);

                lightningskills.AddSkill(SpellList.Ionohelix);
                lightningskills.AddSkill(SpellList.Thunder_II);
                lightningskills.AddSkill(SpellList.Thunder_III);
                lightningskills.AddSkill(SpellList.Thunder_IV);
                #endregion

                #region Darkness
                SkillCollection darknessskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Dark);

                darknessskills.AddSkill(SpellList.Noctohelix);
                darknessskills.AddSkill(SpellList.Bio_II);
                darknessskills.AddSkill(SpellList.Blind);
                darknessskills.AddSkill(SpellList.Dispel);
                #endregion

                #region Light
                SkillCollection lightskills = new SkillCollection(VoidwatchProcType.BlackMagic, Elements.Light);
                SkillCollection wlightskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Light);

                lightskills.AddSkill(SpellList.Luminohelix);
                wlightskills.AddSkill(SpellList.Dia_II);
                #endregion

                #region FindProc
                SkillCollection findprocskills = new SkillCollection(VoidwatchProcType.FindProcSpell, Elements.Neutral);

                findprocskills.AddSkill(SpellList.Bio);
                findprocskills.AddSkill(SpellList.Bio_II);

                this.mSkillCollections.Add(findprocskills);
                #endregion

                this.mSkillCollections.Add(fireskills);
                this.mSkillCollections.Add(earthskills);
                this.mSkillCollections.Add(waterskills);
                this.mSkillCollections.Add(windskills);
                this.mSkillCollections.Add(iceskills);
                this.mSkillCollections.Add(lightningskills);
                this.mSkillCollections.Add(darknessskills);
                this.mSkillCollections.Add(lightskills);

                this.mSkillCollections.Add(wearthskills);
                this.mSkillCollections.Add(wiceskills);
                this.mSkillCollections.Add(wlightskills);
            }
            #endregion

            #region Warrior
            if (_Job == Job.WAR)
            {
                #region Axe
                SkillCollection axe = new SkillCollection(VoidwatchProcType.Axe, Elements.Neutral);

                axe.AddSkill(WeaponSkillList.Raging_Axe);
                axe.AddSkill(WeaponSkillList.Smash_Axe);
                axe.AddSkill(WeaponSkillList.Gale_Axe);
                axe.AddSkill(WeaponSkillList.Avalanche_Axe);
                axe.AddSkill(WeaponSkillList.Spinning_Axe);
                axe.AddSkill(WeaponSkillList.Rampage);
                axe.AddSkill(WeaponSkillList.Calamity);
                axe.AddSkill(WeaponSkillList.Mistral_Axe);
                axe.AddSkill(WeaponSkillList.Decimation);
                axe.AddSkill(WeaponSkillList.Bora_Axe);
                #endregion

                #region Club
                SkillCollection club = new SkillCollection(VoidwatchProcType.Club, Elements.Neutral);

                club.AddSkill(WeaponSkillList.Shining_Strike);
                club.AddSkill(WeaponSkillList.Brainshaker);
                club.AddSkill(WeaponSkillList.Skullbreaker);
                club.AddSkill(WeaponSkillList.True_Strike);
                club.AddSkill(WeaponSkillList.Judgment);
                club.AddSkill(WeaponSkillList.Black_Halo);
                club.AddSkill(WeaponSkillList.Flash_Nova);
                #endregion

                #region Dagger
                SkillCollection dagger = new SkillCollection(VoidwatchProcType.Dagger, Elements.Neutral);

                dagger.AddSkill(WeaponSkillList.Wasp_Sting);
                dagger.AddSkill(WeaponSkillList.Gust_Slash);
                dagger.AddSkill(WeaponSkillList.Shadowstitch);
                dagger.AddSkill(WeaponSkillList.Viper_Bite);
                dagger.AddSkill(WeaponSkillList.Cyclone);
                dagger.AddSkill(WeaponSkillList.Energy_Steal);
                dagger.AddSkill(WeaponSkillList.Evisceration);
                dagger.AddSkill(WeaponSkillList.Aeolian_Edge);
                #endregion

                #region GreatAxe
                SkillCollection gaxe = new SkillCollection(VoidwatchProcType.GreatAxe, Elements.Neutral);

                gaxe.AddSkill(WeaponSkillList.Shield_Break);
                gaxe.AddSkill(WeaponSkillList.Iron_Tempest);
                gaxe.AddSkill(WeaponSkillList.Sturmwind);
                gaxe.AddSkill(WeaponSkillList.Armor_Break);
                gaxe.AddSkill(WeaponSkillList.Keen_Edge);
                gaxe.AddSkill(WeaponSkillList.Weapon_Break);
                gaxe.AddSkill(WeaponSkillList.Raging_Rush);
                gaxe.AddSkill(WeaponSkillList.Full_Break);
                gaxe.AddSkill(WeaponSkillList.Steel_Cyclone);
                gaxe.AddSkill(WeaponSkillList.Fell_Cleave);
                #endregion

                #region GreatSword
                SkillCollection gs = new SkillCollection(VoidwatchProcType.GreatSword, Elements.Neutral);

                gs.AddSkill(WeaponSkillList.Hard_Slash);
                gs.AddSkill(WeaponSkillList.Power_Slash);
                gs.AddSkill(WeaponSkillList.Frostbite);
                gs.AddSkill(WeaponSkillList.Freezebite);
                gs.AddSkill(WeaponSkillList.Shockwave);
                gs.AddSkill(WeaponSkillList.Crescent_Moon);
                gs.AddSkill(WeaponSkillList.Sickle_Moon);
                gs.AddSkill(WeaponSkillList.Spinning_Slash);
                gs.AddSkill(WeaponSkillList.Ground_Strike);
                gs.AddSkill(WeaponSkillList.Herculean_Slash);
                #endregion

                #region Polearm
                SkillCollection pole = new SkillCollection(VoidwatchProcType.Polearm, Elements.Neutral);

                pole.AddSkill(WeaponSkillList.Double_Thrust);
                pole.AddSkill(WeaponSkillList.Thunder_Thrust);
                pole.AddSkill(WeaponSkillList.Raiden_Thrust);
                pole.AddSkill(WeaponSkillList.Leg_Sweep);
                #endregion

                #region Sword
                SkillCollection sword = new SkillCollection(VoidwatchProcType.Sword, Elements.Neutral);

                sword.AddSkill(WeaponSkillList.Fast_Blade);
                sword.AddSkill(WeaponSkillList.Burning_Blade);
                sword.AddSkill(WeaponSkillList.Red_Lotus_Blade);
                sword.AddSkill(WeaponSkillList.Flat_Blade);
                sword.AddSkill(WeaponSkillList.Shining_Blade);
                sword.AddSkill(WeaponSkillList.Circle_Blade);
                sword.AddSkill(WeaponSkillList.Vorpal_Blade);
                sword.AddSkill(WeaponSkillList.Savage_Blade);
                sword.AddSkill(WeaponSkillList.Sanguine_Blade);
                #endregion

                #region Staff
                SkillCollection staff = new SkillCollection(VoidwatchProcType.Staff, Elements.Neutral);

                staff.AddSkill(WeaponSkillList.Heavy_Swing);
                staff.AddSkill(WeaponSkillList.Rock_Crusher);
                staff.AddSkill(WeaponSkillList.Earth_Crusher);
                staff.AddSkill(WeaponSkillList.Starburst);
                staff.AddSkill(WeaponSkillList.Full_Swing);
                #endregion

                #region FindProc
                SkillCollection findprocskills = new SkillCollection(VoidwatchProcType.FindProcSpell, Elements.Neutral);

                findprocskills.AddSkill(WeaponSkillList.Flat_Blade);
                findprocskills.AddSkill(AbilityList.Provoke);

                this.mSkillCollections.Add(findprocskills);
                #endregion

                this.mSkillCollections.Add(axe);
                this.mSkillCollections.Add(club);
                this.mSkillCollections.Add(dagger);
                this.mSkillCollections.Add(gaxe);
                this.mSkillCollections.Add(gs);
                this.mSkillCollections.Add(pole);
                this.mSkillCollections.Add(sword);
                this.mSkillCollections.Add(staff);

            }
            #endregion

            #region White mage
            if (_Job == Job.WHM)
            {
                #region Fire
                SkillCollection fireskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Fire);

                fireskills.AddSkill(SpellList.Addle);
                #endregion

                #region Earth
                SkillCollection earthskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Earth);

                earthskills.AddSkill(SpellList.Slow);
                #endregion

                #region Water
                #endregion

                #region Wind
                #endregion

                #region Ice
                SkillCollection iceskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Ice);

                iceskills.AddSkill(SpellList.Paralyze);
                #endregion

                #region Lightning
                #endregion

                #region Darkness
                #endregion

                #region Light
                SkillCollection lightskills = new SkillCollection(VoidwatchProcType.WhiteMagic, Elements.Light);

                lightskills.AddSkill(SpellList.Banish_II);
                lightskills.AddSkill(SpellList.Banish_III);
                lightskills.AddSkill(SpellList.Banishga);
                lightskills.AddSkill(SpellList.Banishga_II);
                lightskills.AddSkill(SpellList.Holy);
                lightskills.AddSkill(SpellList.Flash);
                lightskills.AddSkill(SpellList.Dia_II);
                lightskills.AddSkill(SpellList.Diaga);
                #endregion

                #region FindProc
                SkillCollection findprocskills = new SkillCollection(VoidwatchProcType.FindProcSpell, Elements.Neutral);

                findprocskills.AddSkill(SpellList.Dia);
                findprocskills.AddSkill(SpellList.Dia_II);

                this.mSkillCollections.Add(findprocskills);
                #endregion

                this.mSkillCollections.Add(fireskills);
                this.mSkillCollections.Add(earthskills);
                this.mSkillCollections.Add(iceskills);
                this.mSkillCollections.Add(lightskills);
            }
            #endregion

            #region Bard
            if (_Job == Job.BRD)
            {
                #region Fire
                SkillCollection fireskills = new SkillCollection(VoidwatchProcType.Song, Elements.Fire);

                fireskills.AddSkill(SpellList.Ice_Threnody);
                #endregion

                #region Earth
                SkillCollection earthskills = new SkillCollection(VoidwatchProcType.Song, Elements.Earth);

                earthskills.AddSkill(SpellList.Lightning_Threnody);
                earthskills.AddSkill(SpellList.Battlefield_Elegy);
                earthskills.AddSkill(SpellList.Carnage_Elegy);
                #endregion

                #region Water
                SkillCollection waterskills = new SkillCollection(VoidwatchProcType.Song, Elements.Water);

                waterskills.AddSkill(SpellList.Fire_Threnody);
                #endregion

                #region Wind
                SkillCollection windskills = new SkillCollection(VoidwatchProcType.Song, Elements.Wind);

                windskills.AddSkill(SpellList.Earth_Threnody);
                #endregion

                #region Ice
                SkillCollection iceskills = new SkillCollection(VoidwatchProcType.Song, Elements.Ice);

                iceskills.AddSkill(SpellList.Wind_Threnody);
                #endregion

                #region Lightning
                SkillCollection lightningskills = new SkillCollection(VoidwatchProcType.Song, Elements.Lightning);

                lightningskills.AddSkill(SpellList.Water_Threnody);
                #endregion

                #region Darkness
                SkillCollection darknessskills = new SkillCollection(VoidwatchProcType.Song, Elements.Dark);

                darknessskills.AddSkill(SpellList.Light_Threnody);
                darknessskills.AddSkill(SpellList.Magic_Finale);
                #endregion

                #region Light
                SkillCollection lightskills = new SkillCollection(VoidwatchProcType.Song, Elements.Light);

                lightskills.AddSkill(SpellList.Dark_Threnody);
                #endregion

                #region FindProc
                SkillCollection findprocskills = new SkillCollection(VoidwatchProcType.FindProcSpell, Elements.Neutral);

                findprocskills.AddSkill(SpellList.Fire_Threnody);
                findprocskills.AddSkill(SpellList.Ice_Threnody);
                findprocskills.AddSkill(SpellList.Wind_Threnody);
                findprocskills.AddSkill(SpellList.Dark_Threnody);
                findprocskills.AddSkill(SpellList.Light_Threnody);

                this.mSkillCollections.Add(findprocskills);
                #endregion

                this.mSkillCollections.Add(fireskills);
                this.mSkillCollections.Add(earthskills);
                this.mSkillCollections.Add(waterskills);
                this.mSkillCollections.Add(windskills);
                this.mSkillCollections.Add(iceskills);
                this.mSkillCollections.Add(lightningskills);
                this.mSkillCollections.Add(darknessskills);
                this.mSkillCollections.Add(lightskills);
            }
            #endregion
        }

        #region SkillCollection methods
        private SkillCollection getSkillCollection(VoidwatchProcType _type, Elements _element)
        {
            foreach (SkillCollection skillCollec in mSkillCollections)
            {
                if ((skillCollec.Elemental == _element || skillCollec.Elemental == Elements.Neutral) 
                    && skillCollec.ProcType == _type)
                {
                    return skillCollec;
                }
            }
            return null;
        }

        public int AbilitiesToTryCount(VoidwatchWeakness _weakness)
        { 
            SkillCollection skillCollection = this.getSkillCollection(_weakness.ProcType, _weakness.Elemental);

            if (skillCollection == null)
                return 0;

            int count = 0;

            foreach (SpellList spell in skillCollection.MagicSkill)
            {
                if (_weakness.haveToTry(spell))
                    count++;
            }

            foreach (AbilityList ability in skillCollection.JobSkill)
            {
                if (_weakness.haveToTry(ability))
                    count++;
            }

            foreach (WeaponSkillList ws in skillCollection.WeaponSkill)
            {
                if (_weakness.haveToTry(ws))
                    count++;
            }

            return count;
        }

        public SpellList? getNextSpell(VoidwatchWeakness _weakness)
        {
            SkillCollection skillCollection = this.getSkillCollection(_weakness.ProcType, _weakness.Elemental);

            if (skillCollection == null)
                return null;

            foreach (SpellList spell in skillCollection.MagicSkill)
            {
                if (_weakness.haveToTry(spell) && this.instance.Timer.GetSpellRecast(spell) == 0)
                    return spell;
            }
            return null;
        }

        public AbilityList? getNextAbility(VoidwatchWeakness _weakness)
        {
            SkillCollection skillCollection = this.getSkillCollection(_weakness.ProcType, _weakness.Elemental);

            if (skillCollection == null)
                return null;

            foreach (AbilityList ability in skillCollection.JobSkill)
            {
                if (_weakness.haveToTry(ability) && this.instance.Timer.GetAbilityRecast(ability) == 0)
                    return ability;
            }
            return null;
        }

        public WeaponSkillList? getNextWS(VoidwatchWeakness _weakness)
        {
            SkillCollection skillCollection = this.getSkillCollection(_weakness.ProcType, _weakness.Elemental);

            if (skillCollection == null)
                return null;

            if (this.instance.Player.TPCurrent >= 1000)
            {
                foreach (WeaponSkillList ws in skillCollection.WeaponSkill)
                {
                    if (_weakness.haveToTry(ws))
                        return ws;
                }
            }
            return null;
        }

        public object getBestSkillProcAttempt(List<VoidwatchWeakness> _weaknessList)
        {
            VoidwatchWeakness currentWeakness = null;

            foreach (VoidwatchWeakness weakness in _weaknessList)
            {
                if ((currentWeakness == null || currentWeakness.strength < weakness.strength)
                    && !weakness.procced)
                {
                    if (AbilitiesToTryCount(weakness) > 0)
                    {
                        return this.TryNextSkillOnWeakness(weakness);
                    }
                }
            }
            return null;
        }

        public object TryNextSkillOnWeakness(VoidwatchWeakness _weakness)
        {
            SpellList? nexts = this.getNextSpell(_weakness);
            AbilityList? nexta = this.getNextAbility(_weakness);
            WeaponSkillList? nextw = this.getNextWS(_weakness);

            if (!nexts.HasValue && !nexta.HasValue && !nextw.HasValue)
                return null;

            this.CurrentSkillCollectionUsed = this.getSkillCollection(_weakness.ProcType, _weakness.Elemental);

            if (nexts.HasValue)
                return nexts.Value;
            if (nexta.HasValue)
                return nexta.Value;
            return nextw.Value;
        }
    }
    #endregion
}
