using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Toolworks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace GimmeOneSeedPlz.ModPatches
{
	[HarmonyPatchCategory("GimmeOneSeedPlz_CollectibleBehaviorFelling")]
	[HarmonyPatch(typeof(CollectibleBehaviorFelling), "OnBlockBrokenWith")]
	public class Patch_CollectibleBehaviorFelling_OnBlockBrokenWith
	{
		static bool Prefix(CollectibleBehaviorFelling __instance, ref GimmeOneSeedPlzModSystem.FelledTreeData __state, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier, ref EnumHandling handled)
		{
			if (world.Side.IsServer())
			{
				// Save first tree block to be used later
				int num;
				int woodTier;
				Stack<BlockPos> foundPositions = __instance.FindTree(world, blockSel.Position, out num, out woodTier);

				if (foundPositions.Count < GimmeOneSeedPlzConfig.Loaded.MinRequiredBlocksBrokenOnFullFellCount)
				{
					return true;
				}

				BlockPos firstPos = blockSel.Position;
				
				if (firstPos != null)
				{
					BlockLeaves blockLeaves = GimmeOneSeedPlzModSystem.GetLeavesFromTreeStack(world, foundPositions);
					__state = new GimmeOneSeedPlzModSystem.FelledTreeData(world.BlockAccessor.GetBlock(firstPos), (foundPositions != null) ? foundPositions.Count : -1, blockLeaves);
				}
				else
				{
					__state = new GimmeOneSeedPlzModSystem.FelledTreeData(null, -1, null);
				}
			}

			return true; // continue with original method
		}

		static void Postfix(CollectibleBehaviorFelling __instance, ref GimmeOneSeedPlzModSystem.FelledTreeData __state, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier, ref EnumHandling handled)
		{
			// Log itemcode will be log-{type}-{wood}-{rotation}
			// Tree seed itemcode will be treeseed-{wood}

			if (world.Side.IsServer())
			{
				if (__state.AxedBlock == null || __state.NumTreeBlocks == -1)
				{
					return;
				}

				// Positions remaining is 0, I am 99% sure a tree was fully felled, so have at it!
				int num;
				int woodTier;
				Stack<BlockPos> foundPositionsBelow = __instance.FindTree(world, blockSel.Position.DownCopy(), out num, out woodTier);

				// If tree is not missing, we need to check for total number of blocks felled from original
				if (foundPositionsBelow.Count != 0)
				{
					// If we didn't cut 35 blocks, was not felled
					if (__state.NumTreeBlocks - foundPositionsBelow.Count < GimmeOneSeedPlzConfig.Loaded.MinRequiredBlocksBrokenOnPartialFellCount)
					{
						return;
					}
				}

				Block woodBlock = __state.AxedBlock;
				BlockLeaves leavesBlock = __state.FirstLeafBlock;

				string domain = woodBlock.Code.Domain;

				// We need to get seed from leaf first, then log if that doesnt work
				Item seedItem = null;
				if (leavesBlock != null)
				{
					if (leavesBlock.Variant["type"] != "placed")
					{
						string woodtype = leavesBlock.Variant["wood"];
						seedItem = world.SearchItems(new AssetLocation(domain, "treeseed-" + woodtype)).FirstOrDefault<Item>();
						if (seedItem == null)
						{
							world.Api.Logger.Warning("[GimmeOneSeedPlz] Could not find tree seed for leaf block " + leavesBlock.Code.ToString());
						}
					}
				}
				else
				{
					if (woodBlock.Code.BeginsWith(domain, "log") && woodBlock.Variant["type"] == "grown")
					{
						string woodtype = woodBlock.Variant["wood"];
						seedItem = world.SearchItems(new AssetLocation(domain, "treeseed-" + woodtype)).FirstOrDefault<Item>();
						if (seedItem == null)
						{
							world.Api.Logger.Warning("[GimmeOneSeedPlz] Could not find tree seed for log block " + woodBlock.Code.ToString());
						}
					}
				}

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
			}
		}
	}
}