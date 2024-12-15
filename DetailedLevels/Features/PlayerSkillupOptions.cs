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
            Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton");
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
            customSliderSkillUpMessage.updateValue(ConfigurationFile.skillUpValueText.Value.Replace("{0}", customSliderSkillUpMessage.getValue().ToString()));
            customSliderSkillUpBigMessage.sliderLabelDescription.text = ConfigurationFile.skillUpBigMessageText.Value;
            customSliderSkillUpBigMessage.updateValue(ConfigurationFile.skillUpValueText.Value.Replace("{0}", customSliderSkillUpBigMessage.getValue().ToString()));
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
                ConfigurationFile.saveSkillBuffs.Value = value == 1f;
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
                valueDesc: ConfigurationFile.skillUpValueText.Value.Replace("{0}", ConfigurationFile.skillUpMessageAfterMultipleLevel.Value.ToString())
            );
            customSliderSkillUpMessage.getGameObject().transform.SetParent(parent, false);
            customSliderSkillUpMessage.OnValueChanged((value) =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSkillUpMessage.updateValue(ConfigurationFile.skillUpValueText.Value.Replace("{0}", value.ToString()));
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
                valueDesc: ConfigurationFile.skillUpValueText.Value.Replace("{0}", ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value.ToString())
            );
            customSliderSkillUpBigMessage.getGameObject().transform.SetParent(parent, false);
            customSliderSkillUpBigMessage.OnValueChanged((value) =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSkillUpBigMessage.updateValue(ConfigurationFile.skillUpValueText.Value.Replace("{0}", value.ToString()));
                ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value = (int)value;
            });
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
            Player.m_localPlayer.GetSkills().m_DeathLowerFactor = ConfigurationFile.deathSkillLoss.Value / 100f;
            
            //2 - Add selected buffs before dying
            if (ConfigurationFile.saveSkillBuffs.Value)
            {
                //Workaround to manipulate dictionary while entries can be removed or added dinamically in AddSKillBuff
                var list = new List<SkillType>();
                foreach (SkillType skillType in PlayerUtils.skillStatusEffects.Keys)
                {
                    list.Add(skillType);
                }
                foreach (SkillType skillType in list)
                {
                    PlayerBuffs.AddSkillBuff(
                        Player.m_localPlayer,
                        PlayerBuffs.skills.GetValueSafe(skillType),
                        PlayerBuffs.sprites.GetValueSafe(skillType)
                    );
                }
            }
        }
    }
}
