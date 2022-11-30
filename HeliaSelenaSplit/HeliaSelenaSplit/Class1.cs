using BepInEx;
using BepInEx.Configuration;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using TileTypes;
using System.Reflection.Emit;
using System.Reflection;
using I2.Loc;
using Random = System.Random;
using System.Collections;

namespace ExpertPlusMod
{
    [BepInPlugin(GUID, "HeliaSelenaSplit", version)]
    [BepInProcess("ChronoArk.exe")]
    public class ExpertPlusPlugin : BaseUnityPlugin
    {
        public const string GUID = "windy.heliaselena";
        public const string version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        [HarmonyPatch(typeof(StartPartySelect), "Select")]
        class SelectPatch
        {
            static bool Prefix(StartPartySelect __instance)
            {
                    MasterAudio.PlaySound("SE_ClickButton", 1f, null, 0f, null, null, false, false);
                    for (int i = __instance.Locked; i < __instance.Selected.Length; i++)
                    {
                        if (__instance.Selected[i].CharacterNum == -1)
                        {
                            __instance.Selected[i].Input(__instance.NowSelectedVIew);
                            break;
                        }
                    }
                    __instance.CBListGrayOn();
                    return false;
                }
        }

        [HarmonyPatch(typeof(SR_Solo), "PickSetting")]
        class Picksetting
        {
            static bool Prefix(StartPartySelect __instance, ref PickSetting __result)
            {
                __result = new PickSetting
                {
                    BanCharacter =
                    {
                        GDEItemKeys.Character_Phoenix
                    },
                    MaxParty = 1
                };

                return false;
            }
        }

 
    }
}