using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ExitGames.Client.Photon;

using Photon.Realtime;
using REPOLib.Modules;
using RepoWebListener;
using UnityEngine;
using MissionUtils;

using static RepoWebListener.PencilUtils;
using static MissionUtils.MissionUtils;

namespace RepoWebListener;

class Events
{
    public class EAction
    {
        public string Name { get; set; }
        public List<string> TeaseMessages { get; set; }
        public bool IsForAll { get; set; }
        public PlayerAvatar Victim { get; set; } = null;

        public EType Type { get; set; } = EType.BAD;
        public bool IsForOnlyDeadPlayers { get; set; } = false;
        public bool IsForOnlyAlivePlayers { get; set; } = false;

        public Func<PlayerAvatar?, string[]> Action { get; set; }

        public EAction(string name, List<string> teaseMessages, bool isForAll, Func<PlayerAvatar?, string[]> action, bool isForOnlyDeadPlayers = false, bool isForOnlyAlivePlayers = false)
        {
            Name = name;
            TeaseMessages = teaseMessages;
            IsForAll = isForAll;
            Action = action;
            IsForOnlyDeadPlayers = isForOnlyDeadPlayers;
            IsForOnlyAlivePlayers = isForOnlyAlivePlayers;
        }

        public string[] Invoke(PlayerAvatar player = null)
        {
            if (IsForAll)
            {
                return Action.Invoke(null);
            }
            else
            {
                if (player == null)
                {
                    return ["Error: Player is null."];
                }
            }
            Victim = player;
            return Action.Invoke(player);
        }


    }
    public static List<EAction> PossibleBadActions = new List<EAction>() { };
    public static List<EAction> PossibleGoodActions = new List<EAction>() { };

    public enum EType
    {
        BAD,
        GOOD
    }
    public class Event
    {

        public EAction Action { get; set; }
        public string ChatterMessage { get; set; }
        public string Chatter { get; set; }

        public Event(EAction action, string chatterMessage, string chatter)
        {
            Action = action;
            ChatterMessage = chatterMessage;
            Chatter = chatter;
        }

        public static Event Generate(string chatter)
        {
            // No type? Pick a random one
            EType type = (EType)Randomizer.Next(0, Enum.GetValues(typeof(EType)).Length);
            // recursion loop check 
            return Generate(chatter, type);
        }
        public static Event Generate(string chatter, EType type)
        {

            EAction action = type == EType.BAD ?
            PossibleBadActions[Randomizer.Next(PossibleBadActions.Count)] :
            PossibleGoodActions[Randomizer.Next(PossibleGoodActions.Count)];

            action.Type = type;
            string chatterMessage = $"{chatter}, you rolled a {action.Name} event! It has been added to the queue.";
            return new Event(action, chatterMessage, chatter);
        }

        public string GenerateTeaseMessage(string template, string[] args, PlayerAvatar? victim = null)
        {
            string message = template;
            message = message.Replace("%chatter%", Chatter);
            message = message.Replace("%victim%", victim.playerName);
            foreach (string item in args)
            {
                message = message.Replace("%" + args.ToList().IndexOf(item) + "%", item);
            }
            return message;
        }

        public string GenerateTeaseMessage(string template, string[] args)
        {
            string message = template;
            message = message.Replace("%chatter%", Chatter);
            foreach (string item in args)
            {
                message = message.Replace("%" + args.ToList().IndexOf(item) + "%", item);
            }
            return message;
        }

        public string Execute(PlayerAvatar player)
        {
            if (this.Action.IsForAll)
            {
                return "Error occurred: Action is for all players, but a specific player was provided.";
            }
            string[] result = Action.Invoke(player);
            string message = Action.TeaseMessages[Randomizer.Next(Action.TeaseMessages.Count)];
            return GenerateTeaseMessage(message, result, player);
        }

        public string Execute()
        {
            if (!this.Action.IsForAll)
            {
                return "Error occurred: Action is for a specific player, but no player was provided.";
            }
            string[] result = Action.Invoke();
            string message = Action.TeaseMessages[Randomizer.Next(Action.TeaseMessages.Count)];
            return GenerateTeaseMessage(message, result);
        }

