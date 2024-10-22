using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Utils;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    [HarmonyPriority(Priority.VeryLow)]
    class SkillValue_Patch
    {
        static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
        {
            if (!ConfigurationFile.modEnabled.Value) return;

            List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
            for (int j = 0; j < skillList.Count; j++)
            {
                GameObject obj = ___m_elements[j];
                Skills.Skill skill = skillList[j];

                string levelText = PlayerUtils.GetCurrentSkillLevelProgress(skill).ToString();

                Utils.FindChild(obj.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text = levelText;
                
                //Refresh background to adjust after sorting list
                string skillName = skill.m_info.m_skill.ToString();
                int nameHash = PlayerUtils.skillStatusEffects.GetValueSafe(skill.m_info.m_skill);
                StatusEffect existingBuff = player.GetSEMan().GetStatusEffect(nameHash);
                if (existingBuff != null)
                {
                    SkillsDialog_SkillStatusEffects_Patch.setSkillRowBackgroundColor(obj, Color.cyan);
                } else
                {
                    SkillsDialog_SkillStatusEffects_Patch.setSkillRowBackgroundColor(obj, new Color(0f, 0f, 0f, 0f));
                }
            }
        }
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Show))]
    public class InventoryGui_Show_Patch
    {
        static void Postfix(InventoryGui __instance)
        {
            if (!ConfigurationFile.modEnabled.Value) return;

            if (__instance.m_player != null)
            {
                var transform = __instance
                    .transform.Find("root")
                    .transform.Find("Info")
                    .transform.Find("Skills");
                if (transform != null)
                {
                    UITooltip buttonTooltip = transform.GetComponent<UITooltip>();

                    if (buttonTooltip != null)
                    {
                        string originalTooltip = "$inventory_skills";
                        string customText = $" ({ConfigurationFile.hotKey.Value})";

                        buttonTooltip.m_text = originalTooltip + customText;
                    }
                }
            }
        }
    }
}
