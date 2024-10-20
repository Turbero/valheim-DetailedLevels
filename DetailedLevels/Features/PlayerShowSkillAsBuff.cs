using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using System.Reflection;
using static Utils;
using static Skills;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    class SkillsDialog_SkillStatusEffects_Patch
    {
        static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
        {
            if (!ConfigurationFile.modEnabled.Value || InventoryGui.instance == null) return;

            // Add listeners to skill rows
            for (int i = 0; i < ___m_elements.Count; i++)
            {
                GameObject skillRow = ___m_elements[i];

                var row = i;

                skillRow.GetComponent<Button>().onClick.RemoveAllListeners(); //remove to avoid creating double click effect
                skillRow.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var currentPlayer = Player.m_localPlayer; // use current player instance to refresh after dying
                    Skill skill = currentPlayer.GetSkills().GetSkillList()[row];
                    OnSkillClicked(currentPlayer, skill, skillRow);
                });
            }
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

        public static void setSkillRowBackgroundColor(GameObject skillRow, Color color)
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
            SEMan seMan = player.GetSEMan();

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

    [HarmonyPatch(typeof(Player), "OnDeath")]
    public class Player_OnDeath_Patch
    {
        static void Postfix(Player __instance)
        {
            //Reset skills background in skillDialog
            Player player = __instance.GetComponent<Player>();
            var field = typeof(SkillsDialog).GetField("m_elements", BindingFlags.NonPublic | BindingFlags.Instance);
            List<GameObject> skillRows = (List<GameObject>) field.GetValue(InventoryGui.instance.m_skillsDialog);
            foreach (GameObject skillRow in skillRows)
            {
                SkillsDialog_SkillStatusEffects_Patch.setSkillRowBackgroundColor(skillRow, new Color(0f, 0f, 0f, 0f));
            }

            //Clear stored buffs
            PlayerUtils.skillStatusEffects.Clear();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.RaiseSkill))]
    public class Player_RaiseSkill_Patch
    {
        static void Postfix(Player __instance, Skills.SkillType skill, float value)
        {
            if (!ConfigurationFile.modEnabled.Value) return;
            bool existBuff = PlayerUtils.skillStatusEffects.TryGetValue(skill, out int nameHash);
            if (existBuff)
                updateSkillTypeBuff(__instance, skill, nameHash);
        }

        public static void updateSkillTypeBuff(Player player, SkillType skillType, int nameHash, bool forceUpdate = false)
        {
            string skillName = skillType.ToString();
            int valueForHashCode = PlayerUtils.GetValueForHashCode(skillType);
            Logger.Log($"skillName to find corresponding buff: {skillName} with hash {valueForHashCode}. Stored nameHash: {nameHash}");

            SEMan seMan = player.GetSEMan();
            StatusEffect existingBuff = seMan.GetStatusEffect(nameHash);
            if (existingBuff != null)
            {
                Skill playerSkill = FindPlayerSkill(player, skillType);
                float currentSkillLevel = PlayerUtils.GetCurrentSkillLevelProgress(playerSkill);

                Logger.Log($"About to update buff: $skill_{skillName.ToLower()} with skill level: {currentSkillLevel}.");

                string newBuffName = $"$skill_{skillType.ToString().ToLower()}: {currentSkillLevel}";
                Logger.Log($"Old buff name: {existingBuff.m_name}. New buff name: {newBuffName}");
                if (existingBuff.m_name != newBuffName || forceUpdate)
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

        private static Skill FindPlayerSkill(Player player, SkillType skillType)
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

    [HarmonyPatch(typeof(Character), "Damage")]
    public class BloodMagic_BuffUpdate_Patch
    {
        static void Postfix(Character __instance, HitData hit)
        {
            if (!ConfigurationFile.modEnabled.Value) return;
            
            if (__instance != null && __instance.IsMonsterFaction(0f))
            {
                Character attacker = hit.GetAttacker();
                if (attacker != null && attacker.IsTamed())
                {
                    _ = WaitForSecondsAsync(0.1f); // Small delay in async method to wait for updating blood magic skill
                }
            }
        }

        private static async Task WaitForSecondsAsync(float seconds)
        {
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milisegundos
            bool existBuff = PlayerUtils.skillStatusEffects.TryGetValue(SkillType.BloodMagic, out int nameHash);
            if (existBuff)
                Player_RaiseSkill_Patch.updateSkillTypeBuff(Player.m_localPlayer, SkillType.BloodMagic, nameHash);
        }
    }
}