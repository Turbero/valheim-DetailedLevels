using System;
using System.IO;
using BepInEx.Configuration;
using BepInEx;
using DetailedLevels.Features;
using UnityEngine;
using ServerSync;

namespace DetailedLevels
{
    public enum SkillsSortOrder
    {
        None,
        AlphabeticalAscending,
        AlphabeticalDescending,
        LevelAscending,
        LevelDescending
    }
    
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
        public static ConfigEntry<bool> saveSkillsOrder;
        public static ConfigEntry<SkillsSortOrder> saveSkillsOrderValue;
        public static ConfigEntry<float> deathSkillLoss;
        public static ConfigEntry<string> deathPenaltyText;
        public static ConfigEntry<string> reloadAfterDyingText;
        public static ConfigEntry<string> numberOfDecimalsText;
        public static ConfigEntry<string> skillUpMessageText;
        public static ConfigEntry<string> skillUpBigMessageText;
        public static ConfigEntry<string> skillUpValueText;
        public static ConfigEntry<string> skillsOrderText;
        public static ConfigEntry<string> statsText;
        public static ConfigEntry<string> statsProgressionText;
        public static ConfigEntry<string> statsTravellingText;
        public static ConfigEntry<string> statsOthersText;

        private static ConfigFile configFile;
        private static readonly string ConfigFileName = DetailedLevels.GUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

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

                _serverConfigLocked = config("1 - General", "Lock Configuration", true, "If on, the configuration is locked and can be changed by server admins only.");
                _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

                debug = config("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)", false);
                hotKey = config("1 - General", "HotKey", KeyCode.F4, "Hot Key to show the skills tab without opening the inventory first (default = F4)", false);
                numberOfDecimals = config("2 - Levels Data", "NumberOfDecimals", 2, "Number of decimals to show in your levels information (default = 2, min = 0, max = 15)", false);
                skillUpMessageAfterMultipleLevel = config("2 - Levels Data", "SkillUpMessageAfterMultipleLevel", 5, "Shows skill up message after the new level is multiple of the indicated level (0 = disabled, default = 5)", false);
                skillUpBigMessageAfterMultipleLevel = config("2 - Levels Data", "SkillUpBigMessageAfterMultipleLevel", 10, "Shows skill up big message after the new level is multiple of the indicated level (0 = disabled, default = 20)", false);
                colorSkillBackground = config("2 - Levels Data", "ColorSkillBackground", Color.cyan, "Choose the color background for selected skills in the skills dialog: red, green, blue, white, black, yellow, cyan, magenta, gray or grey (default = cyan)", false);
                saveSkillBuffs = config("2 - Levels Data", "SaveSkillBuffs", false, "Enable/disable the option to reload tracked skills after dying (default = false)", false);
                saveSkillsOrder = config("2 - Levels Data", "Save Skills Order", false, "Enable/disable the option to save the order selected in the skills dialog (default = false)", false);
                saveSkillsOrderValue = config("2 - Levels Data", "Skills Order Value", SkillsSortOrder.None, "Skills Order to use when skills dialog is opened and the save option is enabled (default = None)", false);
                deathSkillLoss = config("3 - Config", "DeathSkillLoss", 5f, "Amount of skill loss when dying (value between 0 and 100, default = 5 as vanilla)");

                deathPenaltyText = config("4 - Language", "DeathPenaltyText", "Death Penalty", "Translation for <Death Penalty> text");
                reloadAfterDyingText = config("4 - Language", "ReloadAfterDyingText", "Reload after dying", "Translation for <Reload after dying> text");
                numberOfDecimalsText  = config("4 - Language", "NumberOfDecimalsText", "Number of decimals", "Translation for <Number of decimals> text");
                skillUpMessageText = config("4 - Language", "SkillUpMessageText", "Skill up message", "Translation for <Skill up message> text");
                skillUpBigMessageText = config("4 - Language", "SkillUpBigMessageText", "Skill up big message", "Translation for <Skill up big message> text");
                skillUpValueText = config("4 - Language", "SkillUpValueText", "Each {0} levels", "Translation for <Each X levels> text");
                skillsOrderText = config("4 - Language", "SkillsOrderText", "Save Skills Order", "Translation for <Save Skills Order> text");
                statsText = config("4 - Language", "StatsText", "Stats", "Translation for <Stats> text");
                statsProgressionText = config("4 - Language", "StatsProgressionText", "Progression", "Translation for <Progression> text (restart game after change)");
                statsTravellingText = config("4 - Language", "StatsTravellingText", "Travelling", "Translation for <Travelling> text (restart game after change)");
                statsOthersText = config("4 - Language", "StatsOthersText", "Others", "Translation for <Others> text (restart game after change)");
                
                deathSkillLoss.SettingChanged += SettingsChanged;
                SetupWatcher();
            }
        }
        
        private static void SetupWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }
        
        private static void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Logger.Log("Attempting to reload configuration...");
                configFile.Reload();
                SettingsChanged(null, null);
            }
            catch
            {
                Logger.LogError($"There was an issue loading {ConfigFileName}");
            }
        }

        private static void SettingsChanged(object sender, EventArgs e)
        {
            if (0 <= deathSkillLoss.Value && deathSkillLoss.Value <= 100)
            {
                Player.m_localPlayer.GetSkills().m_DeathLowerFactor = deathSkillLoss.Value / 100f;
                Logger.Log("m_DeathLowerFactor: " + Player.m_localPlayer.GetSkills().m_DeathLowerFactor);

                PlayerSkillupOptionsPatch.updateSkillLossPercentage();
            }
            PlayerSkillupOptionsPatch.updateOptionsTexts();
            PlayerSkillupOptionsPatch.reloadTexts();
            PlayerColorBuffs.refreshAllBlueColors(Player.m_localPlayer);
            SkillTypeCraftStationDescriptionsPatch.updated = false;
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
