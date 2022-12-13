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

namespace Alternative_ShadowCurtain
{
    [BepInPlugin(GUID, "Skills Patch", version)]
    [BepInProcess("ChronoArk.exe")]
    public class RareSkillsPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.chronoark.cardmod.skillbalance";
        public const string version = "1.1.0";
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

        // Modify gdata.json
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
                        //Dark Sun and Dark Moon: Gain Tracking
                        if (e.Key == "S_TW_Red_6")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["Track"] = true;
                        }

                        if (e.Key == "S_TW_Blue_8")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["Track"] = true;
                        }

                        // Storming Blade: Gain Tracking
                        if (e.Key == "S_Azar_5")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["Track"] = true;
                        }

                        //Time to Move!: Gain swiftness
                        if (e.Key == "S_Sizz_6")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["NotCount"] = true;
                        }


                        // Godo HP increase
                        if (e.Key == "SR_GunManBoss")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["maxhp"] = 4000;
                        }

                        //// Time to move!: Added Swiftness
                        //if (e.Key == "S_Sizz_6")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NotCount"] = true;
                        //}

                        ////Thread of Life: Removed Swiftness and increased heal ratio by 55->70%, cannot be fixed
                        //if (e.Key == "S_Sizz_9")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NotCount"] = false;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NoBasicSkill"] = true;
                        //}
                        //if (e.Key == "SE_Sizz_9_T")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["HEAL_Per"] = 70;
                        //}

                        //// Sonic Blow: Added Swiftness
                        //if (e.Key == "S_Mement_3")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NotCount"] = true;
                        //}

                        //// Forbidden Flame: lasts 2 turns
                        //if (e.Key == "S_ShadowPriest_12_0")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["AutoDelete"] = 2;
                        //}

                        //// Leer: Taunts all enemies, cc accuracy +20%
                        //if (e.Key == "S_Phoenix_2")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["Target"] = "all_enemy";
                        //}

                        //if (e.Key == "SE_Phoenix_2_T")
                        //{
                        //    List<int> se = new List<int>();
                        //    se.Add(50);
                        //    se.Add(50);
                        //    (masterJson[e.Key] as Dictionary<string, object>)["BuffPlusTagPer"] = se;
                        //}

                        // Face Slap: gain 100% crit chance, cost increased to 0->1
                        //if (e.Key == "S_Phoenix_11")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseAp"] = 1;
                        //}

                        //if (e.Key == "SE_Phoenix_11_T")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["CRI"] = 100;
                        //}

                        ////Shining Aura: Cost reduced to 0, cannot be fixed. 
                        //if (e.Key == "S_Azar_11")
                        //{
                        //    //List<String> se = new List<String>();
                        //    //se.Add("SkillEn_Draw");
                        //    //(masterJson[e.Key] as Dictionary<string, object>)["SkillExtended"] = se;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseAp"] = 0;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NoBasicSkill"] = true;
                        //    //Debug.Log("Shining Aura draw");
                        //}

                        //Hein Rage: targets only self, mana + 2.
                        //if (e.Key == "S_Hein_6")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["Target"] = "self";
                        //    List<String> se = new List<String>();
                        //    se.Add("Extended_MPPotion");
                        //    (masterJson[e.Key] as Dictionary<string, object>)["SkillExtended"] = se;

                        //    //Debug.Log("updooted");
                        //}

                        // Marionette: Cost reduced to 1->0
                        //if(e.Key == "S_Sizz_10")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseAp"] = 0;
                        //}

                        // Double Armor: Cost reduced to 1->0
                        //if (e.Key == "S_Prime_8")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseAp"] = 0;
                        //}

                        // Eve, Help!: Added Swiftness, decreased healing to 35->25%
                        //if (e.Key == "S_Sizz_0")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NotCount"] = true;
                        //}

                        //if (e.Key == "SE_Sizz_0_T")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["HEAL_Per"] = 25;
                        //}

                        // Tracking Scope: Cost increased to 0->1
                        //if (e.Key == "S_Mement_5")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseAp"] = 1;
                        //}

                        // Shadow Step: give evade to self
                        //if (e.key == "se_trisha_6_s")
                        //{
                        //    list<string> se = new list<string>();
                        //    se.add("b_camouflagecloak");
                        //    (masterjson[e.key] as dictionary<string, object>)["buffs"] = se;
                        //    //debug.log("updooted");
                        //}

                        //// shadow step: give taunt to enemy
                        //if (e.key == "se_trisha_6_t")
                        //{
                        //    list<string> se = new list<string>();
                        //    se.add("b_taunt");
                        //    (masterjson[e.key] as dictionary<string, object>)["buffs"] = se;
                        //    //debug.log("updooted");
                        //}

                        // Blazing Regeneration: cost reduced to 1
                        //if (e.Key == "S_Phoenix_1")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseAp"] = 1;
                        //}

                        // Zoom: 100% -> 125% attack
                        //if (e.Key == "SE_MissChain_T_0")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["DMG_Per"] = 125;
                        //}

                        //Sleep...: Gain Once, Ignore Taunt, Cost 2->1, Can be fixed
                        //if (e.Key == "S_Control_6")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["UseAp"] = 1;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["Disposable"] = true;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["IgnoreTaunt"] = true;
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NoBasicSkill"] = false;
                        //}

                        //Solarbolt: 163% -> 220%, Gain Bind (Cannot be exchanged)
                        //if (e.Key == "SE_TW_Red_1_T")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["DMG_Per"] = 220;
                        //}

                        //if (e.Key == "S_TW_Red_1")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["NotChuck"] = true;
                        //}

                        //Flame Arrow: 100 -> 120%
                        //if (e.Key == "SE_Tw_Red_0_T")
                        //{
                        //    (masterJson[e.Key] as Dictionary<string, object>)["DMG_Per"] = 120;
                        //}

                    }
                }
                dataString = Json.Serialize(masterJson);
            }
        }

        [HarmonyPatch(typeof(ArkCode), "Start")]
        class TimeScale2xPatch
        {
            static void Postfix()
            {
                Time.timeScale = 3f;
                //Debug.Log("Sonic Speed");
            }
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

       //Helia Selena Split
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

        //Gives +4HP to Burning Night. 불타는 밤 체력 +4
        //[HarmonyPatch(typeof(EItem.FlameShieldGenerator))]
        // class BurningNight_Patch
        // {
        //     [HarmonyPatch(nameof(EItem.FlameShieldGenerator.Init))]
        //     [HarmonyPostfix]
        //     static void Postfix(EItem.FlameShieldGenerator __instance)
        //     {
        //         __instance.PlusStat.maxhp = 4;
        //     }
        // }

        //___MySkill.AP = 10; // mana cost. ___MySkill.MySkill:int _UseAp is 'default' mana cost.
        //_UseAp should have the same value as AP for mana number icon to be displayed in correct colour. However mana cost is still not updated in encyclopedia
        //___MySkill.IsWaste = true; // cannot exchange keyword


        //___MySkill.NotCount = true; // swiftness keyword
        //___MySkill.NotChuck = true; // bind keyword(no cycling)
        //___MySkill.isExcept = true; // except keyword (if combined with 'once' only 'except' will be displayed)

        // Reduces Blade Starfall's Countdown to 1. 
        //[HarmonyPatch(typeof(PassiveBase), nameof(PassiveBase.Init))]
        //class BladeStarFall_Patch
        //{
        //    static void Postfix(PassiveBase __instance)
        //    {
        //        if (__instance is Extended_Azar_2_New)
        //        {
        //            Skill_Extended se = (Skill_Extended)__instance;
        //            se.MySkill.Counting = 1;
        //        }
        //    }
        //}

        //Rage pain damage reduced - Not working!!!
        //[HarmonyPatch(typeof(B_Hein_S_6))]
        //class RageBuff_Patch
        //{
        //    [HarmonyPatch(nameof(B_Hein_S_6.SKillUseHand_Team))]
        //    [HarmonyPrefix]
        //    static bool RageBuffPrefix(Skill skill, Buff __instance)
        //    {
        //        Buff b = (B_Hein_S_6)__instance;
        //        if (skill.Master == b.BChar)
        //        {
        //            //changed to 25% pain instead of 50%
        //            b.BChar.Damage(__instance.BChar, (int)((float)b.BChar.GetStat.maxhp * 0.25f), false, true, false, 0, false, false);
        //            BattleSystem.DelayInputAfter(BattleSystem.instance.SkillRandomUseIenum(skill.Master, skill.CloneSkill(true, skill.Master, null), false, false, false));
        //            __instance.SelfDestroy();
        //        }
        //        return false;
        //    }
        //}
        //    [HarmonyPatch(nameof(Buff.DescExtended), new Type[] { })]
        //    [HarmonyPostfix]
        //    static void DescExtendedPostfix(ref string __result, Buff __instance)
        //    {
        //        if (__instance is B_Hein_S_6)
        //        {
        //            __result = "Gain 2 mana this turn and remove overload.\nThe next skill you play will cast twice but will cost 50% of your Maximum Health as <color=purple>Pain damage</color>.";
        //        }
        //    }
        //}

        // Increases Dark Sanctuary duration to 3 turns
        //[HarmonyPatch(typeof(Buff))]
        //    class DarkSanctuary_Patch
        //    {
        //        [HarmonyPatch(nameof(Buff.Init))]
        //        [HarmonyPostfix]
        //        static void InitPostfix(Buff __instance)
        //        {
        //            if (__instance is B_ShadowPriest_13_T)
        //            {
        //                __instance.StackInfo[0].RemainTime = 3;
        //            }
        //        }
        //    }

        // Shining Aura Buff: gain armor pen == Disabled for now
        /*[HarmonyPatch(typeof(Azar_11_Ex))]
        class ShiningAura_Patch
        {
            [HarmonyPatch(nameof(Azar_11_Ex.Init))]
            [HarmonyPostfix]
            static void Postfix(Azar_11_Ex __instance)
            {
                __instance.PlusSkillStat.Penetration = 10f;
            }
        }*/

        //Change Shining Aura Desc
        //[HarmonyPatch(typeof(Buff))]
        //class ShiningAuraDesc
        //{
        //    [HarmonyPatch(nameof(Buff.DescExtended), new Type[] { })]
        //    [HarmonyPostfix]
        //    static void DescExtendedPostfix(ref string __result, Buff __instance)
        //    {
        //        if (__instance is B_Azar_11_T)
        //        {
        //            __result = "Draw 1 Skill.\nSkills that cost 0 gain 100% critical hit chance.";
        //        }
        //    }
        //}

        // Nibble Absurdity: gain a little more attack power, lose healing power
        //[HarmonyPatch(typeof(B_Phoenix_7_T))]
        //class Nibble_Patch
        //{
        //    [HarmonyPatch(nameof(B_Phoenix_7_T.Init))]
        //    [HarmonyPostfix]
        //    static void Postfix(B_Phoenix_7_T __instance)
        //    {
        //        __instance.PlusPerStat.Damage = 20;
        //        __instance.PlusPerStat.Heal = 30;
        //    }
        //}

        // Restrained Healing Buff: Greatly increases Aggro
        //[HarmonyPatch(typeof(B_Queen_9_T))]
        //class Restrained_Patch
        //{
        //    [HarmonyPatch(nameof(B_Queen_9_T.Init))]
        //    [HarmonyPostfix]
        //    static void Postfix(B_Queen_9_T __instance)
        //    {
        //        __instance.PlusStat.AggroPer = 80;
        //    }
        //}

        // Combat Roar: gives every skill in hand 100% crit
        //[HarmonyPatch(typeof(S_Lian_4))]
        //class CombatRoar_Patch
        //{
        //    [HarmonyPatch(nameof(S_Lian_4.SkillUseSingle))]
        //    [HarmonyPostfix]
        //    static void Postfix(S_Lian_4 __instance, Skill SkillD, List<BattleChar> Targets)
        //    {
        //        List<Skill> list = new List<Skill>();
        //        list.AddRange(BattleSystem.instance.AllyTeam.Skills);
        //        list.Remove(__instance.MySkill);
        //        // Prevent double increase
        //        if (list.Count > 5)
        //        {
        //            for (int i = 5; i < list.Count; i++)
        //            {
        //                Extended_Cri100 extended = new Extended_Cri100();
        //                list[i].ExtendedAdd(extended);
        //            }
        //        }
        //    }
        //}

        ////Change Combat Roar Desc
        //[HarmonyPatch(typeof(Skill_Extended))]
        //class RoarDesc_Patch
        //{
        //    [HarmonyPatch(nameof(Skill_Extended.DescExtended))]
        //    [HarmonyPostfix]
        //    static void DescExtendedPostfix(ref string __result, Skill_Extended __instance)
        //    {
        //        if (__instance is S_Lian_4)
        //        {
        //            __result = "Give every skill in your hand 100% crit. chance.";
        //        }
        //    }
        //}

        // Imitate Buff: -80% -> -66%, change description
        //[HarmonyPatch(typeof(S_Mement_4))]
        //class Imitate_Patch
        //{
        //    [HarmonyPatch(nameof(S_Mement_4.SkillUseSingle))]
        //    [HarmonyPrefix]
        //    static bool SkillUseSingle(Skill SkillD, List<BattleChar> Targets, S_Mement_4 __instance)
        //    {
        //        List<Skill> list = new List<Skill>();
        //        List<GDESkillData> list2 = new List<GDESkillData>();
        //        using (List<GDESkillData>.Enumerator enumerator = PlayData.ALLSKILLLIST.GetEnumerator())
        //        {
        //            while (enumerator.MoveNext())
        //            {
        //                GDESkillData i = enumerator.Current;
        //                if (BattleSystem.instance.AllyTeam.AliveChars.Find((BattleChar a) => a.Info.KeyData == i.User) != null && !i.Disposable && i.Effect_Target.DMG_Per >= 1)
        //                {
        //                    list2.Add(i);
        //                }
        //            }
        //        }
        //        Skill skill = Skill.TempSkill(list2.Random<GDESkillData>().KeyID, __instance.BChar, __instance.BChar.MyTeam);
        //        BattleSystem.instance.AllyTeam.Add(skill, true);
        //        skill.isExcept = true;
        //        skill._AP = 1;
        //        Skill_Extended skill_Extended = Skill_Extended.DataToExtended(GDEItemKeys.SkillExtended_Mement_4_Ex);

        //        // change is here
        //        skill_Extended.PlusSkillPerStat.Damage = -(int)Misc.PerToNum((float)skill.MySkill.Effect_Target.DMG_Per, 66f);

        //        skill.ExtendedAdd_Battle(skill_Extended);
        //        BuffTag buffTag = new BuffTag();
        //        buffTag.BuffData = new GDEBuffData(GDEItemKeys.Buff_B_Mement_P_S);
        //        buffTag.User = __instance.BChar;
        //        skill_Extended.SelfBuff.Add(buffTag);

        //        return false;
        //    }
        //}

        //Change Imitate Desc
        //[HarmonyPatch(typeof(Skill_Extended))]
        //class ImitateDesc_Patch
        //{
        //    [HarmonyPatch(nameof(Skill_Extended.DescExtended))]
        //    [HarmonyPostfix]
        //    static void DescExtendedPostfix(ref string __result, Skill_Extended __instance)
        //    {
        //        if (__instance is S_Mement_4)
        //        {
        //            __result = "Add a random ally attack skill to hand and give it Exclude. Its cost becomes 1 and it gains a Supply Arrows buff but its damage is reduced by 66%.";
        //        }
        //    }
        //}

        // Pain Equals Happiness: Duration increased
        //[HarmonyPatch(typeof(Buff))]
        //class PEH_Buff_Patch
        //{
        //    [HarmonyPatch(nameof(Buff.Init))]
        //    [HarmonyPostfix]
        //    static void InitPostfix(Buff __instance)
        //    {
        //        if (__instance is B_Queen_10_T)
        //        {
        //            __instance.StackInfo[0].RemainTime = 3;
        //        }
        //    }
        //}


        // HP Change for Godo
        [HarmonyPatch(typeof(P_Gunman), "HPChange")]
        class GodoPatch
        {
            static bool Prefix(P_Gunman __instance)
            {
                if (__instance.MainAI.Phase == 1 && __instance.BChar.HP <= 2500)
                {
                    __instance.BChar.Info.Hp = 2500;
                    __instance.MainAI.Phase = 2;
                    __instance.BChar.BuffAdd(GDEItemKeys.Buff_B_GunmanBoss_Phase2, __instance.BChar, false, 0, false, -1, false);
                }
                return false;
            }
        }

        //[HarmonyPatch(typeof(S_TW_Red_6))]
        //class DarkSunTest
        //{
        //    [HarmonyPatch(nameof(S_TW_Red_6.SkillUseSingle))]
        //    [HarmonyPrefix]
        //    static bool SkillUseSingle(Skill SkillD, List<BattleChar> Targets, S_TW_Red_6 __instance)
        //    {
        //        List<Skill> list = new List<Skill>();
        //        list.AddRange(BattleSystem.instance.AllyTeam.Skills);

        //        Debug.Log("Before removing skills");
        //        foreach (Skill s in list)
        //        {
        //            Debug.Log(s.AP);
        //        }

        //        list.Remove(__instance.MySkill);

        //        Debug.Log("After removing skills");
        //        foreach (Skill s in list)
        //        {
        //            Debug.Log(s.AP);
        //        }

        //        if (list.Count != 0)
        //        {
        //            int ap = list[0].AP;
        //            Debug.Log("list[0] AP: " + ap);

        //            __instance.SkillBasePlus.Target_BaseDMG = (int)((float)ap * (__instance.BChar.GetStat.atk * 0.5f));
        //            Debug.Log("DMG: " + (int)((float)ap * (__instance.BChar.GetStat.atk * 0.5f)));

        //            Skill temp = list[0];
        //            BattleSystem.DelayInput(__instance.Insert(temp));
        //        }
        //        return false;
        //    }
        //}



        /*        class HellArrow_Patch
                {
                    [HarmonyPatch(nameof(S_Mement_R1.DescExtended))]
                    [HarmonyPostfix]
                    static void InitPostfix(Skill __instance)
                    {
                        Debug.Log(__instance.AllExtendeds.Count);
                        for (int i = 0; i < __instance.AllExtendeds.Count; i++)
                        {
                            Debug.Log(__instance.AllExtendeds[i]);
                            if (__instance.AllExtendeds[i] is S_Mement_R1)
                            {
                                __instance.AP = 3;
                                Debug.Log("aa");
                            }
                        }
                    }
                }*/

    }
}