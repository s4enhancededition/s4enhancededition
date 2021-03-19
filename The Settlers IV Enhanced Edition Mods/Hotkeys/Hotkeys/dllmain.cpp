// dllmain.cpp : Definiert den Einstiegspunkt für die DLL-Anwendung.
#include "pch.h"
#include "S4ModApi.h"
#include <string>     // std::string, std::to_string
#pragma comment(lib, "S4ModApi")
int keys[] = { 'U','I','J','K','N',VK_OEM_COMMA,'V','B','O','L', VK_OEM_PERIOD, VK_OEM_1, VK_OEM_MINUS };

static S4API s4; // the interface to the Settlers 4 Mod API

extern "C" __declspec(dllexport) HWND __stdcall GetHWND() {
	return s4->GetHwnd();
}
static HWND S4HWND;
bool notpressed = true;
int RR_S4_OBJECT_BUILDING_EYECATCHER = 1;


static void CleanUp() {
	if (s4 != NULL) {
		s4->Release();
		s4 = NULL;
	}
};

HRESULT S4HCALL S4TickProc(DWORD dwTick, BOOL bHasEvent, BOOL bIsDelayed);


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH: {
		s4 = S4ApiCreate(); // get an interface to the mod api
		if (NULL == s4) break;

		s4->AddTickListener(S4TickProc);
		S4HWND = GetHWND();
		break;
	}
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		CleanUp();
		break;
	}
	return TRUE;
}

inline S4_OBJECT_TYPE DecideOnCurrentMenu(S4_GUI_ENUM menu, S4_OBJECT_TYPE found, S4_OBJECT_TYPE metal, S4_OBJECT_TYPE food, S4_OBJECT_TYPE misc, S4_OBJECT_TYPE military) {
	switch (menu)
	{
	case S4_MENU_BUILDINGS_FOUNDATION:
		return found;
	case S4_MENU_BUILDINGS_METAL:
		return metal;
	case S4_MENU_BUILDINGS_FOOD:
		return food;
	case S4_MENU_BUILDINGS_MISC:
		return misc;
	case S4_MENU_BUILDINGS_MILITARY:
		return military;
	}

	return S4_OBJECT_EMPTY;
}

