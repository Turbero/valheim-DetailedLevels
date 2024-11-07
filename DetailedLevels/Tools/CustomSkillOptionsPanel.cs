using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DetailedLevels.Features;

namespace DetailedLevels.Config
{
    public class CustomSkillOptionsPanel
    {
        private GameObject panel;
        private TextMeshProUGUI titleText;
        private TMP_Text buttonText;

        public CustomSkillOptionsPanel(Transform parent)
        {
            // Crear un panel y configurarlo
            panel = new GameObject("CustomSkillOptionsPanel", typeof(RectTransform));
            panel.SetActive(false);
            panel.transform.SetParent(InventoryGui.instance.transform, false); // Asegúrate de ponerlo en un canvas adecuado

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(512, 512); // Tamaño del panel, ajusta según lo necesario
            panelRect.anchoredPosition = new Vector2(0, 0); // Posición en pantalla (centrado aquí)

            // Añadir fondo con apariencia del juego
            Image panelImage = panel.AddComponent<Image>();
            panelImage.sprite = PlayerUtils.getSprite("woodpanel_512x512"); // Carga el sprite de fondo
            panelImage.type = Image.Type.Sliced; // Permite que el borde no se deforme

            // Crear título del panel
            GameObject titleObject = new GameObject("Title", typeof(TextMeshProUGUI));
            titleObject.transform.SetParent(panel.transform, false);

            titleText = titleObject.GetComponent<TextMeshProUGUI>();            
            titleText.fontSize = 24;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;

            RectTransform titleRect = titleObject.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 220); 

            // Close button
            Transform closeButtonTransform = InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame").transform.Find("Closebutton");
            GameObject buttonTextObject = GameObject.Instantiate(closeButtonTransform.gameObject, parent);
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
