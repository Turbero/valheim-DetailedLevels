using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace DetailedLevels
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class DetailedLevels : BaseUnityPlugin
    {
        public const string GUID = "Turbero.DetailedLevels";
        public const string NAME = "Detailed Levels";
        public const string VERSION = "1.1.1";

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            ConfigurationFile.LoadConfig(this);

            harmony.PatchAll();
        }

        void onDestroy()
        {
            harmony.UnpatchSelf();
        }

        void Update()
        {
            if (!Player.m_localPlayer || !InventoryGui.instance) return;

            // Check if certain keys are hit to close Almanac GUI
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Player.m_localPlayer.IsDead())
            {
                InventoryGui.instance.m_skillsDialog.gameObject.SetActive(false);
            }

            // Hotkey to open almanac
            if (Input.GetKeyDown(ConfigurationFile.hotKey.Value))
            {
                if (InventoryGui.instance.m_skillsDialog.gameObject.activeSelf)
                {
                    InventoryGui.instance.m_skillsDialog.gameObject.SetActive(false);
                    InventoryGui.instance.Hide();
                }
                else
                {
                    InventoryGui.instance.Show(null);
                    InventoryGui.instance.m_skillsDialog.Setup(Player.m_localPlayer);
                    InventoryGui.instance.m_skillsDialog.gameObject.SetActive(true);
                }
            }
        }
    }
}

