using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DetailedLevels.Features;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DetailedLevels.Tools
{
    public class CustomStatsPanelKills
    {
        private static readonly List<string> MONSTERS_EXCEPTIONS = new List<string>{"TrainingDummy", "Root", "T.W.I.G."};
        private readonly GameObject panel;
        private readonly TextMeshProUGUI statsTopicText;
        private readonly CustomStatsPanelScroll scrollPanel;
        
        public CustomStatsPanelKills()
        {
            var trophiesPanel = InventoryGui.instance.m_trophiesPanel;
            if (trophiesPanel == null) return;

            SkillsDialog skillsDialog = InventoryGui.instance.m_skillsDialog;
            
            panel = GameObject.Instantiate(trophiesPanel, skillsDialog.transform);
            panel.name = "PlayerStatsFrameKills";
            panel.SetActive(false);
            
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, 0); // centered

            panel.transform.Find("TrophiesFrame").name = "MainFrame";

            Transform statsTopicTransform = skillsDialog.transform.Find(panel.name+"/MainFrame/topic");
            statsTopicText = statsTopicTransform.GetComponent<TextMeshProUGUI>();
            statsTopicText.text = ConfigurationFile.statsText.Value;

            skillsDialog.transform.Find(panel.name+"/MainFrame/Trophies").name = "Stats";
            skillsDialog.transform.Find(panel.name+"/MainFrame/Stats/TrophyListScroll").name = "StatsListScroll";
            skillsDialog.transform.Find(panel.name+"/MainFrame/Stats/TrophyList").name = "StatList";
            
            scrollPanel = new CustomStatsPanelScroll(skillsDialog.transform.Find(panel.name+"/MainFrame/Stats/StatList"));
            LoadKillStats();

            Transform statsCloseButtonTransform = skillsDialog.transform.Find(panel.name+"/MainFrame/Closebutton");
            Button statsCloseButtonButton = statsCloseButtonTransform.GetComponent<Button>();
            statsCloseButtonButton.onClick = new Button.ButtonClickedEvent();
            statsCloseButtonButton.onClick.AddListener(() =>
            {
                panel.SetActive(false);
                PlayerSkillupOptionsPatch.HideTabButtons();
            });
        }

        private void LoadKillStats()
        {
            Dictionary<string, float> killStats = getDictionaryKillStats();
            Dictionary<string, float> killStatsTranslated = new Dictionary<string, float>();
            foreach (var keyValuePair in killStats)
            {
                if (!MONSTERS_EXCEPTIONS.Contains(keyValuePair.Key))
                {
                    var translation = Localization.instance.Localize(keyValuePair.Key).Replace("<color=orange>", "").Replace("</color>", "");
                    killStatsTranslated.Add(translation, keyValuePair.Value);
                }
            }

            List<string> keys = killStatsTranslated.Keys.ToList();
            keys.Sort();
            Dictionary<string, float> row = new Dictionary<string, float>();
            for (int i = 0; i < keys.Count; i++)
            {
                float value = killStatsTranslated.GetValueSafe(keys[i]);
                if (value > 0)
                {
                    row.Add(keys[i], value);
                    if (row.Count == 5)
                    {
                        scrollPanel.AddRowToScrollList(row);
                        row = new Dictionary<string, float>();
                    }
                }
            }

            if (row.Count > 0)
            {
                scrollPanel.AddRowToScrollList(row);
            }
        }

        private Dictionary<string, float> getDictionaryKillStats()
        {
            var field = typeof(Game).GetField("m_playerProfile", BindingFlags.Instance | BindingFlags.NonPublic);
            return ((PlayerProfile)field?.GetValue(Game.instance))?.m_enemyStats;
        }

        public GameObject getPanel() { return panel; }
        
        public void reloadTexts()
        {
            statsTopicText.text = ConfigurationFile.statsText.Value; //TODO Add config for killStatsTopic text
            Dictionary<string, float> mStats = getDictionaryKillStats();
            if (mStats.Count != scrollPanel.statsTexts.Count)
            {
                scrollPanel.ClearAll();
                LoadKillStats();
            }
            else
            {
                foreach (KeyValuePair<string, TextMeshProUGUI> stat in scrollPanel.statsTexts)
                {
                    stat.Value.text = Localization.instance.Localize(stat.Key) + ": " + mStats.GetValueSafe(stat.Key);
                }
            }

            scrollPanel.reloadHeaderTitles();
        }
    }
}