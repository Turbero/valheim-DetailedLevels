using BepInEx;
using HarmonyLib;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DetailedLevels
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class DetailedLevels : BaseUnityPlugin
    {
        public const string GUID = "Turbero.DetailedLevels";
        public const string NAME = "Detailed Levels";
        public const string VERSION = "1.7.3";

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
                hideCustomPanelAndSkillsDialog();
            }

            // Hotkey to open/close skills dialog (if game is not paused)
            if (Input.GetKeyDown(ConfigurationFile.hotKey.Value) && Time.timeScale > 0)
            {
                if (InventoryGui.instance.m_skillsDialog.gameObject.activeSelf)
                {
                    hideCustomPanelAndSkillsDialog();
                    InventoryGui.instance.Hide();
                }
                else
                {
                    InventoryGui.instance.Show(null);
                    _ = WaitForSecondsAsync(0.15f); // Small delay to avoid coroutine issue in log to wait for showing skills dialog until it is active
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static void hideCustomPanelAndSkillsDialog()
        {
            if (InventoryGui.instance.m_skillsDialog == null) return;
            
            InventoryGui.instance.m_skillsDialog.gameObject.SetActive(false);

            if (InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/CustomSkillOptionsPanel") == null)
                return;

            InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/CustomSkillOptionsPanel").gameObject.SetActive(false);
            
            InventoryGui.instance.m_skillsDialog.transform.Find("TabStatsButton").gameObject.SetActive(false);
            InventoryGui.instance.m_skillsDialog.transform.Find("TabKillStatsButton").gameObject.SetActive(false);
        }
        private static async Task WaitForSecondsAsync(float seconds)
        {
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milliseconds
            InventoryGui.instance.m_skillsDialog.Setup(Player.m_localPlayer);
            InventoryGui.instance.m_skillsDialog.gameObject.SetActive(true);
        }
    }
    
    [HarmonyPatch]
    public class NoWarnings
    {
        [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), typeof(object), typeof(Object))]
        [HarmonyPrefix]
        private static bool LogWarning(object message, Object context)
        {
            if (message.ToString().Contains("The LiberationSans SDF Font Asset was not found") || 
                message.ToString().Contains("The character used for Ellipsis is not available in font asset"))
            {
                return false;
            }

            return true;
        }
    }
}

