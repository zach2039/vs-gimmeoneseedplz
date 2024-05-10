Gimme One Seed Plz
=================

A server-side mod that keeps your sanity when looking for tree seeds from your favorite trees; trees will always drop at least one seed when felled.

Overview
--------

Changes include:

 * Trees always drop at least one seed when felling with an axe... That's it. That's the mod.


Compatibility
--------

 - Should be compatibile with [Wildcraft Trees](https://mods.vintagestory.at/wildcrafttree), and any other mod that uses "treeseed-{wood}" as an item for their trees.


Config Settings (`VintageStoryData/ModConfig/GimmeOneSeedPlz.json`)
--------

 * `PatchVanillaItemAxeOnBlockBrokenWith`: Enables or disables harmony patch that adds seed drop to vanilla axes (or modded axes that extend ItemAxe); defaults to `true`.

 * `PatchToolworksCollectibleBehaviorFellingOnBlockBrokenWith`: Enables or disables harmony patch that adds seed drop to Toolwork's axes; defaults to `true`.
 
 * `GuaranteedTreeSeedsOnFelledCount`: How many tree seeds to drop from a felled tree; defaults to `1`.

 * `MinRequiredBlocksBrokenOnFullFellCount`: How many blocks need to be broken to spawn a tree seed if the whole tree was felled; defaults to `3`.

 * `MinRequiredBlocksBrokenOnPartialFellCount`: How many blocks need to be broken to spawn a tree seed if the whole tree is not felled; defaults to `35`.


Future Plans
--------

 - None, atm.


Known Issues
--------

 - None, atm.


Extras
--------

 - Note that trees of less than `MinRequiredBlocksBrokenOnFullFellCount` blocks will not drop seeds still, even if fully felled.

 - If a tree is partially cut down, it will only drop a seed if the total number of blocks broken is greater than `MinRequiredBlocksBrokenOnPartialFellCount`.

 - Cool modding fact: This mod was made for my wife. She hates cutting trees and getting nothing for it.

