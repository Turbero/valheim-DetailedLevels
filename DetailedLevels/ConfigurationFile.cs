using BepInEx.Configuration;
using BepInEx;
using UnityEngine;

namespace DetailedLevels
{
    internal class ConfigurationFile
    {
        public static ConfigEntry<bool> debug;
        public static ConfigEntry<KeyCode> hotKey;
        public static ConfigEntry<int> numberOfDecimals;
        public static ConfigEntry<int> skillUpMessageAfterMultipleLevel;
        public static ConfigEntry<int> skillUpBigMessageAfterMultipleLevel;
        public static ConfigEntry<Color> colorSkillBackground;
        public static ConfigEntry<bool> saveSkillBuffs;
        //public static ConfigEntry<bool> birdsSkillIncrease;

        private static ConfigFile config;

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                config = plugin.Config;

                debug = config.Bind<bool>("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)");
                hotKey = config.Bind<KeyCode>("1 - General", "HotKey", KeyCode.F4, "Hot Key to show the skills tab without opening the inventory first (default = F7)");
                numberOfDecimals = config.Bind<int>("2 - Levels Data", "NumberOfDecimals", 2, "Number of decimals to show in your levels information (default = 2, min = 0, max = 15)");
                skillUpMessageAfterMultipleLevel = config.Bind<int>("2 - Levels Data", "SkillUpMessageAfterMultipleLevel", 5, "Shows skill up message after the new level is multiple of the indicated level (0 = disabled, default = 5)");
                skillUpBigMessageAfterMultipleLevel = config.Bind<int>("2 - Levels Data", "SkillUpBigMessageAfterMultipleLevel", 20, "Shows skill up big message after the new level is multiple of the indicated level (0 = disabled, default = 5)");
                colorSkillBackground = config.Bind<Color>("2 - Levels Data", "ColorSkillBackground", Color.cyan, "Choose the color background for selected skills in the skills dialog: red, green, blue, white, black, yellow, cyan, magenta, gray or grey (default = cyan)");
                saveSkillBuffs = config.Bind<bool>("2 - Levels Data", "SaveSkillBuffs", false, "Enable/disable the option to reload tracked skills after dying (default = false");
                //birdsSkillIncrease = config.Bind<bool>("3 - Improvements", "birdsSkillIncrease", true, "Allows skilling up when killing birds and getting their feathers with the used weapon (default = true)");
            }
        }
    }
}
