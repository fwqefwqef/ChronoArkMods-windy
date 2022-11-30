using BepInEx;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using I2.Loc;

namespace LegendaryItemsPatch
{
    [BepInPlugin(GUID, "Fox Orb Genderlock Patch", version)]
    [BepInProcess("ChronoArk.exe")]
    public class LegendadryItemsPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.chronoark.itemmod.foxorbgenderlock";
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

        // Everyone is female now, seethe and cope
        [HarmonyPatch(typeof(Misc), "IsFemale")]
        class Foxorb_GenderLock_Patch
        {
            public static bool Prefix(ref bool __result)
            {
                __result = true;
                return false;
            }
        }

    }
}
