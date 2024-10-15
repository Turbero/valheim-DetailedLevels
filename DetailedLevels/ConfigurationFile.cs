using BepInEx.Configuration;
using BepInEx;
using UnityEngine;

namespace DetailedLevels
{
    internal class ConfigurationFile
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> debug;
        public static ConfigEntry<KeyCode> hotKey;
        public static ConfigEntry<int> numberOfDecimals;
        public static ConfigEntry<int> skillUpMessageAfterMultipleLevel;
        public static ConfigEntry<bool> equipBuffs;
        //public static ConfigEntry<bool> birdsSkillIncrease;

        private static ConfigFile config;

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                config = plugin.Config;

                modEnabled = config.Bind<bool>("1 - General", "Enabled", true, "Enabling/Disabling the mod (default = true)");
                debug = config.Bind<bool>("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)");
                hotKey = config.Bind<KeyCode>("1 - General", "HotKey", KeyCode.F, "Hot Key to show the skills tab without opening the inventory first (default = K)");
                numberOfDecimals = config.Bind<int>("2 - Levels Data", "NumberOfDecimals", 2, "Number of decimals to show in your levels information (default = 2, min = 0, max = 15)");
                skillUpMessageAfterMultipleLevel = config.Bind<int>("2 - Levels Data", "SkillUpMessageAfterMultipleLevel", 5, "Shows skill up message after the new level is multiple of the indicated level (0 = disabled, default = 5)");
                equipBuffs = config.Bind<bool>("2 - Levels Data", "equipBuffs", true, "Allows selecting buffs in the skills dialog to show them as buffs and look at how it is progressed");
                //birdsSkillIncrease = config.Bind<bool>("3 - Improvements", "birdsSkillIncrease", true, "Allows skilling up when killing birds and getting their feathers with the used weapon (default = true)");
            }
        }
    }
}
