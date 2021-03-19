/**
The MIT License (MIT)
Copyright © 2021 nyfrk@gmx.net

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the “Software”), to deal in 
the Software without restriction, including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.

**/


#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <hlib.h>

using namespace hlib;
using namespace std;

// Assigning a carry job to a carrier for execution
#define PATTERN_CARRIER_ASSIGNMENT_TASK_GE  "EB 23 83 F8 0F 75 1E B9"          
#define PATTERN_CARRIER_ASSIGNMENT_TASK_HE  "83 FE 0F 75 74 A1 ? ? ? ? 3D FE 00 00 00" 

//// CBuildingSiteRole Vfunc3 (building site role is creating a carry job)
//#define PATTERN_CARRIER_JOBCREATION_SITE_DELAY_GE "6A 01 56 FF 50 58 6A 0F 8B CE" // at +7
//#define PATTERN_CARRIER_JOBCREATION_SITE_DELAY_HE "FF 50 58 0F B7 43 08 50 6A 0F E8" // at 9
//
// CDeliveryPileRole Vfunc10 (delivery pile role is creating a carry job)
#define PATTERN_CARRIER_JOBCREATION_PROD_DELAY_GE "FF 50 3C 50 57 8B CB E8 ? ? ? ? 5F 5B 6A 1f 8B CE E8 ? ? ? ? 5E C2 04 00" // at +15
#define PATTERN_CARRIER_JOBCREATION_PROD_DELAY_HE "5E 0F B7 43 08 50 6A 1F E8 ? ? ? ? 89 43 30" // at +7


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {

	if (ul_reason_for_call != DLL_PROCESS_ATTACH && 
		ul_reason_for_call != DLL_PROCESS_DETACH) {
		return TRUE;
	}

	const HANDLE hProcess = GetCurrentProcess();
	
	static struct {
		unsigned
			isCarrierAssignmentTaskPatch : 1,
			isCarrierProdJobCreationPatch : 1,
			isCarrierSiteJobCreationPatch : 1;
	} AppliedPatches = { 0 };

	static Patch 
		CarrierAssignmentTaskPatch,
		CarrierProdJobCreationPatch,
		CarrierSiteJobCreationPatch;


    switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH: {
		DWORD S4_Main = (DWORD)GetModuleHandleA(NULL);
		const bool isGE = GetModuleHandleA("GfxEngine.dll"); // if this module is present, we assume gold edition

		const DWORD CarrierAssignmentTaskPattern = (DWORD)FindPattern(hProcess, S4_Main,
			StringPattern(isGE ? PATTERN_CARRIER_ASSIGNMENT_TASK_GE : PATTERN_CARRIER_ASSIGNMENT_TASK_HE));
		const DWORD CarrierProdJobCreationPattern = (DWORD)FindPattern(hProcess, S4_Main,
			StringPattern(isGE ? PATTERN_CARRIER_JOBCREATION_PROD_DELAY_GE : PATTERN_CARRIER_JOBCREATION_PROD_DELAY_HE));
		//const DWORD CarrierSiteJobCreationPattern = (DWORD)FindPattern(hProcess, S4_Main,
		//	StringPattern(isGE ? PATTERN_CARRIER_JOBCREATION_SITE_DELAY_GE : PATTERN_CARRIER_JOBCREATION_SITE_DELAY_HE));

		if (!CarrierAssignmentTaskPattern 
			/*|| !CarrierProdJobCreationPattern || 
			!CarrierSiteJobCreationPattern */) {
			MessageBoxW(NULL,
				L"Sorry, your game version is not supported by the CoalBug plugin. "
				"Please open a ticket here: https://github.com/nyfrk/Settlers4-Coalfix/issues\n\n"
				"Entschuldige, deine Spielversion wird nicht vom Coalbug Plugin unterstützt. "
				"Bitte öffne ein Ticket hier: https://github.com/nyfrk/Settlers4-Coalfix/issues",
				L"S4 CoalBug", MB_ICONWARNING | MB_OK);
		}
		else {
			static const BYTE CarrierAssignmentTaskExpectedPatternGE[] = { 0x83, 0xF8, 0x0F, 0x75, 0x1E };
			static const BYTE CarrierAssignmentTaskExpectedPatternHE[] = { 0x83, 0xFE, 0x0F, 0x75, 0x74 };
			static const BYTE nop5[] = { 0x0F, 0x1F, 0x44, 0x00, 0x00 };
			BYTE const * const  CarrierAssignmentTaskExpectedPattern = isGE ? CarrierAssignmentTaskExpectedPatternGE : CarrierAssignmentTaskExpectedPatternHE;
			
			CarrierAssignmentTaskPatch = Patch(CarrierAssignmentTaskPattern + (isGE ? 2 : 0), nop5, CarrierAssignmentTaskExpectedPattern, sizeof(nop5));
			CarrierProdJobCreationPatch = Patch((DWORD)CarrierProdJobCreationPattern + (isGE ? 15 : 7), (BYTE)1, (BYTE)31);
			/*CarrierSiteJobCreationPatch = Patch((DWORD)CarrierSiteJobCreationPattern + (isGE ? 7 : 9), (BYTE)1, (BYTE)15);*/

			AppliedPatches.isCarrierAssignmentTaskPatch = true;
			AppliedPatches.isCarrierProdJobCreationPatch = true;
			//AppliedPatches.isCarrierSiteJobCreationPatch = true;

			CarrierAssignmentTaskPatch.patch(hProcess);
			CarrierProdJobCreationPatch.patch(hProcess);
			//CarrierSiteJobCreationPatch.patch(hProcess);
		}
		break;
	}
    case DLL_PROCESS_DETACH:
		if (AppliedPatches.isCarrierAssignmentTaskPatch) CarrierAssignmentTaskPatch.unpatch(hProcess);
		if (AppliedPatches.isCarrierProdJobCreationPatch) CarrierProdJobCreationPatch.unpatch(hProcess);
		//if (AppliedPatches.isCarrierSiteJobCreationPatch) CarrierSiteJobCreationPatch.unpatch(hProcess);
		
        break;
    }
    return TRUE;
}

