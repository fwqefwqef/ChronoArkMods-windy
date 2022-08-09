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

namespace ExpertPlusMod
{
    public class RandomBossOrder
    {

        [HarmonyPatch(typeof(GDEEnemyQueueData), nameof(GDEEnemyQueueData.LoadFromDict))]
        class GDEEnemyQueueData_Patch
        {
            static void RandomizeWaveOrder(GDEEnemyQueueData data,  Dictionary<string, object> dict)
            {

                dict.TryGetCustomList("Enemys", out List<GDEEnemyData> ogEnemies);
                dict.TryGetCustomList("Wave2", out List<GDEEnemyData> ogWave2);
                dict.TryGetCustomList("Wave3", out List<GDEEnemyData> ogWave3);

                var waves = new List<List<GDEEnemyData>>();
                waves.Add(ogEnemies);
                waves.Add(ogWave2);
                waves.Add(ogWave3);


                waves = Misc.Shuffle(waves);
                data.Enemys = waves[0];
                data.Wave2 = waves[1];
                data.Wave3 = waves[2];
            }
            static void Postfix(GDEEnemyQueueData __instance, Dictionary<string, object> dict)
            {
                if (ExpertPlusPlugin.DespairMode.Value)
                {
                    if (__instance.Key == GDEItemKeys.EnemyQueue_Queue_Witch)
                    {
                        RandomizeWaveOrder(__instance, dict);
                    }
                    else if (__instance.Key == GDEItemKeys.EnemyQueue_Queue_S2_Joker)
                    {
                        RandomizeWaveOrder(__instance, dict);
                    }
                    else if (__instance.Key == GDEItemKeys.EnemyQueue_Queue_S2_MainBoss_Luby)
                    {
                        RandomizeWaveOrder(__instance, dict);
                    }
                    else if (__instance.Key == GDEItemKeys.EnemyQueue_Queue_S3_PharosLeader)
                    {
                        RandomizeWaveOrder(__instance, dict);
                    }
                }
            }
        }


        [HarmonyPatch(typeof(Buff), nameof(Buff.SelfDestroy))]
        class BossDeathEvents_Patch
        {
            static void RemoveCard(string extendKey)
            {
                while (true)
                {
                    Skill skill = BattleSystem.instance.AllyTeam.Skills_Deck.Find((Skill a) => a.ExtendedFind(extendKey, false) != null);
                    if (skill == null)
                    {
                        break;
                    }
                    else
                    {
                        BattleSystem.instance.AllyTeam.Skills_Deck.Remove(skill);
                    }
                }
                while (true)
                {
                    Skill skill = BattleSystem.instance.AllyTeam.Skills_UsedDeck.Find((Skill a) => a.ExtendedFind(extendKey, false) != null);
                    if (skill == null)
                    {
                        break;
                    }
                    else
                    {
                        BattleSystem.instance.AllyTeam.Skills_UsedDeck.Remove(skill);
                    }
                }
                while (true)
                {
                    Skill skill = BattleSystem.instance.AllyTeam.Skills.Find((Skill a) => a.ExtendedFind(extendKey, false) != null);
                    if (skill == null)
                    {
                        break;
                    }
                    else
                    {
                        BattleSystem.instance.AllyTeam.Skills.Remove(skill);
                    }
                }
            }

            static void Postfix(Buff __instance)
            {
                if (ExpertPlusPlugin.DespairMode.Value)
                {
                    if (__instance is B_Witch_P_0)
                    {
                        foreach (BattleChar b in BattleSystem.instance.AllyTeam.Chars)
                        {
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_P_0_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                            b.BuffRemove("B_Witch_2_T", true);
                        }
                    }
                    else if (__instance is P_DorchiX)
                    {
                        //Revive, heal everyone by 200% (66%)
                        foreach (BattleChar b in BattleSystem.instance.AllyTeam.Chars)
                        {
                            if (b.Info.Incapacitated)
                            {
                                b.Info.Incapacitated = false;
                                b.HP = 1;
                            }
                            int num = (int)Misc.PerToNum((float)b.GetStat.maxhp, 200f);
                            b.Heal(b, (float)num, false);
                        }
                    }
                    else if (__instance is B_Joker_P_0)
                    {
                        // experiment deez
                        InventoryManager.Reward(ItemBase.GetItem(GDEItemKeys.Item_Consume_SodaWater, 1));

                        // Remove Joker Card from deck
                        RemoveCard("SkillExtended_Joker_0");
                    }

                    else if (__instance is P_S2_MainBoss_1_Left) // || __instance is P_S2_MainBoss_1_Right
                    {
                        foreach (BattleChar b in BattleSystem.instance.AllyTeam.Chars)
                        {
                            b.BuffRemove("B_S2_Mainboss_1_LeftDebuff", true);
                            b.BuffRemove("B_S2_Mainboss_1_LeftDebuff", true);
                            b.BuffRemove("B_S2_Mainboss_1_LeftDebuff", true);

                            b.BuffRemove("B_S2_Mainboss_1_RightDebuf", true);
                            b.BuffRemove("B_S2_Mainboss_1_RightDebuf", true);
                            b.BuffRemove("B_S2_Mainboss_1_RightDebuf", true);
                        }


                        // Remove cleanse
                        RemoveCard("Extended_S2_MainBoss_1_Lucy_0");

                    }
                    else if (__instance is P_BombClown_0)
                    {
                        // Remove time bombs
                        RemoveCard("S_BombClown_B_0");
                    }
                    else if (__instance is B_MBoss2_1_P)
                    {
                        foreach (var bc in BattleSystem.instance.AllyTeam.AliveChars)
                        {
                            while (bc.BuffFind(GDEItemKeys.Buff_B_Mboss2_1_P2, false))
                                bc.BuffRemove(GDEItemKeys.Buff_B_Mboss2_1_P2, true);
                        }
                    }
                    else if (__instance is B_S3_Boss_Pope_P_0)
                    {
                        foreach (var bc in BattleSystem.instance.AllyTeam.AliveChars)
                        {
                            while (bc.BuffFind(GDEItemKeys.Buff_B_S3_Pope_P_2, false))
                                bc.BuffRemove(GDEItemKeys.Buff_B_S3_Pope_P_2, true);
                        }
                    }
                    else if (__instance is TheLight_P_1)
                    {
                        foreach (var bc in BattleSystem.instance.AllyTeam.AliveChars)
                        {
                            while (bc.BuffFind(GDEItemKeys.Buff_TheLight_P_0, false))
                                bc.BuffRemove(GDEItemKeys.Buff_TheLight_P_0, true);
                        }

                        RemoveCard("SkillExtended_S_S_TheLight_P_1");

                        // Remove Karaela Burn
                        foreach (BattleChar b in BattleSystem.instance.AllyTeam.Chars)
                        {
                            b.BuffRemove("B_TheLight_2_T", true);
                            b.BuffRemove("B_TheLight_2_T", true);
                            b.BuffRemove("B_TheLight_2_T", true);
                            b.BuffRemove("B_TheLight_2_T", true);
                        }
                    }
                    else if (__instance is B_Enemy_Boss_Reaper_P)
                    {
                        // death sentences can stay ))
                    }


                }
            }
        }



    }
}
