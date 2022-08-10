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
    [BepInPlugin(GUID, "Expert Plus Mod", version)]
    [BepInProcess("ChronoArk.exe")]
    public class ExpertPlusPlugin : BaseUnityPlugin
    {
        public const string GUID = "windy.expertplus";
        public const string version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(GUID);

        public static ConfigEntry<bool> PermaMode;
        public static ConfigEntry<bool> VanillaCurses;
        public static ConfigEntry<bool> AscensionMode;
        public static ConfigEntry<bool> DespairMode;
        public static ConfigEntry<bool> hardTransitions;


        void Awake()
        {
            PermaMode = Config.Bind("Generation config", "Permadeath Mode", false, "Permadeath Mode\nCampfires cannot revive allies. Removed revive option in Medical Tent. Golden Bread cannot be used on fallen allies.");
            VanillaCurses = Config.Bind("Generation config", "Vanilla Curses", false, "Vanilla Curses\nReverts the nerfs to Cursed Mob stats. The challenge is designed around weaker cursed mobs, but toggle this on if you want.");
            AscensionMode = Config.Bind("Generation config", "Ascension Mode", false, "Ascension Mode\nA mimic of Slay The Spire's Ascension Mode.\n1. Add Slow Response Curse to deck at the start of the game.\n2. Maximum potion uses per battle reduced to 2.\n3. Character Equipment Slots reduced to 1. (Equipment Drop Rates reduced)\n4. Relic Slots reduced to 2.");
            DespairMode = Config.Bind("Generation config", "Despair Mode", false, "Despair Mode\nWarning: Very Difficult.\n1. Lifting Scrolls do not spawn in battle.\n2. After Misty Garden 1, fight all possible bosses for each stage. Godo and TFK fight is harder.\n3. More features coming soon.");
            hardTransitions = Config.Bind("Generation config", "Hard Transitions", false, "Removes most of the boss transition handicap. Despair Mode needs to be on.");

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
                        // Despair Mode: Give Stat Bonuses
                        //if (((Dictionary<string, object>)e.Value)["_gdeSchema"].Equals("Enemy"))
                        //{
                        //    if (DespairMode.Value)
                        //    {
                        //        (masterJson[e.Key] as Dictionary<string, object>)["atk"] = (long)((masterJson[e.Key] as Dictionary<string, object>)["maxhp"]) * 1.2;
                        //        (masterJson[e.Key] as Dictionary<string, object>)["atk"] = (long)((masterJson[e.Key] as Dictionary<string, object>)["atk"]) * 1.2;
                        //        (masterJson[e.Key] as Dictionary<string, object>)["atk"] = (long)((masterJson[e.Key] as Dictionary<string, object>)["reg"]) * 2;
                        //    }
                        //}

                        // Despair Mode: Golden Apple can be used on fallen allies, cannot be used in battle
                        //if (e.Key == "GoldenApple")
                        //{
                        //    if (DespairMode.Value)
                        //    {
                        //        (masterJson[e.Key] as Dictionary<string, object>)["Target"] = "deathally";
                        //        (masterJson[e.Key] as Dictionary<string, object>)["active_battle"] = false;
                        //    }
                        //}

                        /// Despair Mode
                        if (e.Key == "S4_King_0")
                        {
                            if (DespairMode.Value)
                            {
                                (masterJson[e.Key] as Dictionary<string, object>)["maxhp"] = 1500;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 18;
                            }
                        }
                        if (e.Key == "Queue_Witch")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("Boss_Golem");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 99;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 12;
                            }
                        }

                        if (e.Key == "Stage1_2")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("Queue_Witch");
                                (masterJson[e.Key] as Dictionary<string, object>)["Bosses"] = a;
                            }
                        }

                        if (e.Key == "Queue_DorchiX")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("S1_WitchBoss");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 99;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 18;

                                List<string> b = new List<string>();
                                b.Add("Boss_Golem");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3"] = b;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3Turn"] = 100;
                            }
                        }

                        if (e.Key == "Queue_S2_Joker")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("MBoss2_0");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 99;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 12;
                            }
                        }

                        if (e.Key == "Stage2_1")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("Queue_S2_Joker");
                                (masterJson[e.Key] as Dictionary<string, object>)["Bosses"] = a;
                            }
                        }

                        if (e.Key == "CrimsonQueue_GunManBoss")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("SR_Shotgun");
                                a.Add("SR_Outlaw");
                                a.Add("SR_Blade");
                                a.Add("SR_Sniper");
                                (masterJson[e.Key] as Dictionary<string, object>)["Enemys"] = a;

                                List<string> b = new List<string>();
                                b.Add("SR_GunManBoss");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = b;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 4;

                                (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = true;
                                Dictionary<string, int> pos1 = new Dictionary<string, int>();
                                pos1.Add("x", -10);
                                pos1.Add("y", 0);
                                pos1.Add("z", 6);
                                Dictionary<string, int> pos2 = new Dictionary<string, int>();
                                pos2.Add("x", 5);
                                pos2.Add("y", 0);
                                pos2.Add("z", 0);
                                Dictionary<string, int> pos3 = new Dictionary<string, int>();
                                pos3.Add("x", 0);
                                pos3.Add("y", 0);
                                pos3.Add("z", 2);
                                Dictionary<string, int> pos4 = new Dictionary<string, int>();
                                pos4.Add("x", -4);
                                pos4.Add("y", 0);
                                pos4.Add("z", -1);
                                List<Dictionary<string, int>> c = new List<Dictionary<string, int>>();
                                c.Add(pos2);
                                c.Add(pos3);
                                c.Add(pos4);
                                c.Add(pos1);
                                (masterJson[e.Key] as Dictionary<string, object>)["Pos"] = c;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 11;
                            }
                        }

                        if (e.Key == "Queue_S2_MainBoss_Luby")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("S2_BombClownBoss");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 99;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 16;


                                List<string> b = new List<string>();
                                b.Add("MBoss2_1");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3"] = b;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3Turn"] = 100;
                            }
                        }

                        if (e.Key == "Stage2_2")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("Queue_S2_MainBoss_Luby");
                                (masterJson[e.Key] as Dictionary<string, object>)["Bosses"] = a;
                            }
                        }

                        if (e.Key == "Queue_S3_Reaper")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("S3_Boss_TheLight");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                                (masterJson[e.Key] as Dictionary<string, object>)["Lock"] = false;
                                (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = false;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 99;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 16;

                                List<string> b = new List<string>();
                                b.Add("Queue_S3_PharosLeader");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3"] = b;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3Turn"] = 100;
                            }
                        }

                        if (e.Key == "Queue_S3_TheLight")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("Queue_S3_PharosLeader");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                                (masterJson[e.Key] as Dictionary<string, object>)["Lock"] = false;
                                (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = false;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 99;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 16;

                                List<string> b = new List<string>();
                                b.Add("S3_Pharos_HighPriest");
                                b.Add("S3_Boss_Reaper");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3"] = b;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3Turn"] = 100;
                            }
                        }

                        if (e.Key == "Queue_S3_PharosLeader")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("S3_Boss_TheLight");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2"] = a;
                                (masterJson[e.Key] as Dictionary<string, object>)["Lock"] = false;
                                (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = false;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 99;
                                (masterJson[e.Key] as Dictionary<string, object>)["CustomeFogTurn"] = 16;

                                List<string> b = new List<string>();
                                b.Add("S3_Pharos_HighPriest");
                                b.Add("S3_Boss_Reaper");
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3"] = b;
                                (masterJson[e.Key] as Dictionary<string, object>)["Wave3Turn"] = 100;
                            }
                        }

                        if (e.Key == "Stage3")
                        {
                            if (DespairMode.Value)
                            {
                                List<string> a = new List<string>();
                                a.Add("Queue_S3_PharosLeader");
                                (masterJson[e.Key] as Dictionary<string, object>)["Bosses"] = a;
                            }
                        }

                        if (e.Key == "GoldenBread")
                        {
                            if (PermaMode.Value)
                            {
                                (masterJson[e.Key] as Dictionary<string, object>)["Target"] = "ally";
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
                            (masterJson[e.Key] as Dictionary<string, object>)["Lock"] = false;
                            (masterJson[e.Key] as Dictionary<string, object>)["UseCustomPosition"] = false;
                            (masterJson[e.Key] as Dictionary<string, object>)["Wave2Turn"] = 2;
                            //(masterJson[e.Key] as Dictionary<string, object>)["Wave3"] = a;
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

                        // Ascension Mode, drop rates reduced
                        if (e.Key == "BossEquipRandomDrop")
                        {
                            if (AscensionMode.Value)
                            {
                                /* 0/8/55/30/5 -> 0/4/35/10/1 */
                                (masterJson[e.Key] as Dictionary<string, object>)["Common"] = 0;
                                (masterJson[e.Key] as Dictionary<string, object>)["UnCommon"] = 4;
                                (masterJson[e.Key] as Dictionary<string, object>)["Rare"] = 35;
                                (masterJson[e.Key] as Dictionary<string, object>)["Unique"] = 10;
                                (masterJson[e.Key] as Dictionary<string, object>)["Legendary"] = 1;
                                (masterJson[e.Key] as Dictionary<string, object>)["NoItem"] = 50;
                            }
                        }

                        if (e.Key == "BattleRandomDrop")
                        {
                            if (AscensionMode.Value)
                            {
                                (masterJson[e.Key] as Dictionary<string, object>)["NoItem"] = 100;
                            }
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
            [HarmonyPostfix]
            static void StageStartPostfix()
            {
                if (!DespairMode.Value)
                {
                    if (PlayData.TSavedata.StageNum == 0 /*&& !PlayData.TSavedata.GameStarted*/)
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
        }

        // Sniper Curse can be removed by lifting scroll
        [HarmonyPatch(typeof(SkillExtended_UnCurse))]
        class SniperCurse_Patch
        {
            [HarmonyPatch(nameof(SkillExtended_UnCurse.SkillUseSingle))]
            [HarmonyPrefix]
            static bool Prefix(SkillExtended_UnCurse __instance)
            {
                //Debug.Log("Vanished Char");
                foreach (BattleEnemy battleEnemy in BattleSystem.instance.EnemyTeam.AliveChars_Vanish)
                {
                    foreach (Buff buff in battleEnemy.Buffs)
                    {
                        if (buff is B_CursedMob)
                        {
                            (buff as B_CursedMob).Uncurse();
                        }
                    }
                }
                foreach (BattleEnemy battleEnemy2 in BattleSystem.instance.EnemyTeam.AliveChars_Vanish)
                {
                    battleEnemy2.BuffRemove(GDEItemKeys.Buff_B_CursedMob_0, false);
                    battleEnemy2.BuffRemove(GDEItemKeys.Buff_B_CursedMob_1, false);
                    battleEnemy2.BuffRemove(GDEItemKeys.Buff_B_CursedMob_2, false);
                    battleEnemy2.BuffRemove(GDEItemKeys.Buff_B_CursedMob_3, false);
                    battleEnemy2.BuffRemove(GDEItemKeys.Buff_B_CursedMob_4, false);
                    battleEnemy2.BuffRemove(GDEItemKeys.Buff_B_CursedMob_5, false);
                }
                BattleSystem.instance.CurseBattle = false;
                PartyInventory.InvenM.DelItem(GDEItemKeys.Item_Scroll_Scroll_Uncurse, 1);
                UnityEngine.Object.Instantiate<GameObject>(__instance.MySkill.MySkill.Particle);

                return false;
            }
        }

        // Mana reduced by 1 per charcter dead. Cannot fall below 3.
        [HarmonyPatch(typeof(BattleTeam))]
        class ManaRemove_Patch
        {
            [HarmonyPatch(nameof(BattleTeam.MyTurn))]
            [HarmonyPrefix]
            static bool Prefix(BattleTeam __instance)
            {
                __instance.DummyCharAlly.Dummy = true;
                foreach (BattleChar battleChar in __instance.AliveChars)
                {
                    battleChar.ActionCount = 1;
                    battleChar.Overload = 0;
                    battleChar.ActionNum = 0;
                    battleChar.SkillUseDraw = false;
                }
                __instance.LucyAlly.ActionCount = 1;
                __instance.LucyAlly.Overload = 0;
                __instance.LucyAlly.ActionNum = 0;
                __instance.UsedDeckToDeckNum = 0;
                __instance.DiscardCount = __instance.GetDiscardCount;
                __instance.WaitCount = 1 + PlayData.PartySpeed;
                if (__instance.WaitCount >= 3)
                {
                    __instance.WaitCount = 3;
                }
                if (__instance.WaitCount <= 1)
                {
                    __instance.WaitCount = 1;
                }
                if (__instance.AliveChars.Find((BattleChar a) => a.Info.KeyData == GDEItemKeys.Character_LucyC) != null)
                {
                    __instance.WaitCount = 1;
                }
                __instance.AP = __instance.MAXAP;
                __instance.TurnActionNum = 0;
                List<BattleChar> list = new List<BattleChar>();
                AccessTools.FieldRef<BattleTeam, List<BattleChar>> G_Ref = AccessTools.FieldRefAccess<List<BattleChar>>(typeof(BattleTeam), "G_AliveChars");
                list.AddRange(G_Ref(__instance));
                List<BattleChar.TickInfo> list2 = new List<BattleChar.TickInfo>();
                for (int i = 0; i < list.Count; i++)
                {
                    list2.Add(list[i].TickDamageReturn());
                }
                for (int j = 0; j < list.Count; j++)
                {
                    List<Buff> list3 = new List<Buff>();
                    list3.AddRange(list[j].Buffs);
                    for (int k = 0; k < list3.Count; k++)
                    {
                        Buff buff = list3[k];
                        list3[k].TurnUpdate();
                        if (list3.Count == 0)
                        {
                            break;
                        }
                        if (k >= list3.Count)
                        {
                            k--;
                        }
                        else if (buff != list3[k])
                        {
                            k--;
                        }
                    }
                }
                for (int l = 0; l < list.Count; l++)
                {
                    list[l].TickUpdate(list2[l]);
                }
                foreach (BattleChar battleChar2 in list)
                {
                    battleChar2.BattleUpdate();
                }
                if (__instance.LucyChar.IsLucyNoC)
                {
                    __instance.LucyChar.TickUpdate(__instance.LucyChar.TickDamageReturn());
                    __instance.LucyChar.BattleUpdate();
                    List<Buff> list4 = new List<Buff>();
                    list4.AddRange(__instance.LucyChar.Buffs);
                    for (int m = 0; m < list4.Count; m++)
                    {
                        Buff buff2 = list4[m];
                        list4[m].TurnUpdate();
                        if (list4.Count == 0)
                        {
                            break;
                        }
                        if (m >= list4.Count)
                        {
                            m--;
                        }
                        else if (buff2 != list4[m])
                        {
                            m--;
                        }
                    }
                }
                AccessTools.FieldRef<BattleTeam, int> G2_Ref = AccessTools.FieldRefAccess<int>(typeof(BattleTeam), "G_AliveCharGetFrame");
                G2_Ref(__instance) = 0;
                int num = PlayData.GetDraw;
                BattleSystem.instance.ActWindow.gameObject.SetActive(true);
                if (BattleSystem.instance.TurnNum == 0)
                {
                    num = PlayData.GetDraw + __instance.AliveChars.Count;
                    if (PlayData.TSavedata.SpRule != null)
                    {
                        num += PlayData.TSavedata.SpRule.RuleChange.PlusFirstTurnDraw;
                    }
                    List<Skill> list5 = new List<Skill>();
                    list5.AddRange(__instance.Skills_Deck);
                    foreach (Skill skill in list5)
                    {
                        foreach (Skill_Extended skill_Extended in skill.AllExtendeds)
                        {
                            skill_Extended.BattleStartDeck(__instance.Skills_Deck);
                        }
                    }
                    foreach (IP_FirstDrawBefore ip_FirstDrawBefore in BattleSystem.instance.IReturn<IP_FirstDrawBefore>())
                    {
                        if (ip_FirstDrawBefore != null)
                        {
                            ip_FirstDrawBefore.FirstDrawBefore(__instance.Skills_Deck);
                        }
                    }
                }
                __instance.Draw(num);
                for (int n = 0; n < __instance.Chars.Count; n++)
                {
                    __instance.BasicSkillRefill(__instance.Chars[n], __instance.Skills_Basic[n]);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(BattleChar))]
        class ManaRemove2_Patch
        {
            [HarmonyPatch(nameof(BattleChar.AllyDeadCheck))]
            [HarmonyPrefix]
            static bool Prefix(BattleChar __instance)
            {
                bool flag = false;
                if (__instance.Info.Passive is P_Phoenix)
                {
                    flag = true;
                }
                if (flag)
                {
                    if (__instance.Info.Hp <= 0)
                    {
                        if (!__instance.BuffFind(GDEItemKeys.Buff_B_Phoenix_P, false))
                        {
                            __instance.BuffAdd(GDEItemKeys.Buff_B_Phoenix_P, __instance.MyTeam.DummyChar, false, 0, false, -1, false);
                        }
                        if (!__instance.BuffFind(GDEItemKeys.Buff_B_Phoenix_P_0, false))
                        {
                            __instance.BuffAdd(GDEItemKeys.Buff_B_Phoenix_P_0, __instance.MyTeam.DummyChar, false, 0, false, -1, false);
                        }
                    }
                    else if (__instance.BuffFind(GDEItemKeys.Buff_B_Phoenix_P_0, false))
                    {
                        __instance.BuffRemove(GDEItemKeys.Buff_B_Phoenix_P_0, true);
                    }
                }
                else if (__instance.Recovery <= 0)
                {
                    __instance.Dead(false);

                    //Here
                    //Debug.Log("Stage: " + StageSystem.instance.Map.StageData.Key);
                    if (StageSystem.instance.Map.StageData.Key != GDEItemKeys.Stage_Stage_Crimson)
                    {
                        BattleSystem.instance.AllyTeam.AP--;
                        //Debug.Log("Mana decreased due to death");
                    }
                }
                else
                {
                    __instance.UI.CharAni.SetBool("Dead", false);
                    if (__instance.Info.Hp <= 0)
                    {
                        if (!__instance.BuffFind(GDEItemKeys.Buff_B_Neardeath, false))
                        {
                            __instance.BuffAdd(GDEItemKeys.Buff_B_Neardeath, __instance.MyTeam.DummyChar, false, 0, false, -1, false);
                            foreach (IP_NearDeath ip_NearDeath in __instance.IReturn<IP_NearDeath>(null))
                            {
                                if (ip_NearDeath != null)
                                {
                                    ip_NearDeath.NearDeath();
                                }
                            }
                            if (!__instance.BuffFind(GDEItemKeys.Buff_B_S3_Pope_P_2, false))
                            {
                                if (__instance.Info.KeyData == GDEItemKeys.Character_Phoenix)
                                {
                                    __instance.BattleInfo.ScriptOut.LowHPAlly();
                                    __instance.BattleInfo.ScriptOut.LowHP(__instance);
                                }
                                else if (Misc.RandomPer(100, 40))
                                {
                                    __instance.BattleInfo.ScriptOut.LowHP(__instance);
                                }
                                else
                                {
                                    __instance.BattleInfo.ScriptOut.LowHPAlly();
                                }
                            }
                        }
                        __instance.UI.CharAni.SetBool("NearDead", true);
                        TutorialSystem.TutorialFlag(15);
                    }
                    else
                    {
                        if (__instance.BuffFind(GDEItemKeys.Buff_B_Neardeath, false))
                        {
                            __instance.BuffRemove(GDEItemKeys.Buff_B_Neardeath, true);
                        }
                        __instance.UI.CharAni.SetBool("NearDead", false);
                    }
                }
                return false;
            }
        }

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
                    // HP increase dropped to 40%, remove debuff resistance buff
                    __instance.PlusPerStat.MaxHP = __instance.PlusPerStat.MaxHP - 20;
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
                    ___Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, 50));
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
                        Random rand = new Random();

                        int a = rand.Next(1, 101); // 1-100
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
                            ItemBase item = ItemBase.GetItem(PlayData.GetEquipRandom(1));
                            (item as Item_Equip).Curse = EquipCurse.RandomCurse(item as Item_Equip);
                            __instance.Itemviews.Add(item);
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
                        ___Itemviews.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, 50));

                    }
                }

                // Hard Sanctuary Fights: Extra Reward
                flag = false;
                if ((__instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Guard_1 /*|| __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Golem || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Golem2 || __instance.BChar.Info.KeyData == GDEItemKeys.Enemy_S4_Summoner*/))
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

                // Despair Mode: Sniper remove action count
                //if (DespairMode.Value)
                //{
                //    if (__instance.BChar.Info.KeyData == GDEItemKeys.Enemy_SR_Sniper)
                //    {
                //        __instance.BChar.Info.PlusActCount.RemoveAt(__instance.BChar.Info.PlusActCount.Count - 1);
                //    }
                //}

            }
        }

        // Sturdy Curse reworked - Shield only goes up once, Armor +20%
        //[HarmonyPatch(typeof(B_CursedMob_2))]
        //class Robust_Patch
        //{
        //    // Shield doesn't refresh after being removed. Skip the method entirely
        //    [HarmonyPatch(nameof(B_CursedMob_2.Turn))]
        //    [HarmonyPrefix]
        //    static bool Prefix()
        //    {
        //        if (VanillaCurses.Value)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    // Add Armor +20%, Add one-time Shield
        //    [HarmonyPatch(nameof(B_CursedMob_2.Init))]
        //    [HarmonyPostfix]
        //    static void Postfix(B_CursedMob_2 __instance)
        //    {
        //        if (!VanillaCurses.Value)
        //        {
        //            __instance.PlusStat.def = 20f;
        //            __instance.BChar.BuffAdd(GDEItemKeys.Buff_B_Armor_P_1, __instance.BChar, false, 0, false, -1, false);
        //        }
        //    }


        //[HarmonyPatch(nameof(Buff.DescExtended))]
        //[HarmonyPostfix]
        //static void DescExtendedPostfix(ref string __result, Buff __instance)
        //{
        //    if (__instance is B_CursedMob_2)
        //    {
        //        __result = "Gain 1 action count\nBlock one incoming attack.";
        //    }
        //}
        //}


        // Add Lifting Scroll in some shops
        //[HarmonyPatch(typeof(FieldStore))]
        //class FieldStore_Patch
        //{
        //    [HarmonyPatch(nameof(FieldStore.Init))]
        //    [HarmonyPostfix]
        //    static void Postfix(FieldStore __instance)
        //    {

        //        if (PlayData.TSavedata.StageNum == 2 || PlayData.TSavedata.StageNum == 4)
        //        {
        //            __instance.StoreItems.Add(ItemBase.GetItem(GDEItemKeys.Item_Scroll_Scroll_Uncurse));
        //        }
        //    }
        //}

        // Add Pain Debuff Resist -10% to Pierrot (and living armor) debuff
        //[HarmonyPatch(typeof(B_Pierrot_Bat_1_T))]
        //class PierrotDebuff_Patch
        //{
        //    [HarmonyPatch(nameof(B_Pierrot_Bat_1_T.Init))]
        //    [HarmonyPostfix]
        //    static void Postfix(B_Pierrot_Bat_1_T __instance)
        //    {
        //        __instance.PlusStat.RES_DOT = -10f;
        //    }
        //}

        /// <summary>
        /// Despair Mode
        /// </summary>
        /// 

        // Despair Mode: Do not spawn Lifting Scroll in battle
        [HarmonyPatch(typeof(B_CursedMob), "BattleStart")]
        class CursedStart_Patch
        {
            static bool Prefix()
            {
                if (DespairMode.Value)
                {
                    return false;
                }
                return true;
            }
        }

       

        // Crimson Boss Battle: Spawn 2 Decisive Strike
        [HarmonyPatch(typeof(B_Sniper_0), nameof(B_Sniper_0.Turn1))]
        class SniperDrawFire_Patch
        {
            static bool Prefix(B_Sniper_0 __instance)
            {
                if (DespairMode.Value)
                {
                    
                    if (BattleSystem.instance.TurnNum == 1)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            BattleSystem.instance.AllyTeam.Add(Skill.TempSkill(GDEItemKeys.Skill_S_Sniper_1, BattleSystem.instance.AllyTeam.LucyChar, BattleSystem.instance.AllyTeam), true);
                        }
                        // Crimson Boss Fight
                        if (!BattleSystem.instance.CurseBattle)
                        {
                            Skill s = Skill.TempSkill(GDEItemKeys.Skill_S_Lucy_25, BattleSystem.instance.AllyTeam.LucyChar, BattleSystem.instance.AllyTeam);
                            s.AP = -1;
                            BattleSystem.instance.AllyTeam.Add(s, true);
                            if (!hardTransitions.Value)
                            {
                                Skill s2 = Skill.TempSkill(GDEItemKeys.Skill_S_Lucy_25, BattleSystem.instance.AllyTeam.LucyChar, BattleSystem.instance.AllyTeam);
                                s2.AP = -1;
                                BattleSystem.instance.AllyTeam.Add(s2, true); 
                            }
                        }
                    }
                        return false;
                }
                return true;
            }
        }

        //TFK: Phase 2 starts at 750HP
        [HarmonyPatch(typeof(P_King))]
        class TFKPatch
        {
            [HarmonyPatch(nameof(P_King.HPChange))]
            [HarmonyPrefix]
            static bool Prefix(P_King __instance)
            {
                if (DespairMode.Value)
                {
                    if (__instance.MainAI.Phase == 1 && __instance.BChar.HP <= 750)
                    {
                        __instance.BChar.Info.Hp = 750;
                        __instance.BChar.BuffAdd(GDEItemKeys.Buff_B_S4_King_P1_Half, __instance.BChar, false, 0, false, -1, false);
                        if (__instance.MainAI.Phase == 1)
                        {
                            (__instance.BChar as BattleEnemy).ChangeSprite(((__instance.BChar as BattleEnemy).MyComponent as C_King).Phase_1_2);
                            __instance.BChar.UI.CharShake.ShakeEndbled(50f, 20f, 60);
                        }
                    }
                }
                return true;
            }
        }

        // Godo 20 soulstones
        [HarmonyPatch(typeof(BattleSystem), "ClearBattle")]
        class GD20_Patch
        {
            static void Postfix(BattleSystem __instance)
            {
                Debug.Log(__instance.MainQueueData.Key);
                if (DespairMode.Value)
                {
                        if (__instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_CrimsonQueue_GunManBoss)
                        {
                            __instance.Reward.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul,20));
                        }
                }
            }
        }

        // Time Trial: time adjusted
        [HarmonyPatch(typeof(EventBattle_TrialofTime), "SetTimer")]

        class TimeTrialBosses
        {
            static bool Prefix(ref float __result)
            {
                if (DespairMode.Value)
                {
                    float result = 601f;
                    if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Garden_Midboss)
                    {
                        result = 30f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_MBoss_0)
                    {
                        result = 40f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_Witch)
                    {
                        result = 385f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_Golem)
                    {
                        result = 385f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_DorchiX)
                    {
                        result = 385f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_S2_Joker)
                    {
                        result = 160f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_MBoss2_0)
                    {
                        result = 160f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_S2_MainBoss_Luby)
                    {
                        result = 420f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_S2_BombClown)
                    {
                        result = 420f;
                    }
                    else if (BattleSystem.instance.MainQueueData.Key == GDEItemKeys.EnemyQueue_Queue_S2_TimeEater)
                    {
                        result = 420f;
                    }
                    __result = result;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Permadeath Mode
        /// </summary>

        // Permadeath Mode: Ban Medical Tent
        [HarmonyPatch(typeof(RandomEventBaseScript))]
        class MedTent_Patch
        {
            [HarmonyPatch(nameof(RandomEventBaseScript.EventOpen_Base))]
            [HarmonyPostfix]
            static void Postfix(RandomEventBaseScript __instance)
            {
                if (__instance is RE_Medicaltent)
                {
                    //Debug.Log("Here");
                    __instance.ButtonOff(0);
                }
            }
        }

        /// <summary>
        /// Ascension Mode
        /// </summary>

        // Ascension Mode: Add Slow Response to deck
        [HarmonyPatch(typeof(StartPartySelect))]
        class Ascension_Patch
        {
            [HarmonyPatch(nameof(StartPartySelect.Apply))]
            [HarmonyPostfix]
            static void Postfix(StartPartySelect __instance)
            {
                // If Ascension Mode, add Slow Response
                if (AscensionMode.Value && PlayData.TSavedata.StageNum == 0)
                {
                    //Debug.Log("Added Slow Response");
                    PlayData.TSavedata.LucySkills.Add(GDEItemKeys.Skill_S_LucyCurse_Late);

                    //Debug.Log("Relic Slots reduced");
                    PlayData.TSavedata.Passive_Itembase.Remove(null);
                    PlayData.TSavedata.Passive_Itembase.Remove(null);
                    PlayData.TSavedata.ArkPassivePlus -= 2;
                }
            }
        }

        // Ascension Mode: Equip Slots reduced
        [HarmonyPatch(typeof(FieldSystem))]
        class Ascension_Patch2
        {
            [HarmonyPatch(nameof(FieldSystem.PartyAdd), new Type[] { typeof(GDECharacterData), typeof(int) })]
            [HarmonyPrefix]
            static bool Prefix(GDECharacterData CData, int Levelup = 0)
            {
                if (AscensionMode.Value)
                {
                    Character character = new Character();
                    character.Set_AllyData(CData);
                    character.Hp = character.get_stat.maxhp;
                    PlayData.TSavedata.DonAliveChars.Add(CData.Key);
                    PlayData.TSavedata.Party.Add(character);
                    if (FieldSystem.instance != null)
                    {
                        FieldSystem.instance.PartyWindowInit();
                    }
                    UIManager.inst.CharstatUI.GetComponent<CharStatV3>().Init();
                    for (int i = 0; i < Levelup; i++)
                    {
                        UIManager.inst.CharstatUI.GetComponent<CharStatV3>().CWindows[PlayData.TSavedata.Party.Count - 1].Upgrade(true);
                    }

                    //Remove equip slot here
                    //Debug.Log("Removed equip slot");
                    character.Equip.Remove(null);
                    return false;
                }
                return true;
            }
        }

        // Ascension Mode: Reduce Potion Num
        [HarmonyPatch(typeof(BattleSystem))]
        class Ascension_Patch3
        {
            [HarmonyPatch(nameof(BattleSystem.Start))]
            [HarmonyPostfix]
            static void Postfix()
            {
                // If Ascension Mode, reduce potion num
                if (AscensionMode.Value)
                {
                    //Debug.Log("Potion Slots reduced");
                    BattleSystem.instance.AllyTeam.MaxPotionNum = 2;
                }
            }
        }
        // Ascension Mode: Ilya Swords buff (compensation for equip slot reduced)
        [HarmonyPatch(typeof(EItem.Ilya_Sword_0))]
        class Ascension_Patch4
        {
            [HarmonyPatch(nameof(EItem.Ilya_Sword_0.Init))]
            [HarmonyPostfix]
            static void Postfix(EItem.Ilya_Sword_0 __instance)
            {
                // If Ascension Mode, buff stats
                if (AscensionMode.Value)
                {
                    __instance.PlusStat.cri = 15f;
                }
            }
        }

        // Ascension Mode: Ilya Swords buff (compensation for equip slot reduced)
        [HarmonyPatch(typeof(EItem.Ilya_Sword_1))]
        class Ascension_Patch5
        {
            [HarmonyPatch(nameof(EItem.Ilya_Sword_1.Init))]
            [HarmonyPostfix]
            static void Postfix(EItem.Ilya_Sword_1 __instance)
            {
                // If Ascension Mode, buff stats
                if (AscensionMode.Value)
                {
                    __instance.PlusStat.hit = 10f;
                }
            }
        }

        //Ascension Mode : Equip Slot Centered
        [HarmonyPatch(typeof(ChildClear), "Start")]
        class CenterItemSlotPatch
        {
            static void Postfix(ChildClear __instance)
            {
                if (AscensionMode.Value)
                {
                    var transform = __instance.GetComponent<Transform>();
                    if (transform.name == "EquipAlign")
                    {
                        // party view
                        if (transform.parent.parent.name == "CloseView")
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x - 30f, transform.localPosition.y, transform.localPosition.z);
                        }
                        // blacksmith
                        //if (transform.parent.name == "EquipView")
                        //{
                        //    transform.localPosition = new Vector3(transform.localPosition.x - 35f, transform.localPosition.y, transform.localPosition.z);
                        //}
                    }
                }
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
                    //if (___Map.StageData.Key != GDEItemKeys.Stage_Stage_Crimson)
                    //{
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
                    //}
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

        // Neo Shrimp Code
        //Lift Card Placement at bottom
        [HarmonyPatch(typeof(BattleTeam))]
        class Move_Uncurse_Card_Patch
        {
            [HarmonyPatch(nameof(BattleTeam.MyTurn))]
            [HarmonyPostfix]
            static void MyTurnPostfix()
            {
                if (BattleSystem.instance != null)
                {
                    if (BattleSystem.instance.TurnNum == 0)
                    {
                        Skill temp = BattleSystem.instance.AllyTeam.Skills.Find(x => x.MySkill.KeyID == GDEItemKeys.Skill_S_UnCurse);
                        if (temp != null)
                        {
                            BattleSystem.instance.AllyTeam.Skills.Remove(temp);
                            BattleSystem.instance.AllyTeam.Add(temp, true);
                        }
                    }
                }
            }
        }

        // Ban Miniboss Curse from CW
        [HarmonyPatch(typeof(BattleSystem), nameof(BattleSystem.CurseEnemySelect))]

        class CurseCWminiBosses
        {
            static void Postfix(ref string __result, List<GDEEnemyData> Enemydatas, BattleSystem __instance)
            {
                var cwMiniBosses = new HashSet<string>() { GDEItemKeys.Enemy_SR_GuitarList, GDEItemKeys.Enemy_SR_Shotgun, GDEItemKeys.Enemy_SR_Blade, GDEItemKeys.Enemy_SR_Outlaw, GDEItemKeys.Enemy_SR_Sniper };
                var mbList = Enemydatas.FindAll(data => !cwMiniBosses.Contains(data.Key));
                if (mbList.Count > 0)
                {
                    __result = mbList.Random().Key;
                }
            }
        }


        // Despair Mode: No revival in campfire
        // This implementation is kinda stinky but it works
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
                            if (!PermaMode.Value)
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
                    if (PlayData.TSavedata.SpRule == null || !PlayData.TSavedata.SpRule.RuleChange.CantNewPartymember)
                    {
                        if (SaveManager.NowData.GameOptions.CasualMode)
                        {
                            __instance.CasualPartyAdd = true;
                        }
                        __instance.Button_AddParty.gameObject.SetActive(true);
                    }
                    else
                    {
                        __instance.Button_AddParty.gameObject.SetActive(false);
                    }
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
