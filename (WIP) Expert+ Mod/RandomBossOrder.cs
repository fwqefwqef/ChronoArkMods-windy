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
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace ExpertPlusMod
{
    public class RandomBossOrder
    {


        [HarmonyPatch(typeof(BattleSystem), "BattleStart")]
        class RandomizeQueues_Patch
        {
            static void Prefix()
            {
                var queuesToRandomize = new HashSet<string>() {
                    GDEItemKeys.EnemyQueue_Queue_Witch,
                    GDEItemKeys.EnemyQueue_Queue_S2_Joker,
                    GDEItemKeys.EnemyQueue_Queue_S2_MainBoss_Luby,
                    GDEItemKeys.EnemyQueue_Queue_S3_PharosLeader,
                    GDEItemKeys.EnemyQueue_Queue_DorchiX
                };
                if (queuesToRandomize.Contains(PlayData.BattleQueue))
                {
                    var q = (Dictionary<string, object>)GDEDataManager.masterData[PlayData.BattleQueue];
                    var waves = new List<List<object>>();
                    waves.Add((List<object>)q["Enemys"]);

                    if (((List<object>)q["Wave2"]).Count > 0)
                        waves.Add((List<object>)q["Wave2"]);
                    if (((List<object>)q["Wave3"]).Count > 0)
                        waves.Add((List<object>)q["Wave3"]);


                    waves = Misc.Shuffle(waves);
                    q["Enemys"] = waves[0];
                    if (waves.Count >= 2)
                        q["Wave2"] = waves[1];
                    if (waves.Count >= 3)
                        q["Wave3"] = waves[2];


                }
            }

        }



        [HarmonyPatch(typeof(GDEEnemyData), nameof(GDEEnemyData.LoadFromDict))]
        class GDEEnemyData_Patch
        {
            static void Postfix(GDEEnemyData __instance, Dictionary<string, object> dict)
            {
                if (__instance.Key == GDEItemKeys.Enemy_S2_Joker)
                {
                    __instance.AI = typeof(CustomJokerAI).AssemblyQualifiedName;
                }
                else if (__instance.Key == GDEItemKeys.Enemy_S2_BombClownBoss)
                {
                    __instance.AI = typeof(CustomBombClownAI).AssemblyQualifiedName;

                }

            }
        }





        public class CustomJokerAI : AI_Joker
        {

            int spawnTurn = 0;
            public override Skill SkillSelect(int ActionCount)
            {

                if (spawnTurn == 0)
                    spawnTurn = BattleSystem.instance.TurnNum;

                if (BattleSystem.instance.EnemyList.Count <= 2 && (BattleSystem.instance.TurnNum == spawnTurn
                    || BattleSystem.instance.TurnNum == spawnTurn + 3
                    || BattleSystem.instance.TurnNum == spawnTurn + 6))
                {
                    return Skill.TempSkill(GDEItemKeys.Skill_S_Joker_2, this.BChar, this.BChar.MyTeam);
                }
                return base.SkillSelect(ActionCount);
            }
        }

        public class CustomBombClownAI : AI_BomeClown
        {
            int spawnTurn = 0;

            public override Skill SkillSelect(int ActionCount)
            {

                if (spawnTurn == 0)
                    spawnTurn = BattleSystem.instance.TurnNum;

                if (ActionCount == 0)
                {
                    if (BattleSystem.instance.EnemyList.Count <= 2 && (BattleSystem.instance.TurnNum == spawnTurn 
                        || BattleSystem.instance.TurnNum == spawnTurn + 3 
                        || BattleSystem.instance.TurnNum == spawnTurn + 6))
                    {
                        return this.BChar.Skills[1];
                    }
                    return this.BChar.Skills[2];
                }
                else
                {
                    if (ActionCount == 1 || ActionCount == 2)
                    {
                        return this.BChar.Skills[0];
                    }
                    return base.SkillSelect(ActionCount);
                }
            }

        }

        [HarmonyPatch(typeof(P_S2_MainBoss_1), nameof(P_S2_MainBoss_1.Init))]
        class P_S2_MainBoss_1_Patch
        {
            // add cleanse
            static void Postfix(P_S2_MainBoss_1 __instance)
            {
                if (__instance is P_S2_MainBoss_1_Left)
                {
                    BattleSystem.instance.AllyTeam.Add(Skill.TempSkill(GDEItemKeys.Skill_S_S2_MainBoss_1_Lucy_0, BattleSystem.instance.AllyTeam.LucyChar, BattleSystem.instance.AllyTeam), true);
                }
            }
        }




        [HarmonyPatch(typeof(P_S2_MainBoss_1_Left), nameof(P_S2_MainBoss_1_Left.Turn1))]
        class P_S2_MainBoss_1_Left_Patch
        {
            // remove cleanse addition on turn1
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                var nopMode = true;
                foreach (var ci in instructions)
                {

                    if (ci.opcode == OpCodes.Newobj)
                    {
                        nopMode = false;
                    }
                    if (nopMode)
                    {
                        yield return new CodeInstruction(OpCodes.Nop);
                    }
                    else
                    {
                        yield return ci;
                    }
                }
            }

        }






        [HarmonyPatch(typeof(Buff), nameof(Buff.Init))]
        class Buff_Patch
        {
            static void Postfix(Buff __instance)
            {
                if(__instance is B_Enemy_Boss_Reaper_P)
                {
                    
                    if(BattleSystem.instance != null && BattleSystem.instance.TurnNum >= 2)
                    {
                        UnityEngine.Object.Instantiate<GameObject>(new GDEGameobjectDatasData(GDEItemKeys.GameobjectDatas_GUI_ReaperBattleUI).Gameobject, BattleSystem.instance.MainUICanvas.transform);
                    
                    }

                }
            }

        }





        [HarmonyPatch(typeof(Buff), nameof(Buff.SelfDestroy))]
        class BossDeathEvents_Patch
        {
            static void RemoveCards(string extendKey)
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
                        skill.MyButton.Waste();
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
                            if (!ExpertPlusPlugin.hardTransitions.Value)
                            {
                                // Remove Witch Curse from hand
                                while (true)
                                {
                                    Skill skill = BattleSystem.instance.AllyTeam.Skills.Find((Skill a) => (a.AP == 0 && a.isExcept == true && a.NoExchange == true));
                                    Debug.Log("ayy");
                                    if (skill == null)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        skill.MyButton.Waste();
                                    }
                                }
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
                            else
                            {
                                // reduces debuff lifetime rather than outright removing it
                                if (b.BuffFind("B_Witch_P_0_T", false))
                                    foreach (var si in b.BuffReturn("B_Witch_P_0_T").StackInfo)
                                        si.RemainTime = Math.Min(si.RemainTime, 2);

                                if (b.BuffFind("B_Witch_2_T", false))
                                {
                                    var buff = b.BuffReturn("B_Witch_2_T");
                                    buff.TimeUseless = false;
                                    buff.CantDisable = false;
                                    foreach (var si in buff.StackInfo)
                                        si.RemainTime = 2;
                                }
                                    
                            }
                        }
                    }
                    else if (__instance is P_DorchiX)
                    {
                        if (!ExpertPlusPlugin.hardTransitions.Value)
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
                    }
                    else if (__instance is B_Joker_P_0)
                    {
                        if (!ExpertPlusPlugin.hardTransitions.Value)
                        {
                            // Remove Joker Card from deck
                            RemoveCards("SkillExtended_Joker_0");
                        }
                        else
                        {
                            // rewards pills immediately 
                            InventoryManager.Reward(ItemBase.GetItem(GDEItemKeys.Item_Consume_SodaWater, 1));
                        }
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
                        RemoveCards("Extended_S2_MainBoss_1_Lucy_0");

                    }
                    else if (__instance is P_BombClown_0)
                    {
                        if (!ExpertPlusPlugin.hardTransitions.Value)
                        {
                            // Remove time bombs
                            RemoveCards("S_BombClown_B_0"); 
                        }
                    }
                    // timer yeater
                    else if (__instance is B_MBoss2_1_P)
                    {
                        // remove amplify time
                        // skill countdown still remains until used
                        foreach (var bc in BattleSystem.instance.AllyTeam.AliveChars)
                        {
                            while (bc.BuffFind(GDEItemKeys.Buff_B_Mboss2_1_P2, false))
                                bc.BuffRemove(GDEItemKeys.Buff_B_Mboss2_1_P2, true);
                        }
                    }
                    else if (__instance is B_S3_Boss_Pope_P_0)
                    {
                        if (!ExpertPlusPlugin.hardTransitions.Value)
                        {
                            foreach (var bc in BattleSystem.instance.AllyTeam.AliveChars)
                            {
                                while (bc.BuffFind(GDEItemKeys.Buff_B_S3_Pope_P_2, false))
                                    bc.BuffRemove(GDEItemKeys.Buff_B_S3_Pope_P_2, true);
                            } 
                        }
                    }
                    else if (__instance is TheLight_P_1)
                    {
                        foreach (var bc in BattleSystem.instance.AllyTeam.AliveChars)
                        {
                            while (bc.BuffFind(GDEItemKeys.Buff_TheLight_P_0, false))
                                bc.BuffRemove(GDEItemKeys.Buff_TheLight_P_0, true);
                        }

                        RemoveCards("SkillExtended_S_S_TheLight_P_1");

                        if (!ExpertPlusPlugin.hardTransitions.Value)
                        {
                            // Remove Karaela Burn
                            foreach (BattleChar b in BattleSystem.instance.AllyTeam.Chars)
                            {
                                b.BuffRemove("B_TheLight_2_T", true);
                                b.BuffRemove("B_TheLight_2_T", true);
                                b.BuffRemove("B_TheLight_2_T", true);
                                b.BuffRemove("B_TheLight_2_T", true);
                            } 
                        }
                    }
                    else if (__instance is B_Enemy_Boss_Reaper_P)
                    {
                        var markTransform = BattleSystem.instance.MainUICanvas.transform.Find("ReaperBossUI(Clone)");
                        if (markTransform != null)
                        {
                            GameObject.Destroy(markTransform.gameObject);
                        }


                        // death sentences can stay ))
                    }


                }
            }
        }



    }
}