S4_OBJECT_TYPE SetBuilding(int vKey, S4_GUI_ENUM menu) {
	S4_OBJECT_TYPE playerId = s4->GetPlayerTribe();

	if (playerId == S4_OBJECT_TRIBE_DARK) {
		switch (vKey)
		{
		case('U'):
			return DecideOnCurrentMenu(menu,
				S4_OBJECT_BUILDING_WOODCUTTERHUT,
				S4_OBJECT_EMPTY,
				S4_OBJECT_EMPTY,
				S4_OBJECT_EMPTY,
				S4_OBJECT_BUILDING_BARRACKS);
		case('I'):
			return DecideOnCurrentMenu(menu,
				S4_OBJECT_BUILDING_STONECUTTERHUT,
				S4_OBJECT_EMPTY,
				S4_OBJECT_EMPTY,
				S4_OBJECT_EMPTY,
				S4_OBJECT_EMPTY);
		}

		return S4_OBJECT_EMPTY;
	}

	switch (vKey)
	{
	case('U'):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_WOODCUTTERHUT,
			S4_OBJECT_BUILDING_COALMINE,
			S4_OBJECT_BUILDING_GRAINFARM,
			S4_OBJECT_BUILDING_STORAGEAREA,
			S4_OBJECT_BUILDING_BARRACKS);
	case('I'):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_STONECUTTERHUT,
			S4_OBJECT_BUILDING_IRONMINE,
			S4_OBJECT_BUILDING_ANIMALRANCH,
			S4_OBJECT_BUILDING_MARKETPLACE,
			S4_OBJECT_BUILDING_GUARDTOWERSMALL);
	case('J'):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_SAWMILL,
			S4_OBJECT_BUILDING_GOLDMINE,
			S4_OBJECT_BUILDING_MILL,
			S4_OBJECT_BUILDING_DONKEYRANCH,
			S4_OBJECT_BUILDING_GUARDTOWERBIG);
	case('K'):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_FORESTERHUT,
			S4_OBJECT_BUILDING_SULFURMINE,
			S4_OBJECT_BUILDING_BAKERY,
			S4_OBJECT_BUILDING_PORT,
			S4_OBJECT_BUILDING_LOOKOUTTOWER);
	case('N'):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_RESIDENCESMALL,
			S4_OBJECT_BUILDING_SMELTIRON,
			S4_OBJECT_BUILDING_SLAUGHTERHOUSE,
			S4_OBJECT_BUILDING_SHIPYARD,
			S4_OBJECT_BUILDING_CASTLE);
	case(VK_OEM_COMMA):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_RESIDENCEMEDIUM,
			S4_OBJECT_BUILDING_SMELTGOLD,
			S4_OBJECT_BUILDING_FISHERHUT,
			S4_OBJECT_BUILDING_BIGTEMPLE,
			S4_OBJECT_BUILDING_AMMOMAKERHUT);
	case('V'):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_RESIDENCEBIG,
			S4_OBJECT_BUILDING_TOOLSMITH,
			S4_OBJECT_BUILDING_HUNTERHUT,
			S4_OBJECT_BUILDING_SMALLTEMPLE,
			S4_OBJECT_BUILDING_HEALERHUT);
	case('B'):
		return DecideOnCurrentMenu(menu,
			S4_OBJECT_BUILDING_STONEMINE,
			S4_OBJECT_BUILDING_WEAPONSMITH,
			S4_OBJECT_BUILDING_WATERWORKHUT,
			S4_OBJECT_EMPTY, //unused
			S4_OBJECT_BUILDING_VEHICLEHALL);
	case('O'):
		if (menu == S4_MENU_BUILDINGS_FOOD) {
			switch (playerId) {
			case(S4_OBJECT_TYPE::S4_OBJECT_TRIBE_ROMAN): //romans
				return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_VINYARD;
			case(S4_OBJECT_TRIBE_VIKING): //wikings
				return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_BEEKEEPERHUT;
			case(S4_OBJECT_TRIBE_MAYA): //maya
				return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_AGAVEFARMERHUT;
			case(S4_OBJECT_TRIBE_TROJAN): //trojan
				return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_SUNFLOWERFARMERHUT;
			}
		}
		break;
	case('L'):
		if (menu == S4_MENU_BUILDINGS_FOOD) {
			switch (playerId) {
			case(S4_OBJECT_TYPE::S4_OBJECT_TRIBE_ROMAN): //romans
				break;//return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_WIN;
			case(S4_OBJECT_TRIBE_VIKING): //wikings
				return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_MEADMAKERHUT;
			case(S4_OBJECT_TRIBE_MAYA): //maya
				return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_TEQUILAMAKERHUT;
			case(S4_OBJECT_TRIBE_TROJAN): //trojan
				return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_SUNFLOWEROILMAKERHUT;
			}
		}
		break;
	case(VK_OEM_PERIOD):
	case(VK_OEM_1):
	case(VK_OEM_MINUS):
		switch (RR_S4_OBJECT_BUILDING_EYECATCHER) {
		default:
		case(1):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER01;
			break;
		case(2):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER02;
			break;
		case(3):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER03;
			break;
		case(4):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER04;
			break;
		case(5):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER05;
			break;
		case(6):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER06;
			break;
		case(7):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER07;
			break;
		case(8):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER08;
			break;
		case(9):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER09;
			break;
		case(10):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER10;
			break;
		case(11):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER11;
			break;
		case(12):
			return S4_OBJECT_TYPE::S4_OBJECT_BUILDING_EYECATCHER12;
			break;
		}
		break;
	}
	return S4_OBJECT_EMPTY;
}

