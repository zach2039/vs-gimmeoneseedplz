using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.MathTools;
using GimmeOneSeedPlz.ModPatches;
using Vintagestory.GameContent;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using System;

namespace GimmeOneSeedPlz
{
	public class GimmeOneSeedPlzModSystem : ModSystem
	{
		public Harmony harmony;

        private NullabilityInfoContext _nullabilityContext = new NullabilityInfoContext();

        public override void StartPre(ICoreAPI api)
		{
			string cfgFileName = "GimmeOneSeedPlz.json";

			try 
			{
				GimmeOneSeedPlzConfig cfgFromDisk;
				if ((cfgFromDisk = api.LoadModConfig<GimmeOneSeedPlzConfig>(cfgFileName)) == null)
				{
					// File with defaults generated and stored in ModConfig directory
					api.StoreModConfig(GimmeOneSeedPlzConfig.Loaded, cfgFileName);
                    api.Logger.Notification("[GimmeOneSeedPlz] Config file generated from defaults");
                }
				else
				{
                    // File loaded successfully
                    GimmeOneSeedPlzConfig.Loaded = cfgFromDisk;
					api.Logger.Notification("[GimmeOneSeedPlz] Config file loaded");

                }
			} 
			catch 
			{
                // File with defaults re-generated and stored in ModConfig directory due to errors reading
                api.StoreModConfig(GimmeOneSeedPlzConfig.Loaded, cfgFileName);
                api.Logger.Error("[GimmeOneSeedPlz] Config file re-generated from defaults due to error");
            }
			finally
			{
				bool saveNewValues = false;
				GimmeOneSeedPlzConfig defaultConfig = GimmeOneSeedPlzConfig.GetDefault();

                foreach (PropertyInfo prop in typeof(GimmeOneSeedPlzConfig).GetProperties())
                {
                    var nullabilityInfo = _nullabilityContext.Create(prop);
                    if (nullabilityInfo.WriteState is NullabilityState.Nullable)
                    {
                        if (prop.GetValue(GimmeOneSeedPlzConfig.Loaded) == null)
						{
							var defaultValue = prop.GetValue(defaultConfig);
							prop.SetValue(GimmeOneSeedPlzConfig.Loaded, defaultValue);
							saveNewValues = true;
                            api.Logger.Warning($"[GimmeOneSeedPlz] Missing {prop.Name} in loaded config; will populate with default value {defaultValue} and append to existing config");
                        }
                    }
                }

                if (saveNewValues)
                {
                    // File with defaults for missing properties generated and stored in ModConfig directory
                    api.StoreModConfig(GimmeOneSeedPlzConfig.Loaded, cfgFileName);
                    api.Logger.Warning("[GimmeOneSeedPlz] Config file re-written");
                }
            }

			base.StartPre(api);
		}

		public override void StartServerSide(ICoreServerAPI sapi)
		{
			if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
				harmony = new Harmony(Mod.Info.ModID);

				if (GimmeOneSeedPlzConfig.Loaded.PatchVanillaItemAxeOnBlockBrokenWith.Value)
				{
					var original = typeof(ItemAxe).GetMethod("OnBlockBrokenWith", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var prefix = typeof(Patch_ItemAxe_OnBlockBrokenWith).GetMethod("Prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					var postfix = typeof(Patch_ItemAxe_OnBlockBrokenWith).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					
					harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));			

					sapi.Logger.Notification("Applied patch to VintageStory's ItemAxe.OnBlockBrokenWith from Gimme One Seed Plz!");
				}

				// Mod compatibility with IDG, but only if that mod is present
                bool indappledgroves_enabled = sapi.ModLoader.IsModEnabled("indappledgroves");
                if (GimmeOneSeedPlzConfig.Loaded.PatchIDGCollectibleBehaviorWoodChoppingOnBlockBrokenWith.Value && indappledgroves_enabled)
                {
                    PatchIDG();

                    sapi.Logger.Notification("Applied patch to InDappledGroves' BehaviorWoodChopping.OnBlockBroken from Gimme One Seed Plz!");
                }
            }

			base.StartServerSide(sapi);

			sapi.Logger.Notification("Loaded Gimme One Seed Plz!");
		}

        private void PatchIDG()
        {
            var typeBehaviorWoodChopping = AccessTools.TypeByName("InDappledGroves.BehaviorWoodChopping");
			var onBlockBrokenWithMethod = AccessTools.Method(typeBehaviorWoodChopping, "OnBlockBrokenWith");
            //var original = typeBehaviorWoodChopping.GetMethod("OnBlockBrokenWith", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var prefix = typeof(Patch_BehaviorWoodChopping_OnBlockBrokenWith).GetMethod("Prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var postfix = typeof(Patch_BehaviorWoodChopping_OnBlockBrokenWith).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            harmony.Patch(onBlockBrokenWithMethod, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
        }

        public override void Dispose()
		{

		}

		public static void DropItemStack(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ItemStack itemStack)
		{
			if (world.Side.IsServer() && (byPlayer == null || byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative))
			{
				ItemStack stack;
                if (GimmeOneSeedPlzConfig.Loaded.UseAvgVarDropSettings.Value)
				{
                    BlockDropItemStack randomDrop = new BlockDropItemStack(itemStack, GimmeOneSeedPlzConfig.Loaded.TreeSeedDropAvg.Value);
					randomDrop.Quantity.var = GimmeOneSeedPlzConfig.Loaded.TreeSeedDropVar.Value;
                    stack = randomDrop.GetNextItemStack();
                }
				else
				{
                    stack = itemStack.Clone();
                }

                world.SpawnItemEntity(stack, new Vec3d((double)pos.X + 0.5, (double)pos.Y + 0.5, (double)pos.Z + 0.5), null);
            }
		}

		public static BlockLeaves GetLeavesFromTreeStack(IWorldAccessor world, Stack<BlockPos> treePositionStack)
		{
			while (treePositionStack.Count > 0)
			{
				BlockPos blockPos = treePositionStack.Pop();
				Block block = world.BlockAccessor.GetBlock(blockPos);

				if (block.BlockMaterial == EnumBlockMaterial.Leaves && block is BlockLeaves blockLeaves)
				{
					return blockLeaves;
				}
			}

			return null;
		}

		#nullable enable
		public record struct FelledTreeData(Block? AxedBlock, int NumTreeBlocks, BlockLeaves? FirstLeafBlock);
		#nullable disable
	}
}
