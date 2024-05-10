using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace GimmeOneSeedPlz.ModPatches
{
	[HarmonyPatchCategory("GimmeOneSeedPlz_ItemAxe")]
	[HarmonyPatch(typeof(ItemAxe), "OnBlockBrokenWith")]
	static class Patch_ItemAxe_OnBlockBrokenWith
	{
		static bool Prefix(ItemAxe __instance, ref Tuple<Block, Stack<BlockPos>> __state, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier)
		{
			if (world.Side.IsServer())
			{
				// Save first tree block to be used later
				int num;
				int woodTier;
				Stack<BlockPos> foundPositions = __instance.FindTree(world, blockSel.Position, out num, out woodTier);

				if (foundPositions.Count == 0)
				{
					return true;
				}

				BlockPos firstPos = blockSel.Position;

				if (firstPos != null)
				{
					__state = new Tuple<Block, Stack<BlockPos>>(world.BlockAccessor.GetBlock(firstPos), foundPositions);
				}
				else
				{
					__state = null;
				}
			}
			
			return true; // continue with original method
		}

		static void Postfix(ItemAxe __instance, ref Tuple<Block, Stack<BlockPos>> __state, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier)
		{
			// Log itemcode will be log-{type}-{wood}-{rotation}
			// Tree seed itemcode will be treeseed-{wood}

			if (world.Side.IsServer())
			{
                if (__state == null)
                {
                    return;
                }

				// Positions remaining is 0, I am 99% sure a tree was fully felled, so have at it!
				if (__state.Item1 != null && __state.Item2 != null)
				{
                    int num;
    				int woodTier;
                    Stack<BlockPos> foundPositions = __instance.FindTree(world, blockSel.Position, out num, out woodTier);

                    // Check if we can find a tree now; if not, then it is considered felled
                    // If we can find a tree, compare the counts of the blocks, and drop if felled block count greater than 35.
                    if (foundPositions.Count != 0)
                    {
						if (__state.Item2.Count - foundPositions.Count < 35)
						{
							return;
						}
                    }

					Block woodBlock = __state.Item1;

					string domain = woodBlock.Code.Domain;

					// Block better be a log
					if (woodBlock.Code.BeginsWith(domain, "log"))
					{
						string woodtype = woodBlock.Variant["wood"];

						Item seedItem = world.SearchItems(new AssetLocation(domain, "treeseed-" + woodtype)).FirstOrDefault<Item>();

						// Drop some stuff if we found the seed
						if (seedItem != null)
						{
							IPlayer byPlayer = null;
							if (byEntity is EntityPlayer)
							{
								byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
							}

							ItemStack seedItemStack = new ItemStack(seedItem, GimmeOneSeedPlzConfig.Loaded.GuaranteedTreeSeedsOnFelledCount);
							GimmeOneSeedPlzModSystem.DropItemStack(world, blockSel.Position, byPlayer, seedItemStack);
						}	
						else
						{
							world.Api.Logger.Warning("[GimmeOneSeedPlz] Could not find tree seed for block " + woodBlock.Code.ToString());
						}
					}
				}
			}
		}
	}
}