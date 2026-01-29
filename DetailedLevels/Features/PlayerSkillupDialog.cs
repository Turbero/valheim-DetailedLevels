using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Skills;
using static Utils;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(Game), "Start")]
    public class GameStartPatch {
        private static void Postfix() {
            //Initialize order
            Logger.Log($"Sort preparation: {ConfigurationFile.saveSkillsOrder.Value} {ConfigurationFile.saveSkillsOrderValue.Value}");
            if (ConfigurationFile.saveSkillsOrder.Value)
            {
                if (ConfigurationFile.saveSkillsOrderValue.Value == SkillsSortOrder.AlphabeticalAscending)
                {
                    SkillsDialogAdditions_Patch.azSorted = 1;
                    SkillsDialogAdditions_Patch.levelSorted = 0;
                }
                else if (ConfigurationFile.saveSkillsOrderValue.Value == SkillsSortOrder.AlphabeticalDescending)
                {
                    SkillsDialogAdditions_Patch.azSorted = 2;
                    SkillsDialogAdditions_Patch.levelSorted = 0;
                }
                else if (ConfigurationFile.saveSkillsOrderValue.Value == SkillsSortOrder.LevelAscending)
                {
                    SkillsDialogAdditions_Patch.azSorted = 0;
                    SkillsDialogAdditions_Patch.levelSorted = 1;
                }
                else if (ConfigurationFile.saveSkillsOrderValue.Value == SkillsSortOrder.LevelDescending)
                {
                    SkillsDialogAdditions_Patch.azSorted = 0;
                    SkillsDialogAdditions_Patch.levelSorted = 2;
                }
                Logger.Log($"Sort selected: azSorted {SkillsDialogAdditions_Patch.azSorted}, levelSorted {SkillsDialogAdditions_Patch.levelSorted}");
            }
        }
    }
    
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    [HarmonyPriority(Priority.VeryLow)]
    public class SkillsDialogAdditions_Patch
    {
        public static int azSorted = 0;
        public static int levelSorted = 0;
        private static GameObject azButtonObject;
        private static GameObject levelButtonObject;

        static void Postfix(SkillsDialog __instance, ref Player player, ref List<GameObject> ___m_elements)
        {
            Logger.Log($"Sort selected: azSorted {azSorted}, levelSorted {levelSorted}");
            
            List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
            for (int j = 0; j < skillList.Count; j++)
            {
                GameObject obj = ___m_elements[j];
                Skills.Skill skill = skillList[j];

                float level = PlayerUtils.GetCurrentSkillLevelProgress(skill);
                float skillLevelModifier = PlayerUtils.FindActiveModifierValue(player, skill.m_info.m_skill);
                string levelTextWithoutModifier = PlayerUtils.GetSkillValueToShow(level, 0);

                Utils.FindChild(obj.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text = levelTextWithoutModifier;

                //Refresh background to adjust after sorting list
                string skillName = skill.m_info.m_skill.ToString();
                int nameHash = PlayerUtils.skillStatusEffects.GetValueSafe(skill.m_info.m_skill);
                StatusEffect existingBuff = player.GetSEMan().GetStatusEffect(nameHash);
                if (existingBuff != null)
                {
                    PlayerUtils.setSkillRowBackgroundColor(obj, ConfigurationFile.colorSkillBackground.Value);
                    string levelText = PlayerUtils.GetSkillValueToShow(level, skillLevelModifier);
                    existingBuff.m_name = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}: {levelText}"; //Refresh value in buff section
                }
                else
                {
                    PlayerUtils.setSkillRowBackgroundColor(obj, new Color(0f, 0f, 0f, 0f));
                }
            }

            Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton");
            Button closeButton = closeButtonTransform.GetComponent<Button>();
            if (azButtonObject == null)
                azButton(closeButton);
            if (levelButtonObject == null)
                levelButton(closeButton);
        }

        private static void azButton(Button baseButton)
        {
            azButtonObject = GameObject.Instantiate(baseButton.gameObject, baseButton.transform.parent);
            azButtonObject.name = "AZButton";

            RectTransform azButtonRect = azButtonObject.GetComponent<RectTransform>();
            
            azButtonRect.anchoredPosition = new Vector2(-133, 633);
            azButtonRect.sizeDelta = new Vector2(50, 35);

            TMP_Text buttonText = azButtonObject.GetComponentInChildren<TMP_Text>();
            buttonText.text = azSorted != 1 ? "A-Z" : "Z-A";

            Button azButton = azButtonObject.GetComponent<Button>(); 
            azButton.onClick = new Button.ButtonClickedEvent();
            azButton.onClick.AddListener(() =>
            {
                Logger.Log("AZButton clicked.");
                levelSorted = 0;
                if (azSorted != 1)
                {
                    azSorted = 1;
                    buttonText.text = "Z-A";
                    if (ConfigurationFile.saveSkillsOrder.Value)
                        ConfigurationFile.saveSkillsOrderValue.Value = SkillsSortOrder.AlphabeticalAscending;
                }
                else
                {
                    azSorted = 2;
                    buttonText.text = "A-Z";
                    if (ConfigurationFile.saveSkillsOrder.Value)
                        ConfigurationFile.saveSkillsOrderValue.Value = SkillsSortOrder.AlphabeticalDescending;
                }
                InventoryGui.instance.m_skillsDialog.Setup(Player.m_localPlayer);
            });
        }

        private static void levelButton(Button baseButton)
        {
            levelButtonObject = GameObject.Instantiate(baseButton.gameObject, baseButton.transform.parent);
            levelButtonObject.name = "LevelButton";

            RectTransform levelButtonRect = levelButtonObject.GetComponent<RectTransform>();

            levelButtonRect.anchoredPosition = new Vector2(133, 633);
            levelButtonRect.sizeDelta = new Vector2(50, 35);

            TMP_Text buttonText = levelButtonObject.GetComponentInChildren<TMP_Text>();
            buttonText.text = levelSorted != 1 ? "1-100" : "100-1";

            Button levelButton = levelButtonObject.GetComponent<Button>();
            levelButton.onClick = new Button.ButtonClickedEvent();
            levelButton.onClick.AddListener(() =>
            {
                Logger.Log("LevelButton clicked.");
                azSorted = 0;
                if (levelSorted != 1)
                {
                    levelSorted = 1;
                    buttonText.text = "100-1";
                    if (ConfigurationFile.saveSkillsOrder.Value)
                        ConfigurationFile.saveSkillsOrderValue.Value = SkillsSortOrder.LevelAscending;
                }
                else
                {
                    levelSorted = 2;
                    buttonText.text = "1-100";
                    if (ConfigurationFile.saveSkillsOrder.Value)
                        ConfigurationFile.saveSkillsOrderValue.Value = SkillsSortOrder.LevelDescending;
                }
                InventoryGui.instance.m_skillsDialog.Setup(Player.m_localPlayer);
            });
        }
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Show))]
    public class InventoryGui_Show_Patch
    {
        static void Postfix(InventoryGui __instance)
        {
            if (__instance.m_player != null)
            {
                var transform = __instance.transform.Find("root/Info/Skills");
                if (transform != null)
                {
                    //Add hotKey to tooptip
                    UITooltip buttonTooltip = transform.GetComponent<UITooltip>();
                    if (buttonTooltip != null)
                    {
                        string originalTooltip = "$inventory_skills";
                        string customText = $" ({ConfigurationFile.hotKey.Value})";

                        buttonTooltip.m_text = originalTooltip + customText;
                    }

                    //Reload texts in skill options
                    if (PlayerSkillupOptionsPatch.panel != null)
                    {
                        PlayerSkillupOptionsPatch.reloadTexts();
                        //Hide skill options if open
                        PlayerSkillupOptionsPatch.panel.getPanel()?.gameObject?.SetActive(false);
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(Skills), nameof(Skills.GetSkillList))]
    public class SkillsDialog_GetSkillListSorted_Patch
    {
        static void Postfix(ref List<Skill> __result)
        {
            if (SkillsDialogAdditions_Patch.azSorted == 1)
            {
                Logger.Log("SkillsDialog_SkillStatusEffects_Patch.Postfix azSorted = 1");
                __result.Sort((a, b) => GetSkillTranslation(a).CompareTo(GetSkillTranslation(b)));
            }
            else if (SkillsDialogAdditions_Patch.azSorted == 2)
            {
                Logger.Log("SkillsDialog_SkillStatusEffects_Patch.Postfix azSorted = 2");
                __result.Sort((a, b) => GetSkillTranslation(b).CompareTo(GetSkillTranslation(a)));
            }
            else if (SkillsDialogAdditions_Patch.levelSorted == 1)
            {
                Logger.Log("SkillsDialog_SkillStatusEffects_Patch.Postfix levelSorted = 1");
                __result.Sort((a, b) => PlayerUtils.GetCurrentSkillLevelProgress(a).CompareTo(PlayerUtils.GetCurrentSkillLevelProgress(b)));
            }
            else if (SkillsDialogAdditions_Patch.levelSorted == 2)
            {
                Logger.Log("SkillsDialog_SkillStatusEffects_Patch.Postfix levelSorted = 2");
                __result.Sort((a, b) => PlayerUtils.GetCurrentSkillLevelProgress(b).CompareTo(PlayerUtils.GetCurrentSkillLevelProgress(a)));
            }
        }

        private static string GetSkillTranslation(Skill skill)
        {
            return Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower());
        }
    }
}
