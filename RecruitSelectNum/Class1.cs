using BepInEx;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Configuration;

namespace RecruitSelectNum
{
    [BepInPlugin(GUID, "Recruit Select Num", version)]
    [BepInProcess("ChronoArk.exe")]
    public class RecruitPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.chronoark.recruitmod.recruitselectnum";
        public const string version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(GUID);

		private static ConfigEntry<int> PartyNum;
		void Awake()
        {
			PartyNum = Config.Bind("Generation config", "Campfire Recruit Choice Num", 8, "Change number of party members available in campfire recruit.");
			harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        [HarmonyPatch(typeof(StartPartySelect), "Init")]
        class Recruit_Patch
        {
            public static bool Prefix(int select, int locked, StartPartySelect __instance, Camp CampEvent = null)
            {
				Debug.Log("Here");
				__instance.Locked = locked;
				__instance.SelectNum = select;
				SelectedAlly[] array = new SelectedAlly[select + locked];
				if (select == 1)
				{
					__instance.ModeSelectObject.SetActive(false);
				}
				if (CampEvent != null)
				{
					AccessTools.FieldRef<StartPartySelect, Camp> isCampRef = AccessTools.FieldRefAccess<Camp>(typeof(StartPartySelect), "isCamp");
					isCampRef(__instance) = CampEvent;
					UIManager.inst.PartyInven.gameObject.SetActive(false);
					FieldSystem.instance.MainUICanvas.gameObject.SetActive(false);
					__instance.CharAlignBG.SetActive(true);
					__instance.BGBlack.SetActive(false);
				}
				for (int i2 = 0; i2 < __instance.Selected.Length; i2++)
				{
					if (i2 >= select + __instance.Locked)
					{
						__instance.Selected[i2].gameObject.SetActive(false);
					}
				}
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = __instance.Selected[j];
				}
				__instance.Selected = array;
				for (int k = 0; k < __instance.Selected.Length; k++)
				{
					__instance.Selected[k].Main = __instance;
					if (__instance.Locked > k)
					{
						__instance.Selected[k].InitLock(k);
					}
				}
				ChildClear.Clear(__instance.Align);
				List<string> list = new List<string>();
				GDEDataManager.GetAllDataKeysBySchema(GDESchemaKeys.Character, out list);
				foreach (string key in list)
				{
					SaveManager.NowData.statistics.GetCharData(key);
				}
				__instance.Chars.Clear();
				if (select >= 2)
				{
					if (!string.IsNullOrEmpty(SaveManager.NowData.SaveedKey))
					{
						__instance.SavedItem.gameObject.SetActive(true);
						List<ItemBase> list2 = new List<ItemBase>();
						ItemBase item = ItemBase.GetItem(SaveManager.NowData.SaveedKey);
						list2.Add(item);
						__instance.SavedItem.CreateInven(list2, false, false);
					}
					foreach (string key2 in list)
					{
						GDECharacterData gdecharacterData = new GDECharacterData(key2);
						if (!gdecharacterData.Off)
						{
							__instance.Chars.Add(gdecharacterData);
						}
					}
					for (int l = 0; l < __instance.Chars.Count; l++)
					{
						CharacterButton component = UnityEngine.Object.Instantiate<GameObject>(__instance.CharSelect, __instance.Align).GetComponent<CharacterButton>();
						component.Init(__instance, __instance.Chars[l], l);
						component.Index = l;
						__instance.CBList.Add(component);
					}
					int num = 20 - __instance.Chars.Count;
					for (int m = 0; m < num; m++)
					{
						CharacterButton component2 = UnityEngine.Object.Instantiate<GameObject>(__instance.CharSelect, __instance.Align).GetComponent<CharacterButton>();
						component2.Init(__instance, null, 100);
					}
					__instance.SelectDifficulty.SetActive(true);
					if (SaveManager.NowData.GameOptions.Difficulty == 1)
					{
						__instance.SelectCasual();
					}
					else if (SaveManager.NowData.GameOptions.Difficulty == 2)
					{
						__instance.SelectOriginN();
					}
					else
					{
						__instance.SelectOrigin();
					}
				}
				else if (select == 1)
				{
					__instance.SelectDifficulty.SetActive(false);
					List<GDECharacterData> list3 = new List<GDECharacterData>();
					using (List<string>.Enumerator enumerator3 = list.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							string i = enumerator3.Current;
							if (i != GDEItemKeys.Character_TW_Blue && i != GDEItemKeys.Character_TW_Red)
							{
								GDECharacterData gdecharacterData2 = new GDECharacterData(i);
								if (PlayData.TSavedata.DonAliveChars.Find((string d) => d == i) == null && !gdecharacterData2.Off)
								{
									if (!gdecharacterData2.Lock)
									{
										list3.Add(gdecharacterData2);
									}
									else if (gdecharacterData2.Lock && SaveManager.IsUnlock(gdecharacterData2.Key, SaveManager.NowData.unlockList.UnlockCharacter))
									{
										list3.Add(gdecharacterData2);
									}
								}
							}
						}
					}
					int num2 = 3;
					if (SaveManager.IsUnlock(GDEItemKeys.ArkUpgrade_RecruitSlot))
					{
						num2 = PartyNum.Value;
					}
					for (int n = 0; n < num2; n++)
					{
						bool flag = false;
						bool flag2 = false;
						bool flag3 = false;
						if (n == 0)
						{
							flag = true;
							flag2 = true;
							flag3 = true;
							foreach (Character character in PlayData.TSavedata.Party)
							{
								if (character.GetData.Role.Key == GDEItemKeys.CharRole_Role_DPS)
								{
									flag2 = false;
								}
								if (character.GetData.Role.Key == GDEItemKeys.CharRole_Role_Support)
								{
									flag = false;
								}
								if (character.GetData.Role.Key == GDEItemKeys.CharRole_Role_Tank)
								{
									flag3 = false;
								}
							}
						}
						if (list3.Count != 0)
						{
							GDECharacterData gdecharacterData3 = list3.Random<GDECharacterData>();
							if (flag)
							{
								List<GDECharacterData> list4 = new List<GDECharacterData>();
								foreach (GDECharacterData gdecharacterData4 in list3)
								{
									if (gdecharacterData4.Role.Key == GDEItemKeys.CharRole_Role_Support)
									{
										list4.Add(gdecharacterData4);
									}
								}
								if (list4.Count != 0)
								{
									gdecharacterData3 = list4.Random<GDECharacterData>();
								}
							}
							else if (flag2)
							{
								List<GDECharacterData> list5 = new List<GDECharacterData>();
								foreach (GDECharacterData gdecharacterData5 in list3)
								{
									if (gdecharacterData5.Role.Key == GDEItemKeys.CharRole_Role_DPS)
									{
										list5.Add(gdecharacterData5);
									}
								}
								if (list5.Count != 0)
								{
									gdecharacterData3 = list5.Random<GDECharacterData>();
								}
							}
							else if (flag3)
							{
								List<GDECharacterData> list6 = new List<GDECharacterData>();
								foreach (GDECharacterData gdecharacterData6 in list3)
								{
									if (gdecharacterData6.Role.Key == GDEItemKeys.CharRole_Role_Tank)
									{
										list6.Add(gdecharacterData6);
									}
								}
								if (list6.Count != 0)
								{
									gdecharacterData3 = list6.Random<GDECharacterData>();
								}
							}
							CharacterButton component3 = UnityEngine.Object.Instantiate<GameObject>(__instance.CharSelect, __instance.Align).GetComponent<CharacterButton>();
							__instance.Chars.Add(gdecharacterData3);
							component3.Init(__instance, gdecharacterData3, n);
							component3.Index = n;
							__instance.CBList.Add(component3);
							list3.Remove(gdecharacterData3);
						}
					}
				}
				return false;
			}
        }

    }
}
