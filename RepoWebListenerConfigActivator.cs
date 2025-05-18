using System.Collections.Generic;
using BepInEx.Configuration;
using Unity.VisualScripting.FullSerializer;

namespace RepoWebListener;

class RepoWebListenerConfigActivator
{
    // We define our config variables in a public scope

    readonly ConfigEntry<int> webServerListenPort;
    public int WebServerListenPort => webServerListenPort.Value;
    readonly ConfigEntry<string> webServerListenIP;
    public string WebServerListenIP => webServerListenIP.Value;
    readonly ConfigEntry<bool> webServerEnabled;
    public bool WebServerEnabled => webServerEnabled.Value;

    // Levels
    readonly ConfigEntry<bool> enabledInShopLevel;
    public bool EnabledInShopLevel => enabledInShopLevel.Value;

    readonly ConfigEntry<bool> enabledInArenaLevel;
    public bool EnabledInArenaLevel => enabledInArenaLevel.Value;
    readonly ConfigEntry<bool> goodThings;
    public bool GoodThings => goodThings.Value;

    readonly ConfigEntry<bool> badThings;
    public bool BadThings => badThings.Value;
    readonly ConfigEntry<bool> goodEventHealAll;
    public bool GoodEventHealAll => goodEventHealAll.Value;
    readonly ConfigEntry<bool> goodEventHealSpecific;
    public bool GoodEventHealSpecific => goodEventHealSpecific.Value;
    readonly ConfigEntry<int> goodEventHealMinAmount;
    public int GoodEventHealMinAmount => goodEventHealMinAmount.Value;
    readonly ConfigEntry<int> goodEventHealMaxAmount;
    public int GoodEventHealMaxAmount => goodEventHealMaxAmount.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllEnergy;
    public bool GoodEventUpgradeAllEnergy => goodEventUpgradeAllEnergy.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificEnergy;
    public bool GoodEventUpgradeSpecificEnergy => goodEventUpgradeSpecificEnergy.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllHealth;
    public bool GoodEventUpgradeAllHealth => goodEventUpgradeAllHealth.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificHealth;
    public bool GoodEventUpgradeSpecificHealth => goodEventUpgradeSpecificHealth.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllGrabStrength;
    public bool GoodEventUpgradeAllGrabStrength => goodEventUpgradeAllGrabStrength.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificGrabStrength;
    public bool GoodEventUpgradeSpecificGrabStrength => goodEventUpgradeSpecificGrabStrength.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllRange;
    public bool GoodEventUpgradeAllRange => goodEventUpgradeAllRange.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificRange;
    public bool GoodEventUpgradeSpecificRange => goodEventUpgradeSpecificRange.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllSpeed;
    public bool GoodEventUpgradeAllSpeed => goodEventUpgradeAllSpeed.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificSpeed;
    public bool GoodEventUpgradeSpecificSpeed => goodEventUpgradeSpecificSpeed.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllExtraJump;
    public bool GoodEventUpgradeAllExtraJump => goodEventUpgradeAllExtraJump.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificExtraJump;
    public bool GoodEventUpgradeSpecificExtraJump => goodEventUpgradeSpecificExtraJump.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllTumbleLaunch;
    public bool GoodEventUpgradeAllTumbleLaunch => goodEventUpgradeAllTumbleLaunch.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificTumbleLaunch;
    public bool GoodEventUpgradeSpecificTumbleLaunch => goodEventUpgradeSpecificTumbleLaunch.Value;
    readonly ConfigEntry<bool> goodEventUpgradeAllMapPlayerCount;
    public bool GoodEventUpgradeAllMapPlayerCount => goodEventUpgradeAllMapPlayerCount.Value;
    readonly ConfigEntry<bool> goodEventUpgradeSpecificMapPlayerCount;
    public bool GoodEventUpgradeSpecificMapPlayerCount => goodEventUpgradeSpecificMapPlayerCount.Value;

    readonly ConfigEntry<bool> goodEventSpawnRandomItem;
    public bool GoodEventSpawnRandomItem => goodEventSpawnRandomItem.Value;
    public Dictionary<string, ConfigEntry<bool>> WhitelistedItems = new Dictionary<string, ConfigEntry<bool>>();

    readonly ConfigEntry<bool> goodEventSpawnRandomValuable;
    public bool GoodEventSpawnRandomValuable => goodEventSpawnRandomValuable.Value;
    public Dictionary<string, ConfigEntry<bool>> WhitelistedValuables = new Dictionary<string, ConfigEntry<bool>>();

