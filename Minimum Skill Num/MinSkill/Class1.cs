using BepInEx;
using BepInEx.Configuration;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
namespace MinSkill
{
    [BepInPlugin(GUID, "Minimum Skill Num", version)]
    [BepInProcess("ChronoArk.exe")]
    public class RareSkillsPlugin : BaseUnityPlugin
    {
        public const string GUID = "windy.minskill";
        public const string version = "1.0.0";

        private static ConfigEntry<int> MinSkillNum;

        private static readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            MinSkillNum = Config.Bind("Generation config", "Minimum Skill Num", 1, "Number of minimum skills a character can have.");
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        [HarmonyPatch(typeof(CharacterWindow), "Update")]
        class Modify
        {
            static void Postfix(CharacterWindow __instance)
            {
                if (BattleSystem.instance == null && __instance.SkillAlign.transform.childCount > MinSkillNum.Value)
                {
                    //Debug.Log("Here");
                    __instance.ForgetBtn.interactable = true;
                }
            }
        }
    }
}

