using HarmonyLib;
using System;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(Player), nameof(Player.OnSkillLevelup))]
    class Player_Skillup_Patch
    {
        static void Postfix(ref Skills.SkillType skill, ref float level)
        {
            if (!ConfigurationFile.modEnabled.Value) return;

            int multipleValue = Math.Max(1, Math.Min(100, ConfigurationFile.skillUpMessageAfterMultipleLevel.Value));
            if ((int)level % multipleValue == 0)
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"$msg_skillup $skill_{skill.ToString().ToLower()}: {level}");
            }
        }
    }
}
