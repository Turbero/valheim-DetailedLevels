using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using static Skills;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    [HarmonyPriority(Priority.VeryHigh)]
    static class SkillsDialog_SkillStatusEffects_Patch
    {
        static void Postfix(SkillsDialog __instance, ref List<GameObject> ___m_elements)
        {
            Logger.Log("** SkillsDialog_SkillStatusEffects_Patch.Postfix");
            if (InventoryGui.instance == null) return;

            // Add listeners to skill rows
            for (int i = 0; i < ___m_elements.Count; i++)
            {
                GameObject skillRow = ___m_elements[i];

                var row = i;

                skillRow.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); //avoids creating double click effect
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

            bool skillStatusEffectsContainsKey = PlayerUtils.skillStatusEffects.ContainsKey(skillType);
            Logger.Log($"skillStatusEffects.ContainsKey: {skillStatusEffectsContainsKey}");
            if (!skillStatusEffectsContainsKey)
            {
                Sprite skillIcon = GetSkillIcon(skillRow);
                PlayerBuffs.AddSkillBuff(player, skill, skillIcon);
                PlayerUtils.setSkillRowBackgroundColor(skillRow, ConfigurationFile.colorSkillBackground.Value);
            }
            else
            {
                PlayerBuffs.RemoveSkillBuff(player, skill);
                PlayerUtils.setSkillRowBackgroundColor(skillRow, new Color(0f, 0f, 0f, 0f));
            }
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
    }

    [HarmonyPatch(typeof(Player), nameof(Player.RaiseSkill))]
    public class Player_RaiseSkill_Patch
    {
        static void Postfix(Player __instance, SkillType skill, float value)
        {
            Logger.Log($"Entering RaiseSkill.Postfix with skill {skill.ToString()} and value {value}");
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
            if (seMan.GetStatusEffect(nameHash) is SE_SkillBuff existingBuff)
            {
                Skill playerSkill = PlayerUtils.FindPlayerSkill(player, skillType);
                float currentSkillLevel = PlayerUtils.GetCurrentSkillLevelProgress(playerSkill);
                float skillLevelModifier = PlayerUtils.FindActiveModifierValue(player, skillType);

                Logger.Log($"About to update buff: $skill_{skillName.ToLower()} with skill level {currentSkillLevel} and skill level modifier {skillLevelModifier}.");

                string skillValueToShow = PlayerUtils.GetSkillValueToShow(currentSkillLevel, skillLevelModifier);
                if (!existingBuff.skillValue.Equals(skillValueToShow) || forceUpdate)
                {
                    existingBuff.UpdateBuffText(skillValueToShow);
                    Logger.Log($"Updated buff: {skillName} with skill level: {currentSkillLevel}");
                }
                else
                {
                    Logger.Log("No need to update buff");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), "Damage")]
    public class BloodMagic_BuffUpdate_Patch
    {
        static void Postfix(Character __instance, HitData hit)
        {
            if (__instance  != null)
            {
                Character attacker = hit.GetAttacker();
                if (attacker != null)
                {
                    //Blood magic
                    Logger.Log("Checking blood magic skill up...");
                    // if attacker is a pet/invocation and attacked is a monster there is a bloodmagic skillup!
                    if (attacker.IsTamed() && __instance.IsMonsterFaction(0f))
                        _ = WaitForSecondsAsyncBloodMagic(null, 0.1f);
                    // attacker is a monster and attacked is a player. If the magic barriers breaks, there is a bloodmagic skillup!
                    else if  (attacker.IsMonsterFaction(0f) && __instance.GetType() == typeof(Player))
                        _ = WaitForSecondsAsyncBloodMagic(__instance as Player, 0.1f);
                    // attacker is a Troll_Summoned and attacked is Player or Tamed
                    else if (attacker.name.Contains("Troll_Summoned"))
                        if (__instance.GetType() == typeof(Player))
                            _ = WaitForSecondsAsyncBloodMagic(__instance as Player, 0.1f);
                        else if (__instance.IsTamed())
                            _ = WaitForSecondsAsyncBloodMagic(null, 0.1f);
                }
            }
        }

        private static async Task WaitForSecondsAsyncBloodMagic(Player player, float seconds)
        {
            // Small delay in async method to wait for updating blood magic skill
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milliseconds
            bool existBuff = PlayerUtils.skillStatusEffects.TryGetValue(SkillType.BloodMagic, out int nameHash);
            if (existBuff)
                Player_RaiseSkill_Patch.updateSkillTypeBuff(player == null ? Player.m_localPlayer : player, SkillType.BloodMagic, nameHash);
        }
    }

    [HarmonyPatch(typeof(StatusEffect), "GetIconText")]
    public class SE_Shield_GetIconText_Patch
    {
        public static bool Prefix(StatusEffect __instance, ref string __result)
        {
            if (__instance is SE_Shield)
            {
                float m_totalAbsorbDamage = (float) ReflectionUtils.GetPrivateValue(__instance, "m_totalAbsorbDamage");
                float m_damage = (float) ReflectionUtils.GetPrivateValue(__instance, "m_damage");
                float remainingAbsorbDamage = (float)Math.Round(m_totalAbsorbDamage - m_damage, Math.Min(15, Math.Max(0, ConfigurationFile.numberOfDecimals.Value)));
                float m_time = (float) ReflectionUtils.GetPrivateValue(__instance, "m_time");
                string remainingTime = StatusEffect.GetTimeString(__instance.m_ttl - m_time);
                if (ConfigurationFile.skillBuffValuePosition.Value == SkillBuffValuePosition.Above)
                {
                    __instance.m_name = $"$se_shield: {remainingAbsorbDamage}";
                    __result = remainingTime;
                }
                else
                {
                    __instance.m_name = "$se_shield";
                    __result = remainingAbsorbDamage + " (" + remainingTime + ")";
                }

                return false;
            }

            return true;
        } 
    }

    [HarmonyPatch(typeof(StatusEffect), "ResetTime")]
    public class SE_Shield_ResetTime_Patch
    {
        public static void Postfix(StatusEffect __instance)
        {
            if (__instance is SE_Shield)
            {
                //Should reset damage, game bug fixed with this!
                ReflectionUtils.SetPrivateValue(__instance, "m_damage", 0);

                float m_totalAbsorbDamage = (float) ReflectionUtils.GetPrivateValue(__instance, "m_totalAbsorbDamage");
                if (ConfigurationFile.skillBuffValuePosition.Value == SkillBuffValuePosition.Above)
                {
                    __instance.m_name = $"$se_shield: {m_totalAbsorbDamage}";
                }
                else
                {
                    __instance.m_name = "$se_shield";
                }
            }
        }
    }

    [HarmonyPatch]
    public class Dodge_BuffUpdate_Patch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Player), "Dodge");
        }

        static void Postfix(ref Player __instance, Vector3 dodgeDir)
        {
            Logger.Log("Checking dodge skill up...");
            _ = WaitForSecondsAsync(__instance, 0.1f); // Small delay in async method to wait for updating dodge skill
        }
        private static async Task WaitForSecondsAsync(Player player, float seconds)
        {
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milliseconds
            bool existBuff = PlayerUtils.skillStatusEffects.TryGetValue(SkillType.Dodge, out int nameHash);
            if (existBuff)
                Player_RaiseSkill_Patch.updateSkillTypeBuff(player, SkillType.Dodge, nameHash);
        }
    }

    [HarmonyPatch(typeof(Humanoid), "EquipItem")]
    [HarmonyPriority(Priority.VeryLow)]
    public class PlayerEquipItemPatch
    {
        [HarmonyPrefix]
        public static void Postfix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects)
        {
            _ = PlayerInventoryChanges.recalculateSkillsAsync(__instance, item, 0.1f);
        }
    }

    [HarmonyPatch(typeof(Humanoid), "DropItem")]
    [HarmonyPriority(Priority.VeryLow)]
    public class PlayerDropItemPatch
    {
        public static void Postfix(Inventory inventory, ItemDrop.ItemData item, int amount, ref Humanoid __instance)
        {
            _ = PlayerInventoryChanges.recalculateSkillsAsync(__instance, item, 0.1f);
        }
    }
    
    [HarmonyPatch(typeof(Humanoid), "UnequipItem")]
    [HarmonyPriority(Priority.VeryLow)]
    public class PlayerUnequipItemPatch
    {
        public static void Postfix(ItemDrop.ItemData item, bool triggerEquipEffects, ref Humanoid __instance)
        {
            _ = PlayerInventoryChanges.recalculateSkillsAsync(__instance, null, 0.1f);
        }
    }
    
    [HarmonyPatch(typeof(StatusEffect), "RemoveStartEffects")]
    public class StatusEffect_RemoveStartEffects_Patch
    {
        public static void Postfix(StatusEffect __instance)
        {
            Logger.Log("RemoveStartEffects | calling recalculateSkillsAsync after a buff is removed");
            if (__instance.m_character is Humanoid humanoid)
                _ = PlayerInventoryChanges.recalculateSkillsAsync(humanoid, null, 0.1f);
        }
    }
    
    static class PlayerInventoryChanges
    {
        public static async Task recalculateSkillsAsync(Humanoid __instance, ItemDrop.ItemData item, float seconds)
        {
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milliseconds
            if (__instance is Player)
            {
                if (item == null || PlayerUtils.GetTypesThatModifyStats().Contains(item.m_shared.m_itemType))
                {
                    Player player = __instance as Player;
                    //Recalculate skill buffs if some of them has changed
                    foreach (KeyValuePair<SkillType, int> skillStatusEffect in PlayerUtils.skillStatusEffects)
                    {
                        SkillType skillType = skillStatusEffect.Key;
                        int nameHash = skillStatusEffect.Value;
                        if (player.GetSEMan().GetStatusEffect(nameHash) is SE_SkillBuff existingBuff) {
                            Skill playerSkill = PlayerUtils.FindPlayerSkill(player, skillType);
                            float currentSkillLevel = PlayerUtils.GetCurrentSkillLevelProgress(playerSkill);
                            float skillLevelModifier = PlayerUtils.FindActiveModifierValue(player, skillType);

                            Logger.Log($"About to update buff: $skill_{skillType.ToString().ToLower()} with skill level {currentSkillLevel} and skill level modifier {skillLevelModifier}.");
                            existingBuff.UpdateBuffText(PlayerUtils.GetSkillValueToShow(currentSkillLevel, skillLevelModifier));
                            Logger.Log($"Updated buff: {existingBuff.Print()}");
                        }
                    }

                    PlayerColorBuffs.refreshAllBlueColors(player);
                }
            }
        }
    }
}