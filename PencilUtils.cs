using System;
using System.Collections.Generic;
using System.Linq;
using RepoWebListener;
namespace RepoWebListener;

class PencilUtils
{
    public static System.Random Randomizer = new System.Random();
    public static RepoWebListenerConfigActivator PencilConfig { get; private set; } = null!;

    public static void Initialize(RepoWebListenerConfigActivator config)
    {
        PencilConfig = config;
    }

    public static bool IsBlacklistedLevel()
    {
        HashSet<Level> blacklist = new HashSet<Level>
            {
                RunManager.instance.levelLobby,
                RunManager.instance.levelTutorial,
                RunManager.instance.levelLobbyMenu,
                RunManager.instance.levelMainMenu,
                RunManager.instance.levelRecording
            };
        if (!PencilConfig.EnabledInShopLevel)
        {
            blacklist.Add(RunManager.instance.levelShop);
        }
        if (!PencilConfig.EnabledInArenaLevel)
        {
            blacklist.Add(RunManager.instance.levelArena);
        }
        return blacklist.Contains(RunManager.instance.levelCurrent);
    }

    public static List<PlayerAvatar> GetAllPlayers()
    {
        List<PlayerAvatar> players = [.. SemiFunc.PlayerGetAll()];
        return players;
    }
    public static List<PlayerAvatar> GetAlivePlayers()
    {
        List<PlayerAvatar> players = new List<PlayerAvatar>();

        foreach (PlayerAvatar item in GetAllPlayers())
        {
            if (item.playerHealth.health > 0)
            {
                players.Add(item);
            }
        }
        return players;
    }

    public static List<PlayerAvatar> GetDeadPlayers()
    {
        List<PlayerAvatar> players = new List<PlayerAvatar>();

        foreach (PlayerAvatar item in GetAllPlayers())
        {
            if (item.playerHealth.health <= 0)
            {
                players.Add(item);
            }
        }
        return players;
    }

    public static Dictionary<string, string> GetAllowedItems()
    {
        Dictionary<string, string> allowedItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string item in PencilConfig.WhitelistedItems.Keys)
        {
            if (PencilConfig.WhitelistedItems[item].Value)
            {
                allowedItems.Add(item, Dictionaries.ItemPaths[item]);
            }
        }
        return allowedItems;
    }
    public static Dictionary<string, string> GetAllowedValuables()
    {
        Dictionary<string, string> allowedValuables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string item in PencilConfig.WhitelistedValuables.Keys)
        {
            if (PencilConfig.WhitelistedValuables[item].Value)
            {
                allowedValuables.Add(item, Dictionaries.ValuablePaths[item]);
            }
        }
        return allowedValuables;
    }
    
        public static Dictionary<string, EnemySetup> GetAllowedEnemies()
    {
        List<EnemySetup> list =
        [
            .. EnemyDirector.instance.enemiesDifficulty1,
            .. EnemyDirector.instance.enemiesDifficulty2,
            .. EnemyDirector.instance.enemiesDifficulty3,
        ];
        Dictionary<string, EnemySetup> allowedEnemies = new Dictionary<string, EnemySetup>(StringComparer.OrdinalIgnoreCase);
        foreach (string item in PencilConfig.WhitelistedEnemies.Keys)
        {
            if (PencilConfig.WhitelistedEnemies[item].Value)
            {
                EnemySetup enemySetup = list.FirstOrDefault(x => x.name == $"Enemy - {item}");
                if (enemySetup != null)
                {
                    // Logger.LogInfo($"Registering enemy {item}");
                    allowedEnemies.Add(item, enemySetup);
                }
                else
                {
                RepoWebListener.Logger.LogError($"Enemy {item} not found. Cannot add to allowed enemies.");
                }
            }
        }
        return allowedEnemies;
    }
}