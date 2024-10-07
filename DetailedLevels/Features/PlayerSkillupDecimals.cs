using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Utils;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    class SkillValue_Patch
    {
        static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
        {
            if (!ConfigurationFile.modEnabled.Value) return;

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
                    levelText = Math.Round(skillLevel + levelPercentage, Math.Min(15, Math.Max(0, ConfigurationFile.numberOfDecimals.Value))).ToString();
                }

                Utils.FindChild(obj.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text = levelText;

                float nextLevelRequirement = GetNextLevelRequirement(skill.m_level);
                Logger.Log($"******* {skill.m_info.m_skill.ToString()} *********");
                Logger.Log($"skillLevel: {skill.m_level}");
                Logger.Log($"skillLevelRounded: {skillLevel}");
                Logger.Log($"accumulator: {accumulator}");
                Logger.Log($"nextLevelRequirement: {nextLevelRequirement}");
                Logger.Log($"levelPercentage: {levelPercentage}");
                Logger.Log($"levelText: {levelText}");
            }
        }

        //method is private in game code, I bring it here. Hopefully it will never change!
        private static float GetNextLevelRequirement(float m_level)
        {
            return Mathf.Pow(Mathf.Floor(m_level + 1f), 1.5f) * 0.5f + 0.5f;
        }
    }
}
