using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Skills;
using static Utils;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    [HarmonyPriority(Priority.VeryLow)]
    public class SkillsDialogAdditions_Patch
    {
        public static int azSorted = 0;
        public static int levelSorted = 0;
        private static bool init = false;

        static void Postfix(SkillsDialog __instance, ref Player player, ref List<GameObject> ___m_elements)
        {
            List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
            for (int j = 0; j < skillList.Count; j++)
            {
                GameObject obj = ___m_elements[j];
                Skills.Skill skill = skillList[j];

                string levelText = PlayerUtils.GetCurrentSkillLevelProgress(skill).ToString();

                Utils.FindChild(obj.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text = levelText;

                //Refresh background to adjust after sorting list
                string skillName = skill.m_info.m_skill.ToString();
                int nameHash = PlayerUtils.skillStatusEffects.GetValueSafe(skill.m_info.m_skill);
                StatusEffect existingBuff = player.GetSEMan().GetStatusEffect(nameHash);
                if (existingBuff != null)
                {
                    PlayerUtils.setSkillRowBackgroundColor(obj, ConfigurationFile.colorSkillBackground.Value);
                    existingBuff.m_name = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}: {levelText}"; //Refresh value in buff section
                }
                else
                {
                    PlayerUtils.setSkillRowBackgroundColor(obj, new Color(0f, 0f, 0f, 0f));
                }
            }

            if (!init)
            {
                Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame").transform.Find("Closebutton");
                Button closeButton = closeButtonTransform.GetComponent<Button>();
                azButton(closeButton);
                levelButton(closeButton);
                init = true;
            }
        }

        private static void azButton(Button baseButton)
        {
            GameObject azButtonObject = GameObject.Instantiate(baseButton.gameObject, baseButton.transform.parent);
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
                }
                else
                {
                    azSorted = 2;
                    buttonText.text = "A-Z";
                }
                InventoryGui.instance.m_skillsDialog.Setup(Player.m_localPlayer);
            });
        }

        private static void levelButton(Button baseButton)
        {
            GameObject levelButtonObject = GameObject.Instantiate(baseButton.gameObject, baseButton.transform.parent);
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
                }
                else
                {
                    levelSorted = 2;
                    buttonText.text = "1-100";
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
                var transform = __instance
                    .transform.Find("root")
                    .transform.Find("Info")
                    .transform.Find("Skills");
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
                        PlayerSkillupOptionsPatch.panel.getPanel().gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(Skills), nameof(Skills.GetSkillList))]
    public static class SkillsDialog_GetSkillListSorted_Patch
    {
        static void Postfix(ref List<Skill> __result)
        {
            if (SkillsDialogAdditions_Patch.azSorted == 1)
            {
                Logger.Log("SkillsDialog_SkillStatusEffects_Patch.Postfix aZsorted = 1");
                __result.Sort((a, b) => GetSkillTranslation(a).CompareTo(GetSkillTranslation(b)));
            }
            else if (SkillsDialogAdditions_Patch.azSorted == 2)
            {
                Logger.Log("SkillsDialog_SkillStatusEffects_Patch.Postfix aZsorted = 2");
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

        private static string GetSkillTranslation(this Skill skill)
        {
            return Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower());
        }
    }
}
