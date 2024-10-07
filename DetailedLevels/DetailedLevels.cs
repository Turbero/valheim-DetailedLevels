using BepInEx;
using HarmonyLib;
using static Utils;

namespace DetailedLevels
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class DetailedLevels : BaseUnityPlugin
    {
        public const string GUID = "Turbero.DetailedLevels";
        public const string NAME = "Detailed Levels";
        public const string VERSION = "1.0.2";

        private readonly Harmony harmony = new Harmony("Turbero.DetailedLevels");

        void Awake()
        {
            ConfigurationFile.LoadConfig(this);
            
            harmony.PatchAll();
        }

        void onDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}

