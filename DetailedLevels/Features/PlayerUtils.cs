using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static Skills;

namespace DetailedLevels.Features
{
    public class PlayerUtils
    {
        private static Dictionary<string, Sprite> cachedSprites = new Dictionary<string, Sprite>();

        // active status effects
        public static Dictionary<SkillType, int> skillStatusEffects = new Dictionary<SkillType, int>();

        public static float GetCurrentSkillLevelProgress(Skill skill)
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

            float nextLevelRequirement = GetNextLevelRequirement(skill);
            Logger.Log($"******* {skill.m_info.m_skill.ToString()} *********");
            Logger.Log($"skillLevel: {skill.m_level}, skillLevelRounded: {skillLevel}, accumulator: {accumulator}");
            Logger.Log($"nextLevelRequirement: {nextLevelRequirement}, levelPercentage: {levelPercentage}, levelText: {levelText}");

            return levelText;
        }

        private static float GetNextLevelRequirement(Skills.Skill skill)
        {
            var getSkillMethod = skill.GetType().GetMethod("GetNextLevelRequirement", BindingFlags.NonPublic | BindingFlags.Instance);
            return getSkillMethod != null ? (float)getSkillMethod.Invoke(skill, new object[] {} ) : 0;
        }
        public static void setSkillRowBackgroundColor(GameObject skillRow, Color color)
        {
            //using temp variable to avoid CS1612
            ColorBlock skillRowButtonColors = skillRow.GetComponentInChildren<Button>().colors;
            skillRowButtonColors.normalColor = color;
            skillRow.GetComponentInChildren<Button>().colors = skillRowButtonColors;
        }

        public static string GetValueForNameHash(Skills.Skill skill)
        {
            return skill.m_info.m_skill.ToString();
        }
        public static int GetValueForHashCode(Skills.Skill skill)
        {
            return GetValueForNameHash(skill).GetHashCode();
        }
        public static int GetValueForHashCode(Skills.SkillType skillType)
        {
            return skillType.ToString().GetHashCode();
        }

        public static Sprite getSprite(String name)
        {
            if (!cachedSprites.ContainsKey(name))
            {
                Logger.Log($"Finding {name} sprite...");
                var allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
                for (var i = 0; i < allSprites.Length; i++)
                {
                    var sprite = allSprites[i];
                    if (sprite.name == name)
                    {
                        Logger.Log($"{name} sprite found.");
                        cachedSprites.Add(name, sprite);
                        return sprite;
                    }
                }
                Logger.Log($"{name} sprite NOT found.");
                return null;
            } else
            {
                return cachedSprites.GetValueSafe(name);
            }
        }
    }
}
