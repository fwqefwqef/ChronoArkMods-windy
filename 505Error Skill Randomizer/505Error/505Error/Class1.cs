using BepInEx;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using DarkTonic.MasterAudio;
using System.Collections.Generic;
using BepInEx.Configuration;
using I2.Loc;

namespace FiveErrorMod
{
    [BepInPlugin(GUID, "505Error Mod", version)]
    [BepInProcess("ChronoArk.exe")]
    public class RareSkillsPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.chronoark.cardmod.randomskillmod";
        public const string version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(GUID);

        private static ConfigEntry<bool> ChaosMode;
        void Awake()
        {
            ChaosMode = Config.Bind("Generation config", "Chaos Mode", false, "Include every existing skill in the game into the selection pool. Gets silly quickly. (true/false)");
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        // modify 505Error
        [HarmonyPatch(typeof(CharacterWindow))]
        class Error_Patch
        {
            [HarmonyPatch(nameof(CharacterWindow.GetRandomSkill))]
            [HarmonyPrefix]
            static bool GetRandomSkill(CharacterWindow __instance, ref List<Skill> __result)
            {
                //Debug.Log("I wuz here");
                // Accounting for Relic that adds 4 choices instead of 3
                int num;
                if (PlayData.Passive.Find((Item_Passive passive) => passive.itemkey == GDEItemKeys.Item_Passive_OldRule) != null)
                {
                    num = 4;
                }
                else
                {
                    num = 3;
                }

                List<GDESkillData> list2 = new List<GDESkillData>();
                foreach (GDESkillData gdeskillData2 in PlayData.ALLSKILLLIST)
                {
                    if (!ChaosMode.Value)
                    {
                        if (gdeskillData2.User != string.Empty && gdeskillData2.Category.Key != GDEItemKeys.SkillCategory_LucySkill && gdeskillData2.Category.Key != GDEItemKeys.SkillCategory_DefultSkill && !gdeskillData2.NoDrop && !gdeskillData2.Lock)
                        {
                            GDECharacterData gdecharacterData = new GDECharacterData(gdeskillData2.User);
                            if (!(gdeskillData2.KeyID == GDEItemKeys.Skill_S_Phoenix_6) && !(gdeskillData2.Key == GDEItemKeys.Skill_S_Phoenix_6))
                            {
                                if (gdecharacterData != null)
                                {
                                    if (!gdecharacterData.Lock)
                                    {
                                        list2.Add(gdeskillData2);
                                    }
                                    else if (SaveManager.IsUnlock(gdeskillData2.User, SaveManager.NowData.unlockList.UnlockCharacter))
                                    {
                                        list2.Add(gdeskillData2);
                                    }
                                }
                            }
                        }
                    }
                    else //if Chaos Mode == true
                    {
                        list2.Add(gdeskillData2);
                    }
                }
                
                List<GDESkillData> list3 = new List<GDESkillData>();
                List<Skill> list4 = new List<Skill>();
                list3.AddRange(list2.Random(num));
                foreach (GDESkillData gdeskillData3 in list3)
                {
                    list4.Add(Skill.TempSkill(gdeskillData3.Key, __instance.AllyCharacter, __instance.AllyCharacter.MyTeam));
                }
                // returning list4 means that the original 3 skills won't be returned
                __result = list4;
                return false;
            }
        }


        // Modify Skill Booko
        [HarmonyPatch(typeof(UseItem.SkillBookCharacter), new Type[] {})]
        class SB_Patch
        {
            [HarmonyPatch(nameof(UseItem.SkillBookCharacter.Use))]
            [HarmonyPrefix]
            static bool Prefix(UseItem.SkillBookCharacter __instance, ref bool __result)
            {
                //Debug.Log("wuz here2");
                int count = PlayData.TSavedata.Party.Count;
                List<Skill> list = new List<Skill>();
                List<BattleAlly> battleallys = PlayData.Battleallys;
                BattleTeam tempBattleTeam = PlayData.TempBattleTeam;

                //generate random list
                List<GDESkillData> list2 = new List<GDESkillData>();
                foreach (GDESkillData gdeskillData2 in PlayData.ALLSKILLLIST)
                {
                    if (!ChaosMode.Value)
                    {
                        if (gdeskillData2.User != string.Empty && gdeskillData2.Category.Key != GDEItemKeys.SkillCategory_LucySkill && gdeskillData2.Category.Key != GDEItemKeys.SkillCategory_DefultSkill && !gdeskillData2.NoDrop && !gdeskillData2.Lock)
                        {
                            GDECharacterData gdecharacterData = new GDECharacterData(gdeskillData2.User);
                            if (!(gdeskillData2.KeyID == GDEItemKeys.Skill_S_Phoenix_6) && !(gdeskillData2.Key == GDEItemKeys.Skill_S_Phoenix_6))
                            {
                                if (gdecharacterData != null)
                                {
                                    if (!gdecharacterData.Lock)
                                    {
                                        list2.Add(gdeskillData2);
                                    }
                                    else if (SaveManager.IsUnlock(gdeskillData2.User, SaveManager.NowData.unlockList.UnlockCharacter))
                                    {
                                        list2.Add(gdeskillData2);
                                    }
                                }
                            }
                        }
                    }
                    else //if Chaos Mode == true
                    {
                        list2.Add(gdeskillData2);
                    }
                }

                // pop skills equal to the number of party members
                List<GDESkillData> a = list2.Random(PlayData.TSavedata.Party.Count);

                // add skills
                for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
                {
                    list.Add(Skill.TempSkill(a[i].KeyID, battleallys[i], tempBattleTeam));
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
                List<Skill> list = new List<Skill>();
                List<BattleAlly> battleallys = PlayData.Battleallys;
                BattleTeam tempBattleTeam = PlayData.TempBattleTeam;


                //generate random list
                List<GDESkillData> list2 = new List<GDESkillData>();
                foreach (GDESkillData gdeskillData2 in PlayData.ALLSKILLLIST)
                {
                    if (!ChaosMode.Value)
                    {
                        if (gdeskillData2.User != string.Empty && gdeskillData2.Category.Key != GDEItemKeys.SkillCategory_LucySkill && gdeskillData2.Category.Key != GDEItemKeys.SkillCategory_DefultSkill && !gdeskillData2.NoDrop && !gdeskillData2.Lock)
                        {
                            GDECharacterData gdecharacterData = new GDECharacterData(gdeskillData2.User);
                            if (!(gdeskillData2.KeyID == GDEItemKeys.Skill_S_Phoenix_6) && !(gdeskillData2.Key == GDEItemKeys.Skill_S_Phoenix_6))
                            {
                                if (gdecharacterData != null)
                                {
                                    if (!gdecharacterData.Lock)
                                    {
                                        list2.Add(gdeskillData2);
                                    }
                                    else if (SaveManager.IsUnlock(gdeskillData2.User, SaveManager.NowData.unlockList.UnlockCharacter))
                                    {
                                        list2.Add(gdeskillData2);
                                    }
                                }
                            }
                        }
                    }
                    else //if Chaos Mode == true
                    {
                        list2.Add(gdeskillData2);
                    }
                }
                // pop skills equal to the number of party members x 5
                List<GDESkillData> a = list2.Random(PlayData.TSavedata.Party.Count*5);
                int k = 0;
                for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        list.Add(Skill.TempSkill(a[k].KeyID, PlayData.TSavedata.Party[i].GetBattleChar, PlayData.TempBattleTeam));
                        k++;
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


        // Modify Golden Skill Book
        [HarmonyPatch(typeof(UseItem.SkillBookCharacter_Rare), new Type[] { })]
        class SBG_Patch
        {
            [HarmonyPatch(nameof(UseItem.SkillBookCharacter_Rare.Use))]
            [HarmonyPrefix]
            static bool Use(UseItem.SkillBookCharacter_Rare __instance, ref bool __result)
            {
                List<Skill> list = new List<Skill>();
                List<BattleAlly> battleallys = PlayData.Battleallys;
                BattleTeam tempBattleTeam = PlayData.TempBattleTeam;


                // List of character names to pull from.
                List<string> names = new List<string> {
                    GDEItemKeys.Character_Azar, GDEItemKeys.Character_Control, GDEItemKeys.Character_Hein, GDEItemKeys.Character_Ilya, GDEItemKeys.Character_Joey,
                    GDEItemKeys.Character_Lian, GDEItemKeys.Character_Mement, GDEItemKeys.Character_MissChain, GDEItemKeys.Character_Phoenix, GDEItemKeys.Character_Priest,
                    GDEItemKeys.Character_Prime, GDEItemKeys.Character_Queen, GDEItemKeys.Character_ShadowPriest, GDEItemKeys.Character_SilverStein, GDEItemKeys.Character_Sizz,
                    GDEItemKeys.Character_Trisha, GDEItemKeys.Character_TW_Blue, GDEItemKeys.Character_TW_Red, GDEItemKeys.Character_Ilya,GDEItemKeys.Character_Leryn};

                for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
                {
                    bool flag = false;
                    foreach (CharInfoSkillData charInfoSkillData in PlayData.TSavedata.Party[i].SkillDatas)
                    {
                        if (charInfoSkillData.Skill.Rare)
                        {
                            flag = true;
                        }
                    }
                    if (PlayData.TSavedata.Party[i].BasicSkill.Rare)
                    {
                        flag = true;
                    }
                    if (!flag)
                    {
                        //Add random rare. I used names list here
                        list.Add(Skill.TempSkill(PlayData.GetMySkills(names.Random(), true).Random<GDESkillData>().KeyID, battleallys[i], tempBattleTeam));
                    }
                }
                if (list.Count == 0)
                {
                    EffectView.SimpleTextout(FieldSystem.instance.TopWindow.transform, ScriptLocalization.System.CantRareSkill, 1f, false, 1f);
                    __result = false;
                }
                foreach (Skill skill in list)
                {
                    if (!SaveManager.IsUnlock(skill.MySkill.KeyID, SaveManager.NowData.unlockList.SkillPreView))
                    {
                        SaveManager.NowData.unlockList.SkillPreView.Add(skill.MySkill.KeyID);
                    }
                }
                MasterAudio.PlaySound("BookFlip", 1f, default(float?), 0f, null, default(double?), false, false);
                FieldSystem.DelayInput(BattleSystem.I_OtherSkillSelect(list, new SkillButton.SkillClickDel(__instance.SkillAdd), ScriptLocalization.System_Item.SkillAdd, false, true, true, true, true));
                __result = true;

                return false;
            }
        }


    }
}

