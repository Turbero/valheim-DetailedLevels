using HarmonyLib;
using System;
using System.Reflection;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(Player), nameof(Player.OnSkillLevelup))]
    class Player_Skillup_Patch
    {
        static void Postfix(ref Skills.SkillType skill, ref float level)
        {
            //Check big message first
            int multipleBigValue = Math.Max(0, Math.Min(100, ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value));
            if (multipleBigValue > 0 && ((int)level % multipleBigValue == 0 || (int)level == 100) && !isFightingBoss())
            {
                MessageHud.instance.ShowBiomeFoundMsg($"$skill_{skill.ToString().ToLower()}: {(int)level}", true);
                return;
            }

            //Check yellow message
            int multipleValue = Math.Max(0, Math.Min(100, ConfigurationFile.skillUpMessageAfterMultipleLevel.Value));
            if (multipleValue > 0 && (int)level % multipleValue == 0 && !isFightingBoss())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"$msg_skillup $skill_{skill.ToString().ToLower()}: {(int)level}");
            }
        }
        
        private static bool isFightingBoss()
        {
            var getCurrentMusic = typeof(MusicMan).GetMethod("GetCurrentMusic", BindingFlags.NonPublic | BindingFlags.Instance);
            string currentMusic = getCurrentMusic != null ? (string)getCurrentMusic.Invoke(MusicMan.instance, new object[] {} ) : "";
            if (currentMusic.Contains("boss"))
            {
                Logger.LogInfo("Skipping message, you're in an important fight!");
                return true;
            }

            return false;
        }
    }
}
