using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.MathTools;
using GimmeOneSeedPlz.ModPatches;
using Vintagestory.GameContent;
using HarmonyLib;

namespace GimmeOneSeedPlz
{
	public class GimmeOneSeedPlzModSystem : ModSystem
	{
		public Harmony harmony;

		public override void StartPre(ICoreAPI api)
        {
            string cfgFileName = "GimmeOneSeedPlz.json";

            try 
            {
                GimmeOneSeedPlzConfig cfgFromDisk;
                if ((cfgFromDisk = api.LoadModConfig<GimmeOneSeedPlzConfig>(cfgFileName)) == null)
                {
                    api.StoreModConfig(GimmeOneSeedPlzConfig.Loaded, cfgFileName);
                }
                else
                {
                    GimmeOneSeedPlzConfig.Loaded = cfgFromDisk;
                }
            } 
            catch 
            {
                api.StoreModConfig(GimmeOneSeedPlzConfig.Loaded, cfgFileName);
            }

            base.StartPre(api);
        }

		public override void StartServerSide(ICoreServerAPI sapi)
		{
			if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
				harmony = new Harmony(Mod.Info.ModID);

				if (GimmeOneSeedPlzConfig.Loaded.PatchVanillaItemAxeOnBlockBrokenWith)
				{
					var original = typeof(ItemAxe).GetMethod("OnBlockBrokenWith", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var prefix = typeof(Patch_ItemAxe_OnBlockBrokenWith).GetMethod("Prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					var postfix = typeof(Patch_ItemAxe_OnBlockBrokenWith).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					
					harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));			

					sapi.Logger.Notification("Applied patch to VintageStory's ItemAxe.OnBlockBrokenWith from Gimme One Seed Plz!");
				}

				// Mod compatibility with Toolworks, but only if that mod is present
				bool toolworks_enabled = sapi.ModLoader.IsModEnabled("toolworks");
				if (GimmeOneSeedPlzConfig.Loaded.PatchToolworksCollectibleBehaviorFellingOnBlockBrokenWith && toolworks_enabled)
				{
					PatchToolworks();

					sapi.Logger.Notification("Applied patch to Toolworks' CollectibleBehaviorFelling.OnBlockBrokenWith from Gimme One Seed Plz!");
				}					
			}

			base.StartServerSide(sapi);

			sapi.Logger.Notification("Loaded Gimme One Seed Plz!");
		}

		private void PatchToolworks()
		{
			var original = typeof(Toolworks.CollectibleBehaviorFelling).GetMethod("OnBlockBrokenWith", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			var prefix = typeof(Patch_CollectibleBehaviorFelling_OnBlockBrokenWith).GetMethod("Prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			var postfix = typeof(Patch_CollectibleBehaviorFelling_OnBlockBrokenWith).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			
			harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
		}

		public override void Dispose()
		{

		}

		public static void DropItemStack(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ItemStack itemStack)
		{
			if (world.Side.IsServer() && (byPlayer == null || byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative))
			{
				ItemStack stack = itemStack.Clone();
                world.SpawnItemEntity(stack, new Vec3d((double)pos.X + 0.5, (double)pos.Y + 0.5, (double)pos.Z + 0.5), null);
			}
		}
	}
}
