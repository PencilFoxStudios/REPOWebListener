# Changelog

## B1.0.8 (5/27/2025)
- **Hotfix:** Fixed ``manifest.json`` to include the correct version number for [MissionUtils](https://thunderstore.io/c/repo/p/PencilFoxStudios/MissionUtils/).

## B1.0.7 (5/22/2025)

- **New Dependency**: Added a new dependency mod, [MissionUtils](https://thunderstore.io/c/repo/p/PencilFoxStudios/MissionUtils/).
- **Improvement**: Replaced the way events are announced in the **FOCUS** UI. It also allows for dead players to see the event messages.
- **New Config Option**: Added `Events.General.DeadPlayersSeeEvents` option to allow dead players to see the event messages, which is `true` by default.
- **Tweak**: Made the event messages call enemies by their in-game name instead of their internal name. Nobody knows what a "Slow Mouth" is, apparently.
- **Fix**: Addressed an issue where events would not trigger in the Arena level, regardless of the config settings.
- **Fix**: Improved stability by realizing that apparently Unity hates multi-threading in relation to their API objects, and thus removed the multi-threading from the event system. This should prevent crashes and other issues related to threading. Nobody told me. This is really all just trial and error.
- **Fix**: Fixed an issue that caused the game to miss the first event after an extraction point was reached/the vanilla message was sent. There is now a queue system thanks to the new dependency.
- **Fix**: Fixed an issue where the valuable dictionary was not being properly defined, causing the game to throw an error when trying to spawn valuables. The valuables should now spawn correctly.
- **Fix**: Fixed an issue where the item dictionary was not being properly defined, causing the game to throw an error when trying to spawn items. The items should now spawn correctly.
- **Fix**: Fixed the logo overlapping the border a bit.

## B1.0.6 (5/21/2025)

- **New Feature**: The plugin will now respond to the GET requests with a message indicating that the event was triggered, as well as the event that was triggered. This would be useful for chatbots and other integrations that want to know what event was triggered.
- **New Config Option**: Added `Events.General.MinimumTimeBetweenEvents` option to allow the user to set a minimum time between events. This is useful for preventing run-ending spam. The hard-coded minimum is 3 seconds, because I don't want semiwork to get sad.
- **Tweak**: Changed the way event messages are generated, and added a few more messages. The messages are now more unique and varied, and should be more fun to read. Let me know if you have any suggestions for more messages!
- **Tweak**: Changed the way items are spawned.
- **Tweak**: Changed the way valuables are spawned.
- **Fix**: Fixed a group of bugs that would cause the game to keep the ``Events.General.GoodEvents`` and ``Events.General.BadEvents`` at ``false``, even if there existed respective events that were set to ``true``. 
- **Fix**: Fixed a bug with the revive events that would try to revive players that were already alive. This would cause the game to raise a warning and not revive anyone.
- **Housekeeping**: Refactored the dictionaries used to store valueables, items, and enemies into their own file. This should make it easier to add new items and enemies in the future.
- **Housekeeping**: Refactored the event logic into its own file. This should make it easier to add new events in the future.

## B1.0.5 (5/20/2025)

- **Feature**: Included the rest of R.E.P.O.'s enemies in the list of possible enemies to spawn (and their respective configs). If a certain enemy causes issues, it may be temporarily removed until a fix is found.
- **Tweak**: Modified how RWL registered enemies. Now uses a patch of ``RunManager``'s ``ChangeLevel`` method (see [RunManagerPatch.cs](https://github.com/PencilFoxStudios/REPOWebListener/blob/main/RunManagerPatch.cs) if you're interested). If this goes well, expect to see items and valuables *fully* registered in the same way in the future (right now they were kinda just thrown in there).
- **Fix**: Fixed enemy spawn network desync issue. Enemy spawning is still wonky, but expect it to be more reliable with each update.
- **Fix**: Fixed a bug that ignored the ``Bad Events.Damage.BadEventDamageCanKill`` config option and killed players regardless of the setting if the event was for all players. That was rude.
- **Fix**: Fixed a bug that caused dead players to recieve healing and upgrade events.
- Excluded ``RepoWebListener.deps.json`` from the ZIP release. This file is not needed for the plugin to run and was included by mistake. If you have it, you can safely delete it.
- Clarified ``README.md`` to include what determines the event that is triggered.

## B1.0.4 (5/19/2025)

- **New Event**: Revive All Players
- **New Event**: Revive Specific Player
- **New Config Option**: Added `Good Events.Healing.GoodEventReviveAll` option to allow/restrict the use of the Revive All Players event.
- **New Config Option**: Added `Good Events.Healing.GoodEventReviveSpecificPlayer` option to allow/restrict the use of the Revive Specific Player event.
- **New Config Option**: Added `Bad Events.Damage.BadEventDamageCanKill` option to allow/prevent killing players with damage events.
- Added ``CHANGELOG.md`` to the repository.
- Updated ``README.md`` to include the new events.

## B1.0.3 (5/18/2025)

- Added ``README.md`` to the repository.
- Added proper credit to the RWL logo creator, [N3onAssassin](https://bsky.app/profile/n3onassassin.bsky.social).

## B1.0.0 (5/18/2025)

Initial release of the RepoWebListener plugin for R.E.P.O.