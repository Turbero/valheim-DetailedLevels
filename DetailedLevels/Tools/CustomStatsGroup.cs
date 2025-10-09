using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DetailedLevels.Features;

namespace DetailedLevels.Tools
{

    public class CustomStatsGroup
    {
        public CustomStatsGroup(Transform parent, Vector2 position, string title, List<string> entries)
        {
            InventoryGui inventoryGui = InventoryGui.instance;
            
            GameObject customPanel = new GameObject("CustomStatsGroup", typeof(RectTransform), typeof(Image));
            customPanel.transform.SetParent(parent.transform, false);
            Object.Destroy(customPanel.GetComponent<Image>()); //transparent

            RectTransform panelRT = customPanel.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.pivot = new Vector2(0.5f, 0.5f);
            panelRT.anchoredPosition = position;
            panelRT.sizeDelta = new Vector2(320, 300);

            Image bgImage = customPanel.GetComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.6f);

            // --- TÍTULO ---
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(customPanel.transform, false);
            RectTransform titleRT = titleObj.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0.5f, 1);
            titleRT.anchorMax = new Vector2(0.5f, 1);
            titleRT.pivot = new Vector2(0.5f, 1);
            titleRT.anchoredPosition = new Vector2(0, -10);
            titleRT.sizeDelta = new Vector2(300, 30);

            TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
            titleText.text = title;
            titleText.font = inventoryGui.m_recipeName.font; // igual que el título de recetas
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.yellow;

            // --- SCROLLRECT ---
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            scrollObj.transform.SetParent(customPanel.transform, false);
            Object.Destroy(scrollObj.GetComponent<Image>()); //transparent
            RectTransform scrollRT = scrollObj.GetComponent<RectTransform>();
            scrollRT.anchorMin = new Vector2(0, 0);
            scrollRT.anchorMax = new Vector2(1, 1);
            scrollRT.offsetMin = new Vector2(0, 0);
            scrollRT.offsetMax = new Vector2(-20, -40); // deja espacio para el título y el scrollbar

            scrollObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);

            ScrollRect scrollRect = scrollObj.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;

            // --- VIEWPORT ---
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(scrollObj.transform, false);
            RectTransform vpRT = viewport.GetComponent<RectTransform>();
            vpRT.anchorMin = Vector2.zero;
            vpRT.anchorMax = Vector2.one;
            vpRT.offsetMin = Vector2.zero;
            vpRT.offsetMax = Vector2.zero;

            viewport.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
            viewport.GetComponent<Mask>().showMaskGraphic = false;
            scrollRect.viewport = vpRT;

            // --- CONTENT ---
            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = Vector2.zero;
            contentRT.sizeDelta = new Vector2(0, 0);

            var layout = content.GetComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.spacing = 20f; // más separación

            var fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRT;

            // --- SCROLLBAR (igual al del juego) ---
            GameObject recipeScroll = inventoryGui.m_recipeListScroll.gameObject;
            GameObject scrollbarObj = Object.Instantiate(recipeScroll, customPanel.transform);
            scrollbarObj.name = "Scrollbar";

            RectTransform sbRT = scrollbarObj.GetComponent<RectTransform>();
            sbRT.anchorMin = new Vector2(1, 0);
            sbRT.anchorMax = new Vector2(1, 1);
            sbRT.pivot = new Vector2(1, 1);
            sbRT.sizeDelta = new Vector2(20, 0);
            sbRT.anchoredPosition = Vector2.zero;

            Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            // --- ENTRADAS ---
            foreach (string entryText in entries)
            {
                GameObject entry = new GameObject("Stat", typeof(RectTransform), typeof(TextMeshProUGUI));
                entry.transform.SetParent(content.transform, false);

                TextMeshProUGUI txt = entry.GetComponent<TextMeshProUGUI>();
                txt.text = entryText;
                txt.font = PlayerUtils.getFontAsset("Valheim-AveriaSansLibre");
                txt.fontSize = 18;
                txt.color = Color.white;
                txt.alignment = TextAlignmentOptions.Left;
            }
        }
    }

}