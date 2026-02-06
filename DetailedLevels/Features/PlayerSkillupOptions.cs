using System;
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
        private static CustomSkillOptionsPanel panel;
        private static readonly Color TITLE_COLOR = new Color(1f, 0.7176f, 0.3603f);
        private static TextMeshProUGUI buttonOptionsText;
        private static TextMeshProUGUI buttonStatsText;
        private static TextMeshProUGUI lossPercentageTextComponent;
        private static TextMeshProUGUI lossPercentageValueComponent;
        private static CustomSlider customSliderSaveSwitch;
        private static CustomSlider customSliderNumberOfDecimals;
        private static CustomSlider customSliderSkillValuesFormat;
        private static CustomSlider customSliderSkillUpMessage;
        private static CustomSlider customSliderSkillUpBigMessage;
        private static CustomSlider customSliderSaveSkillsOrder;
        private static CustomStatsPanel statsPanel;
        private static CustomStatsPanelKills statsPanelKills;
        private static GameObject tabStatsButtonObject;
        private static GameObject tabKillStatsButtonObject;

        static void Postfix(SkillsDialog __instance)
        {
            Transform closeButtonTransform = __instance.transform.Find("SkillsFrame/Closebutton");
            (closeButtonTransform as RectTransform).anchoredPosition = new Vector2(146, 45);
            (closeButtonTransform as RectTransform).sizeDelta = new Vector2(140, 46);
            Button closeButton = closeButtonTransform.GetComponent<Button>();

            // New Options button
            GameObject dlOptionsButtonObject =
                GameObject.Instantiate(closeButton.gameObject, closeButton.transform.parent);
            dlOptionsButtonObject.name = "DLOptionsButton";

            RectTransform dlOptionsButtonRect = dlOptionsButtonObject.GetComponent<RectTransform>();
            dlOptionsButtonRect.anchoredPosition = new Vector2(-145, 45);
            dlOptionsButtonRect.sizeDelta = new Vector2(140, 46);

            buttonOptionsText = dlOptionsButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonOptionsText.fontStyle = FontStyles.Normal;
            buttonOptionsText.color = TITLE_COLOR;
            buttonOptionsText.alignment = TextAlignmentOptions.Center;

            // New Stats button
            GameObject dlStatsButtonObject =
                GameObject.Instantiate(closeButton.gameObject, closeButton.transform.parent);
            dlStatsButtonObject.name = "DLStatsButton";

            RectTransform dlStatsButtonRect = dlStatsButtonObject.GetComponent<RectTransform>();
            dlStatsButtonRect.anchoredPosition = new Vector2(0, 45);
            dlStatsButtonRect.sizeDelta = new Vector2(140, 46);
            Button dlStatsButton = dlStatsButtonObject.GetComponent<Button>();
            dlStatsButton.onClick = new Button.ButtonClickedEvent();
            dlStatsButton.onClick.AddListener(() =>
            {
                statsPanel.reloadTexts();
                statsPanel.getPanel().SetActive(true);
                statsPanelKills.reloadTexts();
                statsPanelKills.getPanel().SetActive(false);
                tabStatsButtonObject.SetActive(true);
                tabStatsButtonObject.GetComponent<Button>().interactable = false;
                tabKillStatsButtonObject.SetActive(true);
                tabKillStatsButtonObject.GetComponent<Button>().interactable = true;
            });

            buttonStatsText = dlStatsButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonStatsText.fontStyle = FontStyles.Normal;
            buttonStatsText.color = TITLE_COLOR;
            buttonStatsText.alignment = TextAlignmentOptions.Center;

            // Custom panels
            panel = new CustomSkillOptionsPanel(closeButton.transform.parent);
            statsPanel = new CustomStatsPanel();
            statsPanelKills = new CustomStatsPanelKills();

            Button dlOptionsButton = dlOptionsButtonObject.GetComponent<Button>();
            dlOptionsButton.onClick = new Button.ButtonClickedEvent();
            dlOptionsButton.onClick.AddListener(() =>
            {
                panel.getPanel().SetActive(true);
            });

            initStatsTabButtons(__instance, statsPanel, statsPanelKills);
            addSoftDeathInfo(panel.getPanel().transform);
            addSaveSwitchButton(panel.getPanel().transform);
            addNumberOfDecimalsSlider(panel.getPanel().transform);
            addSkillValueFormatSlider(panel.getPanel().transform);
            addSkillUpMessage(panel.getPanel().transform);
            addSkillUpBigMessage(panel.getPanel().transform);
            addSkillsOrderSlider(panel.getPanel().transform);

            reloadTexts();
        }

        private static void initStatsTabButtons(SkillsDialog skillsDialog, CustomStatsPanel customStatsPanel, CustomStatsPanelKills customStatsPanelKills)
        {
            Transform closeButtonTransform = skillsDialog.transform.Find("SkillsFrame/Closebutton");
            
            //Tab Stats button
            tabStatsButtonObject = GameObject.Instantiate(closeButtonTransform.gameObject, skillsDialog.transform);
            tabStatsButtonObject.name = "TabStatsButton";
            RectTransform tabStatsButtonRect = tabStatsButtonObject.GetComponent<RectTransform>();
            tabStatsButtonRect.anchoredPosition = new Vector2(-550, 843);
            tabStatsButtonRect.sizeDelta = new Vector2(140, 46);
            Button tabStatsButton = tabStatsButtonObject.GetComponent<Button>();
            tabStatsButton.onClick = new Button.ButtonClickedEvent();
            tabStatsButton.interactable = false;
            TextMeshProUGUI buttonStatsText = tabStatsButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonStatsText.fontStyle = FontStyles.Normal;
            buttonStatsText.color = TITLE_COLOR;
            buttonStatsText.alignment = TextAlignmentOptions.Center;
            buttonStatsText.text = "Main Stats";
            tabStatsButtonObject.SetActive(false);
            
            //Tab Kill stats button
            tabKillStatsButtonObject = GameObject.Instantiate(closeButtonTransform.gameObject, skillsDialog.transform);
            tabKillStatsButtonObject.name = "TabKillStatsButton";
            RectTransform tabKillStatsButtonRect = tabKillStatsButtonObject.GetComponent<RectTransform>();
            tabKillStatsButtonRect.anchoredPosition = new Vector2(-400, 843);
            tabKillStatsButtonRect.sizeDelta = new Vector2(140, 46);
            Button tabKillStatsButton = tabKillStatsButtonObject.GetComponent<Button>();
            tabKillStatsButton.onClick = new Button.ButtonClickedEvent();
            tabKillStatsButton.interactable = true;
            TextMeshProUGUI buttonKillStatsText = tabKillStatsButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonKillStatsText.fontStyle = FontStyles.Normal;
            buttonKillStatsText.color = TITLE_COLOR;
            buttonKillStatsText.alignment = TextAlignmentOptions.Center;
            buttonKillStatsText.text = "Kill Stats";
            tabKillStatsButtonObject.SetActive(false);
            
            tabStatsButton.onClick.AddListener(() =>
            {
                tabStatsButton.interactable = false;
                tabKillStatsButton.interactable = true;
                statsPanel.getPanel().SetActive(true);
                statsPanelKills.getPanel().SetActive(false);
                customStatsPanel.getPanel().SetActive(true); 
                customStatsPanelKills.getPanel().SetActive(false);
                
            });
            tabKillStatsButton.onClick.AddListener(() =>
            {
                tabStatsButton.interactable = true;
                tabKillStatsButton.interactable = false;
                statsPanel.getPanel().SetActive(false);
                statsPanelKills.getPanel().SetActive(true);
                customStatsPanel.getPanel().SetActive(false); 
                customStatsPanelKills.getPanel().SetActive(true);
            });
        }

        public static void reloadTexts()
        {
            buttonOptionsText.text = Localization.instance.Localize("$button_ps_start");
            buttonStatsText.text = ConfigurationFile.statsText.Value;
            panel.reloadTexts();
            statsPanel.reloadTexts();
            statsPanelKills.reloadTexts();
            updateSkillLossPercentage();
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

        private static void updateSkillLossPercentage()
        {
            if (lossPercentageTextComponent != null)
            {
                lossPercentageTextComponent.text = $"{ConfigurationFile.deathPenaltyText.Value}";
                var lossPercentage = Math.Round(Player.m_localPlayer.GetSkills().m_DeathLowerFactor * Game.m_skillReductionRate * 100f, 2);
                lossPercentageValueComponent.text = $"= {(lossPercentage > 0 ? "-" : "")}{lossPercentage}%";
            }
        }

        public static void updateOptionsTexts()
        {
            //UI not shown/loaded yet
            if (customSliderSaveSwitch == null) return;
            
            customSliderSaveSwitch.sliderLabelDescription.text = ConfigurationFile.reloadAfterDyingText.Value;
            //TODO customSliderSaveSwitch.updateValue(ConfigurationFile.saveSkillBuffs.Value ? 0 : 1);
            
            customSliderNumberOfDecimals.sliderLabelDescription.text = ConfigurationFile.numberOfDecimalsText.Value;
            customSliderNumberOfDecimals.updateValue(ConfigurationFile.numberOfDecimals.Value);
            
            customSliderSkillValuesFormat.sliderLabelDescription.text = ConfigurationFile.skillValuesFormatText.Value;
            customSliderSkillValuesFormat.updateValue(ConfigurationFile.skillValuesFormat.Value == SkillValuesFormat.Decimals ? 0 : 1);
            
            customSliderSkillUpMessage.sliderLabelDescription.text = ConfigurationFile.skillUpMessageText.Value;
            customSliderSkillUpMessage.updateValue(ConfigurationFile.skillUpMessageAfterMultipleLevel.Value);
            
            customSliderSkillUpBigMessage.sliderLabelDescription.text = ConfigurationFile.skillUpBigMessageText.Value;
            customSliderSkillUpBigMessage.updateValue(ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value);
            
            customSliderSaveSkillsOrder.sliderLabelDescription.text = ConfigurationFile.skillsOrderText.Value;
            //TODO customSliderSaveSkillsOrder.updateValue(ConfigurationFile.saveSkillsOrder.Value ? 0 : 1);
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
            customSliderSaveSwitch.OnValueChanged(value =>
            {
                customSliderSaveSwitch.updateTextValue(value.Equals(1f).ToString());
                ConfigurationFile.saveSkillBuffs.Value = value.Equals(1f);
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
            customSliderNumberOfDecimals.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderNumberOfDecimals.updateTextValue(value.ToString());
                ConfigurationFile.numberOfDecimals.Value = (int)value;
            });
        }
        
        private static void addSkillValueFormatSlider(Transform parent)
        {
            customSliderSkillValuesFormat = new CustomSlider(
                name: "SkillValuesFormatSlider",
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(-17, 105),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -124,
                description: ConfigurationFile.skillValuesFormatText.Value,
                posXValue: 123,
                initValue: (int)ConfigurationFile.skillValuesFormat.Value,
                valueDesc: GetSkillValuesFormatRepresentation(ConfigurationFile.skillValuesFormat.Value)
            );
            customSliderSkillValuesFormat.getGameObject().transform.SetParent(parent, false);
            customSliderSkillValuesFormat.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                SkillValuesFormat format = value == 0 ? SkillValuesFormat.Decimals : SkillValuesFormat.Percentage;
                customSliderSkillValuesFormat.updateTextValue(GetSkillValuesFormatRepresentation(format));
                ConfigurationFile.skillValuesFormat.Value = format;
            });
        }

        private static string GetSkillValuesFormatRepresentation(SkillValuesFormat skillValuesFormat)
        {
            if (skillValuesFormat == SkillValuesFormat.Decimals)
                return "X,YY";
            else
                return "X (YY%)";
        }

        private static void addSkillUpMessage(Transform parent)
        {
            customSliderSkillUpMessage = new CustomSlider(
                name: "SkillUpMessageSlider",
                maxValue: 100,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(45, 75),
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
                customSliderSkillUpMessage.updateTextValue(calculateSkillupSliderValue(value));
                ConfigurationFile.skillUpMessageAfterMultipleLevel.Value = (int)value;
            });
        }

        private static void addSkillUpBigMessage(Transform parent)
        {
            customSliderSkillUpBigMessage = new CustomSlider(
                name: "SkillUpBigMessageSlider",
                maxValue: 100,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(45, 45),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: ConfigurationFile.skillUpBigMessageText.Value,
                posXValue: 185,
                initValue: ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value,
                valueDesc: calculateSkillupSliderValue(ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value)
            );
            customSliderSkillUpBigMessage.getGameObject().transform.SetParent(parent, false);
            customSliderSkillUpBigMessage.OnValueChanged(value =>
            {
                Logger.Log("bigMessage slider changed to " + value);
                customSliderSkillUpBigMessage.updateTextValue(calculateSkillupSliderValue(value));
                ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value = (int)value;
            });
        }
        
        private static void addSkillsOrderSlider(Transform parent)
        {
            customSliderSaveSkillsOrder = new CustomSlider(
                name: "SaveSkillsOrderSlider",
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(-14, 15),
                posXIcon: -1,
                spriteName: null,
                posXDescription: -124,
                description: ConfigurationFile.skillsOrderText.Value,
                posXValue: 123,
                initValue: ConfigurationFile.saveSkillsOrder.Value ? 1 : 0,
                valueDesc: ConfigurationFile.saveSkillsOrder.Value.ToString()
            );
            customSliderSaveSkillsOrder.getGameObject().transform.SetParent(parent, false);
            customSliderSaveSkillsOrder.OnValueChanged(value =>
            {
                customSliderSaveSkillsOrder.updateTextValue(value.Equals(1f).ToString());
                ConfigurationFile.saveSkillsOrder.Value = value.Equals(1f);
            });
        }

        private static string calculateSkillupSliderValue(float value)
        {
            return value.Equals(0)
                ? Localization.instance.Localize("$menu_none")
                : ConfigurationFile.skillUpValueText.Value.Replace("{0}", value.ToString());
        }

        public static void HideTabButtons()
        {
            tabStatsButtonObject.SetActive(false);
            tabKillStatsButtonObject.SetActive(false);
        }

        public static void InventoryShow()
        {
            if (panel != null)
            {
                reloadTexts();
                panel.getPanel()?.gameObject?.SetActive(false);
                statsPanel.getPanel()?.gameObject?.SetActive(false);
                statsPanelKills.getPanel()?.gameObject?.SetActive(false);
                tabStatsButtonObject.SetActive(false);
                tabKillStatsButtonObject.SetActive(false);
            }
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
            //Add selected buffs before dying
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
