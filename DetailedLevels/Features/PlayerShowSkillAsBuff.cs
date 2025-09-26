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
        static void Postfix(SkillsDialog __instance, ref Player player, ref List<GameObject> ___m_elements)
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
                PlayerBuffs.AddSkillBuff(player, skill, skillIcon, skillRow);
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
            if (__instance  != null)
            {
                Character attacker = hit.GetAttacker();
                if (attacker != null)
                {
                    //Staff shield
                    
                    if (__instance.GetType() == typeof(Player))
                    {
                        bool autoDamageWithShield = attacker.GetType() == typeof(Player) && (__instance as Player)?.GetPlayerName() == (attacker as Player)?.GetPlayerName();
                        Logger.Log($"Checking staff shield autoDamage {autoDamageWithShield}...");
                        _ = WaitForSecondsAsyncStaffShield(__instance as Player, 0.1f, autoDamageWithShield);
                    }
                    
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

        private static async Task WaitForSecondsAsyncStaffShield(Player player, float seconds, bool newShield)
        {
            // Small delay in async method to wait for updating blood magic skill
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milliseconds
            //Search SE_Shield and update
            Logger.Log("BloodMagic_BuffUpdate_Patch - Initialize staff shield value");
            SE_Shield_Setup_Patch.updateShieldBuffTextIfExists(player, newShield);
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

    [HarmonyPatch(typeof(SE_Shield), "Setup")]
    public class SE_Shield_Setup_Patch
    {
        static void Postfix(Character __instance)
        {
            if (__instance != null && __instance.GetType() == typeof(Player))
            {
                Logger.Log("SE_Shield_Setup_Patch - Initialize staff shield value");
                updateShieldBuffTextIfExists(__instance as Player, true);
                
                //Detect if __instance is NOT the local player, and update the buff text to the m_localplayer if his buff exists on him (meaning he was in range)
                if (__instance != Player.m_localPlayer)
                {
                    Logger.Log("SE_Shield_Setup_Patch - Another player did the spell. Checking local player");
                    updateShieldBuffTextIfExists(Player.m_localPlayer, true);
                }
            }
        }

        public static void updateShieldBuffTextIfExists(Player player, bool newShield)
        {
            StatusEffect statusEffect = player.GetSEMan().GetStatusEffects().Find(se => se.m_name.Contains("$se_shield"));
            if (statusEffect != null && statusEffect.GetType() == typeof(SE_Shield))
            {
                SE_Shield staffShield = statusEffect as SE_Shield;
                float m_totalAbsorbDamage = (float) typeof(SE_Shield).GetField("m_totalAbsorbDamage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(staffShield);
                if (newShield)
                {
                    staffShield.m_name = $"$se_shield: {m_totalAbsorbDamage}";
                    Logger.Log($"New shield: m_totalAbsorbDamage {m_totalAbsorbDamage}");
                }
                else
                {
                    //Show remaining damage absorb
                    float m_damage = (float) typeof(SE_Shield).GetField("m_damage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(staffShield);
                    float remainingAbsorbDamage = (float)Math.Round(m_totalAbsorbDamage - m_damage, Math.Min(15, Math.Max(0, ConfigurationFile.numberOfDecimals.Value)));
                    staffShield.m_name = $"$se_shield: {remainingAbsorbDamage}";
                    Logger.Log($"Shield numbers: m_totalAbsorbDamage {m_totalAbsorbDamage}, m_damage {m_damage}, remainingAbsorbDamage {remainingAbsorbDamage}");
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
}