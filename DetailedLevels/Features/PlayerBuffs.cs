using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using static Skills;
using static Utils;

namespace DetailedLevels.Features
{
    public class PlayerBuffs
    {
        private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        public static void AddSkillBuff(Player player, Skill skill, Sprite skillIcon, GameObject skillRow = null)
        {
            SEMan seMan = player.GetSEMan();
            
            string value = skillRow != null
                ? Utils.FindChild(skillRow.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text
                : PlayerUtils.GetCurrentSkillLevelProgress(skill).ToString();
            Logger.Log("Skill current value: " + value);

            // Create new custom status effect
            SE_Stats customBuff = ScriptableObject.CreateInstance<SE_Stats>();
            customBuff.m_name = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}: {value}";
            customBuff.m_tooltip = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}_description";
            customBuff.m_icon = skillIcon; // Use skill icon
            customBuff.name = PlayerUtils.GetValueForNameHash(skill); // to produce distinct hash values

            // Apply buff to player
            int nameHash = customBuff.NameHash();
            Logger.Log($"name: {customBuff.name}, m_name: {customBuff.m_name}, nameHash: {nameHash}");

            seMan.AddStatusEffect(customBuff);
            PlayerUtils.skillStatusEffects.Add(skill.m_info.m_skill, nameHash);

            Logger.Log($"Added buff: {customBuff.m_name}");
        }

        public static void RemoveSkillBuff(Player player, Skill skill)
        {
            SEMan seMan = player.GetSEMan();

            // Find and delete buff
            string skillName = skill.m_info.m_skill.ToString();
            int nameHash = PlayerUtils.skillStatusEffects.GetValueSafe(skill.m_info.m_skill);
            StatusEffect existingBuff = seMan.GetStatusEffect(nameHash);
            if (existingBuff != null)
            {
                seMan.RemoveStatusEffect(existingBuff);
                PlayerUtils.skillStatusEffects.Remove(skill.m_info.m_skill);

                Logger.Log($"Deleted buff: {existingBuff.m_name}");
            }
        }
    }
}
