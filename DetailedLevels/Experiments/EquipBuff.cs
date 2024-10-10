using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;  // Para trabajar con archivos del sistema

namespace DetailedLevels
{
    public class EquipBuff : MonoBehaviour
    {
        private Image buffIcon;
        private Dictionary<string, Sprite> weaponIcons;
        private ItemDrop.ItemData lastEquippedItem;

        void Awake()
        {
            /*Logger.Log($"mensaje de EquipItem: awake");
            Start();*/
        }

        void Start()
        {
            /*Logger.Log("Start EquipItem_Patch");

            // Inicializar diccionario de iconos para diferentes armas
            weaponIcons = new Dictionary<string, Sprite>()
                {
                    { "SwordIron", LoadSprite("path/to/sword_icon.png") },
                    { "BowHuntsman", LoadSprite("path/to/bow_icon.png") },
                    { "AxeBlackMetal", LoadSprite("path/to/axe_icon.png") }
                };

            // Crear la UI para el icono de buff
            CreateBuffIconUI();

            // Suscribirse al evento de cambio de inventario del jugador
            Player.m_localPlayer.GetInventory().m_onChanged += OnInventoryChanged;*/
        }

        void onDestroy()
        {
            /*Logger.Log("OnDestroy EquipItem_Patch");
            // Desuscribirse del evento cuando el mod se desactive o el objeto sea destruido
            if (Player.m_localPlayer != null)
            {
                Player.m_localPlayer.GetInventory().m_onChanged -= OnInventoryChanged;
            }*/
        }

        // Método que se ejecuta cuando el inventario del jugador cambia
        private void OnInventoryChanged()
        {
            Logger.Log("OnInventoryChanged EquipItem_Patch");
            UpdateBuffIcon();
        }

        // Actualiza el icono de buff según el arma equipada
        private void UpdateBuffIcon()
        {
            Logger.Log("UpdateBuffIcon EquipItem_Patch");
            if (Player.m_localPlayer != null && buffIcon != null)
            {
                List<ItemDrop.ItemData> equippedItems = Player.m_localPlayer.GetInventory().GetEquippedItems();

                for (int i = 0; i < equippedItems.Count; i++)
                {
                    ItemDrop.ItemData equippedItem = equippedItems[i];

                    if (equippedItem != null && weaponIcons.ContainsKey(equippedItem.m_shared.m_name))
                    {
                        // Solo actualizar si el arma equipada ha cambiado
                        if (equippedItem != lastEquippedItem)
                        {
                            buffIcon.sprite = weaponIcons[equippedItem.m_shared.m_name];
                            buffIcon.enabled = true;
                            lastEquippedItem = equippedItem; // Guardar el arma equipada
                        }
                    }
                    else
                    {
                        // Si no hay un arma equipada o no está en el diccionario, ocultar el icono
                        buffIcon.enabled = false;
                        lastEquippedItem = null; // Limpiar el último ítem equipado
                    }
                }
            }
        }

        // Crear el icono de buff en la UI
        private void CreateBuffIconUI()
        {
            Logger.Log("CreateBuffIconUI EquipItem_Patch");
            GameObject canvasObject = new GameObject("BuffIconCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject buffIconObject = new GameObject("BuffIcon");
            buffIconObject.transform.SetParent(canvasObject.transform);

            buffIcon = buffIconObject.AddComponent<Image>();
            buffIcon.rectTransform.sizeDelta = new Vector2(64, 64);
            buffIcon.rectTransform.anchoredPosition = new Vector2(100, 100);
            buffIcon.enabled = false;  // Empezar con el icono desactivado
        }

        // Método para cargar un Sprite desde un archivo de imagen usando UnityWebRequest
        private Sprite LoadSprite(string filePath)
        {
            Logger.Log("LoadSprite EquipItem_Patch");
            if (!File.Exists(filePath)) return null;

            // Crear la URL del archivo local (file://)
            string fileURL = "file://" + filePath;

            // Crear un UnityWebRequest para cargar la imagen
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(fileURL);
            var asyncOp = uwr.SendWebRequest();

            // Esperar a que el UnityWebRequest se complete
            while (!asyncOp.isDone) { }

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Logger.LogError("No se pudo cargar la imagen: " + filePath);
                return null;
            }

            // Obtener la textura desde UnityWebRequest
            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

            // Crear un sprite a partir de la textura
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
        class EquipItem_Patch
        {

            static void Postfix(ref ItemDrop.ItemData item, ref bool triggerEquipEffects)
            {
                //Logger.Log($"mensaje de EquipItem: {item} / {triggerEquipEffects}");
            }

            
        }
    }
}
