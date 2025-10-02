using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DetailedLevels.Features
{
    public static class PlayerUtils
    {
        private static readonly Dictionary<string, Sprite> cachedSprites = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, TMP_FontAsset> cachedFonts = new Dictionary<string, TMP_FontAsset>();

        // active status effects
        public static readonly Dictionary<Skills.SkillType, int> skillStatusEffects = new Dictionary<Skills.SkillType, int>();

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

        public static TMP_FontAsset getFontAsset(String name)
        {
            if (!cachedFonts.ContainsKey(name))
            {
                Logger.Log($"Finding {name} font...");
                var allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                for (var i = 0; i < allFonts.Length; i++)
                {
                    var font = allFonts[i];
                    if (font.name == name)
                    {
                        Logger.Log($"{name} font found.");
                        cachedFonts.Add(name, font);
                        return font;
                    }
                }
                Logger.Log($"{name} font NOT found.");
                return null;
            }
            else
            {
                return cachedFonts.GetValueSafe(name);
            }
        }

        public static float FindActiveModifierValue(Player player, Skills.SkillType skillType)
        {
            float totalModifierValue = 0f;
            List<StatusEffect> activeEffects = player.GetSEMan().GetStatusEffects();
            foreach (StatusEffect statusEffect in activeEffects)
            {
                if (statusEffect.GetType() == typeof(SE_Stats))
                {
                    SE_Stats se = (SE_Stats)statusEffect;
                    if (se.m_skillLevel == skillType)
                        totalModifierValue += se.m_skillLevelModifier;
                    if (se.m_skillLevel2 == skillType)
                        totalModifierValue += se.m_skillLevelModifier2;
                }
                    
            }
            
            
            return totalModifierValue;
        }

        public static List<ItemDrop.ItemData.ItemType> GetTypesThatModifyStats()
        {
            List<ItemDrop.ItemData.ItemType> types = new List<ItemDrop.ItemData.ItemType>
            {
                ItemDrop.ItemData.ItemType.OneHandedWeapon,
                ItemDrop.ItemData.ItemType.Bow,
                ItemDrop.ItemData.ItemType.Shield,
                ItemDrop.ItemData.ItemType.Helmet,
                ItemDrop.ItemData.ItemType.Chest,
                ItemDrop.ItemData.ItemType.Legs,
                ItemDrop.ItemData.ItemType.Hands,
                ItemDrop.ItemData.ItemType.TwoHandedWeapon,
                ItemDrop.ItemData.ItemType.Utility,
                ItemDrop.ItemData.ItemType.Attach_Atgeir,
                ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft
            };
            return types;
        }

        public static Skills.Skill FindPlayerSkill(Player player, Skills.SkillType skillType)
        {
            foreach (var skill in player.GetSkills().GetSkillList())
            {
                if (skill.m_info.m_skill == skillType)
                {
                    return skill;
                }
            }
            return null;
        }
    }
}
