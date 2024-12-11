using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DetailedLevels.Features;

namespace DetailedLevels.Tools
{
    public class CustomSkillOptionsPanel
    {
        private GameObject panel;
        private TextMeshProUGUI titleText;
        private TMP_Text buttonText;

        public CustomSkillOptionsPanel(Transform copyForCloseButton)
        {
            // Panel
            panel = new GameObject("CustomSkillOptionsPanel", typeof(RectTransform));
            panel.SetActive(false);
            panel.transform.SetParent(InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame").transform, false);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(512, 512);
            panelRect.anchoredPosition = new Vector2(0, 0); // (0,0) = centered

            // Background
            Image panelImage = panel.AddComponent<Image>();
            panelImage.sprite = PlayerUtils.getSprite("woodpanel_512x512"); // Background sprite
            panelImage.type = Image.Type.Sliced;

            // Title
            GameObject titleObject = new GameObject("Title", typeof(TextMeshProUGUI));
            titleObject.transform.SetParent(panel.transform, false);

            titleText = titleObject.GetComponent<TextMeshProUGUI>();
            titleText.font = PlayerUtils.getFontAsset("Valheim-Norsebold");
            titleText.fontSize = 36;
            titleText.color = Color.yellow;
            titleText.alignment = TextAlignmentOptions.Center;

            RectTransform titleRect = titleObject.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 220); 

            // Close button
            Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame").transform.Find("Closebutton");
            GameObject buttonTextObject = GameObject.Instantiate(closeButtonTransform.gameObject, copyForCloseButton);
            buttonTextObject.name = "DLOptionsCloseButton";
            buttonTextObject.transform.SetParent(panel.transform, false);

            RectTransform buttonTextRect = buttonTextObject.GetComponent<RectTransform>();
            buttonTextRect.anchoredPosition = new Vector2(0, 40);

            buttonText = buttonTextObject.GetComponentInChildren<TMP_Text>();            

            Button dlOptionsButton = buttonTextObject.GetComponent<Button>();
            dlOptionsButton.onClick = new Button.ButtonClickedEvent();
            dlOptionsButton.onClick.AddListener(() =>
            {
                panel.SetActive(false);
                //Reload levels in case of change
                InventoryGui.instance.m_skillsDialog.Setup(Player.m_localPlayer);
                    
            });

            reloadTexts();
        }

        public GameObject getPanel() { return panel; }

        public void reloadTexts()
        {
            titleText.text = Localization.instance.Localize("$button_ps_start");
            buttonText.text = Localization.instance.Localize("$menu_close");
        }
    }

}
