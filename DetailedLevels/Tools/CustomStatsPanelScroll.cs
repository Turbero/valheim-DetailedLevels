using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DetailedLevels.Features;

namespace DetailedLevels.Tools
{
    public class CustomStatsPanelScroll
    {
        private readonly GameObject content;
        public readonly Dictionary<PlayerStatType, TextMeshProUGUI> statsTexts = new Dictionary<PlayerStatType, TextMeshProUGUI>();

        public CustomStatsPanelScroll(Transform parent)
        {
            InventoryGui inventoryGui = InventoryGui.instance;
            
            GameObject customPanel = new GameObject("CustomStatsGroup", typeof(RectTransform), typeof(Image));
            customPanel.transform.SetParent(parent.transform, false);
            Object.Destroy(customPanel.GetComponent<Image>()); //transparent

            RectTransform panelRT = customPanel.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.pivot = new Vector2(0.5f, 0.5f);
            panelRT.anchoredPosition = new Vector2(0, 0);
            panelRT.sizeDelta = new Vector2(1260, 650); //Size dimension

            Image bgImage = customPanel.GetComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.6f);

            // --- SCROLLRECT ---
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            scrollObj.transform.SetParent(customPanel.transform, false);
            Object.Destroy(scrollObj.GetComponent<Image>()); //transparent
            RectTransform scrollRT = scrollObj.GetComponent<RectTransform>();
            scrollRT.anchorMin = new Vector2(0, 0);
            scrollRT.anchorMax = new Vector2(1, 1);
            scrollRT.offsetMin = new Vector2(0, 0);
            scrollRT.offsetMax = new Vector2(0, 0);

            scrollObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);

            ScrollRect scrollRect = scrollObj.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 40f;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

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
            content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
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
            layout.spacing = 25f;
            layout.padding = new RectOffset(0, 0, 10, 20);

            var fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRT;

            // --- SCROLLBAR ---
            GameObject recipeScroll = inventoryGui.m_recipeListScroll.gameObject;
            GameObject scrollbarObj = Object.Instantiate(recipeScroll, customPanel.transform);
            scrollbarObj.name = "Scrollbar";

            RectTransform sbRT = scrollbarObj.GetComponent<RectTransform>();
            sbRT.anchorMin = new Vector2(1, 0);
            sbRT.anchorMax = new Vector2(1, 1);
            sbRT.pivot = new Vector2(1, 1);
            sbRT.sizeDelta = new Vector2(11, 0);
            sbRT.anchoredPosition = Vector2.zero;

            Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        }

        public void AddHeaderToScrollList(string title)
        {
            GameObject titleObj = new GameObject("GroupHeader", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(content.transform, false);
            RectTransform titleRT = titleObj.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0.5f, 1);
            titleRT.anchorMax = new Vector2(0.5f, 1);
            titleRT.pivot = new Vector2(0.5f, 1);
            titleRT.anchoredPosition = new Vector2(0, -10);
            titleRT.sizeDelta = new Vector2(300, 30);

            TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
            titleText.text = title;
            titleText.font = InventoryGui.instance.m_recipeName.font; // same font as recipe name window
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.yellow;
        }

        public void AddRowToScrollList(Dictionary<PlayerStatType, float> entries)
        {
            GameObject entryRow = new GameObject("StatsRow", typeof(RectTransform));
            entryRow.transform.SetParent(content.transform, false);

            RectTransform rowRT = entryRow.GetComponent<RectTransform>();
            rowRT.anchorMin = new Vector2(0, 1);
            rowRT.anchorMax = new Vector2(1, 1);
            rowRT.pivot = new Vector2(0, 1);
            rowRT.sizeDelta = new Vector2(0, 30);

            float columnSpacing = 250f; // space betweens columns
            float startX = 40f;         // left row offset

            int columnIndex = 0;
            foreach (var entry in entries)
            {
                GameObject entryGO = new GameObject("Stat", typeof(RectTransform), typeof(TextMeshProUGUI));
                entryGO.transform.SetParent(entryRow.transform, false);

                RectTransform entryRT = entryGO.GetComponent<RectTransform>();
                entryRT.anchorMin = new Vector2(0, 0.5f);
                entryRT.anchorMax = new Vector2(0, 0.5f);
                entryRT.pivot = new Vector2(0, 0.5f);
                entryRT.anchoredPosition = new Vector2(startX + columnSpacing * columnIndex, 0);
                entryRT.sizeDelta = new Vector2(240, 25); // column size

                TextMeshProUGUI entryText = entryGO.GetComponent<TextMeshProUGUI>();
                entryText.text = entry.Key + ": " + entry.Value;
                entryText.font = PlayerUtils.getFontAsset("Valheim-AveriaSansLibre");
                entryText.fontSize = 18;
                entryText.color = Color.white;
                entryText.alignment = TextAlignmentOptions.Left;

                statsTexts.Add(entry.Key, entryText);

                columnIndex++;
            }
        }
    }
}