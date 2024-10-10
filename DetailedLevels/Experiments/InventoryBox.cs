using UnityEngine;
using UnityEngine.UI;

namespace DetailedLevels
{
    public class InventoryBox
    {
        private GameObject boxObject;
        private bool inventoryOpen = false;

        // Método para crear la caja en el centro de la pantalla
        private void CreateBoxUI()
        {
            // Crear un objeto para la caja
            GameObject canvasObject = new GameObject("BasicBoxCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Crear un objeto para la caja dentro del canvas
            boxObject = new GameObject("BasicBox");
            boxObject.transform.SetParent(canvasObject.transform);

            // Agregar componente Image para que la caja sea visible
            Image boxImage = boxObject.AddComponent<Image>();
            //boxImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);  // Color gris semitransparente
            boxImage.color = Color.cyan;

            // Configurar el tamaño y posición de la caja
            RectTransform rectTransform = boxObject.GetComponent<RectTransform>();
            //rectTransform.sizeDelta = new Vector2(200, 200);  // Tamaño de la caja (200x200 píxeles)
            rectTransform.sizeDelta = new Vector2(600, 600);
            rectTransform.anchoredPosition = new Vector2(0, 0);  // Posición en el centro de la pantalla

            // Inicialmente, ocultar la caja
            //boxObject.SetActive(false);
            boxObject.SetActive(true);
        }

        public void UpdateBox()
        {
            // Verificar si el inventario está abierto usando InventoryGui.IsVisible()
            if (InventoryGui.IsVisible() && !inventoryOpen)
            {
                Logger.Log("*** InventoryBox Update ToOpen");
                if (boxObject == null) CreateBoxUI();
                inventoryOpen = true;
                boxObject.SetActive(true);  // Mostrar la caja cuando se abre el inventario
            }
            else if (!InventoryGui.IsVisible() && inventoryOpen)
            {
                Logger.Log("*** InventoryBox Update ToClose");
                if (boxObject == null) CreateBoxUI();
                inventoryOpen = false;
                boxObject.SetActive(false);  // Ocultar la caja cuando se cierra el inventario
            }
        }
    }
}
