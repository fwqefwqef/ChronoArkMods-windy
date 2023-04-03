using BepInEx;
using BepInEx.Configuration;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using TileTypes;
using System.Reflection.Emit;
using System.Reflection;
using I2.Loc;
using DarkTonic.MasterAudio;

namespace DamageModifier
{
    [BepInPlugin(GUID, "Damage Patch", version)]
    [BepInProcess("ChronoArk.exe")]
    public class RareSkillsPlugin : BaseUnityPlugin
    {
        public const string GUID = "windy.damagemod";
        public const string version = "1.0.0";
        private static readonly Harmony harmony = new Harmony(GUID);

        public static ConfigEntry<double> enemyMult;
        public static ConfigEntry<double> playerMult;

        void Awake()
        {
            enemyMult = Config.Bind("Generation config", "Enemy Damage Multiplier", 1.5, "Multiplies enemy damage by this amount.");
            playerMult = Config.Bind("Generation config", "Player Damage Multiplier", 1.5, "Multiplies player damage by this amount.");
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        // Modify gdata.json
        [HarmonyPatch(typeof(GDEDataManager), nameof(GDEDataManager.InitFromText))]
        class ModifyGData
        {
            static void Prefix(ref string dataString)
            {
                Dictionary<string, object> masterJson = (Json.Deserialize(dataString) as Dictionary<string, object>);
                foreach (var e in masterJson)
                {
                    if (((Dictionary<string, object>)e.Value).ContainsKey("_gdeSchema"))
                    {
                        if (((Dictionary<string, object>)e.Value)["_gdeSchema"].Equals("Enemy"))
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["atk"] = (long)((long)(masterJson[e.Key] as Dictionary<string, object>)["atk"] * enemyMult.Value);
                        }

                        else if (((Dictionary<string, object>)e.Value)["_gdeSchema"].Equals("Character"))
                        {
                            ((masterJson[e.Key] as Dictionary<string, object>)["ATK"] as Dictionary<string, object>)["x"] = (long)((long)((masterJson[e.Key] as Dictionary<string, object>)["ATK"] as Dictionary<string, object>)["x"] * playerMult.Value);
                            ((masterJson[e.Key] as Dictionary<string, object>)["ATK"] as Dictionary<string, object>)["y"] = (long)((long)((masterJson[e.Key] as Dictionary<string, object>)["ATK"] as Dictionary<string, object>)["y"] * playerMult.Value);
                        }
                    }
                }
                dataString = Json.Serialize(masterJson);
            }
        }

    }
}