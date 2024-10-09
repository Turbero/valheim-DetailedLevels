using BepInEx;
using HarmonyLib;

namespace DetailedLevels
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class DetailedLevels : BaseUnityPlugin
    {
        public const string GUID = "Turbero.DetailedLevels";
        public const string NAME = "Detailed Levels";
        public const string VERSION = "1.1.0";

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
    }
}

