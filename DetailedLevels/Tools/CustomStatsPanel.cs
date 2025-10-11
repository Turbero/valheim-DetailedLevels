using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DetailedLevels.Tools
{
    public class CustomStatsPanel
    {
        private readonly GameObject panel;
        private readonly TextMeshProUGUI statsTopicText;
        private readonly CustomStatsPanelScroll scrollPanel;
        
        public CustomStatsPanel()
        {
            var trophiesPanel = InventoryGui.instance.m_trophiesPanel;
            if (trophiesPanel == null) return;

            SkillsDialog skillsDialog = InventoryGui.instance.m_skillsDialog;
            
            panel = GameObject.Instantiate(trophiesPanel, skillsDialog.transform);
            panel.name = "PlayerStatsFrame";
            panel.SetActive(false);
            
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, 0); // centered

            panel.transform.Find("TrophiesFrame").name = "MainFrame";

            Transform statsTopicTransform = skillsDialog.transform.Find("PlayerStatsFrame/MainFrame/topic");
            statsTopicText = statsTopicTransform.GetComponent<TextMeshProUGUI>();
            statsTopicText.text = ConfigurationFile.statsText.Value;

            skillsDialog.transform.Find("PlayerStatsFrame/MainFrame/Trophies").name = "Stats";
            skillsDialog.transform.Find("PlayerStatsFrame/MainFrame/Stats/TrophyListScroll").name = "StatsListScroll";
            skillsDialog.transform.Find("PlayerStatsFrame/MainFrame/Stats/TrophyList").name = "StatList";
            
            Dictionary<PlayerStatType, float> mStats = getPlayerDictionaryStats();
            
            scrollPanel = new CustomStatsPanelScroll(skillsDialog.transform.Find("PlayerStatsFrame/MainFrame/Stats/StatList"));
            scrollPanel.AddHeaderToScrollList("Main Stats");
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.WorldLoads, mStats.GetValueSafe(PlayerStatType.WorldLoads)},
                {PlayerStatType.Cheats, mStats.GetValueSafe(PlayerStatType.Cheats)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.EnemyHits, mStats.GetValueSafe(PlayerStatType.EnemyHits)},
                {PlayerStatType.EnemyKills, mStats.GetValueSafe(PlayerStatType.EnemyKills)},
                {PlayerStatType.EnemyKillsLastHits, mStats.GetValueSafe(PlayerStatType.EnemyKillsLastHits)},
                {PlayerStatType.PlayerHits, mStats.GetValueSafe(PlayerStatType.PlayerHits)},
                {PlayerStatType.PlayerKills, mStats.GetValueSafe(PlayerStatType.PlayerKills)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.HitsTakenEnemies, mStats.GetValueSafe(PlayerStatType.HitsTakenEnemies)},
                {PlayerStatType.HitsTakenPlayers, mStats.GetValueSafe(PlayerStatType.HitsTakenPlayers)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.RavenHits, mStats.GetValueSafe(PlayerStatType.RavenHits)},
                {PlayerStatType.RavenTalk, mStats.GetValueSafe(PlayerStatType.RavenTalk)},
                {PlayerStatType.RavenAppear, mStats.GetValueSafe(PlayerStatType.RavenAppear)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.TombstonesOpenedOwn, mStats.GetValueSafe(PlayerStatType.TombstonesOpenedOwn)},
                {PlayerStatType.TombstonesOpenedOther, mStats.GetValueSafe(PlayerStatType.TombstonesOpenedOther)},
                {PlayerStatType.TombstonesFit, mStats.GetValueSafe(PlayerStatType.TombstonesFit)}                
            });
            scrollPanel.AddHeaderToScrollList("Travelling");
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.DistanceTraveled, mStats.GetValueSafe(PlayerStatType.DistanceTraveled)},
                {PlayerStatType.DistanceWalk, mStats.GetValueSafe(PlayerStatType.DistanceWalk)},
                {PlayerStatType.DistanceRun, mStats.GetValueSafe(PlayerStatType.DistanceRun)},
                {PlayerStatType.DistanceSail, mStats.GetValueSafe(PlayerStatType.DistanceSail)},
                {PlayerStatType.DistanceAir, mStats.GetValueSafe(PlayerStatType.DistanceAir)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.TimeInBase, mStats.GetValueSafe(PlayerStatType.TimeInBase)},
                {PlayerStatType.TimeOutOfBase, mStats.GetValueSafe(PlayerStatType.TimeOutOfBase)},
                {PlayerStatType.Sleep, mStats.GetValueSafe(PlayerStatType.Sleep)},
                {PlayerStatType.Jumps, mStats.GetValueSafe(PlayerStatType.Jumps)}
            });
            scrollPanel.AddHeaderToScrollList("Farming");
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.LogChops, mStats.GetValueSafe(PlayerStatType.LogChops)},
                {PlayerStatType.Logs, mStats.GetValueSafe(PlayerStatType.Logs)},
                {PlayerStatType.TreeChops, mStats.GetValueSafe(PlayerStatType.TreeChops)},
                {PlayerStatType.Tree, mStats.GetValueSafe(PlayerStatType.Tree)},
                {PlayerStatType.TreeTier0, mStats.GetValueSafe(PlayerStatType.TreeTier0)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.TreeTier1, mStats.GetValueSafe(PlayerStatType.TreeTier1)},
                {PlayerStatType.TreeTier2, mStats.GetValueSafe(PlayerStatType.TreeTier2)},
                {PlayerStatType.TreeTier3, mStats.GetValueSafe(PlayerStatType.TreeTier3)},
                {PlayerStatType.TreeTier4, mStats.GetValueSafe(PlayerStatType.TreeTier4)},
                {PlayerStatType.TreeTier5, mStats.GetValueSafe(PlayerStatType.TreeTier5)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.MineHits, mStats.GetValueSafe(PlayerStatType.MineHits)},
                {PlayerStatType.Mines, mStats.GetValueSafe(PlayerStatType.Mines)},
                {PlayerStatType.MineTier0, mStats.GetValueSafe(PlayerStatType.MineTier0)},
                {PlayerStatType.MineTier1, mStats.GetValueSafe(PlayerStatType.MineTier1)},
                {PlayerStatType.MineTier2, mStats.GetValueSafe(PlayerStatType.MineTier2)},
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.MineTier3, mStats.GetValueSafe(PlayerStatType.MineTier3)},
                {PlayerStatType.MineTier4, mStats.GetValueSafe(PlayerStatType.MineTier4)},
                {PlayerStatType.MineTier5, mStats.GetValueSafe(PlayerStatType.MineTier5)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.ItemsPickedUp, mStats.GetValueSafe(PlayerStatType.ItemsPickedUp)},
                {PlayerStatType.CreatureTamed, mStats.GetValueSafe(PlayerStatType.CreatureTamed)},
                {PlayerStatType.FoodEaten, mStats.GetValueSafe(PlayerStatType.FoodEaten)},
                {PlayerStatType.BeesHarvested, mStats.GetValueSafe(PlayerStatType.BeesHarvested)},
                {PlayerStatType.SapHarvested, mStats.GetValueSafe(PlayerStatType.SapHarvested)}
            });
            scrollPanel.AddHeaderToScrollList("Construction");
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.Builds, mStats.GetValueSafe(PlayerStatType.Builds)},
                {PlayerStatType.CraftsOrUpgrades, mStats.GetValueSafe(PlayerStatType.CraftsOrUpgrades)},
                {PlayerStatType.Crafts, mStats.GetValueSafe(PlayerStatType.Crafts)},
                {PlayerStatType.Upgrades, mStats.GetValueSafe(PlayerStatType.Upgrades)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.ArmorStandUses, mStats.GetValueSafe(PlayerStatType.ArmorStandUses)},
                {PlayerStatType.ItemStandUses, mStats.GetValueSafe(PlayerStatType.ItemStandUses)},
                
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.DoorsOpened, mStats.GetValueSafe(PlayerStatType.DoorsOpened)},
                {PlayerStatType.DoorsClosed, mStats.GetValueSafe(PlayerStatType.DoorsClosed)},
                {PlayerStatType.PlaceStacks, mStats.GetValueSafe(PlayerStatType.PlaceStacks)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.TurretAmmoAdded, mStats.GetValueSafe(PlayerStatType.TurretAmmoAdded)},
                {PlayerStatType.TurretTrophySet, mStats.GetValueSafe(PlayerStatType.TurretTrophySet)},
                {PlayerStatType.TrapArmed, mStats.GetValueSafe(PlayerStatType.TrapArmed)},
                {PlayerStatType.TrapTriggered, mStats.GetValueSafe(PlayerStatType.TrapTriggered)}
            });
            scrollPanel.AddHeaderToScrollList("Combat");
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.Deaths, mStats.GetValueSafe(PlayerStatType.Deaths)},
                {PlayerStatType.ArrowsShot, mStats.GetValueSafe(PlayerStatType.ArrowsShot)},
                {PlayerStatType.SkeletonSummons, mStats.GetValueSafe(PlayerStatType.SkeletonSummons)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.PortalsUsed, mStats.GetValueSafe(PlayerStatType.PortalsUsed)},
                {PlayerStatType.PortalDungeonIn, mStats.GetValueSafe(PlayerStatType.PortalDungeonIn)},
                {PlayerStatType.PortalDungeonOut, mStats.GetValueSafe(PlayerStatType.PortalDungeonOut)}
                
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.DeathByEnemyHit, mStats.GetValueSafe(PlayerStatType.DeathByEnemyHit)},
                {PlayerStatType.DeathByPlayerHit, mStats.GetValueSafe(PlayerStatType.DeathByPlayerHit)},
                {PlayerStatType.DeathByFall, mStats.GetValueSafe(PlayerStatType.DeathByFall)},
                {PlayerStatType.DeathByDrowning, mStats.GetValueSafe(PlayerStatType.DeathByDrowning)},
                {PlayerStatType.DeathByBurning, mStats.GetValueSafe(PlayerStatType.DeathByBurning)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.DeathByFreezing, mStats.GetValueSafe(PlayerStatType.DeathByFreezing)},
                {PlayerStatType.DeathByPoisoned, mStats.GetValueSafe(PlayerStatType.DeathByPoisoned)},
                {PlayerStatType.DeathBySmoke, mStats.GetValueSafe(PlayerStatType.DeathBySmoke)},
                {PlayerStatType.DeathByWater, mStats.GetValueSafe(PlayerStatType.DeathByWater)},
                {PlayerStatType.DeathByEdgeOfWorld, mStats.GetValueSafe(PlayerStatType.DeathByEdgeOfWorld)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.DeathByImpact, mStats.GetValueSafe(PlayerStatType.DeathByImpact)},
                {PlayerStatType.DeathByCart, mStats.GetValueSafe(PlayerStatType.DeathByCart)},
                {PlayerStatType.DeathByTree, mStats.GetValueSafe(PlayerStatType.DeathByTree)},
                {PlayerStatType.DeathBySelf, mStats.GetValueSafe(PlayerStatType.DeathBySelf)},
                {PlayerStatType.DeathByStructural, mStats.GetValueSafe(PlayerStatType.DeathByStructural)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.DeathByTurret, mStats.GetValueSafe(PlayerStatType.DeathByTurret)},
                {PlayerStatType.DeathByBoat, mStats.GetValueSafe(PlayerStatType.DeathByBoat)},
                {PlayerStatType.DeathByStalagtite, mStats.GetValueSafe(PlayerStatType.DeathByStalagtite)}
            });
            scrollPanel.AddHeaderToScrollList("Progression");
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.BossKills, mStats.GetValueSafe(PlayerStatType.BossKills)},
                {PlayerStatType.BossLastHits, mStats.GetValueSafe(PlayerStatType.BossLastHits)},
                {PlayerStatType.SetGuardianPower, mStats.GetValueSafe(PlayerStatType.SetGuardianPower)},
                {PlayerStatType.UseGuardianPower, mStats.GetValueSafe(PlayerStatType.UseGuardianPower)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.SetPowerEikthyr, mStats.GetValueSafe(PlayerStatType.SetPowerEikthyr)},
                {PlayerStatType.UsePowerEikthyr, mStats.GetValueSafe(PlayerStatType.UsePowerEikthyr)},
                {PlayerStatType.SetPowerElder, mStats.GetValueSafe(PlayerStatType.SetPowerElder)},
                {PlayerStatType.UsePowerElder, mStats.GetValueSafe(PlayerStatType.UsePowerElder)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.SetPowerBonemass, mStats.GetValueSafe(PlayerStatType.SetPowerBonemass)},
                {PlayerStatType.UsePowerBonemass, mStats.GetValueSafe(PlayerStatType.UsePowerBonemass)},
                {PlayerStatType.SetPowerModer, mStats.GetValueSafe(PlayerStatType.SetPowerModer)},
                {PlayerStatType.UsePowerModer, mStats.GetValueSafe(PlayerStatType.UsePowerModer)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.SetPowerYagluth, mStats.GetValueSafe(PlayerStatType.SetPowerYagluth)},
                {PlayerStatType.UsePowerYagluth, mStats.GetValueSafe(PlayerStatType.UsePowerYagluth)},
                {PlayerStatType.SetPowerQueen, mStats.GetValueSafe(PlayerStatType.SetPowerQueen)},
                {PlayerStatType.UsePowerQueen, mStats.GetValueSafe(PlayerStatType.UsePowerQueen)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.SetPowerAshlands, mStats.GetValueSafe(PlayerStatType.SetPowerAshlands)},
                {PlayerStatType.UsePowerAshlands, mStats.GetValueSafe(PlayerStatType.UsePowerAshlands)},
                {PlayerStatType.SetPowerDeepNorth, mStats.GetValueSafe(PlayerStatType.SetPowerDeepNorth)},
                {PlayerStatType.UsePowerDeepNorth, mStats.GetValueSafe(PlayerStatType.UsePowerDeepNorth)}
            });
            scrollPanel.AddHeaderToScrollList("Others");
            scrollPanel.AddRowToScrollList(new Dictionary<PlayerStatType, float>
            {
                {PlayerStatType.DeathByUndefined, mStats.GetValueSafe(PlayerStatType.DeathByUndefined)},
                {PlayerStatType.Count, mStats.GetValueSafe(PlayerStatType.Count)}
            });

            Transform statsCloseButtonTransform = skillsDialog.transform.Find("PlayerStatsFrame/MainFrame/Closebutton");
            Button statsCloseButtonButton = statsCloseButtonTransform.GetComponent<Button>();
            statsCloseButtonButton.onClick = new Button.ButtonClickedEvent();
            statsCloseButtonButton.onClick.AddListener(() =>
            {
                Logger.Log("statsCloseButtonButton clicked.");
                panel.SetActive(false);
            });
        }

        private static Dictionary<PlayerStatType, float> getPlayerDictionaryStats()
        {
            var field = typeof(Game).GetField("m_playerProfile", BindingFlags.Instance | BindingFlags.NonPublic);
            return ((PlayerProfile)field?.GetValue(Game.instance))?.m_playerStats.m_stats;
        }

        public GameObject getPanel() { return panel; }
        
        public void reloadTexts()
        {
            statsTopicText.text = ConfigurationFile.statsText.Value;
            Dictionary<PlayerStatType, float> mStats = getPlayerDictionaryStats();
            foreach (KeyValuePair<PlayerStatType,TextMeshProUGUI> stat in scrollPanel.statsTexts)
            {
                stat.Value.text = stat.Key + ": " + mStats.GetValueSafe(stat.Key);
            }
        }
    }
}