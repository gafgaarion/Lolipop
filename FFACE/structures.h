/*!\file structures.h 
\brief version 4 structure header. */
#pragma once
#define FFACE_LOAD_OK  0xB001
#define FFACE_LOAD_FAIL  0xB002
#define FFACE_LOAD_LOCK 0xB003
#define FFACE_LOAD_INIT 0xB004
#define FFACE_LOAD_INIT_ERROR 0xB005
#define FFACE_LOAD_ACCESS 0xB006
#define WINDOWER_FILEMAP_FAIL 0xB011
#define WINDOWER_FILEVIEW_FAIL 0xB012
#define WINDOWER_NULL_HANDLE 0xB013
#define WINDOWER_QUEUE_EMPTY 0xB014
#define WINDOWER_COMMAND_TIMEOUT 0xB015
#define WINDOWER_INDEX_OUT_SCOPE 0xB016

#define FFACE_READ 0xB021 /*wparam bool success*/
#define FFACE_WRITE 0xB026 /*wparam bool success*/

struct TRADEITEM
{
	unsigned short ItemID;
	char Index;
	char Count;
};
struct TRADEINFO
{
	unsigned int Gil;
	int TargetID;
	char SelectedBox;
	TRADEITEM Items[8];
};


struct TREASUREITEM
{
	char Flag; //3=no item, 2=item
	short ItemID; 
	char Count; 
	char Status; //0=no action, 1=pass, 2=lot
	short MyLot; 
	short WinLot;
	int WinPlayerSrvID;
	int WinPLayerID;
	int TimeStamp; //utc timestamp
};

struct INVENTORYITEM
{
	unsigned short id; 
	unsigned char index;
	unsigned int count;
	unsigned int Flag; //5=equiped 25=baraar
	unsigned int Price;
	unsigned short extra;//ws points, charges
};

struct CHATEXTRAINFO
{
	short	MessageType;
};


struct ALLIANCEINFO
{
	int AllianceLeaderID;
	int Party0LeaderID;
	int Party1LeaderID;
	int Party2LeaderID;
	char Party0Visible;
	char Party1Visible;
	char Party2Visible;
	char Party0Count;
	char Party1Count;
	char Party2Count;	
	char Invited;
	char unknown;
};

struct PARTYMEMBER
{
	int pad0;
	char Index;
	char MemberNumber;
	char Name[18];
	int SvrID;
	int ID;
	int unknown0;
	int CurrentHP;
	int CurrentMP;
	int CurrentTP;
	char CurrentHPP;
	char CurrentMPP;
	short Zone;
	int pad1;
	unsigned int FlagMask;
	char pad2[20];
	int SvrIDDupe;
	char CurrentHPPDupe;
	char CurrentMPPDupe;
	char Active;
	char pad3;
};
struct PARTYMEMBERS
{
	PARTYMEMBER Member[17];
};

struct SkillLevel
{
	bool capped;
	unsigned short level;
};
struct CraftSkillLevel
{
	bool capped;
	char level;
	char rank;
};
struct PlayerStats
{
	short Str;
	short Dex;
	short Vit;
	short Agi;
	short Int;
	short Mnd;
	short Chr;
};
struct PlayerElements
{
	short Fire;
	short Ice;
	short Wind;
	short Earth;
	short Lightning;
	short Water;
	short Light;
	short Dark;
};
struct PlayerCombatSkills
{
	unsigned short HandToHand;
	unsigned short Dagger;
	unsigned short Sword;
	unsigned short GreatSword;
	unsigned short Axe;
	unsigned short GreatAxe;
	unsigned short Scythe;
	unsigned short Polearm;
	unsigned short Katana;
	unsigned short GreatKatana;
	unsigned short Club;
	unsigned short Staff;
	unsigned short unkweap0;
	unsigned short unkweap1;
	unsigned short unkweap2;
	unsigned short unkweap3;
	unsigned short unkweap4;
	unsigned short unkweap5;
	unsigned short unkweap6;
	unsigned short unkweap7;
	unsigned short unkweap8;
	unsigned short unkweap9;
	unsigned short unkweap10;
	unsigned short unkweap11;
	unsigned short Archery;
	unsigned short Marksmanship;
	unsigned short Throwing;
	unsigned short Guarding;
	unsigned short Evasion;
	unsigned short Shield;
	unsigned short Parrying;
};
struct PlayerMagicSkills
{
	unsigned short Divine;
	unsigned short Healing;
	unsigned short Enhancing;
	unsigned short Enfeebling;
	unsigned short Elemental;
	unsigned short Dark;
	unsigned short Summon;
	unsigned short Ninjitsu;
	unsigned short Singing;
	unsigned short String;
	unsigned short Wind;
	unsigned short BlueMagic;
	unsigned short unkmagic0;
	unsigned short unkmagic1;
	unsigned short unkmagic2;
	unsigned short unkmagic3;
};

struct PlayerCraftLevels
{
	unsigned short Fishing;
	unsigned short Woodworking;
	unsigned short Smithing;
	unsigned short Goldsmithing;
	unsigned short Clothcraft;
	unsigned short Leathercraft;
	unsigned short Bonecraft;
	unsigned short Alchemy;
	unsigned short Cooking;
};

struct PLAYERINFO
{
	int HPMax;
	int MPMax;
	char MainJob;
	char MainJobLVL;
	char SubJob;
	char SubJobLVL;
	unsigned short EXPIntoLVL;
	unsigned short EXPForLVL;
	PlayerStats Stats;
	PlayerStats StatModifiers;
	short Attack;
	short Defense;
	PlayerElements Elements;
	short Title;
	short Rank;
	short RankPts;
	char Nation;
	char Residence;
	int HomePoint;
	PlayerCombatSkills CombatSkills;
	PlayerMagicSkills MagicSkills;
	PlayerCraftLevels CraftLevels;
	char null0[146];
	unsigned short LimitPoints;
	unsigned char MeritPoints;
	unsigned char LimitMode;
	char null1[78];
	unsigned short Buffs[32];
};

struct TARGETINFO
{
	int cID;
	int sID;
	int cSvrID;
	int sSrvID;
	unsigned short cMask;
	unsigned short sMask;
	char Locked;
	char IsSub;
	char HPP;
	char Name[20];
};