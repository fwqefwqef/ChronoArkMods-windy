using BepInEx;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Alternative_ShadowCurtain
{
    [BepInPlugin(GUID, "Remove Fix Limit", version)]
    [BepInProcess("ChronoArk.exe")]
    public class RareSkillsPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.chronoark.cardmod.removefixlimit";
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

        [HarmonyPatch(typeof(GDEDataManager), nameof(GDEDataManager.InitFromText))]
        // modify gdata json
        class ModifyGData
        {
            static void Prefix(ref string dataString)
            {
                Dictionary<string, object> masterJson = (Json.Deserialize(dataString) as Dictionary<string, object>);
                foreach (var e in masterJson)
                {
                    //Debug.Log(e);
                    if (((Dictionary<string, object>)e.Value).ContainsKey("NoBasicSkill")) {
                        (masterJson[e.Key] as Dictionary<string, object>)["NoBasicSkill"] = "false";
                    }
                }
                dataString = Json.Serialize(masterJson);
            }
        }

        // add starting items
        /*[HarmonyPatch(typeof(FieldSystem))]
        class FieldSystem_Patch
        {
            [HarmonyPatch(nameof(FieldSystem.StageStart))]
            [HarmonyPrefix]
            static void StageStartPrefix()
            {
                // copied from FieldSystem.StageStart
                if (PlayData.TSavedata.StageNum == 0 && !PlayData.TSavedata.GameStarted)
                {
                    // identifies transfer scroll
                    if (PlayData.TSavedata.IdentifyItems.Find((string x) => x == GDEItemKeys.Item_Scroll_Scroll_Transfer) == null)
                    {
                        PlayData.TSavedata.IdentifyItems.Add(GDEItemKeys.Item_Scroll_Scroll_Transfer);
                    }
                    PartyInventory.InvenM.AddNewItem(ItemBase.GetItem(GDEItemKeys.Item_Consume_SkillBookCharacter_Rare, 10));
                    PartyInventory.InvenM.AddNewItem(ItemBase.GetItem(GDEItemKeys.Item_Passive_505Error, 1));
                    PartyInventory.InvenM.AddNewItem(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Transfer, 1));
                }
            }
        }*/
    }
}

