using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(Player), nameof(Player.OnSkillLevelup))]
    class Player_Skillup_Patch
    {
        private static readonly List<String> hildirBossesKeys = new List<string>
        {
            "$enemy_skeletonfire",
            "$enemy_fenringcultist_hildir",
            "$enemy_goblinbrute_hildircombined",
            "$enemy_goblinbrute_hildir",
            "$enemy_goblin_hildir"
        };
        static void Postfix(ref Skills.SkillType skill, ref float level)
        {
            if (isFightingBoss()) return;
            
            //Check big message first
            int multipleBigValue = Math.Max(0, Math.Min(100, ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value));
            if (multipleBigValue > 0 && ((int)level % multipleBigValue == 0 || (int)level == 100))
            {
                MessageHud.instance.ShowBiomeFoundMsg($"$skill_{skill.ToString().ToLower()}: {(int)level}", true);
                return;
            }

            //Check yellow message
            int multipleValue = Math.Max(0, Math.Min(100, ConfigurationFile.skillUpMessageAfterMultipleLevel.Value));
            if (multipleValue > 0 && (int)level % multipleValue == 0)
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
                Logger.LogInfo("Skipping skill up message, you're in an important world fight!");
                return true;
            }
            
            //Hildir quest
            var enemies = Character.GetAllCharacters().Where(c =>
                c != null &&
                !c.IsPlayer() &&
                c.GetBaseAI().IsAlerted() &&
                Vector3.Distance(c.transform.position, Player.m_localPlayer.transform.position) < 25f
            );

            foreach (var enemy in enemies)
                if (hildirBossesKeys.Contains(enemy.m_name))
                {
                    Logger.LogInfo("Skipping skill up message, you're in an important Hildir fight!");
                    return true;
                }

            return false;
        }
    }
}
