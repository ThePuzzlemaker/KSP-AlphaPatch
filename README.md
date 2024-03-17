# AlphaPatch for KSP 1.12

This patch fixes out-of-order loading of assemblies and config files, causing issues with ModuleManager. This fix creates the proper behaviour on all filesystems, including those that do not alphabetically sort directory listings, such as BTRFS and FAT32.

Please submit [issues][iss] and [patches][patch] through [SourceHut](https://sr.ht/~thepuzzlemaker/KSP-AlphaPatch).

[iss]: https://todo.sr.ht/~thepuzzlemaker/KSP-AlphaPatch
[patch]: https://lists.sr.ht/~thepuzzlemaker/misc-projects

# Installation

1. Download [BepInEx v5.4.22](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22) from its GitHub page. Extract into the KSP directory. Ensure that `winhttp.dll` is replaced.
2. Start the game, then close it. It does not need to load all the way, only to the point where the Squad logo shows.
3. Download (or build) the `AlphaPatch.dll` file and place it in `BepInEx/patchers` directory.

## Why BepInEx?

This project uses BepInEx because KSP's stock modding facilities do not allow the DLL to be loaded early enough to stop out-of-order patching.

BepInEx's preloader ability allows AlphaPatch to be loaded before anything in KSP, allowing it to patch the file loading code before it can load files out of order.

# License

This overall source tree is based on [HarmonyKSP][1], licensed under MIT.

This project is licensed under the MIT license.

[1]: https://github.com/KSPModdingLibs/HarmonyKSP/