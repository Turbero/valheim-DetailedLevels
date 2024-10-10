using UnityEngine;
using UnityEngine.UI;
using System.IO;  // Para trabajar con archivos del sistema
using System.Collections.Generic;  // Para usar Dictionary
using UnityEngine.Networking;  // Para cargar imágenes con UnityWebRequest

public class BuffIconMod
{
    private Image buffIcon;
    private Dictionary<string, Sprite> weaponIcons;
    private ItemDrop.ItemData lastEquippedItem;

    private void Start()
    {
        /*// Inicializar diccionario de iconos para diferentes armas
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

    private void OnDestroy()
    {
        /*// Desuscribirse del evento cuando el mod se desactive o el objeto sea destruido
        if (Player.m_localPlayer != null)
        {
            Player.m_localPlayer.GetInventory().m_onChanged -= OnInventoryChanged;
        }*/
    }

    // Método que se ejecuta cuando el inventario del jugador cambia
    private void OnInventoryChanged()
    {
        UpdateBuffIcon();
    }

    // Actualiza el icono de buff según el arma equipada
    private void UpdateBuffIcon()
    {
        if (Player.m_localPlayer != null && buffIcon != null)
        {
            ItemDrop.ItemData equippedItem = Player.m_localPlayer.GetInventory().GetEquippedItems()[0];

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

    // Crear el icono de buff en la UI
    private void CreateBuffIconUI()
    {
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
            Debug.LogError("No se pudo cargar la imagen: " + filePath);
            return null;
        }

        // Obtener la textura desde UnityWebRequest
        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

        // Crear un sprite a partir de la textura
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
