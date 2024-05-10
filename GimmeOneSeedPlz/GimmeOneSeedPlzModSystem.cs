
using Vintagestory.API.Common;
using Vintagestory.API.Server;

using HarmonyLib;
using Vintagestory.API.MathTools;

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
					harmony.PatchCategory("GimmeOneSeedPlz_ItemAxe");
					sapi.Logger.Notification("Applied patch to VintageStory's ItemAxe.OnBlockBrokenWith from Gimme One Seed Plz!");
				}

                // Mod compatibility with Toolworks, but only if that mod is present
				if (GimmeOneSeedPlzConfig.Loaded.PatchToolworksCollectibleBehaviorFellingOnBlockBrokenWith && sapi.ModLoader.IsModEnabled("toolworks"))
				{
					harmony.PatchCategory("GimmeOneSeedPlz_CollectibleBehaviorFelling");
					sapi.Logger.Notification("Applied patch to Toolworks' CollectibleBehaviorFelling.OnBlockBrokenWith from Gimme One Seed Plz!");
				}
			}

			base.StartServerSide(sapi);

			sapi.Logger.Notification("Loaded Gimme One Seed Plz!");
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
