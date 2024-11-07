using DetailedLevels.Config;
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
        private static readonly Color TITLE_COLOR = new Color(1f, 0.7176f, 0.3603f, 1f);

        static void Postfix(SkillsDialog __instance)
        {
            Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame").transform.Find("Closebutton");
            (closeButtonTransform as RectTransform).anchoredPosition = new Vector2(101, 45);
            Button closeButton = closeButtonTransform.GetComponent<Button>();

            GameObject dlOptionsButtonObject = GameObject.Instantiate(closeButton.gameObject, closeButton.transform.parent);
            dlOptionsButtonObject.name = "DLOptionsButton";

            RectTransform dlOptionsButtonRect = dlOptionsButtonObject.GetComponent<RectTransform>();
            dlOptionsButtonRect.anchoredPosition = new Vector2(-100, 45);

            TextMeshProUGUI buttonText = dlOptionsButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = $"{"$button_ps_start"}";
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

            /*// Separador ────────────────────── 
            TextMeshProUGUI separatorText = panel.getPanel().AddComponent<TextMeshProUGUI>();
            separatorText.text = $" ────────────────────── {"$button_ps_start"} ────────────────────── ";
            separatorText.fontStyle = FontStyles.Normal;
            separatorText.color = TITLE_COLOR;
            separatorText.alignment = TextAlignmentOptions.Center;*/

            addNumberOfDecimalsSlider(panel.getPanel().transform);
            addSkillUpMessage(panel.getPanel().transform);
            addSkillUpBigMessage(panel.getPanel().transform);
        }

        private static void addSoftDeathInfo(Transform parent)
        {
            //Icon
            GameObject iconObject = new GameObject("NoSkillDrainIcon");
            Image iconImage = iconObject.AddComponent<Image>();
            iconImage.sprite = PlayerUtils.getSprite("SoftDeath");

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            iconRect.SetParent(parent, false);
            iconRect.sizeDelta = new Vector2(25, 25);
            iconRect.anchoredPosition = new Vector2(-77, 190);

            //Additional text
            float lossPercentage = Player.m_localPlayer.GetSkills().m_DeathLowerFactor * 100f;
            GameObject textObject = new GameObject("NoSkillDrainText");
            TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
            textComponent.text = $"Death penalty         = -{lossPercentage}%";
            //textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontStyle = FontStyles.Normal;
            textComponent.color = Color.white;
            textComponent.fontSize = 18;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.SetParent(parent, false);
            textRect.sizeDelta = new Vector2(200, 50);
            textRect.anchoredPosition = new Vector2(-107, 173);
        }

        private static void addSaveSwitchButton(Transform parent)
        {
            CustomSlider customSlider = new CustomSlider(
                name: "SkillsLevelSaveSlider",
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(-42, 163),
                posXIcon: -35,
                spriteName: "save_icon",
                posXDescription: -150,
                description: "Reload after dying",
                posXValue: 123,
                initValue: ConfigurationFile.saveSkillBuffs.Value ? 1 : 0,
                valueDesc: ConfigurationFile.saveSkillBuffs.Value.ToString()
            );
            customSlider.getGameObject().transform.SetParent(parent, false);
            customSlider.OnValueChanged((value) =>
            {
                ConfigurationFile.saveSkillBuffs.Value = value == 1;
                customSlider.updateValue(ConfigurationFile.saveSkillBuffs.Value.ToString());
            });
        }

        private static void addNumberOfDecimalsSlider(Transform parent)
        {
            CustomSlider customSlider = new CustomSlider(
                name: "NumberOfDecimalsSlider",
                maxValue: 15,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(20, 135),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Number of decimals",
                posXValue: 185,
                initValue: ConfigurationFile.numberOfDecimals.Value,
                valueDesc: ConfigurationFile.numberOfDecimals.Value.ToString()
            );
            customSlider.getGameObject().transform.SetParent(parent, false);
            customSlider.OnValueChanged((value) =>
            {
                Logger.Log("slider changed to " + value);
                customSlider.updateValue(value.ToString());
                ConfigurationFile.numberOfDecimals.Value = (int)value;
            });
        }

        private static void addSkillUpMessage(Transform parent)
        {
            CustomSlider customSlider = new CustomSlider(
                name: "SkillUpMessageSlider",
                maxValue: 100,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(20, 105),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Skill up message",
                posXValue: 185,
                initValue: ConfigurationFile.skillUpMessageAfterMultipleLevel.Value,
                valueDesc: $"Each {ConfigurationFile.skillUpMessageAfterMultipleLevel.Value} levels"
            );
            customSlider.getGameObject().transform.SetParent(parent, false);
            customSlider.OnValueChanged((value) =>
            {
                Logger.Log("slider changed to " + value);
                customSlider.updateValue($"Each {value} levels");
                ConfigurationFile.skillUpMessageAfterMultipleLevel.Value = (int)value;
            });
        }

        private static void addSkillUpBigMessage(Transform parent)
        {
            CustomSlider customSlider = new CustomSlider(
                name: "SkillUpBigMessageSlider",
                maxValue: 100,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(20, 75),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Skill up big message",
                posXValue: 185,
                initValue: ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value,
                valueDesc: $"Each {ConfigurationFile.skillUpBigMessageAfterMultipleLevel.Value} levels"
            );
            customSlider.getGameObject().transform.SetParent(parent, false);
            customSlider.OnValueChanged((value) =>
            {
                Logger.Log("slider changed to " + value);
                customSlider.updateValue($"Each {value} levels");
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
            Player player = __instance.GetComponent<Player>();
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
