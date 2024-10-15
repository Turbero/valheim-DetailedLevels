using DetailedLevels.Features;
using HarmonyLib;

namespace DetailedLevels
{
    [HarmonyPatch(typeof(Localization), "SetLanguage")]
    public class Localization_SetLanguage_Patch
    {
        static void Postfix(string language)
        {
            Logger.Log($"Language changed to: {language}");

            //Update current skill buffs text to new language
            foreach (var skillType in PlayerUtils.skillStatusEffects.Keys)
            {
                int nameHash = PlayerUtils.skillStatusEffects.GetValueSafe(skillType);
                Player_RaiseSkill_Patch.updateSkillTypeBuff(Player.m_localPlayer, skillType, nameHash, true);
            }
        }
    }
}
