using HarmonyLib;
using TMPro;
using UnityEngine;

namespace DetailedLevels.Features
{
    static class CampfireCheck
    {
        public static bool campfireCheckActive = false;

        [HarmonyPatch(typeof(SEMan), "AddStatusEffect", typeof(StatusEffect), typeof(bool), typeof(int), typeof(float))]
        public class SEManAddStatusPatch1
        {
            [HarmonyPrefix]
            public static void Postfix(StatusEffect statusEffect, bool resetTime, int itemLevel, float skillLevel, ref Character ___m_character)
            {
                if (SEMan.s_statusEffectCampFire == statusEffect.GetHashCode() && !campfireCheckActive)
                    campfireCheckActive = true;

                Logger.Log("SEMan.AddStatusEffect - SEManAddStatusPatch1");
                _ = PlayerInventoryChanges.recalculateSkillsAsync(___m_character as Humanoid, null, 0.1f);
            }
        }

        [HarmonyPatch(typeof(SEMan), "AddStatusEffect", typeof(int), typeof(bool), typeof(int), typeof(float))]
        public class SEManAddStatusPatch2
        {

            [HarmonyPrefix]
            public static void Postfix(int nameHash, bool resetTime, int itemLevel, float skillLevel, ref Character ___m_character)
            {
                if (nameHash == SEMan.s_statusEffectCampFire && !campfireCheckActive)
                {
                    campfireCheckActive = true;
                    Logger.Log("SEMan.AddStatusEffect - SEManAddStatusPatch2");
                    _ = PlayerInventoryChanges.recalculateSkillsAsync(___m_character as Humanoid, null, 0.1f);
                }
            }
        }

        [HarmonyPatch(typeof(SEMan), "RemoveStatusEffect", typeof(StatusEffect), typeof(bool))]
        public class SEManRemoveStatusPatch1
        {
            [HarmonyPrefix]
            public static void Postfix(StatusEffect se, bool quiet, ref Character ___m_character, ref bool __result)
            {
                if (SEMan.s_statusEffectCampFire == se.GetHashCode() && campfireCheckActive)
                    campfireCheckActive = false;
                
                Logger.Log("SEMan.RemoveStatusEffect - SEManRemoveStatusPatch1");
                _ = PlayerInventoryChanges.recalculateSkillsAsync(___m_character as Humanoid, null, 0.1f);
            }
        }

        [HarmonyPatch(typeof(SEMan), "RemoveStatusEffect", typeof(int), typeof(bool))]
        public class SEManRemoveStatusPatch2
        {
            [HarmonyPrefix]
            public static void Postfix(int nameHash, bool quiet, ref Character ___m_character, ref bool __result)
            {
                if (nameHash == SEMan.s_statusEffectCampFire && campfireCheckActive)
                {
                    campfireCheckActive = false;
                    //if (__result) {
                    Logger.Log("SEMan.RemoveStatusEffect - SEManRemoveStatusPatch2");
                    _ = PlayerInventoryChanges.recalculateSkillsAsync(___m_character as Humanoid, null, 0.1f);
                    //}
                }
            }
        }
    }

    static class PlayerColorBuffs {
        public static void refreshAllBlueColors(Player player)
        {
            foreach (Skills.Skill skillValue in PlayerBuffs.skills.Values)
            {
                float skillLevelModifer = PlayerUtils.FindActiveModifierValue(player, skillValue.m_info.m_skill);
                if (skillLevelModifer > 0)
                {
                    string textToFind =
                        Localization.instance.Localize($"$skill_{skillValue.m_info.m_skill.ToString().ToLower()}");
                    for (int i = 0; i < Hud.instance.m_statusEffectListRoot.childCount; i++)
                    {
                        TextMeshProUGUI text = Hud.instance.m_statusEffectListRoot.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
                        Logger.Log($"Finding text {textToFind} in {text.text}");
                        if (text.text.Contains(textToFind) &&
                            text.text.Length > textToFind.Length) // buff with skill name and skill level
                        {
                            text.faceColor = new Color32(0, 189, 255, 255); // Extra skill blue color used: (0, 188.955, 255, 255)
                            Logger.Log("Color updated.");
                            break;
                        }
                    }
                }
            }
        }
    }
}