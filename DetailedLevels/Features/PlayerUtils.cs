using System;
using System.Reflection;
using UnityEngine;

namespace DetailedLevels.Features
{
    public class PlayerUtils
    {
        public static string FIELD_BUFFS = "m_seman";
        public static object getPlayerNonPublicField(Player player, string fieldName)
        {
            // Using reflection to obtain field
            var field = typeof(Player).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                Logger.LogError($"Couldn't access the player field {fieldName}.");
                return null;
            }
            return field.GetValue(player);
        }
        public static float GetCurrentSkillLevelProgress(Skills.Skill skill)
        {
            float accumulator = skill.m_accumulator;

            int skillLevel = (int)skill.m_level; //Remove possible decimals and round down at the same time
            float levelText = skillLevel;
            float levelPercentage = skill.GetLevelPercentage();

            if (accumulator > 0)
            {
                // Number of decimals
                levelText = (float)Math.Round(skillLevel + levelPercentage, Math.Min(15, Math.Max(0, ConfigurationFile.numberOfDecimals.Value)));
            }

            float nextLevelRequirement = GetNextLevelRequirement(skill.m_level);
            Logger.Log($"******* {skill.m_info.m_skill.ToString()} *********");
            Logger.Log($"skillLevel: {skill.m_level}, skillLevelRounded: {skillLevel}, accumulator: {accumulator}");
            Logger.Log($"nextLevelRequirement: {nextLevelRequirement}, levelPercentage: {levelPercentage}, levelText: {levelText}");

            return levelText;
        }

        //method is private in game code, I bring it here. Hopefully it will never change!
        private static float GetNextLevelRequirement(float m_level)
        {
            return Mathf.Pow(Mathf.Floor(m_level + 1f), 1.5f) * 0.5f + 0.5f;
        }
        public static string GetValueForNameHash(Skills.Skill skill)
        {
            return skill.m_info.m_skill.ToString();
        }
        public static int GetValueForHashCode(Skills.Skill skill)
        {
            return GetValueForNameHash(skill).GetHashCode();
        }
    }
}
