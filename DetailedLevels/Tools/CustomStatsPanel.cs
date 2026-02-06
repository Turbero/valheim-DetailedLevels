using System;
using System.Collections.Generic;
using System.Reflection;
using DetailedLevels.Features;
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

            Transform statsTopicTransform = skillsDialog.transform.Find(panel.name + "/MainFrame/topic");
            statsTopicText = statsTopicTransform.GetComponent<TextMeshProUGUI>();
            statsTopicText.text = ConfigurationFile.statsText.Value;

            skillsDialog.transform.Find(panel.name + "/MainFrame/Trophies").name = "Stats";
            skillsDialog.transform.Find(panel.name + "/MainFrame/Stats/TrophyListScroll").name = "StatsListScroll";
            skillsDialog.transform.Find(panel.name + "/MainFrame/Stats/TrophyList").name = "StatList";

            //Close button
            Transform statsCloseButtonTransform = skillsDialog.transform.Find(panel.name + "/MainFrame/Closebutton");
            Button statsCloseButtonButton = statsCloseButtonTransform.GetComponent<Button>();
            statsCloseButtonButton.onClick = new Button.ButtonClickedEvent();
            statsCloseButtonButton.onClick.AddListener(() =>
            {
                Logger.Log("statsCloseButtonButton clicked.");
                panel.SetActive(false);
                PlayerSkillupOptionsPatch.HideTabButtons();
            });

            scrollPanel = new CustomStatsPanelScroll(skillsDialog.transform.Find(panel.name+"/MainFrame/Stats/StatList"));
            Dictionary<PlayerStatType, float> mStats = getPlayerDictionaryStats();
            
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.WorldLoads.ToString(), mStats.GetValueSafe(PlayerStatType.WorldLoads)},
                {PlayerStatType.Cheats.ToString(), mStats.GetValueSafe(PlayerStatType.Cheats)}
            });
            scrollPanel.AddHeaderToScrollList("$menu_combat");
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.Deaths.ToString(), mStats.GetValueSafe(PlayerStatType.Deaths)},
                {PlayerStatType.HitsTakenEnemies.ToString(), mStats.GetValueSafe(PlayerStatType.HitsTakenEnemies)},
                {PlayerStatType.HitsTakenPlayers.ToString(), mStats.GetValueSafe(PlayerStatType.HitsTakenPlayers)},
                {PlayerStatType.ArrowsShot.ToString(), mStats.GetValueSafe(PlayerStatType.ArrowsShot)},
                {PlayerStatType.SkeletonSummons.ToString(), mStats.GetValueSafe(PlayerStatType.SkeletonSummons)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.EnemyHits.ToString(), mStats.GetValueSafe(PlayerStatType.EnemyHits)},
                {PlayerStatType.EnemyKills.ToString(), mStats.GetValueSafe(PlayerStatType.EnemyKills)},
                {PlayerStatType.EnemyKillsLastHits.ToString(), mStats.GetValueSafe(PlayerStatType.EnemyKillsLastHits)},
                {PlayerStatType.PlayerHits.ToString(), mStats.GetValueSafe(PlayerStatType.PlayerHits)},
                {PlayerStatType.PlayerKills.ToString(), mStats.GetValueSafe(PlayerStatType.PlayerKills)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.TombstonesOpenedOwn.ToString(), mStats.GetValueSafe(PlayerStatType.TombstonesOpenedOwn)},
                {PlayerStatType.TombstonesOpenedOther.ToString(), mStats.GetValueSafe(PlayerStatType.TombstonesOpenedOther)},
                {PlayerStatType.TombstonesFit.ToString(), mStats.GetValueSafe(PlayerStatType.TombstonesFit)}                
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.PortalsUsed.ToString(), mStats.GetValueSafe(PlayerStatType.PortalsUsed)},
                {PlayerStatType.PortalDungeonIn.ToString(), mStats.GetValueSafe(PlayerStatType.PortalDungeonIn)},
                {PlayerStatType.PortalDungeonOut.ToString(), mStats.GetValueSafe(PlayerStatType.PortalDungeonOut)}
                
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.DeathByEnemyHit.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByEnemyHit)},
                {PlayerStatType.DeathByPlayerHit.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByPlayerHit)},
                {PlayerStatType.DeathByFall.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByFall)},
                {PlayerStatType.DeathByDrowning.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByDrowning)},
                {PlayerStatType.DeathByBurning.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByBurning)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.DeathByFreezing.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByFreezing)},
                {PlayerStatType.DeathByPoisoned.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByPoisoned)},
                {PlayerStatType.DeathBySmoke.ToString(), mStats.GetValueSafe(PlayerStatType.DeathBySmoke)},
                {PlayerStatType.DeathByWater.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByWater)},
                {PlayerStatType.DeathByEdgeOfWorld.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByEdgeOfWorld)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.DeathByImpact.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByImpact)},
                {PlayerStatType.DeathByCart.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByCart)},
                {PlayerStatType.DeathByTree.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByTree)},
                {PlayerStatType.DeathBySelf.ToString(), mStats.GetValueSafe(PlayerStatType.DeathBySelf)},
                {PlayerStatType.DeathByStructural.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByStructural)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.DeathByTurret.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByTurret)},
                {PlayerStatType.DeathByBoat.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByBoat)},
                {PlayerStatType.DeathByStalagtite.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByStalagtite)}
            });
            scrollPanel.AddHeaderToScrollList(ConfigurationFile.statsProgressionText.Value);
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.BossKills.ToString(), mStats.GetValueSafe(PlayerStatType.BossKills)},
                {PlayerStatType.BossLastHits.ToString(), mStats.GetValueSafe(PlayerStatType.BossLastHits)},
                {PlayerStatType.SetGuardianPower.ToString(), mStats.GetValueSafe(PlayerStatType.SetGuardianPower)},
                {PlayerStatType.UseGuardianPower.ToString(), mStats.GetValueSafe(PlayerStatType.UseGuardianPower)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.SetPowerEikthyr.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerEikthyr)},
                {PlayerStatType.UsePowerEikthyr.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerEikthyr)},
                {PlayerStatType.SetPowerElder.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerElder)},
                {PlayerStatType.UsePowerElder.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerElder)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.SetPowerBonemass.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerBonemass)},
                {PlayerStatType.UsePowerBonemass.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerBonemass)},
                {PlayerStatType.SetPowerModer.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerModer)},
                {PlayerStatType.UsePowerModer.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerModer)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.SetPowerYagluth.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerYagluth)},
                {PlayerStatType.UsePowerYagluth.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerYagluth)},
                {PlayerStatType.SetPowerQueen.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerQueen)},
                {PlayerStatType.UsePowerQueen.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerQueen)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.SetPowerAshlands.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerAshlands)},
                {PlayerStatType.UsePowerAshlands.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerAshlands)},
                {PlayerStatType.SetPowerDeepNorth.ToString(), mStats.GetValueSafe(PlayerStatType.SetPowerDeepNorth)},
                {PlayerStatType.UsePowerDeepNorth.ToString(), mStats.GetValueSafe(PlayerStatType.UsePowerDeepNorth)}
            });
            scrollPanel.AddHeaderToScrollList(ConfigurationFile.statsTravellingText.Value);
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.DistanceTraveled.ToString(), (float)Math.Round(mStats.GetValueSafe(PlayerStatType.DistanceTraveled), 2)},
                {PlayerStatType.DistanceWalk.ToString(), (float)Math.Round(mStats.GetValueSafe(PlayerStatType.DistanceWalk), 2)},
                {PlayerStatType.DistanceRun.ToString(), (float)Math.Round(mStats.GetValueSafe(PlayerStatType.DistanceRun), 2)},
                {PlayerStatType.DistanceSail.ToString(), (float)Math.Round(mStats.GetValueSafe(PlayerStatType.DistanceSail), 2)},
                {PlayerStatType.DistanceAir.ToString(), (float)Math.Round(mStats.GetValueSafe(PlayerStatType.DistanceAir), 2)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.TimeInBase.ToString(), (float)Math.Round(mStats.GetValueSafe(PlayerStatType.TimeInBase), 2)},
                {PlayerStatType.TimeOutOfBase.ToString(), (float)Math.Round(mStats.GetValueSafe(PlayerStatType.TimeOutOfBase), 2)},
                {PlayerStatType.Sleep.ToString(), mStats.GetValueSafe(PlayerStatType.Sleep)},
                {PlayerStatType.Jumps.ToString(), mStats.GetValueSafe(PlayerStatType.Jumps)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.RavenHits.ToString(), mStats.GetValueSafe(PlayerStatType.RavenHits)},
                {PlayerStatType.RavenTalk.ToString(), mStats.GetValueSafe(PlayerStatType.RavenTalk)},
                {PlayerStatType.RavenAppear.ToString(), mStats.GetValueSafe(PlayerStatType.RavenAppear)}
            });
            scrollPanel.AddHeaderToScrollList("$menu_resources");
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.LogChops.ToString(), mStats.GetValueSafe(PlayerStatType.LogChops)},
                {PlayerStatType.Logs.ToString(), mStats.GetValueSafe(PlayerStatType.Logs)},
                {PlayerStatType.TreeChops.ToString(), mStats.GetValueSafe(PlayerStatType.TreeChops)},
                {PlayerStatType.Tree.ToString(), mStats.GetValueSafe(PlayerStatType.Tree)},
                {PlayerStatType.TreeTier0.ToString(), mStats.GetValueSafe(PlayerStatType.TreeTier0)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.TreeTier1.ToString(), mStats.GetValueSafe(PlayerStatType.TreeTier1)},
                {PlayerStatType.TreeTier2.ToString(), mStats.GetValueSafe(PlayerStatType.TreeTier2)},
                {PlayerStatType.TreeTier3.ToString(), mStats.GetValueSafe(PlayerStatType.TreeTier3)},
                {PlayerStatType.TreeTier4.ToString(), mStats.GetValueSafe(PlayerStatType.TreeTier4)},
                {PlayerStatType.TreeTier5.ToString(), mStats.GetValueSafe(PlayerStatType.TreeTier5)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.MineHits.ToString(), mStats.GetValueSafe(PlayerStatType.MineHits)},
                {PlayerStatType.Mines.ToString(), mStats.GetValueSafe(PlayerStatType.Mines)},
                {PlayerStatType.MineTier0.ToString(), mStats.GetValueSafe(PlayerStatType.MineTier0)},
                {PlayerStatType.MineTier1.ToString(), mStats.GetValueSafe(PlayerStatType.MineTier1)},
                {PlayerStatType.MineTier2.ToString(), mStats.GetValueSafe(PlayerStatType.MineTier2)},
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.MineTier3.ToString(), mStats.GetValueSafe(PlayerStatType.MineTier3)},
                {PlayerStatType.MineTier4.ToString(), mStats.GetValueSafe(PlayerStatType.MineTier4)},
                {PlayerStatType.MineTier5.ToString(), mStats.GetValueSafe(PlayerStatType.MineTier5)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.ItemsPickedUp.ToString(), mStats.GetValueSafe(PlayerStatType.ItemsPickedUp)},
                {PlayerStatType.CreatureTamed.ToString(), mStats.GetValueSafe(PlayerStatType.CreatureTamed)},
                {PlayerStatType.FoodEaten.ToString(), mStats.GetValueSafe(PlayerStatType.FoodEaten)},
                {PlayerStatType.BeesHarvested.ToString(), mStats.GetValueSafe(PlayerStatType.BeesHarvested)},
                {PlayerStatType.SapHarvested.ToString(), mStats.GetValueSafe(PlayerStatType.SapHarvested)}
            });
            scrollPanel.AddHeaderToScrollList("$hud_building");
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.Builds.ToString(), mStats.GetValueSafe(PlayerStatType.Builds)},
                {PlayerStatType.CraftsOrUpgrades.ToString(), mStats.GetValueSafe(PlayerStatType.CraftsOrUpgrades)},
                {PlayerStatType.Crafts.ToString(), mStats.GetValueSafe(PlayerStatType.Crafts)},
                {PlayerStatType.Upgrades.ToString(), mStats.GetValueSafe(PlayerStatType.Upgrades)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.ArmorStandUses.ToString(), mStats.GetValueSafe(PlayerStatType.ArmorStandUses)},
                {PlayerStatType.ItemStandUses.ToString(), mStats.GetValueSafe(PlayerStatType.ItemStandUses)},
                
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.DoorsOpened.ToString(), mStats.GetValueSafe(PlayerStatType.DoorsOpened)},
                {PlayerStatType.DoorsClosed.ToString(), mStats.GetValueSafe(PlayerStatType.DoorsClosed)},
                {PlayerStatType.PlaceStacks.ToString(), mStats.GetValueSafe(PlayerStatType.PlaceStacks)}
            });
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.TurretAmmoAdded.ToString(), mStats.GetValueSafe(PlayerStatType.TurretAmmoAdded)},
                {PlayerStatType.TurretTrophySet.ToString(), mStats.GetValueSafe(PlayerStatType.TurretTrophySet)},
                {PlayerStatType.TrapArmed.ToString(), mStats.GetValueSafe(PlayerStatType.TrapArmed)},
                {PlayerStatType.TrapTriggered.ToString(), mStats.GetValueSafe(PlayerStatType.TrapTriggered)}
            });
            scrollPanel.AddHeaderToScrollList(ConfigurationFile.statsOthersText.Value);
            scrollPanel.AddRowToScrollList(new Dictionary<string, float>
            {
                {PlayerStatType.DeathByUndefined.ToString(), mStats.GetValueSafe(PlayerStatType.DeathByUndefined)},
                {PlayerStatType.Count.ToString(), mStats.GetValueSafe(PlayerStatType.Count)}
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
            foreach (KeyValuePair<string,TextMeshProUGUI> stat in scrollPanel.statsTexts)
            {
                stat.Value.text = stat.Key + ": " + mStats.GetValueSafe((PlayerStatType)Enum.Parse(typeof(PlayerStatType), stat.Key));
            }
            scrollPanel.reloadHeaderTitles();
        }
    }
}