bool CheckActiveWindowTitle()
{
	HWND handle = GetForegroundWindow();
	char name[15];
	char name2[16];
	GetWindowTextA(handle, name, 15);
	GetWindowTextA(handle, name2, 16);
	if (strcmp(name, "Die Siedler IV") == 0)
	{
		return true;
	}
	else if (strcmp(name2, "The Settlers IV") == 0)
	{
		return true;
	}
	else
	{
		return false;
	}
}

bool StartBuildMode(DWORD currentTick) {
	if (!CheckActiveWindowTitle())
	{
		//s4->ShowTextMessage("kein Fokus", 8, 0);
		return false;
	}
	/*else
	{
		s4->ShowTextMessage("Fokus", 8, 0);
	}*/
	S4_OBJECT_TYPE building;
	bool picked = false;

	S4_GUI_ENUM menu = S4_GUI_ENUM::S4_GUI_UNKNOWN;
	if (s4->IsCurrentlyOnScreen(S4_GUI_ENUM::S4_MENU_BUILDINGS_FOUNDATION)) {
		menu = S4_MENU_BUILDINGS_FOUNDATION;
	}
	else if (s4->IsCurrentlyOnScreen(S4_GUI_ENUM::S4_MENU_BUILDINGS_METAL)) {
		menu = S4_MENU_BUILDINGS_METAL;
	}
	else if (s4->IsCurrentlyOnScreen(S4_GUI_ENUM::S4_MENU_BUILDINGS_FOOD)) {
		menu = S4_MENU_BUILDINGS_FOOD;
	}
	else if (s4->IsCurrentlyOnScreen(S4_GUI_ENUM::S4_MENU_BUILDINGS_MISC)) {
		menu = S4_MENU_BUILDINGS_MISC;
	}
	else if (s4->IsCurrentlyOnScreen(S4_GUI_ENUM::S4_MENU_BUILDINGS_MILITARY)) {
		menu = S4_MENU_BUILDINGS_MILITARY;
	}

	if (menu == S4_GUI_UNKNOWN)
		return false;

	bool EYECATCHER = false;
	bool decrement = false;

	for (int k = 0; k < 13; k++) {
		if (GetAsyncKeyState(keys[k])) {
			if (k == 10)
			{
				EYECATCHER = true;
			}
			if (k == 11 || k == 12)
			{
				EYECATCHER = true;
				decrement = true;
			}
			building = SetBuilding(keys[k], menu);
			picked = building != S4_OBJECT_EMPTY;
			//s4->StartBuildingPlacement();
		}
	}

	if (picked)
	{
		if (notpressed)
		{
			if (EYECATCHER)
			{
				if (!decrement)
				{

					if (RR_S4_OBJECT_BUILDING_EYECATCHER != 12)
					{
						RR_S4_OBJECT_BUILDING_EYECATCHER++;
					}
					else
					{
						RR_S4_OBJECT_BUILDING_EYECATCHER = 1;
					}
				}
				else
				{
					if (RR_S4_OBJECT_BUILDING_EYECATCHER != 1)
					{
						RR_S4_OBJECT_BUILDING_EYECATCHER--;
					}
					else
					{
						RR_S4_OBJECT_BUILDING_EYECATCHER = 12;
					}
				}

			}
			s4->StartBuildingPlacement(building);
		}
		notpressed = false;
	}
	else
	{
		notpressed = true;
	}

	//if ((picked == true && last_building != building) || (picked == true && )) {
	//	s4->StartBuildingPlacement(building);
	//	buildinglast = building;
	//	last_building = currentTick;
	//	return true;
	//}
	return false;
}


HRESULT S4HCALL S4TickProc(DWORD dwTick, BOOL bHasEvent, BOOL bIsDelayed) {
	if (bIsDelayed) return 0;
	StartBuildMode(dwTick);
	return 0;
}


/*
QWERTZ [UIO] P
ASDF [JKL]
 YXC [VBN] M
*/