    readonly ConfigEntry<bool> badEventDamageAll;
    public bool BadEventDamageAll => badEventDamageAll.Value;
    readonly ConfigEntry<bool> badEventDamageSpecific;
    public bool BadEventDamageSpecific => badEventDamageSpecific.Value;
    readonly ConfigEntry<int> badEventDamageMinAmount;
    public int BadEventDamageMinAmount => badEventDamageMinAmount.Value;
    readonly ConfigEntry<int> badEventDamageMaxAmount;
    public int BadEventDamageMaxAmount => badEventDamageMaxAmount.Value;
    readonly ConfigEntry<bool> badEventSpawnRandomEnemy;
    public bool BadEventSpawnRandomEnemy => badEventSpawnRandomEnemy.Value;
    // Dictionary of all enemies
    public Dictionary<string, ConfigEntry<bool>> WhitelistedEnemies = new Dictionary<string, ConfigEntry<bool>>();


    public RepoWebListenerConfigActivator(ConfigFile cfg)
    {
        // Web Server 
        webServerEnabled = cfg.Bind(
            "Web Server",                        // Config section
            "WebServerEnabled",                     // Key of this config
            true,                    // Default value
            "Should I start the web server?\nIf you don't plan on being the host, you can probably turn this off."    // Description
        );
        webServerListenPort = cfg.Bind(
            "Web Server",                        // Config section
            "WebServerListenPort",                     // Key of this config
            7390,                    // Default value
            "What port should I listen on for requests? (i.e. http://localhost:XXXX)"    // Description
        );
        webServerListenIP = cfg.Bind(
            "Web Server",                        // Config section
            "WebServerListenIP",                     // Key of this config
            "localhost",                    // Default value
            "What IP should I listen on for requests? (i.e. http://XXXX:7390)\nIf you're unsure, leave this as localhost!"    // Description
        );
        if (webServerListenPort.Value < 1024 || webServerListenPort.Value > 65535)
        {
            RepoWebListener.Logger.LogError("Port must be between 1024 and 65535. Defaulting to 7390.");
            webServerListenPort.Value = 7390;
        }
        if (string.IsNullOrEmpty(webServerListenIP.Value))
        {
            RepoWebListener.Logger.LogError("IP must be a valid IP address. Defaulting to localhost.");
            webServerListenIP.Value = "localhost";
        }
        // Levels
        enabledInShopLevel = cfg.Bind(
            // Config section
            "Levels",
            // Key of this config
            "EnabledInShopLevel",
            // Default value
            true,
            // Description
            "Should I send events when you're in the shop level?"
        );
        enabledInArenaLevel = cfg.Bind(
            // Config section
            "Levels",
            // Key of this config
            "EnabledInArenaLevel",
            // Default value
            true,
            // Description
            "Should I send events when you're in the arena level?"
        );


        // Good Things
        goodThings = cfg.Bind(
            // Config section
            "Events.General",
            // Key of this config
            "GoodEvents",
            // Default value
            true,
            // Description
            "Should I enable good things happening to players?\nSetting this to false will disable all good events."
        );
        // Bad Things
        badThings = cfg.Bind(
            // Config section
            "Events.General",
            // Key of this config
            "BadEvents",
            // Default value
            true,
            // Description
            "Should I enable bad things happening to players?\nSetting this to false will disable all bad events."
        );
        // Specific Good Events
        // Healing Events
        goodEventHealAll = cfg.Bind(
            // Config section
            "Good Events.Healing",
            // Key of this config
            "GoodEventHealAll",
            // Default value
            true,
            // Description
            "Should healing all players be a possible event?"
        );
        goodEventHealSpecific = cfg.Bind(
            // Config section
            "Good Events.Healing",
            // Key of this config
            "GoodEventHealSpecific",
            // Default value
            true,
            // Description
            "Should healing a specific player be a possible event?\nSetting this to false will ignore GoodEventHealMaxAmount."
        );
        goodEventHealMinAmount = cfg.Bind(
            // Config section
            "Good Events.Healing",
            // Key of this config
            "GoodEventHealMinAmount",
            // Default value
            0,
            // Description
            "What's the minimum healing I'm allowed to grant per goodEventHealAll/GoodEventHealSpecific event?\nA random amount between this value and GoodEventHealMaxAmount and this value will rolled per player."
        );
        goodEventHealMaxAmount = cfg.Bind(
            // Config section
            "Good Events.Healing",
            // Key of this config
            "GoodEventHealMaxAmount",
            // Default value
            25,
            // Description
            "What's the maximum healing I'm allowed to grant per goodEventHealAll/GoodEventHealSpecific event?\nA random amount between GoodEventHealMinAmount and this value will rolled per player."
        );
        // Upgrade Events
        goodEventUpgradeAllEnergy = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllEnergy",
            // Default value
            true,
            // Description
            "Should upgrading all players' stamina be a possible event?"
        );
        goodEventUpgradeSpecificEnergy = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificEnergy",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's stamina be a possible event?"
        );
        goodEventUpgradeAllHealth = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllHealth",
            // Default value
            true,
            // Description
            "Should upgrading all players' health be a possible event?"
        );
        goodEventUpgradeSpecificHealth = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificHealth",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's health be a possible event?"
        );
        goodEventUpgradeAllGrabStrength = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllGrabStrength",
            // Default value
            true,
            // Description
            "Should upgrading all players' strength be a possible event?"
        );
        goodEventUpgradeSpecificGrabStrength = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificGrabStrength",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's strength be a possible event?"
        );
        goodEventUpgradeAllRange = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllRange",
            // Default value
            true,
            // Description
            "Should upgrading all players' grab range be a possible event?"
        );
        goodEventUpgradeSpecificRange = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificRange",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's grab range be a possible event?"
        );
        goodEventUpgradeAllSpeed = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllSpeed",
            // Default value
            true,
            // Description
            "Should upgrading all players' sprint speed be a possible event?"
        );
        goodEventUpgradeSpecificSpeed = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificSpeed",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's sprint speed be a possible event?"
        );
        goodEventUpgradeAllExtraJump = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllExtraJump",
            // Default value
            true,
            // Description
            "Should upgrading all players' extra jumps (+1) be a possible event?"
        );
        goodEventUpgradeSpecificExtraJump = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificExtraJump",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's extra jumps (+1) be a possible event?"
        );
        goodEventUpgradeAllTumbleLaunch = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllTumbleLaunch",
            // Default value
            true,
            // Description
            "Should upgrading all players' tumble launch be a possible event?"
        );
        goodEventUpgradeSpecificTumbleLaunch = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificTumbleLaunch",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's tumble launch be a possible event?"
        );
        goodEventUpgradeAllMapPlayerCount = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeAllMapPlayerCount",
            // Default value
            true,
            // Description
            "Should upgrading all players' map player count be a possible event?"
        );
        goodEventUpgradeSpecificMapPlayerCount = cfg.Bind(
            // Config section
            "Good Events.Upgrading",
            // Key of this config
            "GoodEventUpgradeSpecificMapPlayerCount",
            // Default value
            true,
            // Description
            "Should upgrading a specific player's map player count be a possible event?"
        );
        // Good Spawning Events
        goodEventSpawnRandomItem = cfg.Bind(
            // Config section
            "Good Events.Item Spawning",
            // Key of this config
            "GoodEventSpawnRandomItem",
            // Default value
            true,
            // Description
            "Should spawning a random item near a random player be a possible event?\nSetting this to false will ignore all below events starting with GoodEventSpawnItem"
        );
        bool atLeastOneItemInWhitelist = false;
        foreach (var pair in RepoWebListener.ItemPaths)
        {
            WhitelistedItems[pair.Key] = cfg.Bind(
                // Config section
                "Good Events.Item Spawning",
                // Key of this config
                "GoodEventSpawn" + pair.Key,
                // Default value
                true,
                // Description
                $"Should a \"{pair.Key.Replace("Item", "")}\" item be possible for GoodEventSpawnRandomItem?"
            );
            if (WhitelistedItems[pair.Key].Value)
            {
                atLeastOneItemInWhitelist = true;
            }
        }
        if (!atLeastOneItemInWhitelist)
        {
            goodEventSpawnRandomItem.Value = false;
            RepoWebListener.Logger.LogWarning("All items are disabled. Assuming GoodEventSpawnRandomItem config entry to false.");
        }
        // Good Valuable Spawning Events
        goodEventSpawnRandomValuable = cfg.Bind(
            // Config section
            "Good Events.Valuable Spawning",
            // Key of this config
            "GoodEventSpawnRandomValuable",
            // Default value
            true,
            // Description
            "Should spawning a random Valuable be a possible event?\nSetting this to false will ignore all below events starting with GoodEventSpawnValuable."
        );

        bool atLeastOneValuableInWhitelist = false;
        foreach (var pair in RepoWebListener.ValuablePaths)
        {
            WhitelistedValuables[pair.Key] = cfg.Bind(
                // Config section
                "Good Events.Valuable Spawning",
                // Key of this config
                "GoodEventSpawn" + pair.Key,
                // Default value
                true,
                // Description
                $"Should a \"{pair.Key.Replace("Valuable", "")}\" valuable be possible for GoodEventSpawnRandomValuable?"
            );
            if (WhitelistedValuables[pair.Key].Value)
            {
                atLeastOneValuableInWhitelist = true;
            }
        }
        if (!atLeastOneValuableInWhitelist)
        {
            goodEventSpawnRandomValuable.Value = false;
            RepoWebListener.Logger.LogWarning("All valuables are disabled. Assuming GoodEventSpawnRandomValuable config entry to false.");
        }
        bool shouldKeepGoodEventsEnabled = GoodThings && (
            GoodEventHealAll ||
        GoodEventHealSpecific ||
        GoodEventUpgradeAllEnergy ||
        GoodEventUpgradeSpecificEnergy ||
        GoodEventUpgradeAllHealth ||
        GoodEventUpgradeSpecificHealth ||
        GoodEventUpgradeAllGrabStrength ||
        GoodEventUpgradeSpecificGrabStrength ||
        GoodEventUpgradeAllRange ||
        GoodEventUpgradeSpecificRange ||
        GoodEventUpgradeAllSpeed ||
        GoodEventUpgradeSpecificSpeed ||
        GoodEventUpgradeAllExtraJump ||
        GoodEventUpgradeSpecificExtraJump ||
        GoodEventUpgradeAllTumbleLaunch ||
        GoodEventUpgradeSpecificTumbleLaunch ||
        GoodEventUpgradeAllMapPlayerCount ||
        GoodEventUpgradeSpecificMapPlayerCount ||
        GoodEventSpawnRandomItem ||
        GoodEventSpawnRandomValuable
        );
        if (!shouldKeepGoodEventsEnabled)
        {
            goodThings.Value = false;
            RepoWebListener.Logger.LogWarning("All good events are disabled. Assuming GoodThings config entry to false.");
        }

        // Bad Events
        badEventDamageAll = cfg.Bind(
            // Config section
            "Bad Events.Damage",
            // Key of this config
            "BadEventDamageAll",
            // Default value
            true,
            // Description
            "Should damaging all players be a possible event?"
        );
        badEventDamageSpecific = cfg.Bind(
            // Config section
            "Bad Events.Damage",
            // Key of this config
            "BadEventDamageSpecific",
            // Default value
            true,
            // Description
            "Should damaging a specific player be a possible event?"
        );
        badEventDamageMinAmount = cfg.Bind(
            // Config section
            "Bad Events.Damage",
            // Key of this config
            "BadEventDamageMinAmount",
            // Default value
            0,
            // Description
            "What's the minimum damage I'm allowed to deal per BadEventDamageAll/BadEventDamageSpecific event?\nA random amount between this value and BadEventDamageMaxAmount will rolled per player."
        );
        badEventDamageMaxAmount = cfg.Bind(
            // Config section
            "Bad Events.Damage",
            // Key of this config
            "BadEventDamageMaxAmount",
            // Default value
            25,
            // Description
            "What's the maximum damage I'm allowed to deal per BadEventDamageAll/BadEventDamageSpecific event?\nA random amount between BadEventDamageMinAmount and this value will rolled per player."
        );
        // Bad Spawning Events
        badEventSpawnRandomEnemy = cfg.Bind(
            // Config section
            "Bad Events.Enemy Spawning",
            // Key of this config
            "BadEventSpawnRandomEnemy",
            // Default value
            true,
            // Description
            "Should spawning a random enemy near a random player be a possible event?\nSetting this to false will ignore all below events starting with BadEventSpawnEnemy."
        );
        bool atLeastOneEnemyInWhitelist = false;
        foreach (var pair in RepoWebListener.EnemyPaths)
        {
            WhitelistedEnemies[pair.Key] = cfg.Bind(
                // Config section
                "Bad Events.Enemy Spawning",
                // Key of this config
                "BadEventSpawn" + pair.Key,
                // Default value
                true,
                // Description
                $"Should a \"{pair.Key.Replace("Enemy", "")}\" enemy be possible for BadEventSpawnRandomEnemy?"
            );
            if (WhitelistedEnemies[pair.Key].Value)
            {
                atLeastOneEnemyInWhitelist = true;
            }
        }
        if (!atLeastOneEnemyInWhitelist)
        {
            badEventSpawnRandomEnemy.Value = false;
            RepoWebListener.Logger.LogWarning("All enemies are disabled. Assuming BadEventSpawnRandomEnemy config entry to false.");
        }
        bool shouldKeepBadEventsEnabled = BadThings && (
            BadEventDamageAll ||
            BadEventDamageSpecific ||
            BadEventSpawnRandomEnemy
        );
        if (!shouldKeepBadEventsEnabled)
        {
            badThings.Value = false;
            RepoWebListener.Logger.LogWarning("All bad events are disabled. Assuming BadThings config entry to false.");
        }




    }
}