        public override string ToString()
        {
            return $"Event: {Action.Name} ({(Action.IsForAll ? "All" : Action.Victim)}), Chatter: {Chatter}, ChatterMessage: {ChatterMessage}";
        }
    }

    public static void Init()
    {
        // Bad Events
        if (PencilConfig.BadEventDamageSpecific)
        {
            PossibleBadActions.Add(new EAction("Deal Damage", new List<string>() {
            "<b>%chatter%</b> slaps <b>%victim%</b> across the face for <b>-%0%</b> HP!",
            "<b>%chatter%</b> throws a rock at <b>%victim%</b> for <b>-%0%</b> HP!",
            "<b>%victim%</b> couldn't avoid <b>%chatter%</b>'s Ford F150 and took <b>-%0%</b> HP!",
            "<b>%chatter%</b> wanted to play catch with <b>%victim%</b>, but threw the ball too hard and hit them for <b>-%0%</b> HP!",
             }, false, (player) =>
        {
            if (player == null)
            {
                return ["???"];
            }
            int amount = Randomizer.Next(PencilConfig.BadEventDamageMinAmount, PencilConfig.BadEventDamageMaxAmount);
            int originalAmount = amount;
            if (!PencilConfig.BadEventDamageCanKill && player.playerHealth.health - amount < 1)
            {
                if (player.playerHealth.health > 1)
                {
                    amount = player.playerHealth.health - 1;
                }
                else
                {
                    amount = 0;
                }
            }
            player.playerHealth.HurtOther(amount, player.playerHealth.transform.position, false, -1);
            return [originalAmount.ToString()];
        }, isForOnlyAlivePlayers: true));
        }

        if (PencilConfig.BadEventDamageAll)
        { // Deal damage to all players

            PossibleBadActions.Add(new EAction("Deal Damage to All", new List<string>()
            {
            "<b>%chatter%</b> threw a grenade and hurt everyone, with <b>%0%</b> getting hurt the most for <b>-%1%</b> HP!",
            "<b>%chatter%</b> bombed the whole facility, with <b>%0%</b> getting hurt the most for <b>-%1%</b> HP!",
            "Everyone lost a lot of health because of <b>%chatter%</b>, but <b>%0%</b> lost the most with <b>-%1%</b> HP!",
            }, true, (_) =>
            {
                PlayerAvatar playerWithMostDamage = null;
                int mostDamage = 0;
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    if (!playerWithMostDamage)
                    {
                        playerWithMostDamage = item;
                    }
                    int amount = Randomizer.Next(PencilConfig.BadEventDamageMinAmount, PencilConfig.BadEventDamageMaxAmount);
                    if (amount > mostDamage)
                    {
                        mostDamage = amount;
                        playerWithMostDamage = item;
                    }
                    if (!PencilConfig.BadEventDamageCanKill && item.playerHealth.health - amount < 1)
                    {
                        if (item.playerHealth.health > 1)
                        {
                            amount = item.playerHealth.health - 1;
                        }
                        else
                        {
                            amount = 0;
                        }
                    }
                    item.playerHealth.HurtOther(amount, item.playerHealth.transform.position, false, -1);
                }
                if (playerWithMostDamage == null)
                {
                    return ["???", mostDamage.ToString()];
                }
                return [playerWithMostDamage.playerName, mostDamage.ToString()];
            }, isForOnlyAlivePlayers: true));


        }

        if (PencilConfig.BadEventSpawnRandomEnemy)
        { // Spawn a random enemy
            PossibleBadActions.Add(new EAction("Spawn Random Enemy", new List<string>() {
            "<b>%chatter%</b> summoned a <b>%0%</b> to attack <b>%victim%</b>!",
            "<b>%chatter%</b> called for help and a <b>%0%</b> appeared!",
            "<b>%chatter%</b> told a <b>%0%</b> that <b>%victim%</b> said something bad their mom!",
            "<b>%chatter%</b> threw a rock at a <b>%0%</b> and it thought <b>%victim%</b> did it!" }, false, (player) =>
            {
                string randomEnemy = RepoWebListener.AllowedEnemies.Keys.ElementAt(Randomizer.Next(RepoWebListener.AllowedEnemies.Count));
                EnemySetup enemySetup = RepoWebListener.AllowedEnemies[randomEnemy];
                if (enemySetup == null)
                {
                    RepoWebListener.Logger.LogError($"Enemy {randomEnemy} not found. Cannot spawn.");
                    return ["???"];
                }
                Enemies.SpawnEnemy(enemySetup, player.transform.position + player.transform.up * 0.2f, Quaternion.identity, false);
                return [Dictionaries.EnemyPaths[randomEnemy]];
            }, isForOnlyAlivePlayers: true));
        }

