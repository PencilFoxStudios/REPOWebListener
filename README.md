![Repo Web Listener Logo, with a Pencil Fox Face](https://raw.githubusercontent.com/PencilFoxStudios/REPOWebListener/main/RepoWebListenerBanner.png)
A highly-customizable mod that lets you integrate anything that supports sending web requests with R.E.P.O! Currently in beta, so expect bugs and missing features.

I originally created this for use with my Twitch streams + MixItUp to make events happen within our multiplayer game whenever someone cheered bits, but I'd imagine it has other use cases. 

Check out the config file it generates for options!

## Trigger
Any GET request to the endpoint hosted by this mod (``http://localhost:7390?username=<optional_reason_for_event>`` by default) will trigger one of the following events at random. You can turn each one on or off in the configuration.

## Deciding the Event
The event that is triggered is first decided by a 50/50 coin flip of good or bad. If the event is good, it will then randomly select one of the good events. If the event is bad, it will randomly select one of the bad events.
## Events
### Good
#### General
- Heal everyone by a random configurable amount
- Heal a random player by a random configurable amount
- Revive everyone
- Revive a specific player
#### Upgrades
- Upgrade everyone's stamina
- Upgrade a specific player's stamina
- Upgrade everyone's max health
- Upgrade a specific player's max health
- Upgrade everyone's grab strength
- Upgrade a specific player's grab strength
- Upgrade everyone's grab range
- Upgrade a specific player's grab range
- Upgrade everyone's speed
- Upgrade a specific player's speed
- Give everyone an extra jump
- Give a specific player an extra jump
- Upgrade everyone's tumble launch
- Upgrade a specific player's tumble launch
- Upgrade everyone's map player count
- Upgrade a specific player's map player count
#### Spawning
- Spawn a random item next to a random player (configurable whitelist)
- Spawn a random valueable next to a random player (configurable whitelist)
### Bad
#### General
- Damage everyone by a random configurable amount
- Damage a random player by a random configurable amount 
#### Spawning
- Spawn a random enemy next to a random player (configurable whitelist)
___
There is much more to come! If you have any suggestions, feel free to reach out to me on [Discord](https://discord.gg/yip)!
<sub>Logo by [N3onAssassin](https://bsky.app/profile/n3onassassin.bsky.social)</sub>
<br>
<sub>© Pencil Fox Studios SP</sub>