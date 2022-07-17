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

        private static ConfigEntry<bool> Campfire;

        void Awake()
        {
            Campfire = Config.Bind("Generation config", "Campfire Recruit", false, "Despair Mode\nCampfires can no longer revive allies. All enemies Atk+1 Accuracy+5%. Golden Apples cannot be used in battle but can revive allies. (true/false)");
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        //Modify gdata.json

       [HarmonyPatch(typeof(GDEDataManager), nameof(GDEDataManager.InitFromText))]
        class ModifyGData
        {
            static void Prefix(ref string dataString)
            {
                Dictionary<string, object> masterJson = (Json.Deserialize(dataString) as Dictionary<string, object>);
                foreach (var e in masterJson)
                {
                    //Debug.Log(e);

                    if (((Dictionary<string, object>)e.Value).ContainsKey("_gdeSchema"))
                    {
                        ((GDEDataManager.masterData["TW_Red"] as Dictionary<string, object>)["REG"] as Dictionary<string, object>)["x"] = 10;
                        ((GDEDataManager.masterData["TW_Red"] as Dictionary<string, object>)["REG"] as Dictionary<string, object>)["y"] = 22;

                        ((GDEDataManager.masterData["TW_Blue"] as Dictionary<string, object>)["ATK"] as Dictionary<string, object>)["x"] = 14;
                        ((GDEDataManager.masterData["TW_Blue"] as Dictionary<string, object>)["ATK"] as Dictionary<string, object>)["y"] = 26;

                        if (e.Key == "S_TW_Blue_6")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "TW_Red";
                        }
                        if (e.Key == "S_TW_Blue_0")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "TW_Red";
                        }
                        if (e.Key == "S_TW_Blue_8")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "TW_Red";
                        }
                        if (e.Key == "S_TW_Blue_3")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "TW_Red";
                        }


                        if (e.Key == "S_TW_Red_7")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "TW_Blue";
                        }
                        if (e.Key == "S_TW_Red_4")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "TW_Blue";
                        }
                        if (e.Key == "S_TW_Red_3")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "TW_Blue";
                        }


                        if (e.Key == "S_TW_Red_2")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "";
                        }
                        if (e.Key == "S_TW_Red_R0")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "";
                        }
                        if (e.Key == "S_TW_Blue_R0")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["User"] = "";
                        }

                    }
                }
                dataString = Json.Serialize(masterJson);
            }
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

        // Not a good injection point but I needed to put this here in case the player loads from save.
        // StartPartySelect.Init is an alternative, but doesn't cover loading from save

        [HarmonyPatch(typeof(StageSystem), nameof(StageSystem.StageStart))]
        class AddSkillsPatch2
        {
            static void Postfix()
            {
                Debug.Log("StageStart");

                // Helia

                PlayData.ALLSKILLLIST.Find(sd => sd.KeyID == GDEItemKeys.Skill_S_TW_Blue_6).User = "TW_Red";
                PlayData.ALLSKILLLIST.Find(sd => sd.KeyID == GDEItemKeys.Skill_S_TW_Blue_0).User = "TW_Red";
                PlayData.ALLSKILLLIST.Find(sd => sd.KeyID == GDEItemKeys.Skill_S_TW_Blue_8).User = "TW_Red";
                PlayData.ALLSKILLLIST.Find(sd => sd.KeyID == GDEItemKeys.Skill_S_TW_Blue_3).User = "TW_Red";


                // Selena

                PlayData.ALLSKILLLIST.Find(sd => sd.KeyID == GDEItemKeys.Skill_S_TW_Red_7).User = "TW_Blue";
                PlayData.ALLSKILLLIST.Find(sd => sd.KeyID == GDEItemKeys.Skill_S_TW_Red_4).User = "TW_Blue";
                PlayData.ALLSKILLLIST.Find(sd => sd.KeyID == GDEItemKeys.Skill_S_TW_Red_3).User = "TW_Blue";

                // Remove
                foreach (GDESkillData s in PlayData.ALLSKILLLIST)
                {
                    if (s.KeyID == GDEItemKeys.Skill_S_TW_Red_2)
                    {
                        s.User = "";
                        //Debug.Log("Here");
                    }
                }

                foreach (GDESkillData s in PlayData.ALLRARESKILLLIST)
                {
                    if (s.KeyID == GDEItemKeys.Skill_S_TW_Red_R0 || s.KeyID == GDEItemKeys.Skill_S_TW_Blue_R0)
                    {
                        s.User = "";
                        //Debug.Log("Here");
                    }
                }
            }
        }

    }
}