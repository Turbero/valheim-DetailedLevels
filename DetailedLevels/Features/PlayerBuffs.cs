using System;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Skills;

namespace DetailedLevels.Features
{
    public static class PlayerBuffs
    {
        public static readonly Dictionary<SkillType, Sprite> sprites = new Dictionary<SkillType, Sprite>();
        public static readonly Dictionary<SkillType, Skill> skills = new Dictionary<SkillType, Skill>();
        public static void AddSkillBuff(Player player, Skill skill, Sprite skillIcon, GameObject skillRow = null)
        {
            SEMan seMan = player.GetSEMan();
            
            float value = PlayerUtils.GetCurrentSkillLevelProgress(skill);
            float skillLevelModifier = PlayerUtils.FindActiveModifierValue(player, skill.m_info.m_skill);
            Logger.Log("Skill current value: " + value+ ". modifier: "+skillLevelModifier);
            

            // Create new custom status effect
            SE_Stats customBuff = ScriptableObject.CreateInstance<SE_Stats>();
            customBuff.m_name = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}: {PlayerUtils.GetSkillValueToShow(value, skillLevelModifier)}";
            customBuff.m_tooltip = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}_description";
            customBuff.m_icon = skillIcon; // Use skill icon
            customBuff.name = PlayerUtils.GetValueForNameHash(skill); // to produce distinct hash values

            // Apply buff to player
            int nameHash = customBuff.NameHash();
            Logger.Log($"name: {customBuff.name}, m_name: {customBuff.m_name}, nameHash: {nameHash}");

            seMan.AddStatusEffect(customBuff);
            if (PlayerUtils.skillStatusEffects.ContainsKey(skill.m_info.m_skill))
            {
                PlayerUtils.skillStatusEffects.Remove(skill.m_info.m_skill);
            }
            PlayerUtils.skillStatusEffects.Add(skill.m_info.m_skill, nameHash);
            Logger.Log($"Added buff: {customBuff.m_name}");

            if (!sprites.ContainsKey(skill.m_info.m_skill))
            {
                sprites.Add(skill.m_info.m_skill, skillIcon);
                Logger.Log($"Cached sprite for {customBuff.m_name} with sprite.name {skillIcon.name}");
            }

            if (!skills.ContainsKey(skill.m_info.m_skill))
            {
                skills.Add(skill.m_info.m_skill, skill);
                Logger.Log($"Cached skill for {customBuff.m_name}");
            }
            
            
            //Apply color after the buff is in Hud.instance (takes little milliseconds) and refresh the rest
            _ = refreshAllBlueColorsAsync(player, 0.1f);
        }

        public static void RemoveSkillBuff(Player player, Skill skill)
        {
            SEMan seMan = player.GetSEMan();

            // Find and delete buff
            int nameHash = PlayerUtils.skillStatusEffects.GetValueSafe(skill.m_info.m_skill);
            StatusEffect existingBuff = seMan.GetStatusEffect(nameHash);
            if (existingBuff != null)
            {
                seMan.RemoveStatusEffect(existingBuff);
                PlayerUtils.skillStatusEffects.Remove(skill.m_info.m_skill);

                Logger.Log($"Deleted buff: {existingBuff.m_name}");

                sprites.Remove(skill.m_info.m_skill);
                skills.Remove(skill.m_info.m_skill);

                //When removing from seMan the colors are reset in white but extra buffs are still in the name. Have to restore the blue color
                _ = refreshAllBlueColorsAsync(player, 0.1f);
            }
        }

        private static async Task refreshAllBlueColorsAsync(Player player, float seconds)
        {
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milliseconds
            PlayerColorBuffs.refreshAllBlueColors(player);
        }
    }
}
