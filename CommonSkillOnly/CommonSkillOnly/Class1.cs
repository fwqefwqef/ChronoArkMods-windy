using BepInEx;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using DarkTonic.MasterAudio;
using System.Collections.Generic;
using BepInEx.Configuration;
using I2.Loc;

namespace CommonSkillOnly
{
    [BepInPlugin(GUID, "Common Skill Onnly Mod", version)]
    [BepInProcess("ChronoArk.exe")]
    public class RareSkillsPlugin : BaseUnityPlugin
    {
        public const string GUID = "windy.commonskillonly";
        public const string version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(GUID);

        private static ConfigEntry<bool> TrinityMode;
        private static ConfigEntry<bool> RareDisable;
        public static List<string> trinity = new List<string> { "S_Public_36", "S_Public_30", "S_Public_0" };
        void Awake()
        {
            TrinityMode = Config.Bind("Generation config", "Trinity Mode", false, "Trinity Mode\nYou can only learn Boomerang, Harden, Surprise Attack. (true/false)");
            RareDisable = Config.Bind("Generation config", "Rare Disable", false, "Rare Disable\nGolden Skillbook is converted into gold when used. Disable rare learn option from Black Market.(true/false)");
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        static List<string> createList(int num) //return list of common keys, no duplicate
        {
            List<string> list = new List<string>();
            for (int i = 0; i < num; i++)
            {
                Debug.Log(i);
                string rand = PlayData.PublicRandomSkill();
                while (list.Contains(rand))
                {
                    rand = PlayData.PublicRandomSkill();
                    Debug.Log("while");
                }
                list.Add(rand);
            }
            return list;
        }

        [HarmonyPatch(typeof(CharacterWindow))]
        class Error_Patch
        {
            [HarmonyPatch(nameof(CharacterWindow.GetRandomSkill))]
            [HarmonyPrefix]
            static bool GetRandomSkill(CharacterWindow __instance, ref List<Skill> __result)
            {
                List<Skill> list = new List<Skill>();
                if (TrinityMode.Value)
                {
                    list.Add(Skill.TempSkill(trinity[0], __instance.AllyCharacter, __instance.AllyCharacter.MyTeam));
                    list.Add(Skill.TempSkill(trinity[1], __instance.AllyCharacter, __instance.AllyCharacter.MyTeam));
                    list.Add(Skill.TempSkill(trinity[2], __instance.AllyCharacter, __instance.AllyCharacter.MyTeam));
                }
                else
                {
                    List<string> skills = createList(3);
                    list.Add(Skill.TempSkill(skills[0], __instance.AllyCharacter, __instance.AllyCharacter.MyTeam));
                    list.Add(Skill.TempSkill(skills[1], __instance.AllyCharacter, __instance.AllyCharacter.MyTeam));
                    list.Add(Skill.TempSkill(skills[2], __instance.AllyCharacter, __instance.AllyCharacter.MyTeam));
                }
                __result = list;
                return false;
            }
        }


        // Modify Skill Booko
        [HarmonyPatch(typeof(UseItem.SkillBookCharacter), new Type[] { })]
        class SB_Patch
        {
            [HarmonyPatch(nameof(UseItem.SkillBookCharacter.Use))]
            [HarmonyPrefix]
            static bool Prefix(UseItem.SkillBookCharacter __instance, ref bool __result)
            {
                //Debug.Log("wuz here2");
                int count = PlayData.TSavedata.Party.Count;
                List<BattleAlly> battleallys = PlayData.Battleallys;
                BattleTeam tempBattleTeam = PlayData.TempBattleTeam;

                //generate random list
                List<Skill> list = new List<Skill>();
                if (TrinityMode.Value)
                {
                    for (int i = 0; i < count; i++)
                    {
                        list.Add(Skill.TempSkill(trinity.Random(), battleallys[i], tempBattleTeam));
                    }
                }
                else
                {
                    List<string> skills = createList(count);
                    for (int i = 0; i < count; i++)
                    {
                        list.Add(Skill.TempSkill(skills[i], battleallys[i], tempBattleTeam));
                    }
                }

                // This part IDC 
                foreach (Skill skill in list)
                {
                    if (!SaveManager.IsUnlock(skill.MySkill.KeyID, SaveManager.NowData.unlockList.SkillPreView))
                    {
                        SaveManager.NowData.unlockList.SkillPreView.Add(skill.MySkill.KeyID);
                    }
                }
                FieldSystem.DelayInput(BattleSystem.I_OtherSkillSelect(list, new SkillButton.SkillClickDel(__instance.SkillAdd), ScriptLocalization.System_Item.SkillAdd, false, true, true, true, true));
                MasterAudio.PlaySound("BookFlip", 1f, default(float?), 0f, null, default(double?), false, false);
                __result = true;
                return false;
            }
        }

        // Modify Infinite Skill Book
        [HarmonyPatch(typeof(UseItem.SkillBookInfinity), new Type[] { })]
        class SBI_Patch
        {
            [HarmonyPatch(nameof(UseItem.SkillBookInfinity.Use))]
            [HarmonyPrefix]
            static bool Use(UseItem.SkillBookInfinity __instance, ref bool __result)
            {
                //Debug.Log("wuz here3");
                List<BattleAlly> battleallys = PlayData.Battleallys;
                BattleTeam tempBattleTeam = PlayData.TempBattleTeam;
                int count = PlayData.TSavedata.Party.Count;

                //generate random list
                List<Skill> list = new List<Skill>();
                if (TrinityMode.Value)
                {
                    for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            list.Add(Skill.TempSkill(trinity[j], PlayData.TSavedata.Party[i].GetBattleChar, PlayData.TempBattleTeam));
                        }
                    }
                }
                else
                {
                    List<string> skills = createList(count * 5);
                    for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            list.Add(Skill.TempSkill(skills[i * 5 + j], PlayData.TSavedata.Party[i].GetBattleChar, PlayData.TempBattleTeam));
                        }
                    }
                }

