Gimme One Seed Plz
=================

A server-side mod that preserves your sanity when looking for tree seeds from your favorite trees; trees will always drop at least one seed when felled.

Overview
--------

Changes include:

 * Trees always drop at least one seed when felling with an axe... That's it. That's the mod.

 * Fixes a bug in vanilla related to Crimson Maple leaves not being counted as leaves during tree felling.


Compatibility
--------

 - Should be compatibile with [Wildcraft Trees](https://mods.vintagestory.at/wildcrafttree), and any other mod that uses "treeseed-{wood}" as an item for their trees.


Config Settings (`VintageStoryData/ModConfig/GimmeOneSeedPlz.json`)
--------

 * `PatchVanillaItemAxeOnBlockBrokenWith`: Enables or disables harmony patch that adds seed drop to vanilla axes (or modded axes that extend ItemAxe); defaults to `true`.

 * `GuaranteedTreeSeedsOnFelledCount`: How many tree seeds to drop from a felled tree; defaults to `1`.

 * `MinRequiredBlocksBrokenOnFullFellCount`: How many blocks need to be broken to spawn a tree seed if the whole tree was felled; defaults to `3`.

 * `MinRequiredBlocksBrokenOnPartialFellCount`: How many blocks need to be broken to spawn a tree seed if the whole tree is not felled; defaults to `35`.
 
 * `UseAvgVarDropSettings`: Enables or disables using `TreeSeedDropAvg` and `TreeSeedDropVar` to configure drop chances for seeds rather than `GuaranteedTreeSeedsOnFelledCount`; defaults to `false`.
 
 * `TreeSeedDropAvg`: Sets the average quantity of seed drops; defaults to `0.5`.
 
 * `TreeSeedDropVar`: Sets the variance for quantity of seed drops; defaults to `6.0`.
 

Known Issues
--------

 - ~~Incompatibile with In Dappled Groves for now, due to custom felling behavior not implementing OnBlockBrokenWith.~~ Patching now will work with IDG v2.0.5; thanks to VinterNacht for the help!


Extras
--------

 - Note that trees of less than `MinRequiredBlocksBrokenOnFullFellCount` blocks will not drop seeds still, even if fully felled.

 - If a tree is partially cut down, it will only drop a seed if the total number of blocks broken is greater than `MinRequiredBlocksBrokenOnPartialFellCount`.

 - Cool modding fact: This mod was made for my wife. She hates cutting trees and getting nothing for it.

