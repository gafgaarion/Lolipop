#ifndef FFACE_H
#define FFACE_H
#include "structures.h"
#define FFACE_API __declspec( dllimport )
/*!\file FFACE4.h 
\brief version 4 import header. */

//!Main Constructor.
/*!
\param PID A valid FFXI process ID.
\return Pointer to the created instance of FFACE.
*/
FFACE_API void* CreateInstance(DWORD PID);
//!Main Destructor.
/*!
\param Instance - A vailid FFACE instance.
\return nothing.
*/
FFACE_API void DeleteInstance(void* Instance);
/*!Used to check if Contrubtor functions are active.
\param Instance - A vailid FFACE instance.
\return An bool indacating current function access.
*/
FFACE_API bool Access(void* Instance);
/*Windower*/
/*!
\param Instance - A vailid FFACE instance.
\param name - A name to reference the text object.
\return nothing.
*/
FFACE_API void CTHCreateTextObject(void* Instance, char* name);
/*!
\param Instance - A vailid FFACE instance.
\param name - The name used to reference the text object.
\return nothing.
*/
FFACE_API void CTHDeleteTextObject(void* Instance, char* name);
/*!
\param Instance - A vailid FFACE instance.
\param name - The name used to reference the text object.
\param text - The text to be set.
\return nothing.
*/
FFACE_API void CTHSetText(void* Instance, char* name, char* text);
/*!
\param Instance - A vailid FFACE instance.
\param name - The name used to reference the text object.
\param visible 
\return nothing.
*/
FFACE_API void CTHSetVisibility(void* Instance, char* name, bool visible);
FFACE_API void CTHSetFont(void* Instance, char* name, char* font, int height);
FFACE_API void CTHSetColor(void* Instance, char* name, unsigned char a, unsigned char r, unsigned char g, unsigned char b);
FFACE_API void CTHSetLocation(void* Instance, char* name, float x, float y);
FFACE_API void CTHSetBold(void* Instance, char* name, bool bold);
FFACE_API void CTHSetItalic(void* Instance, char* name, bool italic);
FFACE_API void CTHSetBGColor(void* Instance, char* name, unsigned char a, unsigned char r, unsigned char g, unsigned char b);
FFACE_API void CTHSetBGBorderSize(void* Instance, char* name, float pixels);
FFACE_API void CTHSetBGVisibility(void* Instance, char* name, bool visible);
FFACE_API void CTHSetRightJustified(void* Instance, char* name, bool justified);
FFACE_API void CTHFlushCommands(void* Instance);
FFACE_API void CKHSetKey(void* Instance, unsigned char key, bool down);
FFACE_API void CKHBlockInput(void* Instance, bool block);
FFACE_API void CKHSendString(void* Instance, char* string);
FFACE_API int CCHIsNewCommand(void* Instance);
FFACE_API int CCHGetArgCount(void* Instance);
FFACE_API void CCHGetArg(void* Instance, int index, char* buffer);

/*Player Class*/
FFACE_API void GetPlayerInfo(void* Instance, PLAYERINFO* PLAYER);
FFACE_API float GetCastMax(void* Instance);
FFACE_API float GetCastCountDown(void* Instance);
FFACE_API float GetCastPercent(void* Instance);
FFACE_API short GetCastPercentEx(void* Instance);
FFACE_API char GetViewMode(void* Instance);
FFACE_API char GetPlayerStatus(void* Instance);
FFACE_API char GetPLayerServerID(void *Instanceance);

/*Party*/
FFACE_API void GetAllianceInfo(void* Instance, ALLIANCEINFO* AI);
FFACE_API void GetPartyMembers(void* Instance, PARTYMEMBERS* ptm);
FFACE_API void GetPartyMember(void* Instance, char index, PARTYMEMBER* ptm);

/*chat*/
FFACE_API BOOL IsNewLine(void* Instance);
FFACE_API void GetChatLineR(void* Instance, int index, void *buffer, int* size);
FFACE_API void GetChatLine(void* Instance, int index, void *buffer, int* size);
FFACE_API void GetChatLineEx(void* Instance, int index, void *buffer, int* size, CHATEXTRAINFO* ExtraInfo);
FFACE_API int GetChatLineCount(void* Instance);

/*Timers*/
FFACE_API short GetSpellRecast(void* Instance, short id);
FFACE_API char GetAbilityID(void* Instance, char index);
FFACE_API int GetAbilityRecast(void* Instance, char index);
FFACE_API int GetVanaUTC(void* Instance);