                // IDC about this part///////////////////////////
                if (list.Count >= 15)
                {
                    Skill skill = list.Random<Skill>();
                    List<Skill_Extended> enforce = PlayData.GetEnforce(false, skill);
                    skill.ExtendedAdd_Battle(enforce.Random<Skill_Extended>());
                }
                foreach (Skill skill2 in list)
                {
                    if (!SaveManager.IsUnlock(skill2.MySkill.KeyID, SaveManager.NowData.unlockList.SkillPreView))
                    {
                        SaveManager.NowData.unlockList.SkillPreView.Add(skill2.MySkill.KeyID);
                    }
                }
                FieldSystem.DelayInput(BattleSystem.I_OtherSkillSelect(list, new SkillButton.SkillClickDel(__instance.SkillAdd), ScriptLocalization.System_Item.SkillAdd, false, true, false, true, true));
                MasterAudio.PlaySound("BookFlip", 1f, default(float?), 0f, null, default(double?), false, false);
                /////////////////////////////////////////////
                __result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(RE_EatingKnowledge))]
        class LibraryRestaurant
        {
            [HarmonyPatch(nameof(RE_EatingKnowledge.SkillReturn))]
            [HarmonyPrefix]
            static bool GetRandomSkill(RE_EatingKnowledge __instance)
            {
                if (TrinityMode.Value)
                {
                    __instance.SkillLists.AddRange(trinity);
                }
                else
                {
                    __instance.SkillLists.AddRange(createList(5));
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(RE_ParosLibrary))]
        class PharosLibrary
        {
            [HarmonyPatch(nameof(RE_ParosLibrary.EventInit))]
            [HarmonyPrefix]
            static bool Event(RE_ParosLibrary __instance)
            {
                if (TrinityMode.Value)
                {
                    if (__instance.Skills.Count == 0)
                    {
                        for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                string text = trinity[j];
                                __instance.Skills.Add(text);
                                __instance.Enforces.Add(PlayData.GetEnforce(false, Skill.TempSkill(text, PlayData.Battleallys[i], PlayData.TempBattleTeam)).Random<Skill_Extended>().Data.Key);
                                __instance.PartyNum.Add(i);
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // Modify Healing 101
        [HarmonyPatch(typeof(UseItem.SkillBookSuport))]
        class SBSupport
        {
            [HarmonyPatch(nameof(UseItem.SkillBookSuport.Use))]
            [HarmonyPrefix]
            static bool Use(UseItem.SkillBookSuport __instance, Character CharInfo, ref bool __result)
            {
                List<Skill> list = new List<Skill>();
                List<BattleAlly> battleallys = PlayData.Battleallys;
                BattleTeam tempBattleTeam = PlayData.TempBattleTeam;
                for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
                {
                    if (CharInfo == PlayData.TSavedata.Party[i])
                    {
                        if (CharInfo.GetData.Role.Key == GDEItemKeys.CharRole_Role_Support)
                        {
                            if (TrinityMode.Value)
                            {
                                list.Add(Skill.TempSkill(trinity[0], battleallys[i], tempBattleTeam));
                                list.Add(Skill.TempSkill(trinity[1], battleallys[i], tempBattleTeam));
                                list.Add(Skill.TempSkill(trinity[2], battleallys[i], tempBattleTeam));
                            }
                            else
                            {
                                List<string> skills = createList(15);
                                List<GDESkillData> characterSkillNoOverLap = PlayData.GetCharacterSkillNoOverLap(PlayData.TSavedata.Party[i], false, null);
                                for (int j = 0; j < characterSkillNoOverLap.Count; j++)
                                {
                                    list.Add(Skill.TempSkill(skills[j], battleallys[i], tempBattleTeam));
                                }

                            }
                        }
                        else
                        {
                            List<GDESkillData> list2 = PlayData.GetCharacterSkillNoOverLap(PlayData.TSavedata.Party[i], false, null);
                            List<GDESkillData> list3 = PlayData.GetCharacterSkillNoOverLap(PlayData.TSavedata.Party[i], true, null);
                            list2 = BattleTeam.Shuffle<GDESkillData>(list2);
                            list3 = BattleTeam.Shuffle<GDESkillData>(list3);
                            for (int k = 0; k < 2; k++)
                            {
                                if (Misc.RandomPer(200, 1))
                                {
                                    list.Add(Skill.TempSkill(list3.Dequeue<GDESkillData>().KeyID, battleallys[i], tempBattleTeam));
                                }
                                else
                                {
                                    list.Add(Skill.TempSkill(list2.Dequeue<GDESkillData>().KeyID, battleallys[i], tempBattleTeam));
                                }
                            }
                        }
                    }
                    else if (CharInfo.GetData.Role.Key != GDEItemKeys.CharRole_Role_Support)
                    {
                        if (Misc.RandomPer(200, 1))
                        {
                            list.Add(Skill.TempSkill(PlayData.GetCharacterSkillNoOverLap(PlayData.TSavedata.Party[i], true, null).Random<GDESkillData>().KeyID, battleallys[i], tempBattleTeam));
                        }
                        else
                        {
                            list.Add(Skill.TempSkill(PlayData.GetCharacterSkillNoOverLap(PlayData.TSavedata.Party[i], false, null).Random<GDESkillData>().KeyID, battleallys[i], tempBattleTeam));
                        }
                    }
                }
                foreach (Skill skill in list)
                {
                    if (!SaveManager.IsUnlock(skill.MySkill.KeyID, SaveManager.NowData.unlockList.SkillPreView))
                    {
                        SaveManager.NowData.unlockList.SkillPreView.Add(skill.MySkill.KeyID);
                    }
                }
                FieldSystem.DelayInput(BattleSystem.I_OtherSkillSelect(list, new SkillButton.SkillClickDel(__instance.SkillAdd), ScriptLocalization.System_Item.SkillAdd, false, true, true, true, false));
                MasterAudio.PlaySound("BookFlip", 1f, null, 0f, null, null, false, false);
                __result = true;

                return false;
            }
        }


        // Modify Golden Skill Book
        [HarmonyPatch(typeof(UseItem.SkillBookCharacter_Rare), new Type[] { })]
        class SBG_Patch
        {
            [HarmonyPatch(nameof(UseItem.SkillBookCharacter_Rare.Use))]
            [HarmonyPrefix]
            static bool Use(UseItem.SkillBookCharacter_Rare __instance)
            {
                if (RareDisable.Value)
                {
                    InventoryManager.Reward(ItemBase.GetItem(GDEItemKeys.Item_Misc_Gold, 1000));
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // Disable rare learn in black market
        [HarmonyPatch(typeof(RandomEventBaseScript))]
        class MedTent_Patch
        {
            [HarmonyPatch(nameof(RandomEventBaseScript.EventOpen_Base))]
            [HarmonyPostfix]
            static void Postfix(RandomEventBaseScript __instance)
            {
                if (RareDisable.Value)
                {
                    if (__instance is RE_BlackMarket)
                    {
                        //Debug.Log("Here");
                        __instance.ButtonOff(0);
                    }
                }

            }
        }


    }
}

