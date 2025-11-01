using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DetailedLevels.Tools
{
    public class CustomSkillOptionsPanel
    {
        private GameObject panel;
        private TextMeshProUGUI titleText;
        private TMP_Text buttonText;

        public CustomSkillOptionsPanel(Transform copyForCloseButton)
        {
            Init(copyForCloseButton);
        }
        
        private void Init(Transform copyForCloseButton) {
            Transform skillsFrameTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame").transform;

            // Panel
            panel = new GameObject("CustomSkillOptionsPanel", typeof(RectTransform));
            panel.SetActive(false);
            panel.transform.SetParent(skillsFrameTransform, false);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(512, 512);
            panelRect.anchoredPosition = new Vector2(0, 0); // (0,0) = centered

            // Background
            Image original = skillsFrameTransform.Find("bkg").GetComponent<Image>();
            Image clone = GameObject.Instantiate(original, panel.transform);
            clone.name = "bkg_customskills";

            // Title
            GameObject originalTitle = skillsFrameTransform.Find("topic").gameObject;
            GameObject titleClone = GameObject.Instantiate(originalTitle, panel.transform);
            titleClone.name = "Title";
            
            titleText = titleClone.GetComponent<TextMeshProUGUI>();
            titleText.text = Localization.instance.Localize("$button_ps_start");
            
            RectTransform titleRect = titleClone.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, -30); 

            // Close button
            Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton");
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

        public GameObject getPanel()
        {
            if (panel == null)
            {
                Logger.Log("Recreating gameObject panel...");
                Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton");
                Button closeButton = closeButtonTransform.GetComponent<Button>();
                Init(closeButton.transform.parent);
                Logger.Log("gameObject panel recreated.");
            }
            return panel;
        }

        public void reloadTexts()
        {
            titleText.text = Localization.instance.Localize("$button_ps_start");
            buttonText.text = Localization.instance.Localize("$menu_close");
        }
    }

}