/*NPC*/
FFACE_API bool IsNPCclaimed(void* Instance, int index);
FFACE_API int GetNPCclaimID(void* Instance, int index);
FFACE_API char GetNPCType(void* Instance, int index);
FFACE_API BOOL NPCIsActive(void* Instance, int index);
FFACE_API void GetNPCName(void* Instance, int index, void *buffer, int* size);
FFACE_API float GetNPCPosX(void* Instance, int index);
FFACE_API float GetNPCPosY(void* Instance, int index);
FFACE_API float GetNPCPosZ(void* Instance, int index);
FFACE_API float GetNPCPosH(void* Instance, int index);
FFACE_API float SetNPCPosH(void* Instance, int index, float value);
FFACE_API char GetNPCHPP(void* Instance, int index);
FFACE_API short GetNPCTP(void* Instance, int index);
FFACE_API char GetNPCStatus(void* Instance, int index);
FFACE_API int GetNPCPetID(void* Instance, int index);
FFACE_API float GetNPCDistance(void* Instance, int index);
FFACE_API char GetNPCBit(void* Instance, int index);
FFACE_API double GetNPCHeadingToNPC(void* Instance, int index0, int index1);

/*Target*/
FFACE_API void GetTargetInfo(void* Instance, TARGETINFO* Target);
FFACE_API BOOL SetTargetName(void* Instance, char name[], int Size);
FFACE_API BOOL SetTarget(void* Instance, int index);

/*Inventory*/
FFACE_API char GetInventoryMax(void* Instance) ;
FFACE_API char GetSafeMax(void* Instance);
FFACE_API char GetStorageMax(void* Instance);
FFACE_API char GetTempMax(void* Instance);
FFACE_API char GetLockerMax(void* Instance);
FFACE_API char GetSatchelMax(void* Instance);
FFACE_API char GetSackMax(void* Instance);
FFACE_API INVENTORYITEM GetInventoryItem(void* Instance,  int index);
FFACE_API INVENTORYITEM GetSafeItem(void* Instance, int index);
FFACE_API INVENTORYITEM GetStorageItem(void* Instance, int index);
FFACE_API INVENTORYITEM GetTempItem(void* Instance, int index);
FFACE_API INVENTORYITEM GetLockerItem(void* Instance, int index);
FFACE_API INVENTORYITEM GetSatchelItem(void* Instance, int index);
FFACE_API INVENTORYITEM GetSackItem(void* Instance, int index);
FFACE_API void GetSelectedItemName(void* Instance, void *buffer, int* maxlength);
FFACE_API int GetSelectedItemNum(void* Instance);
FFACE_API char GetSelectedItemIndex(void* Instance);
FFACE_API char GetEquippedItemIndex(void* Instance, char slot);
FFACE_API TREASUREITEM GetTreasureItem(void* Instance, char index);

/*Fishing*/
FFACE_API BOOL FishOnLine(void* Instance);
FFACE_API char GetRodPosition(void* Instance);
FFACE_API int GetFishHPMax(void* Instance);
FFACE_API int GetFishHP(void* Instance);
FFACE_API short GetFishOnlineTime(void* Instance);
FFACE_API short GetFishTimeout(void* Instance);
FFACE_API int GetFishID1(void* Instance);
FFACE_API int GetFishID2(void* Instance);
FFACE_API int GetFishID3(void* Instance);
FFACE_API int GetFishID4(void* Instance);

/*MENU*/
FFACE_API short GetDialogID(void* Instance);
FFACE_API int GetDialogIndexCount(void* Instance);
FFACE_API short GetDialogIndex(void* Instance);
FFACE_API void GetDialogStrings(void* Instance, void* buffer, int* size);
FFACE_API BOOL MenuIsOpen(void* Instance);
FFACE_API void MenuName(void* Instance, void* buffer, int* size);
FFACE_API void MenuSelection(void* Instance, void* buffer, int* size);
FFACE_API void MenuHelp(void* Instance, void* buffer, int* size);
FFACE_API char ShopQuantityMax(void* Instance);
FFACE_API void GetNPCTradeInfo(void* Instance, TRADEINFO* TI);
FFACE_API BOOL IsSynthesis(void* Instance);
/*MENU*/
/*SEARCH*/
FFACE_API int GetSearchTotalCount(void* Instance);
FFACE_API char GetSearchPageCount(void* Instance);
FFACE_API char GetSearchZone(void* Instance, short index);
FFACE_API char GetSearchMainJob(void* Instance, short index);
FFACE_API char GetSearchSubJob(void* Instance, short index);
FFACE_API char GetSearchMainlvl(void* Instance, short index);
FFACE_API char GetSearchSublvl(void* Instance, short index);
FFACE_API void GetSearchName(void* Instance, short index, void* Buffer, int* BufferSize);
/*SEARCH*/
#endif // FFACE_H