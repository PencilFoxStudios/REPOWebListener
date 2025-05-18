using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using Photon.Pun;
using BepInEx.Configuration;
using System.Linq;
using REPOLib.Modules;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Bindings;
namespace RepoWebListener;

[BepInPlugin("PencilFoxStudios.RepoWebListener", "RepoWebListener", "1.0")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
public class RepoWebListener : BaseUnityPlugin
{
    internal static RepoWebListener Instance { get; private set; } = null!;
    public new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    static HttpListener listener = new HttpListener();
    string url = "http://localhost";
    private CancellationTokenSource cts = new CancellationTokenSource();
    Queue<string> chatters = new Queue<string>();
    static System.Random random = new System.Random();
    static Dictionary<string, string> AllowedItems;

    static Dictionary<string, string> AllowedValuables;
    static Dictionary<string, string> AllowedEnemies;
    public static NetworkedEvent? NewChatterEvent;
    internal static RepoWebListenerConfigActivator PencilConfig { get; private set; } = null!;
    private void Awake()
    {
        Instance = this;


        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();


        PencilConfig = new RepoWebListenerConfigActivator(Config);





        url = $"http://{PencilConfig.WebServerListenIP}:{PencilConfig.WebServerListenPort}/";






        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        if (PencilConfig.WebServerEnabled)
        {
            Logger.LogInfo("Starting web server...");
            listener.Prefixes.Add(url);
            listener.Start();
            Logger.LogInfo("Listening on " + url);
        }
        else
        {
            Logger.LogInfo("Web server is disabled. Check the config file to enable it.");
        }

        NewChatterEvent = new NetworkedEvent("NewChatterEvent", HandleChatterEvent);
        // Register the event
        PhotonPeer.RegisterType(typeof(MissionOptions), 100, MissionOptions.Serialize, MissionOptions.Deserialize);
        // Set up the listener to handle requests


        Task.Run(() => ListenLoop(cts.Token));
        Task.Run(() => GoThroughChatters(cts.Token));
        AllowedItems = GetAllowedItems();
        AllowedValuables = GetAllowedValuables();
        AllowedEnemies = GetAllowedEnemies();
    }
    public static readonly Dictionary<string, string> ItemPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ItemCartMedium", "items/Item Cart Medium" },
            { "ItemCartSmall", "items/Item Cart Small" },
            { "ItemDroneBattery", "items/Item Drone Battery" },
            { "ItemDroneFeather", "items/Item Drone Feather" },
            { "ItemDroneIndestructible", "items/Item Drone Indestructible" },
            { "ItemDroneTorque", "items/Item Drone Torque" },
            { "ItemDroneZeroGravity", "items/Item Drone Zero Gravity" },
            { "ItemExtractionTracker", "items/Item Extraction Tracker" },
            { "ItemGrenadeDuctTaped", "items/Item Grenade Duct Taped" },
            { "ItemGrenadeExplosive", "items/Item Grenade Explosive" },
            { "ItemGrenadeHuman", "items/Item Grenade Human" },
            { "ItemGrenadeShockwave", "items/Item Grenade Shockwave" },
            { "ItemGrenadeStun", "items/Item Grenade Stun" },
            { "ItemGunHandgun", "items/Item Gun Handgun" },
            { "ItemGunShotgun", "items/Item Gun Shotgun" },
            { "ItemGunTranq", "items/Item Gun Tranq" },
            { "ItemHealthPackLarge", "items/Item Health Pack Large" },
            { "ItemHealthPackMedium", "items/Item Health Pack Medium" },
            { "ItemHealthPackSmall", "items/Item Health Pack Small" },
            { "ItemMeleeBaseballBat", "items/Item Melee Baseball Bat" },
            { "ItemMeleeFryingPan", "items/Item Melee Frying Pan" },
            { "ItemMeleeInflatableHammer", "items/Item Melee Inflatable Hammer" },
            { "ItemMeleeSledgeHammer", "items/Item Melee Sledge Hammer" },
            { "ItemMeleeSword", "items/Item Melee Sword" },
            { "ItemMineExplosive", "items/Item Mine Explosive" },
            { "ItemMineShockwave", "items/Item Mine Shockwave" },
            { "ItemMineStun", "items/Item Mine Stun" },
            { "ItemOrbZeroGravity", "items/Item Orb Zero Gravity" },
            { "ItemPowerCrystal", "items/Item Power Crystal" },
            { "ItemRubberDuck", "items/Item Rubber Duck" },
            { "ItemUpgradeMapPlayerCount", "items/Item Upgrade Map Player Count" },
            { "ItemUpgradePlayerEnergy", "items/Item Upgrade Player Energy" },
            { "ItemUpgradePlayerExtraJump", "items/Item Upgrade Player Extra Jump" },
            { "ItemUpgradePlayerGrabRange", "items/Item Upgrade Player Grab Range" },
            { "ItemUpgradePlayerGrabStrength", "items/Item Upgrade Player Grab Strength" },
            { "ItemUpgradePlayerGrabThrow", "items/Item Upgrade Player Grab Throw" },
            { "ItemUpgradePlayerHealth", "items/Item Upgrade Player Health" },
            { "ItemUpgradePlayerSprintSpeed", "items/Item Upgrade Player Sprint Speed" },
            { "ItemUpgradePlayerTumbleLaunch", "items/Item Upgrade Player Tumble Launch" },
            { "ItemValuableTracker", "items/Item Valuable Tracker" }
    };
    // Valuables
    public static readonly Dictionary<string, string> ValuablePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
 { "ValuableDiamond", "valuables/01 tiny/Valuable Diamond" },
            { "ValuableEmeraldBracelet", "valuables/01 tiny/Valuable Emerald Bracelet" },
            { "ValuableGoblet", "valuables/01 tiny/Valuable Goblet" },
            { "ValuableOcarina", "valuables/01 tiny/Valuable Ocarina" },
            { "ValuablePocketWatch", "valuables/01 tiny/Valuable Pocket Watch" },
            { "ValuableUraniumMug", "valuables/01 tiny/Valuable Uranium Mug" },
            { "ValuableArcticBonsai", "valuables/02 small/Valuable Arctic Bonsai" },
            { "ValuableArcticHDD", "valuables/02 small/Valuable Arctic HDD" },
            { "ValuableChompBook", "valuables/02 small/Valuable Chomp Book" },
            { "ValuableCrown", "valuables/02 small/Valuable Crown" },
            { "ValuableDoll", "valuables/02 small/Valuable Doll" },
            { "ValuableFrog", "valuables/02 small/Valuable Frog" },
            { "ValuableGemBox", "valuables/02 small/Valuable Gem Box" },
            { "ValuableGlobe", "valuables/02 small/Valuable Globe" },
            { "ValuableLovePotion", "valuables/02 small/Valuable Love Potion" },
            { "ValuableMoney", "valuables/02 small/Valuable Money" },
            { "ValuableMusicBox", "valuables/02 small/Valuable Music Box" },
            { "ValuableToyMonkey", "valuables/02 small/Valuable Toy Monkey" },
            { "ValuableUraniumPlate", "valuables/02 small/Valuable Uranium Plate" },
            { "ValuableVaseSmall", "valuables/02 small/Valuable Vase Small" },
            { "ValuableArctic3DPrinter", "valuables/03 medium/Valuable Arctic 3D Printer" },
            { "ValuableArcticLaptop", "valuables/03 medium/Valuable Arctic Laptop" },
            { "ValuableArcticPropaneTank", "valuables/03 medium/Valuable Arctic Propane Tank" },
            { "ValuableArcticSampleSixPack", "valuables/03 medium/Valuable Arctic Sample Six Pack" },
            { "ValuableArcticSample", "valuables/03 medium/Valuable Arctic Sample" },
            { "ValuableBottle", "valuables/03 medium/Valuable Bottle" },
            { "ValuableClown", "valuables/03 medium/Valuable Clown" },
            { "ValuableComputer", "valuables/03 medium/Valuable Computer" },
            { "ValuableFan", "valuables/03 medium/Valuable Fan" },
            { "ValuableGramophone", "valuables/03 medium/Valuable Gramophone" },
            { "ValuableMarbleTable", "valuables/03 medium/Valuable Marble Table" },
            { "ValuableRadio", "valuables/03 medium/Valuable Radio" },
            { "ValuableShipInBottle", "valuables/03 medium/Valuable Ship in a bottle" },
            { "ValuableTrophy", "valuables/03 medium/Valuable Trophy" },
            { "ValuableVase", "valuables/03 medium/Valuable Vase" },
            { "ValuableWizardGoblinHead", "valuables/03 medium/Valuable Wizard Goblin Head" },
            { "ValuableWizardPowerCrystal", "valuables/03 medium/Valuable Wizard Power Crystal" },
            { "ValuableWizardTimeGlass", "valuables/03 medium/Valuable Wizard Time Glass" },
            { "ValuableArcticBarrel", "valuables/04 big/Valuable Arctic Barrel" },
            { "ValuableArcticBigSample", "valuables/04 big/Valuable Arctic Big Sample" },
            { "ValuableArcticCreatureLeg", "valuables/04 big/Valuable Arctic Creature Leg" },
            { "ValuableArcticFlamethrower", "valuables/04 big/Valuable Arctic Flamethrower" },
            { "ValuableArcticGuitar", "valuables/04 big/Valuable Arctic Guitar" },
            { "ValuableArcticSampleCooler", "valuables/04 big/Valuable Arctic Sample Cooler" },
            { "ValuableDiamondDisplay", "valuables/04 big/Valuable Diamond Display" },
            { "ValuableIceSaw", "valuables/04 big/Valuable Ice Saw" },
            { "ValuableScreamDoll", "valuables/04 big/Valuable Scream Doll" },
            { "ValuableTelevision", "valuables/04 big/Valuable Television" },
            { "ValuableVaseBig", "valuables/04 big/Valuable Vase Big" },
            { "ValuableWizardCubeOfKnowledge", "valuables/04 big/Valuable Wizard Cube of Knowledge" },
            { "ValuableWizardMasterPotion", "valuables/04 big/Valuable Wizard Master Potion" },
            { "ValuableAnimalCrate", "valuables/05 wide/Valuable Animal Crate" },
            { "ValuableArcticIceBlock", "valuables/05 wide/Valuable Arctic Ice Block" },
            { "ValuableDinosaur", "valuables/05 wide/Valuable Dinosaur" },
            { "ValuablePiano", "valuables/05 wide/Valuable Piano" },
            { "ValuableWizardGriffinStatue", "valuables/05 wide/Valuable Wizard Griffin Statue" },
            { "ValuableArcticScienceStation", "valuables/06 tall/Valuable Arctic Science Station" },
            { "ValuableHarp", "valuables/06 tall/Valuable Harp" },
            { "ValuablePainting", "valuables/06 tall/Valuable Painting" },
            { "ValuableWizardDumgolfsStaff", "valuables/06 tall/Valuable Wizard Dumgolfs Staff" },
            { "ValuableWizardSword", "valuables/06 tall/Valuable Wizard Sword" },
            { "ValuableArcticServerRack", "valuables/07 very tall/Valuable Arctic Server Rack" },
            { "ValuableGoldenStatue", "valuables/07 very tall/Valuable Golden Statue" },
            { "ValuableGrandfatherClock", "valuables/07 very tall/Valuable Grandfather Clock" },
            { "ValuableWizardBroom", "valuables/07 very tall/Valuable Wizard Broom" }

        };

    public static readonly Dictionary<string, string> EnemyPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Beamer", "Enemies/Enemy - Beamer" },
            { "Duck", "Enemies/Enemy - Duck" },
            { "Robe", "Enemies/Enemy - Robe" },
            // { "Bowtie", "Enemies/Enemy - Bowtie" },
            { "Floater", "Enemies/Enemy - Floater" },
            // { "Gnome", "Enemies/Enemy - Gnome" },
            { "Hunter", "Enemies/Enemy - Hunter" }
        };
    private async Task ListenLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context)); // handle each request separately

            }
        }
        catch (HttpListenerException ex)
        {
            Logger.LogWarning($"Listener stopped: {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in ListenLoop: {ex}");
        }
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

    public static List<PlayerAvatar> GetAlivePlayers()
    {
        List<PlayerAvatar> players = new List<PlayerAvatar>();

        foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
        {
            if (item.playerHealth.health > 0)
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
            if(PencilConfig.WhitelistedItems[item].Value)
            {
                allowedItems.Add(item, ItemPaths[item]);
            }
        }
        return allowedItems;
    }
    public static Dictionary<string, string> GetAllowedValuables()
    {
        Dictionary<string, string> allowedValuables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string item in PencilConfig.WhitelistedValuables.Keys)
        {
            if(PencilConfig.WhitelistedValuables[item].Value)
            {
                allowedValuables.Add(item, ValuablePaths[item]);
            }
        }
        return allowedValuables;
    }
    public static Dictionary<string, string> GetAllowedEnemies()
    {
        Dictionary<string, string> allowedEnemies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string item in PencilConfig.WhitelistedEnemies.Keys)
        {
            if(PencilConfig.WhitelistedEnemies[item].Value)
            {
                allowedEnemies.Add(item, EnemyPaths[item]);
            }
        }
        return allowedEnemies;
    }
    public static string doGoodThingTo(PlayerAvatar player)
    {

        // Do something good to the player
        Logger.LogInfo($"Doing good thing to {player.playerName}");
        string hint = "Yippee!";
        // Get PUN manager
        PunManager punManager = PunManager.instance;

        List<Action> actions = [];

        if (PencilConfig.GoodEventHealAll)
        { // Heal the player 
            actions.Add(() =>
            {
                PlayerAvatar playerWithMostHealth = player;
                int mostHealth = 0;
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    int amount = random.Next(PencilConfig.GoodEventHealMinAmount, PencilConfig.GoodEventHealMaxAmount);
                    if (amount > mostHealth)
                    {
                        mostHealth = amount;
                        playerWithMostHealth = item;
                    }
                    item.playerHealth.HealOther(amount, true);
                }

                Logger.LogInfo($"Healed everyone a bit");
                hint = $"Everyone got healed, but <b>{playerWithMostHealth.playerName}</b> got healed the most with +<b>{mostHealth} HP</b>!";
            });
        }
        if (PencilConfig.GoodEventHealSpecific)
        { // Heal the player
            actions.Add(() =>
            {
                int amount = random.Next(PencilConfig.GoodEventHealMinAmount, PencilConfig.GoodEventHealMaxAmount);
                player.playerHealth.Heal(amount);
                Logger.LogInfo($"Healed {player.playerName} for {amount} HP");
                hint = $"<b>{player.playerName}</b> got healed for +<b>{amount} HP</b>!";
            });
        }
        if (PencilConfig.GoodEventSpawnRandomValuable && AllowedValuables.Count > 0)
        { // Spawn a random valuable
            actions.Add(() =>
            {
                string randomThing = AllowedValuables.Keys.ElementAt(random.Next(AllowedValuables.Count));
                string path = ValuablePaths[randomThing];
                GameObject item = PhotonNetwork.InstantiateRoomObject(path, player.transform.position + player.transform.up * 0.2f, Quaternion.identity, 0);
                item.GetComponent<Rigidbody>().velocity = player.transform.up * 10f;
                item.GetComponent<Rigidbody>().angularVelocity = new Vector3(random.Next(-1, 1), random.Next(-1, 1), random.Next(-1, 1));
                Logger.LogInfo($"Spawned item: {randomThing}");
                hint = $"<b>{player.playerName}</b> should look around!";
            });
        }
        if (PencilConfig.GoodEventSpawnRandomItem && AllowedItems.Count > 0)
        { // Spawn a random item
            actions.Add(() =>
            {
                string randomThing = AllowedItems.Keys.ElementAt(random.Next(AllowedItems.Count));
                string path = ItemPaths[randomThing];
                GameObject item = PhotonNetwork.InstantiateRoomObject(path, player.transform.position + player.transform.up * 0.2f, Quaternion.identity, 0);
                item.GetComponent<Rigidbody>().velocity = player.transform.up * 10f;
                item.GetComponent<Rigidbody>().angularVelocity = new Vector3(random.Next(-1, 1), random.Next(-1, 1), random.Next(-1, 1));
                Logger.LogInfo($"Spawned item: {randomThing}");
                hint = $"<b>{player.playerName}</b> got something!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllEnergy)
        { // Upgrade all players' energy
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradePlayerEnergy(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone feels energetic now!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificEnergy)
        { // Upgrade the player's energy
            actions.Add(() =>
            {
                punManager.UpgradePlayerEnergy(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b> feels energetic now!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllHealth)
        { // Upgrade all players' health
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradePlayerHealth(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone feels healthier!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificHealth)
        { // Upgrade the player's health
            actions.Add(() =>
            {
                punManager.UpgradePlayerHealth(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b> feels healthier!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllGrabStrength)
        { // Upgrade all players' grab strength
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradePlayerGrabStrength(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone feels stronger now!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificGrabStrength)
        { // Upgrade the player's grab strength
            actions.Add(() =>
            {
                punManager.UpgradePlayerGrabStrength(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b> feels stronger!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllRange)
        { // Upgrade all players' grab range
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradePlayerGrabRange(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone feels more agile!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificRange)
        { // Upgrade the player's grab range
            actions.Add(() =>
            {
                punManager.UpgradePlayerGrabRange(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b> feels more agile!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllExtraJump)
        { // Upgrade all players' extra jump
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradePlayerExtraJump(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone feels more nimble!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificExtraJump)
        { // Upgrade the player's extra jump
            actions.Add(() =>
            {
                punManager.UpgradePlayerExtraJump(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b> feels more nimble!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllSpeed)
        { // Upgrade all players' sprint speed
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradePlayerSprintSpeed(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone feels faster!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificSpeed)
        { // Upgrade the player's sprint speed
            actions.Add(() =>
            {
                punManager.UpgradePlayerSprintSpeed(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b> feels faster!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllTumbleLaunch)
        { // Upgrade all players' tumble launch
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradePlayerTumbleLaunch(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone's foreheads just got more powerful!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificTumbleLaunch)
        { // Upgrade the player's tumble launch
            actions.Add(() =>
            {
                punManager.UpgradePlayerTumbleLaunch(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b>'s forehead just got more powerful!";
            });
        }
        if (PencilConfig.GoodEventUpgradeAllMapPlayerCount)
        { // Upgrade all players' map player count
            actions.Add(() =>
            {
                foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
                {
                    punManager.UpgradeMapPlayerCount(item.steamID);
                }
                Logger.LogInfo($"Upgraded all players");
                hint = "Everyone feels more aware of their surroundings!";
            });
        }
        if (PencilConfig.GoodEventUpgradeSpecificMapPlayerCount)
        { // Upgrade the player's map player count
            actions.Add(() =>
            {
                punManager.UpgradeMapPlayerCount(player.steamID);
                Logger.LogInfo($"Upgraded {player.playerName}");
                hint = $"<b>{player.playerName}</b> feels more aware of their surroundings!";
            });
        }



        // Choose a random action from the list
        if (actions.Count == 0)
        {
            Logger.LogError("No actions to perform. This should not happen!");
            return "No actions to perform";
        }
        Action randomAction = actions[random.Next(actions.Count)];
        // Execute the random action
        randomAction.Invoke();
        Logger.LogInfo($"Executed action on {player.playerName}");
        return hint;
    }
    public static string doBadThingTo(PlayerAvatar player)
    {
        // Do something bad to the player
        Logger.LogInfo($"Doing bad thing to {player.playerName}");
        string hint = "Ruh roh...";
        List<Action> actions = [];

        if (PencilConfig.BadEventDamageSpecific)
        { // Deal damage to a player
            actions.Add(() =>
            {
                int amount = random.Next(PencilConfig.BadEventDamageMinAmount, PencilConfig.BadEventDamageMaxAmount);
                player.playerHealth.HurtOther(amount, player.playerHealth.transform.position, false, -1);
                Logger.LogInfo($"Dealt damage to {player.playerName}");
                hint = $"<b>{player.playerName}</b> got hurt for -<b>{amount} HP</b>!";
            });
            
        }

        if (PencilConfig.BadEventDamageAll)
        { // Deal damage to all players
            actions.Add(() =>
            {
                PlayerAvatar playerWithMostDamage = player;
                int mostDamage = 0;
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    int amount = random.Next(PencilConfig.BadEventDamageMinAmount, PencilConfig.BadEventDamageMaxAmount);
                    if (amount > mostDamage)
                    {
                        mostDamage = amount;
                        playerWithMostDamage = item;
                    }
                    item.playerHealth.HurtOther(amount, item.playerHealth.transform.position, false, -1);
                }
                Logger.LogInfo($"Dealt damage to all players");
                hint = $"Everyone got hurt, but <b>{playerWithMostDamage.playerName}</b> got hurt the most with -<b>{mostDamage} HP</b>!";
            });

        }

        if (PencilConfig.BadEventSpawnRandomEnemy)
        { // Spawn a random enemy
            actions.Add(() =>
            {
                string randomEnemy = AllowedEnemies.Keys.ElementAt(random.Next(AllowedEnemies.Count));
                string path = EnemyPaths[randomEnemy];
                GameObject enemy = Resources.Load<GameObject>(path);
                EnemySetup enemySetup = ScriptableObject.CreateInstance<EnemySetup>();
                enemySetup.spawnObjects = [enemy];
                Enemies.RegisterEnemy(enemySetup);
                Enemies.SpawnEnemy(enemySetup, player.transform.position + player.transform.up * 0.2f, Quaternion.identity, false);
                Logger.LogInfo($"Spawned enemy: {randomEnemy}");
                hint = $"<b>{player.playerName}</b> should watch their back!";
            });
        }



        // Choose a random action from the list
        Action randomAction = actions[random.Next(actions.Count)];
        // Execute the random action
        randomAction.Invoke();
        Logger.LogInfo($"Executed action on {player.playerName}");
        return hint;
    }
    private class MissionOptions
    {
        public Color color1 { get; set; }
        public Color color2 { get; set; }
        public string msg { get; set; }
        public float time { get; set; }

        public static byte[] Serialize(object customObject)
        {
            MissionOptions options = (MissionOptions)customObject;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(options.msg);
                    writer.Write(options.color1.r);
                    writer.Write(options.color1.g);
                    writer.Write(options.color1.b);
                    writer.Write(options.color2.r);
                    writer.Write(options.color2.g);
                    writer.Write(options.color2.b);
                    writer.Write(options.time);
                }
                return stream.ToArray();
            }
        }
        public static object Deserialize(byte[] serializedCustomObject)
        {
            using (MemoryStream stream = new MemoryStream(serializedCustomObject))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                MissionOptions options = new MissionOptions();
                options.msg = reader.ReadString();
                options.color1 = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                options.color2 = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                options.time = reader.ReadSingle();
                return options;
            }
        }

    }
    private async Task GoThroughChatters(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (chatters.Count == 0 ||
                !SemiFunc.IsMultiplayer() ||
                 IsBlacklistedLevel() // Check if the current level is blacklisted
                || (RunManager.instance.runStarted == false) // Check if the run has started
                || (RunManager.instance.allPlayersDead == true) // Check if all players are dead
                || (LevelGenerator.Instance.Generated == false) // Check if the level is generated
                || (RoundDirector.instance.extractionPointsCompleted == 0 && (!RoundDirector.instance.extractionPointActive)) // Check if the players haven't left truck yet

                )
                {
                    await Task.Delay(1000, token); // Wait for 1 second if there are no chatters or
                    // Logger.LogInfo("Waiting for the time to be right...");
                    // not in multiplayer
                    continue;
                }
                List<PlayerAvatar> players = GetAlivePlayers();
                if (players.Count == 0)
                {
                    await Task.Delay(1000, token); // Wait for 1 second if there are no players
                    continue;
                }
                string chatter = chatters.Dequeue();

                int randomValue = random.Next(0, 2); // 0 or 1
                // Choose a random player to spawn the object next to from GameDirector.instance.PlayerList
                PlayerAvatar player = players[UnityEngine.Random.Range(0, players.Count)];
                Logger.LogInfo($"Chosen player: {player.playerName}");
                if ((!PencilConfig.BadThings) && randomValue == 1)
                {
                    randomValue = 0; // Force it to do a good thing
                }
                if ((!PencilConfig.GoodThings) && (!PencilConfig.BadThings))
                {
                    Logger.LogWarning("Both GoodThings and BadThings are disabled in the config. Exiting...");
                    break;
                }
                if (randomValue == 0)
                {
                    // Spawn a valuable
                    Logger.LogInfo("Spawning a valuable");
                    // Choose a random message from a list of messages
                    List<string> focusMessages = new List<string>(){
                    $"<color=#7DCC0B>Looks like <b>{chatter}</b> wants to help out!</color>",
                    $"<color=#7DCC0B>Seems like <b>{chatter}</b> is being nice!</color>",
                    $"<color=#7DCC0B>Looks like <b>{chatter}</b> is being generous!</color>",
                    $"<color=#7DCC0B><b>{chatter}</b> is being nice!</color>",
                    $"<color=#7DCC0B><b>{chatter}</b> sends some love!</color>",
                };
                    // Choose a random message from the list
                    string randomMessage = focusMessages[UnityEngine.Random.Range(0, focusMessages.Count)];
                    string hint = doGoodThingTo(player);
                    // Send the message to all players
                    NewChatterEvent.RaiseEvent(new MissionOptions()
                    {
                        msg = $"{randomMessage} {hint}",
                        color1 = Color.green,
                        color2 = Color.green,
                        time = 2f
                    }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
                else
                {
                    // Spawn an enemy
                    Logger.LogInfo("Spawning an enemy");
                    // Choose a random message from a list of messages
                    List<string> focusMessages = new List<string>()
                {
                    $"<color=#BB250B>Looks like <b>{chatter}</b> is causing trouble!</color>",
                    $"<color=#BB250B>Seems like <b>{chatter}</b> is being naughty!</color>",
                    $"<color=#BB250B>Looks like <b>{chatter}</b> is being mean!</color>",
                    $"<color=#BB250B><b>{chatter}</b> is being naughty!</color>",
                    $"<color=#BB250B><b>{chatter}</b> sends some trouble!</color>",
                };
                    // Choose a random message from the list
                    string randomMessage = focusMessages[UnityEngine.Random.Range(0, focusMessages.Count)];

                    string hint = doBadThingTo(player);



                    NewChatterEvent.RaiseEvent(new MissionOptions()
                    {
                        msg = $"{randomMessage} ({hint})",
                        color1 = Color.red,
                        color2 = Color.red,
                        time = 4f
                    }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);



                }
                // Delay for 4.5 seconds before processing the next chatter
                await Task.Delay(4500, token);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in GoThroughChatters: {ex}");
        }
    }
    private static void HandleChatterEvent(EventData eventData)
    {
        MissionOptions options = (MissionOptions)eventData.CustomData;
        MissionUI instance = MissionUI.instance;
        if (instance != null)
        {
            instance.MissionText(
                options.msg,
                options.color1,
                options.color2,
                options.time
            );
        }
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void OnDestroy()
    {
        cts.Cancel();

        if (listener.IsListening)
        {
            listener.Stop();
            listener.Close();
        }

        Unpatch();
    }

    private void HandleRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        Logger.LogInfo($"Received {request.HttpMethod} request");


        // GET Request
        if (request.HttpMethod == "GET")
        {
            // get ?username=chatter parameter
            string requestBody = request.QueryString["username"];
            // Respond back
            HttpListenerResponse response = context.Response;
            string responseString = "GET received";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            chatters.Enqueue(requestBody);
            Logger.LogInfo($"Received chatter: {requestBody}");
            Logger.LogInfo($"Chatters in queue: {chatters.Count}");
        }
        else
        {
            context.Response.StatusCode = 405;
            context.Response.Close();
        }
    }
    private void Update()
    {
        // Code that runs every frame goes here

    }
}