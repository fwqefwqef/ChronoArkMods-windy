﻿using BepInEx;
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
    [BepInPlugin(GUID, "Expert Plus Mod", version)]
    [BepInProcess("ChronoArk.exe")]
    public class ExpertPlusPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.chronoark.difficultymod.expertplus";
        public const string version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(GUID);

        private static ConfigEntry<bool> DespairMode;
        private static ConfigEntry<bool> VanillaCurses;

        void Awake()
        {
            DespairMode = Config.Bind("Generation config", "Despair Mode", false, "A very difficult gameplay mode. Campfires can no longer revive allies. Golden Apples cannot be used in battle but can revive allies. (true/false)");
            VanillaCurses = Config.Bind("Generation config", "Vanilla Curses", false, "Reverts the nerfs to Cursed Mob stats. The challenge is designed around weaker cursed mobs, but if you don't want that, toggle this on. (true/false)");
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
                    
                    /// Despair Mode ///
                    
                    if (((Dictionary<string, object>)e.Value).ContainsKey("_gdeSchema"))
                    {
                        //Despair Mode: Give Stat Bonuses
                        if (((Dictionary<string, object>)e.Value)["_gdeSchema"].Equals("Enemy"))
                        {
                            if (DespairMode.Value)
                            {
                                (masterJson[e.Key] as Dictionary<string, object>)["atk"] = (long)((masterJson[e.Key] as Dictionary<string, object>)["atk"]) + 1;
                                (masterJson[e.Key] as Dictionary<string, object>)["hit"] = (long)((masterJson[e.Key] as Dictionary<string, object>)["hit"]) + 5;
                            }
                        }

                        // Despair Mode: Golden Apple can be used on fallen allies, cannot be used in battle
                        if (e.Key == "GoldenApple")
                        {
                            if (DespairMode.Value)
                            {
                                (masterJson[e.Key] as Dictionary<string, object>)["Target"] = "deathally";
                                (masterJson[e.Key] as Dictionary<string, object>)["active_battle"] = false;
                            }
                        }

                        /// Enemy Hordes ///

                        /// Sanctuary ///
                        
                        // Pikachu Living Armor Horde: Send Pikachu in Wave 2
                        if (e.Key == "FQ_6_5")
                        {
                            List<string> a = new List<string>();
                            a.Add("S4_AngryDochi");
                            (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                            //(masterJson[e.Key] as Dictionary<string, object>)["Wave3"] = a;

                            (masterJson[e.Key] as Dictionary<string, object>)["Lock"] = false;
                            (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = false;
                            (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 2;
                            //(masterJson[e.Key] as Dictionary<string, object>)["Wave3Turn"] = 3;
                        }

                        // 2 Hedgehog Fight: Add Pharos Mage on Wave 2
                        if (e.Key == "FQ_6_4")
                        {
                            List<string> a = new List<string>();
                            //a.Add("S2_PharosWitch");
                            a.Add("S2_Pharos_Mage");
                            (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;

                            (masterJson[e.Key] as Dictionary<string, object>)["Lock"] = false;
                            (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = false;
                            (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 2;
                        }

                        ///// Misty Garden 1 ///

                        //// 1 maid fight: Add another maid 
                        //if (e.Key == "FQ_1_2")
                        //{
                        //    List<string> a = new List<string>();
                        //    a.Add("S1_Maid");
                        //    a.Add("S1_Maid");
                        //    (masterJson[e.Key] as Dictionary<string, object>)["Enemys"] = a;
                        //}

                        ///// Misty Garden 2 ///

                        //// Carpenter Doll fight: add 1 Pharos Mage
                        //if (e.Key == "FQ_2_1")
                        //{
                        //    List<string> a = new List<string>();
                        //    a.Add("S1_CarpenterDoll");
                        //    a.Add("S1_Pharos_Mage");
                        //    (masterJson[e.Key] as Dictionary<string, object>)["Enemys"] = a;
                        //}

                        ///// Bloody Park 1 ///
                        
                        //// 2 Robot Hedgehogs + Horse: Spawn another Horse in Wave 2
                        //if (e.Key == "FQ_3_3")
                        //{
                        //    List<string> a = new List<string>();
                        //    a.Add("S2_Horse");
                        //    (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;

                        //    (masterJson[e.Key] as Dictionary<string, object>)["Lock"] = false;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = false;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 2;
                        //}
                    }
                }
                dataString = Json.Serialize(masterJson);
            }
        }

        // add starting items
        [HarmonyPatch(typeof(FieldSystem))]
        class FieldSystem_Patch
        {
            [HarmonyPatch(nameof(FieldSystem.StageStart))]
            [HarmonyPrefix]
            static void StageStartPrefix()
            {
                // copied from FieldSystem.StageStart
                if (PlayData.TSavedata.StageNum == 0 && !PlayData.TSavedata.GameStarted)
                {
                    // identifies lifting scroll
                    if (PlayData.TSavedata.IdentifyItems.Find((string x) => x == GDEItemKeys.Item_Scroll_Scroll_Uncurse) == null)
                    {
                        PlayData.TSavedata.IdentifyItems.Add(GDEItemKeys.Item_Scroll_Scroll_Uncurse);
                    }

                    // Add 1 lifting scroll
                    PartyInventory.InvenM.AddNewItem(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Uncurse, 1));

                }
            }
        }

        // Despair Mode: Golden Apple now revives ally
        [HarmonyPatch(typeof(UseItem.GoldenApple))]
        class GoldenApple_Patch
        {
            [HarmonyPatch(nameof(UseItem.GoldenApple.Use))]
            [HarmonyPrefix]
            static bool Prefix(Character CharInfo, UseItem.GoldenApple __instance)
            {
                // If Despair Mode, revive effect
                if (DespairMode.Value && CharInfo.Incapacitated)
                {
                    Debug.Log("Revived Using Golden Apple");
                    CharInfo.Incapacitated = false;
                    CharInfo.Hp = 0;
                }
                return true;
            }
        }

        //[HarmonyPatch]
        //class GoldenAppleDesc
        //{
        //    static MethodBase TargetMethod()
        //    {
        //        return AccessTools.PropertyGetter(typeof(ItemBase), "GetDescription");
        //    }
        //    static void Postfix(ref string __result, ItemBase __instance)
        //    {
        //        Debug.Log("Hereeee");
        //            if (DespairMode.Value)
        //            {
        //                if (__instance is UseItem.GoldenApple)
        //                {
        //                   __result = "Overheal an ally by 25 and cure faint status.\nCan also be used as an ingredient for reforging equipment.\nWhen used in a campfire: Increases Accuracy and Crit.Chance of all allies by 4%, including allies recruited in the future.";
        //                }
        //            }
        //    }   
        //}

        //[HarmonyPatch]
        //class RobustDesc
        //{
        //    static MethodBase TargetMethod()
        //    {
        //        return AccessTools.PropertyGetter(typeof(PassiveBase), "GetDescription");
        //    }
        //    static void Postfix(ref string __result, PassiveBase __instance)
        //    {
        //        if (!VanillaCurses.Value)
        //        {
        //            if (__instance is B_CursedMob_2)
        //            {
        //                __result = "Gain 1 action count\nBlock one incoming attack.";
        //            }
        //        }
        //    }
        //}

        // Change stats for cursed mob, change rewards
        [HarmonyPatch(typeof(B_CursedMob))]
        class Curse_Reward_Patch
        {
            [HarmonyPatch(nameof(B_CursedMob.Init))]
            [HarmonyPostfix]
            public static void Postfix(B_CursedMob __instance, List<ItemBase> ___Itemviews)
            {
                if (!VanillaCurses.Value)
                {
                    // HP increase dropped to 20%, remove debuff resistance buff
                    __instance.PlusPerStat.MaxHP = __instance.PlusPerStat.MaxHP - 35;
                    __instance.PlusStat.RES_CC = __instance.PlusStat.RES_CC - 15f;
                    __instance.PlusStat.RES_DEBUFF = __instance.PlusStat.RES_DEBUFF - 15f;
                    __instance.PlusStat.RES_DOT = __instance.PlusStat.RES_DOT - 15f;
                    //Remove extra action count but increase atk by +40%
                    //__instance.PlusPerStat.Damage = __instance.PlusPerStat.Damage + 40;
                    //__instance.BChar.Info.PlusActCount.Remove(1);
                }

                //Gold reward reduced to 0
                if (___Itemviews.RemoveAll(x => x.itemkey == GDEItemKeys.Item_Misc_Gold) > 0)
                {
                    //___Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, 50));
                }

                // Drop uncommons on Bloody Park 2 as well. Item reward reworked. 
                bool flag = false;
                if (__instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S2_Horse || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S2_Pharos_Mage || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S2_PharosWitch || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S3_Fugitive || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S3_Pharos_Assassin || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S3_Deathbringer || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S3_Pharos_Tanker || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S3_SnowGiant_0 || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S3_Pharos_HighPriest)
                {
                    flag = true;
                }
                if (flag)
                {
                    if (___Itemviews.RemoveAll(x => x.GetisEquip) > 0 || PlayData.TSavedata.StageNum == 3)
                    {
                        //ItemBase item = ItemBase.GetItem(PlayData.GetEquipRandom(1));
                        //(item as Item_Equip).Curse = EquipCurse.RandomCurse(item as Item_Equip);

                        Random rand = new Random();

                        int a = rand.Next(1,101); // 1-100
                        // 59% 100G, 25% Useful Scrap Metal, 5% Herb, 5% Tablet, 5% Shield Generator, 1% Celestial
                        if (a <= 59)
                        {
                            __instance.Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, 100));
                        }
                        if (a == 60)
                        {
                            __instance.Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_Celestial));
                        }
                        if (a >= 61 && a <= 85)
                        {
                            __instance.Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Scrap_1));
                        }
                        if (a >= 86 && a <= 90)
                        {
                            __instance.Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_Herb));
                        }
                        if (a >= 91 && a <= 95)
                        {
                            __instance.Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_SodaWater));

                        }
                        if (a >= 96 && a <= 100)
                        {
                            __instance.Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Consume_SmallBarrierMachine));
                        }
                    }
                }

                // Misty Garden 2 removed lifting scroll
                if (PlayData.TSavedata.StageNum == 1)
                {
                    if (___Itemviews.RemoveAll(x => x.itemkey == GDEItemKeys.Item_Scroll_Scroll_Uncurse) > 0)
                    {
                        //___Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, 50));

                    }
                }

                // Hard Sanctuary Fights
                flag = false;
                if ((__instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Guard_1 || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Golem || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Golem2 || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Summoner))
                {
                    flag = true;
                }

                // Sanctuary cursed enemies
                if (PlayData.TSavedata.StageNum == 5)
                {
                    // Hard Fight: Drop legendary
                    if (flag)
                    {
                        ___Itemviews.Add(ItemBase.GetItem(PlayData.GetEquipRandom(4)));
                    }
                    // Easy Fight: Drop 100G
                    else
                    {
                        ___Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, 100));
                    }
                }

            }
        }

        // Sturdy Curse reworked - Shield only goes up once, Armor +20%
        [HarmonyPatch(typeof(B_CursedMob_2))]
        class Robust_Patch
        {
            // Shield doesn't refresh after being removed. Skip the method entirely
            [HarmonyPatch(nameof(B_CursedMob_2.Turn))]
            [HarmonyPrefix]
            static bool Prefix()
            {
                if (VanillaCurses.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Add Armor +20%, Add one-time Shield
            [HarmonyPatch(nameof(B_CursedMob_2.Init))]
            [HarmonyPostfix]
            static void Postfix(B_CursedMob_2 __instance)
            {
                if (!VanillaCurses.Value)
                {
                    __instance.PlusStat.def = 20f;
                    __instance.BChar.BuffAdd(GDEItemKeys.Buff_B_Armor_P_1, __instance.BChar, false, 0, false, -1, false);
                }
            }


            //[HarmonyPatch(nameof(Buff.DescExtended))]
            //[HarmonyPostfix]
            //static void DescExtendedPostfix(ref string __result, Buff __instance)
            //{
            //    if (__instance is B_CursedMob_2)
            //    {
            //        __result = "Gain 1 action count\nBlock one incoming attack.";
            //    }
            //}
        }


        // Add Lifting Scroll in some shops
        [HarmonyPatch(typeof(FieldStore))]
        class FieldStore_Patch
        {
            [HarmonyPatch(nameof(FieldStore.Init))]
            [HarmonyPostfix]
            static void Postfix(FieldStore __instance)
            {

                if (PlayData.TSavedata.StageNum == 2 || PlayData.TSavedata.StageNum == 4)
                {
                    __instance.StoreItems.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Uncurse));
                }
            }
        }

        // Add Pain Debuff Resist -10% to Pierrot (And Living Armor) debuff
        [HarmonyPatch(typeof(B_Pierrot_Bat_1_T))]
        class PierrotDebuff_Patch
        {
            [HarmonyPatch(nameof(B_Pierrot_Bat_1_T.Init))]
            [HarmonyPostfix]
            static void Postfix(B_Pierrot_Bat_1_T __instance)
            {
                __instance.PlusStat.RES_DOT = -10f;
            }
        }

        /// <summary>
        /// Below is Neo's Stuff
        /// </summary>


        //Every Fight is Cursed
        // Fisher–Yates shuffle
        private static void KnuthShuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = UnityEngine.Random.Range(i, list.Count); // Don't select from the entire array on subsequent loops
                T temp = list[i]; list[i] = list[j]; list[j] = temp;
            }
        }

        static private List<MapTile> ogCursedTiles = new List<MapTile>();

        [HarmonyPatch(typeof(StageSystem))]
        class Generate_Cursed_Battles_Patch
        {
            [HarmonyPatch(nameof(StageSystem.InstantiateIsometric))]
            [HarmonyPostfix]
            static void InstantiateIsometric(HexMap ___Map)
            {
                if (!PlayData.TSavedata.IsLoaded)
                {
                    // checks are required for both tile battles and building battles
                    List<MapTile> battleList =
                        ___Map.EventTileList.FindAll(x => (x.Info.Type is Monster) ||
                        (x.TileEventObject != null && x.TileEventObject.ObjectData != null && x.TileEventObject.Monster));

                    ogCursedTiles = battleList.FindAll(x => x.Info.Cursed == true);
                    int curseCount = ogCursedTiles.Count;

                    KnuthShuffle(battleList);
                    foreach (MapTile mt in battleList)
                    {
                        // curses all fights
                        if (curseCount >= 4)
                            break;
                        if (mt.Info.Cursed == false)
                        {
                            mt.Info.Cursed = true;
                            curseCount++;
                        }
                    }
                }
            }
        }

        // Fixes Golem AI and Averages Stats for life link. 
        [HarmonyPatch(typeof(BattleTeam), nameof(BattleTeam.MyTurn))]
        class AvarageLifeLinkMaxHpPatch
        {
            static void Postfix()
            {
                if (BattleSystem.instance != null)
                {
                    if (BattleSystem.instance.TurnNum == 0)
                    {
                        List<float> linkMaxHps = new List<float>();
                        foreach (BattleEnemy battleEnemy in BattleSystem.instance.EnemyList)
                        {
                            if (battleEnemy.BuffFind(GDEItemKeys.Buff_P_Guard_LifeShare, false))
                            {
                                linkMaxHps.Add((float)battleEnemy.Info.get_stat.maxhp);
                            }
                        }

                        if (linkMaxHps.Count > 0)
                        {
                            int avgMaxHp = (int)linkMaxHps.Average();
                            //Debug.Log(avgMaxHp);
                            foreach (BattleEnemy battleEnemy in BattleSystem.instance.EnemyList)
                            {
                                if (battleEnemy.BuffFind(GDEItemKeys.Buff_P_Guard_LifeShare, false))
                                {
                                    battleEnemy.BuffReturn(GDEItemKeys.Buff_P_Guard_LifeShare).PlusStat.maxhp += avgMaxHp - battleEnemy.Info.get_stat.maxhp;
                                }
                            }
                        }
                    }
                }
            }
        }

        // reset max hp modification on in case of uncurse
        [HarmonyPatch(typeof(SkillExtended_UnCurse), nameof(SkillExtended_UnCurse.SkillUseSingle))]
        class UncurseCardPatch
        {
            static void Prefix()
            {
                foreach (BattleEnemy battleEnemy in BattleSystem.instance.EnemyList)
                {
                    if (battleEnemy.BuffFind(GDEItemKeys.Buff_P_Guard_LifeShare, false))
                    {
                        battleEnemy.BuffReturn(GDEItemKeys.Buff_P_Guard_LifeShare).PlusStat.maxhp = 0;
                    }
                }
            }
        }

        // makes golems function properly with multiple actions
        // TODO add config option to disable AI modification for potential compatibility issues
        class SactuaryGolemAIPatch
        {
            // acting speed is hardcoded and independent of party speed
            private static int SanctuaryGolemSpeed(int actionCount, int totalActionNumber)
            {
                int result = 99;
                if (actionCount == totalActionNumber)
                {
                    result = 99;
                }
                else
                {
                    result = 4 + actionCount * 2;
                }
                return result;
            }


            [HarmonyPatch(typeof(AI_S4_Golem))]
            class GreenGolemPatch
            {
                [HarmonyPatch(nameof(AI_S4_Golem.SkillSelect))]
                [HarmonyPrefix]
                static bool SkillSelectPrefix(ref Skill __result, AI_S4_Golem __instance)
                {
                    __result = __instance.BChar.Skills[0];
                    return false;
                }

                [HarmonyPatch(nameof(AI_S4_Golem.SpeedChange))]
                [HarmonyPrefix]
                static bool SpeedChangetPrefix(ref int __result, Skill skill, int ActionCount, int OriginSpeed, AI_S4_Golem __instance)
                {
                    __result = SanctuaryGolemSpeed(ActionCount, __instance.BChar.Info.PlusActCount.Count);
                    return false;
                }

            }

            [HarmonyPatch(typeof(AI_S4_Golem2))]
            class YellowGolemPatch
            {
                [HarmonyPatch(nameof(AI_S4_Golem2.SkillSelect))]
                [HarmonyPrefix]
                static bool SkillSelectPrefix(ref Skill __result, AI_S4_Golem2 __instance)
                {
                    __result = __instance.BChar.Skills[0];
                    return false;
                }
                [HarmonyPatch(nameof(AI_S4_Golem2.SpeedChange))]
                [HarmonyPrefix]
                static bool SpeedChangetPrefix(ref int __result, Skill skill, int ActionCount, int OriginSpeed, AI_S4_Golem2 __instance)
                {
                    __result = SanctuaryGolemSpeed(ActionCount, __instance.BChar.Info.PlusActCount.Count);
                    return false;
                }
            }

        }

        // Despair Mode: No revival in campfire
        // This implementation is kinda stinky but hey it works
        [HarmonyPatch(typeof(CampUI))]
        class CampfireRevival_Patch
        {

            [HarmonyPatch(nameof(CampUI.Init))]
            [HarmonyPrefix]
            static bool Prefix(CampUI __instance, Camp Sc)
            {
                __instance.MainCampScript = Sc;
                if (!__instance.MainCampScript.Healed)
                {
                    __instance.MainCampScript.Healed = true;
                    foreach (Character character in PlayData.TSavedata.Party)
                    {
                        bool flag = false;
                        if (character.Incapacitated)
                        {
                            // If Despair Mode is on, don't revive allies in campfire
                            if (!DespairMode.Value)
                            {
                                character.Incapacitated = false;
                                character.Hp = 1;
                                flag = true;
                                if (SaveManager.NowData.GameOptions.Difficulty == 0)
                                {
                                    character.HealHP((int)Misc.PerToNum((float)character.get_stat.maxhp, 18f), true);
                                }
                                else if (SaveManager.NowData.GameOptions.Difficulty == 2)
                                {
                                    character.HealHP((int)Misc.PerToNum((float)character.get_stat.maxhp, 10f), true);
                                }
                            }
                            // Logger
                            else Debug.Log("Revival Blocked");
                        }
                        if (SaveManager.NowData.GameOptions.Difficulty == 2)
                        {
                            if (!flag)
                            {
                                character.HealHP((int)Misc.PerToNum((float)character.get_stat.maxhp, 20f), true);
                            }
                        }
                        else if (SaveManager.NowData.GameOptions.Difficulty == 1)
                        {
                            character.HealHP((int)Misc.PerToNum((float)character.get_stat.maxhp, 60f), true);
                        }
                        else if (!flag)
                        {
                            character.HealHP((int)Misc.PerToNum((float)character.get_stat.maxhp, 35f), true);
                        }
                        if (character.Passive != null)
                        {
                            IP_CampFire ip_CampFire = character.Passive as IP_CampFire;
                            if (ip_CampFire != null)
                            {
                                ip_CampFire.Camp();
                            }
                        }
                        foreach (ItemBase itemBase in character.Equip)
                        {
                            if (itemBase != null)
                            {
                                IP_CampFire ip_CampFire2 = (itemBase as Item_Equip).ItemScript as IP_CampFire;
                                if (ip_CampFire2 != null)
                                {
                                    ip_CampFire2.Camp();
                                }
                            }
                        }
                    }
                    foreach (ItemBase itemBase2 in PlayData.TSavedata.Inventory)
                    {
                        if (itemBase2 is Item_Equip)
                        {
                            IP_CampFire ip_CampFire3 = (itemBase2 as Item_Equip).ItemScript as IP_CampFire;
                            if (ip_CampFire3 != null)
                            {
                                ip_CampFire3.Camp();
                            }
                        }
                    }
                }
                if (SaveManager.Difficalty != 2)
                {
                    foreach (ItemBase itemBase3 in PartyInventory.InvenM.InventoryItems)
                    {
                        if (itemBase3 != null && (itemBase3.itemkey == GDEItemKeys.Item_Active_LucysNecklace || itemBase3.itemkey == GDEItemKeys.Item_Active_LucysNecklace2 || itemBase3.itemkey == GDEItemKeys.Item_Active_LucysNecklace3 || itemBase3.itemkey == GDEItemKeys.Item_Active_LucysNecklace4))
                        {
                            (itemBase3 as Item_Active).ChargeNow++;
                        }
                    }
                }
                if (PlayData.TSavedata.StageNum == 1 || PlayData.TSavedata.StageNum == 3)
                {
                    if (SaveManager.NowData.GameOptions.CasualMode)
                    {
                        __instance.CasualPartyAdd = true;
                    }
                    __instance.Button_AddParty.gameObject.SetActive(true);
                    __instance.MainCampScript.Enforce = true;
                }
                else
                {
                    __instance.Button_AddParty.gameObject.SetActive(false);
                    __instance.Button_Enforce.gameObject.SetActive(true);
                }
                __instance.VerticalLayout.enabled = false;
                __instance.VerticalLayout.SetLayoutVertical();
                __instance.VerticalLayout.enabled = true;

                return false;
            }
        }

    }

}

