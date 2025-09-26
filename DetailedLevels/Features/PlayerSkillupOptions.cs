using DetailedLevels.Tools;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Skills;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), "Awake")]
    public class PlayerSkillupOptionsPatch
    {
        public static CustomSkillOptionsPanel panel;
        private static readonly Color TITLE_COLOR = new Color(1f, 0.7176f, 0.3603f);
        private static TextMeshProUGUI buttonText;
        private static TextMeshProUGUI lossPercentageTextComponent;
        private static TextMeshProUGUI lossPercentageValueComponent;
        private static CustomSlider customSliderSaveSwitch;
        private static CustomSlider customSliderNumberOfDecimals;
        private static CustomSlider customSliderSkillUpMessage;
        private static CustomSlider customSliderSkillUpBigMessage;

        static void Postfix(SkillsDialog __instance)
        {
            Transform closeButtonTransform = __instance.transform.Find("SkillsFrame/Closebutton");
            (closeButtonTransform as RectTransform).anchoredPosition = new Vector2(101, 45);
            Button closeButton = closeButtonTransform.GetComponent<Button>();

            GameObject dlOptionsButtonObject = GameObject.Instantiate(closeButton.gameObject, closeButton.transform.parent);
            dlOptionsButtonObject.name = "DLOptionsButton";

            RectTransform dlOptionsButtonRect = dlOptionsButtonObject.GetComponent<RectTransform>();
            dlOptionsButtonRect.anchoredPosition = new Vector2(-100, 45);

            buttonText = dlOptionsButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.fontStyle = FontStyles.Normal;
            buttonText.color = TITLE_COLOR;
            buttonText.alignment = TextAlignmentOptions.Center;

            // New Options wood panel
            panel = new CustomSkillOptionsPanel(closeButton.transform.parent);

            Button dlOptionsButton = dlOptionsButtonObject.GetComponent<Button>();
            dlOptionsButton.onClick = new Button.ButtonClickedEvent();
            dlOptionsButton.onClick.AddListener(() =>
            {
                Logger.Log("DLOptionsButton clicked.");
                panel.getPanel().SetActive(true);
                Logger.Log("DLOptionsButton - panel created.");
            });

            addSoftDeathInfo(panel.getPanel().transform);
            addSaveSwitchButton(panel.getPanel().transform);
            addNumberOfDecimalsSlider(panel.getPanel().transform);
            addSkillUpMessage(panel.getPanel().transform);
            addSkillUpBigMessage(panel.getPanel().transform);
            
            reloadTexts();
            
            var trophiesPanel = InventoryGui.instance.m_trophiesPanel;
            if (trophiesPanel == null) return;
            
            GameObject myPanel = GameObject.Instantiate(trophiesPanel, InventoryGui.instance.m_skillsDialog.transform);
            myPanel.name = "PlayerStatsFrame";
            myPanel.SetActive(true);
            
            RectTransform rt = myPanel.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, 0); // centered

            Transform statsTopicTransform = __instance.transform.Find("PlayerStatsFrame/TrophiesFrame/topic");
            TextMeshProUGUI statsTopicText = statsTopicTransform.GetComponent<TextMeshProUGUI>();
            statsTopicText.text = "STATS";

            Transform statsCloseButtonTransform =
                __instance.transform.Find("PlayerStatsFrame/TrophiesFrame/Closebutton");
            Button statsCloseButtonButton = statsCloseButtonTransform.GetComponent<Button>();
            statsCloseButtonButton.onClick = new Button.ButtonClickedEvent();
            statsCloseButtonButton.onClick.AddListener(() =>
            {
                Logger.Log("statsCloseButtonButton clicked.");
                myPanel.SetActive(!myPanel.activeSelf);
            });

            var field = typeof(Game).GetField("m_playerProfile", BindingFlags.Instance | BindingFlags.NonPublic);
            Dictionary<PlayerStatType,float> mStats = ((PlayerProfile)field.GetValue(Game.instance)).m_playerStats.m_stats;
                
            AddEntry(PlayerStatType.Deaths, mStats.GetValueSafe(PlayerStatType.Deaths), new Vector2(-500, 300));
            AddEntry(PlayerStatType.CraftsOrUpgrades, mStats.GetValueSafe(PlayerStatType.CraftsOrUpgrades), new Vector2(-500, 280));
            AddEntry(PlayerStatType.Builds, mStats.GetValueSafe(PlayerStatType.Builds), new Vector2(-500, 260));
            AddEntry(PlayerStatType.Jumps, mStats.GetValueSafe(PlayerStatType.Jumps), new Vector2(-500, 240));
            AddEntry(PlayerStatType.Cheats, mStats.GetValueSafe(PlayerStatType.Cheats), new Vector2(-500, 220));
            AddEntry(PlayerStatType.EnemyHits, mStats.GetValueSafe(PlayerStatType.EnemyHits), new Vector2(-500, 200));
            AddEntry(PlayerStatType.EnemyKills, mStats.GetValueSafe(PlayerStatType.EnemyKills), new Vector2(-500, 180));
            AddEntry(PlayerStatType.EnemyKillsLastHits, mStats.GetValueSafe(PlayerStatType.EnemyKillsLastHits), new Vector2(-500, 160));
            AddEntry(PlayerStatType.PlayerHits, mStats.GetValueSafe(PlayerStatType.PlayerHits), new Vector2(-500, 140));
            AddEntry(PlayerStatType.PlayerKills, mStats.GetValueSafe(PlayerStatType.PlayerKills), new Vector2(-500, 120));
            AddEntry(PlayerStatType.HitsTakenEnemies, mStats.GetValueSafe(PlayerStatType.HitsTakenEnemies), new Vector2(-500, 100));
            AddEntry(PlayerStatType.HitsTakenPlayers, mStats.GetValueSafe(PlayerStatType.HitsTakenPlayers), new Vector2(-500, 80));
            AddEntry(PlayerStatType.ItemsPickedUp, mStats.GetValueSafe(PlayerStatType.ItemsPickedUp), new Vector2(-500, 60));
            AddEntry(PlayerStatType.Crafts, mStats.GetValueSafe(PlayerStatType.Crafts), new Vector2(-500, 40));
            AddEntry(PlayerStatType.Upgrades, mStats.GetValueSafe(PlayerStatType.Upgrades), new Vector2(-500, 20));
            AddEntry(PlayerStatType.PortalsUsed, mStats.GetValueSafe(PlayerStatType.PortalsUsed), new Vector2(-500, 0));
            AddEntry(PlayerStatType.DistanceTraveled, mStats.GetValueSafe(PlayerStatType.DistanceTraveled), new Vector2(-475.5f, -20), new Vector2(250, 50));
            AddEntry(PlayerStatType.DistanceWalk, mStats.GetValueSafe(PlayerStatType.DistanceWalk), new Vector2(-500, -40));
            AddEntry(PlayerStatType.DistanceRun, mStats.GetValueSafe(PlayerStatType.DistanceRun), new Vector2(-500, -60));
            AddEntry(PlayerStatType.DistanceSail, mStats.GetValueSafe(PlayerStatType.DistanceSail), new Vector2(-500, -80));
            AddEntry(PlayerStatType.DistanceAir, mStats.GetValueSafe(PlayerStatType.DistanceAir), new Vector2(-500, -100));
            AddEntry(PlayerStatType.TimeInBase, mStats.GetValueSafe(PlayerStatType.TimeInBase), new Vector2(-500, -120));
            AddEntry(PlayerStatType.TimeOutOfBase, mStats.GetValueSafe(PlayerStatType.TimeOutOfBase), new Vector2(-500, -140));
            AddEntry(PlayerStatType.Sleep, mStats.GetValueSafe(PlayerStatType.Sleep), new Vector2(-500, -160));
            AddEntry(PlayerStatType.ItemStandUses, mStats.GetValueSafe(PlayerStatType.ItemStandUses), new Vector2(-500, -180));
            AddEntry(PlayerStatType.ArmorStandUses, mStats.GetValueSafe(PlayerStatType.ArmorStandUses), new Vector2(-500, -200));
            AddEntry(PlayerStatType.WorldLoads, mStats.GetValueSafe(PlayerStatType.WorldLoads), new Vector2(-250, 300));
            AddEntry(PlayerStatType.TreeChops, mStats.GetValueSafe(PlayerStatType.TreeChops), new Vector2(-250, 280));
            AddEntry(PlayerStatType.Tree, mStats.GetValueSafe(PlayerStatType.Tree), new Vector2(-250, 260));
            AddEntry(PlayerStatType.TreeTier0, mStats.GetValueSafe(PlayerStatType.TreeTier0), new Vector2(-250, 240));
            AddEntry(PlayerStatType.TreeTier1, mStats.GetValueSafe(PlayerStatType.TreeTier1), new Vector2(-250, 220));
            AddEntry(PlayerStatType.TreeTier2, mStats.GetValueSafe(PlayerStatType.TreeTier2), new Vector2(-250, 200));
            AddEntry(PlayerStatType.TreeTier3, mStats.GetValueSafe(PlayerStatType.TreeTier3), new Vector2(-250, 180));
            AddEntry(PlayerStatType.TreeTier4, mStats.GetValueSafe(PlayerStatType.TreeTier4), new Vector2(-250, 160));
            AddEntry(PlayerStatType.TreeTier5, mStats.GetValueSafe(PlayerStatType.TreeTier5), new Vector2(-250, 140));
            AddEntry(PlayerStatType.LogChops, mStats.GetValueSafe(PlayerStatType.LogChops), new Vector2(-250, 120));
            AddEntry(PlayerStatType.Logs, mStats.GetValueSafe(PlayerStatType.Logs), new Vector2(-250, 100));
            AddEntry(PlayerStatType.MineHits, mStats.GetValueSafe(PlayerStatType.MineHits), new Vector2(-250, 80));
            AddEntry(PlayerStatType.Mines, mStats.GetValueSafe(PlayerStatType.Mines), new Vector2(-250, 60));
            AddEntry(PlayerStatType.MineTier0, mStats.GetValueSafe(PlayerStatType.MineTier0), new Vector2(-250, 40));
            AddEntry(PlayerStatType.MineTier1, mStats.GetValueSafe(PlayerStatType.MineTier1), new Vector2(-250, 20));
            AddEntry(PlayerStatType.MineTier2, mStats.GetValueSafe(PlayerStatType.MineTier2), new Vector2(-250, 0));
            AddEntry(PlayerStatType.MineTier3, mStats.GetValueSafe(PlayerStatType.MineTier3), new Vector2(-250, -20));
            AddEntry(PlayerStatType.MineTier4, mStats.GetValueSafe(PlayerStatType.MineTier4), new Vector2(-250, -40));
            AddEntry(PlayerStatType.MineTier5, mStats.GetValueSafe(PlayerStatType.MineTier5), new Vector2(-250, -60));
            AddEntry(PlayerStatType.RavenHits, mStats.GetValueSafe(PlayerStatType.RavenHits), new Vector2(-250, -80));
            AddEntry(PlayerStatType.RavenTalk, mStats.GetValueSafe(PlayerStatType.RavenTalk), new Vector2(-250, -100));
            AddEntry(PlayerStatType.RavenAppear, mStats.GetValueSafe(PlayerStatType.RavenAppear), new Vector2(-250, -120));
            AddEntry(PlayerStatType.CreatureTamed, mStats.GetValueSafe(PlayerStatType.CreatureTamed), new Vector2(-250, -140));
            AddEntry(PlayerStatType.FoodEaten, mStats.GetValueSafe(PlayerStatType.FoodEaten), new Vector2(-250, -160));
            AddEntry(PlayerStatType.SkeletonSummons, mStats.GetValueSafe(PlayerStatType.SkeletonSummons), new Vector2(-250, -180));
            AddEntry(PlayerStatType.ArrowsShot, mStats.GetValueSafe(PlayerStatType.ArrowsShot), new Vector2(-250, -200));
            AddEntry(PlayerStatType.TombstonesOpenedOwn, mStats.GetValueSafe(PlayerStatType.TombstonesOpenedOwn), new Vector2(5, 300), new Vector2(210, 50));
            AddEntry(PlayerStatType.TombstonesOpenedOther, mStats.GetValueSafe(PlayerStatType.TombstonesOpenedOther), new Vector2(5, 280), new Vector2(210, 50));
            AddEntry(PlayerStatType.TombstonesFit, mStats.GetValueSafe(PlayerStatType.TombstonesFit), new Vector2(0, 260));
            AddEntry(PlayerStatType.DeathByUndefined, mStats.GetValueSafe(PlayerStatType.DeathByUndefined), new Vector2(0, 240));
            AddEntry(PlayerStatType.DeathByEnemyHit, mStats.GetValueSafe(PlayerStatType.DeathByEnemyHit), new Vector2(0, 220));
            AddEntry(PlayerStatType.DeathByPlayerHit, mStats.GetValueSafe(PlayerStatType.DeathByPlayerHit), new Vector2(0, 200));
            AddEntry(PlayerStatType.DeathByFall, mStats.GetValueSafe(PlayerStatType.DeathByFall), new Vector2(0, 180));
            AddEntry(PlayerStatType.DeathByDrowning, mStats.GetValueSafe(PlayerStatType.DeathByDrowning), new Vector2(0, 160));
            AddEntry(PlayerStatType.DeathByBurning, mStats.GetValueSafe(PlayerStatType.DeathByBurning), new Vector2(0, 140));
            AddEntry(PlayerStatType.DeathByFreezing, mStats.GetValueSafe(PlayerStatType.DeathByFreezing), new Vector2(0, 120));
            AddEntry(PlayerStatType.DeathByPoisoned, mStats.GetValueSafe(PlayerStatType.DeathByPoisoned), new Vector2(0, 100));
            AddEntry(PlayerStatType.DeathBySmoke, mStats.GetValueSafe(PlayerStatType.DeathBySmoke), new Vector2(0, 80));
            AddEntry(PlayerStatType.DeathByWater, mStats.GetValueSafe(PlayerStatType.DeathByWater), new Vector2(0, 60));
            AddEntry(PlayerStatType.DeathByEdgeOfWorld, mStats.GetValueSafe(PlayerStatType.DeathByEdgeOfWorld), new Vector2(0, 40));
            AddEntry(PlayerStatType.DeathByImpact, mStats.GetValueSafe(PlayerStatType.DeathByImpact), new Vector2(0, 20));
            AddEntry(PlayerStatType.DeathByCart, mStats.GetValueSafe(PlayerStatType.DeathByCart), new Vector2(0, 0));
            AddEntry(PlayerStatType.DeathByTree, mStats.GetValueSafe(PlayerStatType.DeathByTree), new Vector2(0, -20));
            AddEntry(PlayerStatType.DeathBySelf, mStats.GetValueSafe(PlayerStatType.DeathBySelf), new Vector2(0, -40));
            AddEntry(PlayerStatType.DeathByStructural, mStats.GetValueSafe(PlayerStatType.DeathByStructural), new Vector2(0, -60));
            AddEntry(PlayerStatType.DeathByTurret, mStats.GetValueSafe(PlayerStatType.DeathByTurret), new Vector2(0, -80));
            AddEntry(PlayerStatType.DeathByBoat, mStats.GetValueSafe(PlayerStatType.DeathByBoat), new Vector2(0, -100));
            AddEntry(PlayerStatType.DeathByStalagtite, mStats.GetValueSafe(PlayerStatType.DeathByStalagtite), new Vector2(0, -120));
            AddEntry(PlayerStatType.DoorsOpened, mStats.GetValueSafe(PlayerStatType.DoorsOpened), new Vector2(0, -140));
            AddEntry(PlayerStatType.DoorsClosed, mStats.GetValueSafe(PlayerStatType.DoorsClosed), new Vector2(0, -160));
            AddEntry(PlayerStatType.BeesHarvested, mStats.GetValueSafe(PlayerStatType.BeesHarvested), new Vector2(0, -180));
            AddEntry(PlayerStatType.SapHarvested, mStats.GetValueSafe(PlayerStatType.SapHarvested), new Vector2(0, -200));
            AddEntry(PlayerStatType.TurretAmmoAdded, mStats.GetValueSafe(PlayerStatType.TurretAmmoAdded), new Vector2(250, 300));
            AddEntry(PlayerStatType.TurretTrophySet, mStats.GetValueSafe(PlayerStatType.TurretTrophySet), new Vector2(250, 280));
            AddEntry(PlayerStatType.TrapArmed, mStats.GetValueSafe(PlayerStatType.TrapArmed), new Vector2(250, 260));
            AddEntry(PlayerStatType.TrapTriggered, mStats.GetValueSafe(PlayerStatType.TrapTriggered), new Vector2(250, 240));
            AddEntry(PlayerStatType.PlaceStacks, mStats.GetValueSafe(PlayerStatType.PlaceStacks), new Vector2(250, 220));
            AddEntry(PlayerStatType.PortalDungeonIn, mStats.GetValueSafe(PlayerStatType.PortalDungeonIn), new Vector2(250, 200));
            AddEntry(PlayerStatType.PortalDungeonOut, mStats.GetValueSafe(PlayerStatType.PortalDungeonOut), new Vector2(250, 180));
            AddEntry(PlayerStatType.BossKills, mStats.GetValueSafe(PlayerStatType.BossKills), new Vector2(250, 160));
            AddEntry(PlayerStatType.BossLastHits, mStats.GetValueSafe(PlayerStatType.BossLastHits), new Vector2(250, 140));
            AddEntry(PlayerStatType.SetGuardianPower, mStats.GetValueSafe(PlayerStatType.SetGuardianPower), new Vector2(250, 120));
            AddEntry(PlayerStatType.SetPowerEikthyr, mStats.GetValueSafe(PlayerStatType.SetPowerEikthyr), new Vector2(250, 100));
            AddEntry(PlayerStatType.SetPowerElder, mStats.GetValueSafe(PlayerStatType.SetPowerElder), new Vector2(250, 80));
            AddEntry(PlayerStatType.SetPowerBonemass, mStats.GetValueSafe(PlayerStatType.SetPowerBonemass), new Vector2(250, 60));
            AddEntry(PlayerStatType.SetPowerModer, mStats.GetValueSafe(PlayerStatType.SetPowerModer), new Vector2(250, 40));
            AddEntry(PlayerStatType.SetPowerYagluth, mStats.GetValueSafe(PlayerStatType.SetPowerYagluth), new Vector2(250, 20));
            AddEntry(PlayerStatType.SetPowerQueen, mStats.GetValueSafe(PlayerStatType.SetPowerQueen), new Vector2(250, 0));
            AddEntry(PlayerStatType.SetPowerAshlands, mStats.GetValueSafe(PlayerStatType.SetPowerAshlands), new Vector2(250, -20));
            AddEntry(PlayerStatType.SetPowerDeepNorth, mStats.GetValueSafe(PlayerStatType.SetPowerDeepNorth), new Vector2(250, -40));
            AddEntry(PlayerStatType.UseGuardianPower, mStats.GetValueSafe(PlayerStatType.UseGuardianPower), new Vector2(250, -60));
            AddEntry(PlayerStatType.UsePowerEikthyr, mStats.GetValueSafe(PlayerStatType.UsePowerEikthyr), new Vector2(250, -80));
            AddEntry(PlayerStatType.UsePowerElder, mStats.GetValueSafe(PlayerStatType.UsePowerElder), new Vector2(250, -100));
            AddEntry(PlayerStatType.UsePowerBonemass, mStats.GetValueSafe(PlayerStatType.UsePowerBonemass), new Vector2(250, -120));
            AddEntry(PlayerStatType.UsePowerModer, mStats.GetValueSafe(PlayerStatType.UsePowerModer), new Vector2(250, -140));
            AddEntry(PlayerStatType.UsePowerYagluth, mStats.GetValueSafe(PlayerStatType.UsePowerYagluth), new Vector2(250, -160));
            AddEntry(PlayerStatType.UsePowerQueen, mStats.GetValueSafe(PlayerStatType.UsePowerQueen), new Vector2(250, -180));
            AddEntry(PlayerStatType.UsePowerAshlands, mStats.GetValueSafe(PlayerStatType.UsePowerAshlands), new Vector2(250, -200));
            AddEntry(PlayerStatType.UsePowerDeepNorth, mStats.GetValueSafe(PlayerStatType.UsePowerDeepNorth), new Vector2(500, 300));
            AddEntry(PlayerStatType.Count, mStats.GetValueSafe(PlayerStatType.Count), new Vector2(500, 280));
        }

        private static void AddEntry(PlayerStatType type, float value, Vector2 position)
        {
            AddEntry(type, value, position, new Vector2(200, 50)); //Default
        }
        
        private static void AddEntry(PlayerStatType type, float value, Vector2 position, Vector2 sizeDelta)
        {
            Transform contentRoot = 
                InventoryGui.instance.m_skillsDialog.transform.Find(
                    "PlayerStatsFrame/TrophiesFrame/Trophies/TrophyList/ListRoot");
            if (contentRoot == null) return;

            GameObject textObject = new GameObject("Stat_"+type, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(contentRoot, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchoredPosition = position;
            textRect.sizeDelta = sizeDelta;
            TextMeshProUGUI textGUI = textObject.GetComponent<TextMeshProUGUI>();
            textGUI.text = type + ": " + value;
            textGUI.fontSize = 18;
            textGUI.alignment = TextAlignmentOptions.Left;
            textGUI.font = PlayerUtils.getFontAsset("Valheim-AveriaSansLibre");
        }

        public static void reloadTexts()
        {
            buttonText.text = Localization.instance.Localize("$button_ps_start");
            panel.reloadTexts();
        }

        private static void addSoftDeathInfo(Transform parent)
        {
            GameObject textObject = new GameObject("NoSkillDrainText");
            lossPercentageTextComponent = textObject.AddComponent<TextMeshProUGUI>();
            lossPercentageTextComponent.font = PlayerUtils.getFontAsset("Valheim-AveriaSansLibre");
            lossPercentageTextComponent.fontStyle = FontStyles.Normal;
            lossPercentageTextComponent.color = new Color(1, 1, 0);
            lossPercentageTextComponent.fontSize = 18;
            lossPercentageTextComponent.alignment = TextAlignmentOptions.Right;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.SetParent(parent, false);
            textRect.sizeDelta = new Vector2(200, 50);
            textRect.anchoredPosition = new Vector2(-141, 188);
            
            GameObject valueObject = new GameObject("NoSkillDrainValue");
            lossPercentageValueComponent = valueObject.AddComponent<TextMeshProUGUI>();
            lossPercentageValueComponent.font = PlayerUtils.getFontAsset("Valheim-AveriaSansLibre");
            lossPercentageValueComponent.fontStyle = FontStyles.Normal;
            lossPercentageValueComponent.color = new Color(1, 1, 0);
            lossPercentageValueComponent.fontSize = 18;
            lossPercentageValueComponent.alignment = TextAlignmentOptions.Left;

            RectTransform valueRect = valueObject.GetComponent<RectTransform>();
            valueRect.SetParent(parent, false);
            valueRect.sizeDelta = new Vector2(200, 50);
            valueRect.anchoredPosition = new Vector2(65, 188);
            
            updateSkillLossPercentage();
        }

        public static void updateSkillLossPercentage()
        {
            lossPercentageTextComponent.text = $"{ConfigurationFile.deathPenaltyText.Value}";
            float lossPercentage = Player.m_localPlayer.GetSkills().m_DeathLowerFactor * 100f;
            lossPercentageValueComponent.text = $"= {(lossPercentage > 0 ? "-" : "")}{lossPercentage}%";
        }

        public static void updateOptionsTexts()
        {
            customSliderSaveSwitch.sliderLabelDescription.text = ConfigurationFile.reloadAfterDyingText.Value;
            customSliderNumberOfDecimals.sliderLabelDescription.text = ConfigurationFile.numberOfDecimalsText.Value;
            customSliderSkillUpMessage.sliderLabelDescription.text = ConfigurationFile.skillUpMessageText.Value;
            customSliderSkillUpMessage.updateValue(calculateSkillupSliderValue(customSliderSkillUpMessage.getValue()));
            customSliderSkillUpBigMessage.sliderLabelDescription.text = ConfigurationFile.skillUpBigMessageText.Value;
            customSliderSkillUpBigMessage.updateValue(calculateSkillupSliderValue(customSliderSkillUpBigMessage.getValue()));
        }

        private static void addSaveSwitchButton(Transform parent)
        {
            customSliderSaveSwitch = new CustomSlider(
                name: "SkillsLevelSaveSlider",
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(-17, 163),
                posXIcon: -1,
                spriteName: null,
                posXDescription: -124,
                description: ConfigurationFile.reloadAfterDyingText.Value,
                posXValue: 123,
                initValue: ConfigurationFile.saveSkillBuffs.Value ? 1 : 0,
                valueDesc: ConfigurationFile.saveSkillBuffs.Value.ToString()
            );
            customSliderSaveSwitch.getGameObject().transform.SetParent(parent, false);
            customSliderSaveSwitch.OnValueChanged((value) =>
            {
                ConfigurationFile.saveSkillBuffs.Value = value.Equals(1f);
                customSliderSaveSwitch.updateValue(ConfigurationFile.saveSkillBuffs.Value.ToString());
            });
        }

        private static void addNumberOfDecimalsSlider(Transform parent)
        {
            customSliderNumberOfDecimals = new CustomSlider(
                name: "NumberOfDecimalsSlider",
                maxValue: 15,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(45, 135),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: ConfigurationFile.numberOfDecimalsText.Value,
                posXValue: 185,
                initValue: ConfigurationFile.numberOfDecimals.Value,
                valueDesc: ConfigurationFile.numberOfDecimals.Value.ToString()
            );
            customSliderNumberOfDecimals.getGameObject().transform.SetParent(parent, false);
            customSliderNumberOfDecimals.OnValueChanged((value) =>
            {
                Logger.Log("slider changed to " + value);
                customSliderNumberOfDecimals.updateValue(value.ToString());
                ConfigurationFile.numberOfDecimals.Value = (int)value;
            });
        }

        private static void addSkillUpMessage(Transform parent)
        {
            customSliderSkillUpMessage = new CustomSlider(
                name: "SkillUpMessageSlider",
                maxValue: 100,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(45, 105),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: ConfigurationFile.skillUpMessageText.Value,
                posXValue: 185,
                initValue: ConfigurationFile.skillUpMessageAfterMultipleLevel.Value,
                valueDesc: calculateSkillupSliderValue(ConfigurationFile.skillUpMessageAfterMultipleLevel.Value)
            );
            customSliderSkillUpMessage.getGameObject().transform.SetParent(parent, false);
            customSliderSkillUpMessage.OnValueChanged((value) =>
            {
                Logger.Log("message slider changed to " + value);
                customSliderSkillUpMessage.updateValue(calculateSkillupSliderValue(value));
                ConfigurationFile.skillUpMessageAfterMultipleLevel.Value = (int)value;
            });
        }

        private static void addSkillUpBigMessage(Transform parent)
        {
            customSliderSkillUpBigMessage = new CustomSlider(
                name: "SkillUpBigMessageSlider",
                maxValue: 100,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(45, 75),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: ConfigurationFile.skillUpBigMessageText.Value,
                posXValue: 185,
                initValue: ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value,
                valueDesc: calculateSkillupSliderValue(ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value)
            );
            customSliderSkillUpBigMessage.getGameObject().transform.SetParent(parent, false);
            customSliderSkillUpBigMessage.OnValueChanged((value) =>
            {
                Logger.Log("bigMessage slider changed to " + value);
                customSliderSkillUpBigMessage.updateValue(calculateSkillupSliderValue(value));
                ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value = (int)value;
            });
        }

        private static string calculateSkillupSliderValue(float value)
        {
            return value.Equals(0)
                ? Localization.instance.Localize("$menu_none")
                : ConfigurationFile.skillUpValueText.Value.Replace("{0}", value.ToString());
        }
    }

    [HarmonyPatch(typeof(Player), "OnDeath")]
    public class Player_OnDeath_Patch
    {
        static void Postfix(Player __instance)
        {
            //Reset skills background in skillDialog
            var field = typeof(SkillsDialog).GetField("m_elements", BindingFlags.NonPublic | BindingFlags.Instance);
            List<GameObject> skillRows = (List<GameObject>)field.GetValue(InventoryGui.instance.m_skillsDialog);
            foreach (GameObject skillRow in skillRows)
            {
                PlayerUtils.setSkillRowBackgroundColor(skillRow, new Color(0f, 0f, 0f, 0f));
            }

            //Clear stored buffs
            if (!ConfigurationFile.saveSkillBuffs.Value)
                PlayerUtils.skillStatusEffects.Clear();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    public class Player_OnSpawned_Patch
    {
        static void Postfix(Player __instance, bool spawnValkyrie)
        {
            //1 - Update skill loss from config at loading
            __instance.GetSkills().m_DeathLowerFactor = ConfigurationFile.deathSkillLoss.Value / 100f;
            
            //2 - Add selected buffs before dying
            if (ConfigurationFile.saveSkillBuffs.Value)
            {
                //Workaround to manipulate dictionary while entries can be removed or added dynamically in AddSKillBuff
                var list = new List<SkillType>();
                foreach (SkillType skillType in PlayerUtils.skillStatusEffects.Keys)
                {
                    list.Add(skillType);
                }
                foreach (SkillType skillType in list)
                {
                    PlayerBuffs.AddSkillBuff(
                        __instance,
                        PlayerBuffs.skills.GetValueSafe(skillType),
                        PlayerBuffs.sprites.GetValueSafe(skillType)
                    );
                }
            }
        }
    }
}
