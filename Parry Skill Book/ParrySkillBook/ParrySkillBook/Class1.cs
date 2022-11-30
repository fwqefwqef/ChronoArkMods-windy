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

namespace ExpertPlusMod
{
    [BepInPlugin(GUID, "Parry Skill Book Mod", version)]
    [BepInProcess("ChronoArk.exe")]
    public class ExpertPlusPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.chronoark.skillbookmod.lianparry";
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

        // Lian Parry: Dialogue always active
        [HarmonyPatch(typeof(Ark_Lian))]
        class ArkLian_Patch
        {
            [HarmonyPatch(nameof(Ark_Lian.Active))]
            [HarmonyPrefix]
            static bool Prefix(Ark_Lian __instance)
            {

                if (!SaveManager.NowData.LianTutorialBook)
                {
                    __instance.ArkTutorial.Activate();
                }
                else
                {
                    __instance.ArkTutorialAfter.Activate();
                }
                return false;
            }

            [HarmonyPatch(nameof(Ark_Lian.SkillBook))]
            [HarmonyPrefix]
            static bool Prefix2(Ark_Lian __instance)
            {
                SaveManager.NowData.LianTutorialBook = true;
                return false;
            }

            [HarmonyPatch(nameof(Ark_Lian.NeedCredit))]
            [HarmonyPrefix]
            static bool Prefix3(Ark_Lian __instance)
            {
                return false;
            }
        }


    }

}


