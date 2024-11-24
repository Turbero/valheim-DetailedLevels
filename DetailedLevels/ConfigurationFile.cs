using BepInEx.Configuration;
using BepInEx;
using UnityEngine;
using ServerSync;

namespace DetailedLevels
{
    internal class ConfigurationFile
    {
        private static ConfigEntry<bool> _serverConfigLocked = null;

        public static ConfigEntry<bool> debug;
        public static ConfigEntry<KeyCode> hotKey;
        public static ConfigEntry<int> numberOfDecimals;
        public static ConfigEntry<int> skillUpMessageAfterMultipleLevel;
        public static ConfigEntry<int> skillUpBigMessageAfterMultipleLevel;
        public static ConfigEntry<Color> colorSkillBackground;
        public static ConfigEntry<bool> saveSkillBuffs;
        public static ConfigEntry<float> deathSkillLoss;

        private static ConfigFile configFile;

        private static readonly ConfigSync ConfigSync = new ConfigSync(DetailedLevels.GUID)
        {
            DisplayName = DetailedLevels.NAME,
            CurrentVersion = DetailedLevels.VERSION,
            MinimumRequiredVersion = DetailedLevels.VERSION
        };

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                _serverConfigLocked = config("1 - General", "Lock Configuration", true,
                "If on, the configuration is locked and can be changed by server admins only.");
                _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

                debug = config("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)", false);
                hotKey = config("1 - General", "HotKey", KeyCode.F4, "Hot Key to show the skills tab without opening the inventory first (default = F4)", false);
                numberOfDecimals = config("2 - Levels Data", "NumberOfDecimals", 2, "Number of decimals to show in your levels information (default = 2, min = 0, max = 15)", false);
                skillUpMessageAfterMultipleLevel = config("2 - Levels Data", "SkillUpMessageAfterMultipleLevel", 5, "Shows skill up message after the new level is multiple of the indicated level (0 = disabled, default = 5)", false);
                skillUpBigMessageAfterMultipleLevel = config("2 - Levels Data", "SkillUpBigMessageAfterMultipleLevel", 20, "Shows skill up big message after the new level is multiple of the indicated level (0 = disabled, default = 20)", false);
                colorSkillBackground = config("2 - Levels Data", "ColorSkillBackground", Color.cyan, "Choose the color background for selected skills in the skills dialog: red, green, blue, white, black, yellow, cyan, magenta, gray or grey (default = cyan)", false);
                saveSkillBuffs = config("2 - Levels Data", "SaveSkillBuffs", false, "Enable/disable the option to reload tracked skills after dying (default = false)", false);
                deathSkillLoss = config("3 - Config", "DeathSkillLoss", 5f, "Allows skilling up when killing birds and getting their feathers with the used weapon (default = true)");
            }
        }

        private static ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new ConfigDescription(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = configFile.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }
    }
}
