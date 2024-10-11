﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using static Utils;
using static ItemDrop;
using static Skills;

namespace DetailedLevels.Features.SkillBuffs
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    class SkillsDialog_SkillStatusEffects_Patch
    {
        private static bool listenersAdded = false;

        static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
        {
            if (!ConfigurationFile.modEnabled.Value || listenersAdded || InventoryGui.instance == null) return;

            // copy to use ref variable inside listener
            var currentPlayer = player;

            // Add listeners to skill rows
            for (int i = 0; i < ___m_elements.Count; i++)
            {
                GameObject skillRow = ___m_elements[i];

                var row = i;
                skillRow.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Skill skill = currentPlayer.GetSkills().GetSkillList()[row];
                    OnSkillClicked(currentPlayer, skill, skillRow);
                });
            }
            listenersAdded = true;
        }

        private static void OnSkillClicked(Player player, Skill skill, GameObject skillRow)
        {
            SkillType skillType = skill.m_info.m_skill;

            string skillName = skill.m_info.m_skill.ToString();

            bool skillStatusEffectsContainsKey = PlayerUtils.skillStatusEffects.ContainsKey(skillType);
            Logger.Log($"skillStatusEffects.ContainsKey: {skillStatusEffectsContainsKey}");
            if (!skillStatusEffectsContainsKey)
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

        private static void AddSkillBuff(Player player, Skill skill, Sprite skillIcon, GameObject skillRow)
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
            PlayerUtils.skillStatusEffects.Add(skill.m_info.m_skill, nameHash);

            Logger.Log($"Added buff: {customBuff.m_name}");
        }

        private static void RemoveSkillBuff(Player player, Skill skill)
        {
            SEMan seMan = (SEMan)PlayerUtils.getPlayerNonPublicField(player, PlayerUtils.FIELD_BUFFS);

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

    /*[HarmonyPatch(typeof(Player), "OnDeath")]
    public class Player_OnDeath_Patch
    {
        // Este Postfix se ejecuta después de que OnDeath se complete
        static void Postfix(Player __instance)
        {
            //Reset skills in skillDialog
            Player player = __instance.GetComponent<Player>();
            var field = typeof(SkillsDialog).GetField("m_elements", BindingFlags.NonPublic | BindingFlags.Instance);
            List<GameObject> skillRows = (List<GameObject>) field.GetValue(InventoryGui.instance.m_skillsDialog);
            foreach (GameObject skillRow in skillRows)
            {
                SkillsDialog_SkillStatusEffects_Patch.setSkillRowBackgroundColor(skillRow, new Color(0f, 0f, 0f, 0f));
            }

            //Clear stored buffs
            SEMan seMan = (SEMan)PlayerUtils.getPlayerNonPublicField(player, PlayerUtils.FIELD_BUFFS);
            seMan.RemoveAllStatusEffects();
            PlayerUtils.skillStatusEffects.Clear();
        }
    }*/

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.OnAttackTrigger))]
    class Humanoid_OnAttackTrigger_Patch
    {
        static void Postfix()
        {
            if (!ConfigurationFile.equipBuffs.Value) return;

            Logger.Log("Running Humanoid.OnAttackTrigger.Postfix");

            Player player = Player.m_localPlayer;

            bool someUnarmed = false;
            bool someBlocking = false;

            List<ItemData> equippedItems = player.GetInventory().GetEquippedItems();
            for (int i = 0; i < equippedItems.Count; i++)
            {
                ItemData equippedItem = equippedItems[i];
                SkillType skillType = equippedItem.m_shared.m_skillType;
                if (!someUnarmed && skillType == SkillType.Unarmed) someUnarmed = true;
                if (!someBlocking && skillType == SkillType.Blocking) someBlocking = true;

                bool exists = PlayerUtils.skillStatusEffects.TryGetValue(skillType, out int nameHash);
                if (exists)
                    updateSkillTypeBuff(player, skillType, nameHash);
            }
            
            //Update Unarmed in case to have thrown a punch
            bool existBuffUnarmed = PlayerUtils.skillStatusEffects.TryGetValue(SkillType.Unarmed, out int nameHash2);
            if (existBuffUnarmed)
                updateSkillTypeBuff(player, SkillType.Unarmed, nameHash2);

            // Update Blocking in case to have blocked with two handed weapon (claws, staves, etc)
            bool existBuffBlocking = PlayerUtils.skillStatusEffects.TryGetValue(SkillType.Blocking, out int nameHash3);
            if (existBuffBlocking)
                updateSkillTypeBuff(player, SkillType.Blocking, nameHash3);
        }

        private static void updateSkillTypeBuff(Player player, SkillType skillType, int nameHash)
        {
            string skillName = skillType.ToString();
            int valueForHashCode = PlayerUtils.GetValueForHashCode(skillType);
            Logger.Log($"skillName to find corresponding buff: {skillName} with hash {valueForHashCode}. Stored nameHash: {nameHash}");

            SEMan seMan = (SEMan)PlayerUtils.getPlayerNonPublicField(player, PlayerUtils.FIELD_BUFFS);
            StatusEffect existingBuff = seMan.GetStatusEffect(nameHash);
            if (existingBuff != null)
            {
                Skill playerSkill = findPlayerSkill(player, skillType);
                float currentSkillLevel = PlayerUtils.GetCurrentSkillLevelProgress(playerSkill);

                Logger.Log($"About to update buff: $skill_{skillName.ToLower()} with skill level: {currentSkillLevel}.");

                string newBuffName = $"$skill_{skillType.ToString().ToLower()}: {currentSkillLevel}";
                Logger.Log($"Old buff name: {existingBuff.m_name}. New buff name: {newBuffName}");
                if (existingBuff.m_name != newBuffName)
                {
                    existingBuff.m_name = $"$skill_{skillType.ToString().ToLower()}: {currentSkillLevel}";
                    Logger.Log($"Updated buff: {skillName} with skill level: {currentSkillLevel}");
                }
                else
                {
                    Logger.Log("No need to update buff");
                }
            }
        }

        private static Skill findPlayerSkill(Player player, SkillType skillType)
        {
            Skill playerSkill = null;
            foreach (var skill in player.GetSkills().GetSkillList())
            {
                if (skill.m_info.m_skill == skillType)
                {
                    playerSkill = skill;
                    break;
                }
            }

            return playerSkill;
        }
    }
}
