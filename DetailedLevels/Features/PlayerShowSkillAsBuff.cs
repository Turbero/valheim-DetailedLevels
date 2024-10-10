using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using static Utils;
using static ItemDrop;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    class SkillsDialog_SkillStatusEffects_Patch
    {
        private static bool listenersAdded = false;

        // active status effects
        public static Dictionary<string, int> skillStatusEffects = new Dictionary<string, int>();

        static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
        {
            if (!ConfigurationFile.modEnabled.Value || InventoryGui.instance == null || listenersAdded) return;

            // copy to use ref variable inside listener
            var currentPlayer = player;

            // Add listeners to skill rows
            for (int i = 0; i < ___m_elements.Count; i++)
            {
                GameObject skillRow = ___m_elements[i];

                var row = i;
                skillRow.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Skills.Skill skill = currentPlayer.GetSkills().GetSkillList()[row];
                    OnSkillClicked(currentPlayer, skill, skillRow);
                });
            }
            listenersAdded = true;
        }

        private static void OnSkillClicked(Player player, Skills.Skill skill, GameObject skillRow)
        {
            string skillName = skill.m_info.m_skill.ToString();

            Logger.Log($"skillStatusEffects: {skillStatusEffects}");
            Logger.Log($"containsKey: {skillStatusEffects.ContainsKey(skillName)}");

            if (!skillStatusEffects.ContainsKey(skillName))
            {
                Sprite skillIcon = GetSkillIcon(skillRow);
                AddSkillBuff(player, skill, skillIcon, skillRow);
                setSkillRowBackgroundColor(skillRow, Color.cyan);
            }
            else
            {
                RemoveSkillBuff(player, skill);
                setSkillRowBackgroundColor(skillRow, new Color(0f, 0f, 0f, 0f));
            }
        }

        private static void setSkillRowBackgroundColor(GameObject skillRow, Color color)
        {
            //using temp variable to avoid CS1612
            ColorBlock skillRowButtonColors = skillRow.GetComponentInChildren<Button>().colors;
            skillRowButtonColors.normalColor = color;
            skillRow.GetComponentInChildren<Button>().colors = skillRowButtonColors;
        }

        private static Sprite GetSkillIcon(GameObject skillRow)
        {
            // Find icon component under icon_bkg component in the row and return sprite
            Image skillIcon = skillRow
                .transform.Find("icon_bkg").GetComponent<Image>()
                .transform.Find("icon").GetComponent<Image>();
            if (skillIcon != null)
            {
                return skillIcon.sprite;
            }
            return null;
        }

        private static void AddSkillBuff(Player player, Skills.Skill skill, Sprite skillIcon, GameObject skillRow)
        {
            SEMan seMan = (SEMan)PlayerUtils.getPlayerNonPublicField(player, PlayerUtils.FIELD_BUFFS);

            String value = Utils.FindChild(skillRow.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text;
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

            string skillName = skill.m_info.m_skill.ToString();
            skillStatusEffects.Add(skillName, nameHash);

            Logger.Log($"Added buff: {customBuff.m_name}");
        }

        private static void RemoveSkillBuff(Player player, Skills.Skill skill)
        {
            SEMan seMan = (SEMan)PlayerUtils.getPlayerNonPublicField(player, PlayerUtils.FIELD_BUFFS);

            // Find and delete buff
            string skillName = skill.m_info.m_skill.ToString();
            int nameHash = skillStatusEffects.GetValueSafe(skillName);
            StatusEffect existingBuff = seMan.GetStatusEffect(nameHash);
            if (existingBuff != null)
            {
                seMan.RemoveStatusEffect(existingBuff);
                skillStatusEffects.Remove(skillName);
                Logger.Log($"Deleted buff: {existingBuff.m_name}");
            }
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.OnAttackTrigger))]
    class Humanoid_OnAttackTrigger_Patch
    {
        static void Postfix()
        {
            Player player = Player.m_localPlayer;

            List<ItemData> equippedItems = player.GetInventory().GetEquippedItems(); // Equipped weapons

            for (int i = 0; i < equippedItems.Count; i++)
            {
                ItemData equippedItem = equippedItems[i];

                if (equippedItem.m_shared.m_name != "$item_chest_rags") // Only real weapons
                {
                    //Find associated skill
                    var skillType = equippedItem.m_shared.m_skillType;
                    Skills.Skill skill = GetPlayerSkill(player, skillType);
                    if (skill == null)
                        return; // if not found, return

                    Logger.Log($"skill type found: {skill.m_info.m_skill.ToString()}");
                    UpdateWeaponBuffIfExists(player, skill, equippedItem);
                }
            }
        }

        private static Skills.Skill GetPlayerSkill(Player player, Skills.SkillType skillType)
        {
            List<Skills.Skill> playerSkills = player.GetSkills().GetSkillList();
            for (int i = 0; i < playerSkills.Count; i++)
            {
                Skills.Skill skill = playerSkills[i];
                if (skill.m_info.m_skill == skillType)
                    return skill;
            }
            return null;
        }

        private static void UpdateWeaponBuffIfExists(Player player, Skills.Skill skill, ItemDrop.ItemData equippedItem)
        {
            SEMan seMan = (SEMan)PlayerUtils.getPlayerNonPublicField(player, PlayerUtils.FIELD_BUFFS);

            string skillName = skill.m_info.m_skill.ToString();
            int valueForHashCode = PlayerUtils.GetValueForHashCode(skill);
            Logger.Log($"skillName to find corresponding buff: {skillName} with hash {valueForHashCode}");

            StatusEffect existingBuff = seMan.GetStatusEffect(valueForHashCode.GetHashCode());
            if (existingBuff != null)
            {
                float currentSkillLevel = PlayerUtils.GetCurrentSkillLevelProgress(skill);
                Logger.Log($"About to update buff: $skill_{skillName.ToLower()} with skill level: {currentSkillLevel}.");

                string newBuffName = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}: {currentSkillLevel}";
                Logger.Log($"Old buff name: {existingBuff.m_name}. New buff name: {newBuffName}");
                if (existingBuff.m_name != newBuffName)
                {
                    existingBuff.m_name = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}: {currentSkillLevel}";
                    Logger.Log($"Updated buff: {skillName} with skill level: {currentSkillLevel}");
                } else
                {
                    Logger.Log("No need to update buff");
                }

            }
        }
    }
}