        // Good Events
        if (PencilConfig.GoodEventHealAll)
        {
            PossibleGoodActions.Add(new EAction("Heal All", new List<string>() {
            "<b>%chatter%</b> healed everyone a bit, but <b>%0%</b> got healed the most with +<b>%1%</b> HP!",
            "<b>%chatter%</b> used a splash potion, healing everyone with <b>%0%</b> getting healed the most! (+<b>%1%</b> HP)",
            "<b>%chatter%</b> used a health pack on <b>%0%</b> for +<b>%1%</b> HP, but it had a ripple effect on everyone else!",
            }, true, (_) =>
            {
                PlayerAvatar playerWithMostHealth = null;
                int mostHealth = 0;
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    if (!playerWithMostHealth)
                    {
                        playerWithMostHealth = item;
                    }
                    int amount = Randomizer.Next(PencilConfig.GoodEventHealMinAmount, PencilConfig.GoodEventHealMaxAmount);
                    if (amount > mostHealth)
                    {
                        mostHealth = amount;
                        playerWithMostHealth = item;
                    }
                    item.playerHealth.HealOther(amount, true);
                }
                if (playerWithMostHealth == null)
                {
                    return ["???", mostHealth.ToString()];
                }
                return [playerWithMostHealth.playerName, mostHealth.ToString()];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventHealSpecific)
        {
            PossibleGoodActions.Add(new EAction("Heal Specific", new List<string>() {
            "<b>%chatter%</b> healed <b>%victim%</b> for +<b>%0%</b> HP!",
            "<b>%chatter%</b> used a health pack on <b>%victim%</b> for +<b>%0%</b> HP!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                int amount = Randomizer.Next(PencilConfig.GoodEventHealMinAmount, PencilConfig.GoodEventHealMaxAmount);
                player.playerHealth.HealOther(amount, true);
                return [amount.ToString()];
            }));
        }
        if (PencilConfig.GoodEventSpawnRandomItem && GetAllowedItems().Count > 0)
        { // Spawn a random item
            PossibleGoodActions.Add(new EAction("Spawn Random Item", new List<string>() {
            "<b>%chatter%</b> bought a <b>%0%</b> for <b>%victim%</b>!",
           }, false, (player) =>
            {
                if (player == null)
                {
                    RepoWebListener.Logger.LogError("Player is null. Cannot spawn item.");
                    return ["???"];
                }
                string randomItem = RepoWebListener.AllowedItems.Keys.ElementAt(Randomizer.Next(RepoWebListener.AllowedItems.Count));
                Item item = Items.GetItemThatContainsName(randomItem);
                if (item == null)
                {
                    RepoWebListener.Logger.LogError($"Item {randomItem} not found out of {Items.GetItems().Count} items. Cannot spawn.");
                    // Log the first item for debugging
                    RepoWebListener.Logger.LogError($"First item: {Items.GetItems().FirstOrDefault()?.name}");
                    return ["???"];
                }
                Items.SpawnItem(item, player.transform.position + player.transform.up * 0.2f, Quaternion.identity);
                return [randomItem.Replace("Item ", "")];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventSpawnRandomValuable && GetAllowedValuables().Count > 0)
        { // Spawn a random item
            PossibleGoodActions.Add(new EAction("Spawn Random Valuable", new List<string>() {
            "<b>%chatter%</b> found a <b>%0%</b> and put it next to <b>%victim%</b>!",
           }, false, (player) =>
            {
                if (player == null)
                {
                    RepoWebListener.Logger.LogError("Player is null. Cannot spawn item.");

                    return ["???"];
                }
                string randomItem = RepoWebListener.AllowedValuables.Keys.ElementAt(Randomizer.Next(RepoWebListener.AllowedValuables.Count));
                ValuableObject valuable = Valuables.GetValuableThatContainsName(randomItem);
                if (valuable == null)
                {
                    RepoWebListener.Logger.LogError($"Valuable {randomItem} not found out of {Valuables.GetValuables().Count} valuables. Cannot spawn.");
                    // Log the first valueable for debugging
                    RepoWebListener.Logger.LogError($"First valuable: {Valuables.GetValuables().FirstOrDefault()?.name}");
                    return ["???"];
                }
                Valuables.SpawnValuable(valuable, player.transform.position + player.transform.up * 0.2f, Quaternion.identity);
                return [randomItem.Replace("Valuable ", "")];
            }, isForOnlyAlivePlayers: true));
        }
        PunManager punManager = PunManager.instance;
        if (PencilConfig.GoodEventUpgradeAllEnergy)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Energy", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's energy!",
            "<b>%chatter%</b> used a power crystal to upgrade everyone's energy!",
            "Everyone feels more energetic thanks to <b>%chatter%</b>!",
            "<b>%chatter%</b> bought an energy drink and shared it with everyone!",
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradePlayerEnergy(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificEnergy)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Energy", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s energy!",
            "<b>%chatter%</b> used a power crystal to upgrade <b>%victim%</b>'s energy!",
            "<b>%victim%</b> feels more energetic thanks to <b>%chatter%</b>!",
            "<b>%chatter%</b> bought an energy drink and shared it with <b>%victim%</b>!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradePlayerEnergy(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeAllHealth)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Health", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's health!",
            "Everyone feels healthier thanks to <b>%chatter%</b>!",
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradePlayerHealth(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificHealth)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Health", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s health!",
            "<b>%victim%</b> feels healthier thanks to <b>%chatter%</b>!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradePlayerHealth(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeAllGrabStrength)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Grab Strength", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's grab strength!",
            "Everyone feels stronger thanks to <b>%chatter%</b>!",
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradePlayerGrabStrength(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificGrabStrength)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Grab Strength", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s grab strength!",
            "<b>%victim%</b> feels stronger thanks to <b>%chatter%</b>!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradePlayerGrabStrength(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeAllRange)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Range", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's grab range!",
            "Everyone feels more agile thanks to <b>%chatter%</b>!",
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradePlayerGrabRange(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificRange)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Range", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s grab range!",
            "<b>%victim%</b> feels more agile thanks to <b>%chatter%</b>!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradePlayerGrabRange(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeAllExtraJump)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Extra Jump", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's extra jump!",
            "Everyone feels more jumpy thanks to <b>%chatter%</b>!",
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradePlayerExtraJump(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificExtraJump)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Extra Jump", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s extra jump!",
            "<b>%victim%</b> feels more jumpy thanks to <b>%chatter%</b>!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradePlayerExtraJump(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeAllSpeed)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Speed", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's speed!",
            "Everyone feels faster thanks to <b>%chatter%</b>!"
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradePlayerSprintSpeed(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificSpeed)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Speed", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s speed!",
            "<b>%victim%</b> feels faster thanks to <b>%chatter%</b>!",
            "<b>%chatter%</b> gave <b>%victim%</b> a speed boost!",
            "<b>%chatter%</b> is running CIRCLES around y'all!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradePlayerSprintSpeed(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeAllTumbleLaunch)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Tumble Launch", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's tumble launch!",
            "Everyone has stronger foreheads thanks to <b>%chatter%</b>!",
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradePlayerTumbleLaunch(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificTumbleLaunch)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Tumble Launch", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s tumble launch!",
            "<b>%victim%</b> has a stronger forehead thanks to <b>%chatter%</b>!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradePlayerTumbleLaunch(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeAllMapPlayerCount)
        {
            PossibleGoodActions.Add(new EAction("Upgrade All Map Player Count", new List<string>() {
            "<b>%chatter%</b> upgraded everyone's map!",
            "<b>%chatter%</b> wanted to make sure everyone can see each other!",
            "<b>%chatter%</b> wants everyone to press TAB!"
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetAlivePlayers())
                {
                    punManager.UpgradeMapPlayerCount(item.steamID);
                }
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventUpgradeSpecificMapPlayerCount)
        {
            PossibleGoodActions.Add(new EAction("Upgrade Specific Map Player Count", new List<string>() {
            "<b>%chatter%</b> upgraded <b>%victim%</b>'s map!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                punManager.UpgradeMapPlayerCount(player.steamID);
                return [];
            }, isForOnlyAlivePlayers: true));
        }
        if (PencilConfig.GoodEventReviveAll && (GetDeadPlayers().Count > 0))
        {
            PossibleGoodActions.Add(new EAction("Revive All", new List<string>() {
            "<b>%chatter%</b> revived everyone!",
            "Everyone is back to life thanks to <b>%chatter%</b>!",
            }, true, (_) =>
            {
                foreach (PlayerAvatar item in GetDeadPlayers())
                {
                    item.Revive();
                }
                return [];
            }, isForOnlyDeadPlayers: true));
        }
        if (PencilConfig.GoodEventReviveSpecific && (GetDeadPlayers().Count > 0))
        {
            PossibleGoodActions.Add(new EAction("Revive Specific", new List<string>() {
            "<b>%chatter%</b> revived <b>%victim%</b>!",
            "<b>%victim%</b> is back to life thanks to <b>%chatter%</b>!",
            "<b>%chatter%</b> used a revive potion on <b>%victim%</b>!",
            "<b>%chatter%</b> brought <b>%victim%</b> back to life!",
            "<b>%chatter%</b> used a defibrillator on <b>%victim%</b>!",
            "<b>%chatter%</b> brought <b>%victim%</b> back from the dead!",
            }, false, (player) =>
            {
                if (player == null)
                {
                    return ["???"];
                }
                player.Revive();
                return [];
            }, isForOnlyDeadPlayers: true));
        }




    }

    public static Queue<Event> EventQueue = new Queue<Event>();

    public static string AddEventToQueueFrom(string chatter)
    {
        // Choose a random event from the list
        Event randomEvent = Event.Generate(chatter);
        EventQueue.Enqueue(randomEvent);
        RepoWebListener.Logger.LogInfo($"Event added to queue: {randomEvent}");
        // Tell the chatter about it
        return randomEvent.ChatterMessage;
    }
    public static void RunNextEvent()
    {
        // Check if there are any events in the queue
        if (EventQueue.Count == 0)
        {
            return;
        }
        // Get the next event
        Event randomEvent = EventQueue.Dequeue();
        // Do the action in another thread, so we can still keep going

        string resultMessageToShow;
        if (!randomEvent.Action.IsForAll)
        {
            Func<List<PlayerAvatar>> playerGetterToUse;
            if (randomEvent.Action.IsForOnlyDeadPlayers)
            {
                playerGetterToUse = GetDeadPlayers;
            }
            else if (randomEvent.Action.IsForOnlyAlivePlayers)
            {
                playerGetterToUse = GetAlivePlayers;
            }
            else
            {
                playerGetterToUse = GetAllPlayers;
            }
            List<PlayerAvatar> elligiblePlayers = playerGetterToUse();
            if (elligiblePlayers.Count == 0)
            {
                return;
            }
            // Choose a random player to do the action on
            PlayerAvatar player = playerGetterToUse()[Randomizer.Next(GetAlivePlayers().Count)];
            // Execute the action
            resultMessageToShow = randomEvent.Execute(player);
        }
        else
        {
            // Execute the action
            resultMessageToShow = randomEvent.Execute();
        }
        // Log the result
        RepoWebListener.Logger.LogInfo($"Event executed: {randomEvent.Action.Name} ({(randomEvent.Action.IsForAll ? "All" : randomEvent.Action.Victim)})");
        RepoWebListener.Logger.LogInfo($"Result: {resultMessageToShow}");
        MissionOptions missionOptions = MissionOptions.Create(
            randomEvent.Action.Type == EType.BAD ? "<color=#CC250B>" : "<color=#7DCC0B>" +
            resultMessageToShow +
            "</color>",
            Color.white,
            Color.white,
            PencilConfig.MinimumTimeBetweenEvents - (PencilConfig.MinimumTimeBetweenEvents / 4)
        );
        // Show the result message to all players
        MissionUI.instance.MissionText("%broadcast%" + (randomEvent.Action.Type == EType.BAD ? "<color=#CC250B>" : "<color=#7DCC0B>") +
            resultMessageToShow +
            "</color>",
            Color.white,
            Color.white,
            PencilConfig.MinimumTimeBetweenEvents - (PencilConfig.MinimumTimeBetweenEvents / 4));



    }
}