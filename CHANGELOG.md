# Changelog

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