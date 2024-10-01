using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Utils;

namespace DetailedLevels
{
    [BepInPlugin("Turbero.DetailedLevels", "Detailed Levels", "1.0.1")]
    public class DetailedLevels : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("Turbero.DetailedLevels");
        private static ConfigEntry<bool> modEnabled;
        private static ConfigEntry<bool> debug;
        private static ConfigEntry<int> numberOfDecimals;
        private static ConfigEntry<int> skillUpMessageAfterMultipleLevel;

        void Awake()
        {
            modEnabled = Config.Bind<bool>("1 - General", "Enabled", true, "Enabling/Disabling the mod (default = true)");
            debug = Config.Bind<bool>("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)");
            numberOfDecimals = Config.Bind<int>("2 - Levels Data", "NumberOfDecimals", 2, "Number of decimals to show in your levels information (default = 2, min = 0, max = 15)");
            skillUpMessageAfterMultipleLevel = Config.Bind<int>("2 - Levels Data", "SkillUpMessageAfterMultipleLevel", 5, "Shows skill up message after the new level is multiple of the indicated level (0 = disabled, default = 5)");
            
            harmony.PatchAll();
        }

        void onDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
        class SkillValue_Patch
        {
            static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
            {
                if (!modEnabled.Value) return;
                
                List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
                for (int j = 0; j < skillList.Count; j++)
                {
                    GameObject obj = ___m_elements[j];
                    Skills.Skill skill = skillList[j];

                    float accumulator = skill.m_accumulator;

                    int skillLevel = (int)skill.m_level; //Remove possible decimals and round down at the same time
                    String levelText = skillLevel.ToString();
                    float levelPercentage = skill.GetLevelPercentage();

                    // Number of decimals
                    if (accumulator > 0)
                    {
                        levelText = Math.Round(skillLevel + levelPercentage, Math.Min(15, Math.Max(0, numberOfDecimals.Value))).ToString();
                    }
                                        
                    Utils.FindChild(obj.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text = levelText;

                    if (debug.Value)
                    {
                        float nextLevelRequirement = GetNextLevelRequirement(skill.m_level);
                        Debug.Log($"******* {skill.m_info.m_skill.ToString()} *********");
                        Debug.Log($"skillLevel: {skill.m_level}");
                        Debug.Log($"skillLevelRounded: {skillLevel}");
                        Debug.Log($"accumulator: {accumulator}");
                        Debug.Log($"nextLevelRequirement: {nextLevelRequirement}");
                        Debug.Log($"levelPercentage: {levelPercentage}");
                        Debug.Log($"levelText: {levelText}");
                    }
                }
            }

            //method is private in game code, I bring it here. Hopefully it will never change!
            private static float GetNextLevelRequirement(float m_level)
            {
                return Mathf.Pow(Mathf.Floor(m_level + 1f), 1.5f) * 0.5f + 0.5f;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnSkillLevelup))]
        class Player_Skillup_Patch
        {
            static void Postfix(ref Skills.SkillType skill, ref float level)
            {
                int multipleValue = Math.Max(1, Math.Min(100, skillUpMessageAfterMultipleLevel.Value));
                if ((int)level % multipleValue == 0)
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"$msg_skillup $skill_{skill.ToString().ToLower()}: {level}");
                }
            }
        }
    }
}

