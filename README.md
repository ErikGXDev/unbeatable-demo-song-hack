# The "Unbeatable Song Hack"

Most likely the first mod menu for the Unbeatable DEMO.

### Usage

To use, download MelonLoader ( https://melonwiki.xyz/ ) and put a .dll of this mod into the Mods folder. A GUI should pop up.

#### Custom Maps

To add custom maps to the game, download maps (for example off Taco's unbeatable beatmap server) and put them into a `CustomMaps` folder, which is located in the same directory as the game executable (If it's not there you might have to create the server yourself).

You can then play them in the Arcade Mode. Filter by the "local" category in the Arcade to find all local maps easily.

---

### Developing this

If you want to work on this, you should most likely replace all the `<HintPaths>` in `UnbeatableSongHack.csproj` with your paths to the game, as well as the `<Target><Exec>` at the bottom of that file.
