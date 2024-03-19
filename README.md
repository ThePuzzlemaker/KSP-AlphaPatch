# AlphaPatch for KSP 1.12

This patch fixes out-of-order loading of assemblies and config files, causing issues with ModuleManager. This fix creates the proper behaviour on all filesystems, including those that do not alphabetically sort directory listings, such as BTRFS and FAT32.

Please submit [issues][iss] and [patches][patch] through [SourceHut](https://sr.ht/~thepuzzlemaker/KSP-AlphaPatch).

[iss]: https://todo.sr.ht/~thepuzzlemaker/KSP-AlphaPatch
[patch]: https://lists.sr.ht/~thepuzzlemaker/misc-projects

# Installation (Windows)

1. Download [BepInEx v5.4.22](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22) from its GitHub page, specifically `BepInEx_x64_5.4.22.0.zip`. Extract only `winhttp.dll`. **Do not extract the BepInEx folder, or `doorstop_config.ini`.
2. Download `doorstop_config.ini`.
3. Download (or build) `AlphaPatch.exe` file and place it in the root KSP directory.
4. Ensure [Harmony](https://github.com/KSPModdingLibs/HarmonyKSP) is installed. This is a common dependency of other mods, so it likely is already.
4. Run the game as normal.

# Installation (Linux, MacOS, etc.)

> NOTE: This setup is currently untested. Let me know if it gives you any issues, but it *should* work.

1. Download [BepInEx v5.4.22](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22) from its GitHub page, specifically `BepInEx_unix_5.4.22.0.zip`. Extract only the `doorstop_libs` folder. **Do not extract the BepInEx folder, or `run_bepinex.sh`. 
2. Download `run_bepinex.sh`
3. Navigate to your KSP directory in the terminal and run `chmod +x run_bepinex.sh`.
4. Download (or build) `AlphaPatch.exe` file and place it in the root KSP directory.
5. Edit the line `executable_name="NAMEHERE"`, replacing `NAMEHERE` with the executable name. On Linux this is typically `KSP.x86_64`. (I don't know what it is on MacOS, let me know if you do.) 
6. Ensure [Harmony](https://github.com/KSPModdingLibs/HarmonyKSP) is installed. This is a common dependency of other mods, so it likely is already.
7. Run the game with `run_bepinex.sh`. In CKAN you can set the command like to be `./run_bepinex.sh -singleinstance` (and any custom command line arguments as you wish).

## Why BepInEx?

This project uses BepInEx because KSP's stock modding facilities do not allow the DLL to be loaded early enough to stop out-of-order patching.

BepInEx's "doorstop" patch allows AlphaPatch to be loaded before anything in KSP, allowing it to patch the file loading code before it can load files out of order.

# License

This overall source tree is based on [HarmonyKSP][1], licensed under MIT.

This project is licensed under the MIT license.

[1]: https://github.com/KSPModdingLibs/HarmonyKSP/