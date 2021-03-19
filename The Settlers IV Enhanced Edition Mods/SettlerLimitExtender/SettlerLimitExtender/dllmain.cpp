///////////////////////////////////////////////////////////////////////////////
// MIT License
//
// Copyright (c) 2020 nyfrk <nyfrk@gmx.net>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
///////////////////////////////////////////////////////////////////////////////

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <hlib.h>
using namespace hlib;

#define PATTERN_SETTLERS_LIMIT "83 ? 04 7F 07 B8 C4 09 00 00 EB 08 B8 10 27 00 00 99 F7"
#define PATTERN_CARRIER_LIMIT_GE          "81 FE E7 03 00 00 7C 0B 66 C7 47 20 E7 03"
#define PATTERN_CARRIER_LIMIT_HE "C2 08 00 81 FF E7 03 00 00 7C 0F B8 E7 03 00 00"

BOOL APIENTRY DllMain( HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	static Patch UnlimitedSettlersPatches[4];
	static HANDLE hProcess = GetCurrentProcess();

	switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH: {
			const DWORD S4_Main = (DWORD)GetModuleHandleA(NULL);
			const DWORD UnlimitedSettlersPattern = (DWORD)FindPattern(hProcess, S4_Main,
				StringPattern(PATTERN_SETTLERS_LIMIT));
			const bool isGE = GetModuleHandleA("GfxEngine.dll"); // if this module is present, we assume gold edition
			const DWORD CarrierLimitPattern = (DWORD)FindPattern(hProcess, S4_Main,
				StringPattern(isGE ? PATTERN_CARRIER_LIMIT_GE : PATTERN_CARRIER_LIMIT_HE));

			if (!UnlimitedSettlersPattern || !CarrierLimitPattern) {
				MessageBox(NULL, "Sorry, your game version is not supported by the S4 Settler Limit Remover plugin. Please open a ticket here: https://github.com/nyfrk/S4_SettlerLimitRemover/issues", "S4 Unlimited Settlers Plugin", MB_ICONWARNING | MB_OK);
			}
			else {
				//Lokal Limit (8 Player)
				UnlimitedSettlersPatches[0] = Patch(UnlimitedSettlersPattern + 2, (BYTE)0x08, (BYTE)0x04);
				//Lokal Limit (4000)
				UnlimitedSettlersPatches[1] = Patch(UnlimitedSettlersPattern + 6, (BYTE)0xA0, (BYTE)0xC4);
				UnlimitedSettlersPatches[2] = Patch(UnlimitedSettlersPattern + 7, (BYTE)0x0F, (BYTE)0x09);
				//Max Limit (32000)
				UnlimitedSettlersPatches[3] = Patch(UnlimitedSettlersPattern + 13, (DWORD)32000, (DWORD)10000);
				const DWORD carrierLimitExpected = 999, carrierLimitNew = 0x7FFF;
				UnlimitedSettlersPatches[4] = Patch(CarrierLimitPattern + (isGE ? 2 : 5), carrierLimitNew, carrierLimitExpected);
				UnlimitedSettlersPatches[5] = Patch(CarrierLimitPattern + 12, &carrierLimitNew, &carrierLimitExpected, isGE ? sizeof(WORD) : sizeof(DWORD));
				for (auto& p : UnlimitedSettlersPatches) p.patch(hProcess);
			}
			break;
		}
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
			break;
		case DLL_PROCESS_DETACH: {
			for (auto& p : UnlimitedSettlersPatches) p.unpatch(hProcess);
			break;
		}
    }
    return TRUE;
}
