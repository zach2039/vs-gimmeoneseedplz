using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimmeOneSeedPlz
{
    public class GimmeOneSeedPlzConfig
    {
        public static GimmeOneSeedPlzConfig Loaded { get; set; } = GetDefault();

        public bool? PatchVanillaItemAxeOnBlockBrokenWith { get; set; }

        public bool? PatchToolworksCollectibleBehaviorFellingOnBlockBrokenWith { get; set; }

        public bool? PatchIDGCollectibleBehaviorWoodChoppingOnBlockBrokenWith { get; set; }

        public int? GuaranteedTreeSeedsOnFelledCount { get; set; }

        public int? MinRequiredBlocksBrokenOnFullFellCount { get; set; }

        public int? MinRequiredBlocksBrokenOnPartialFellCount { get; set; }

        public bool? UseAvgVarDropSettings { get; set; }

        public float? TreeSeedDropAvg { get; set; }
        public float? TreeSeedDropVar { get; set; }

        public static GimmeOneSeedPlzConfig GetDefault()
        {
            GimmeOneSeedPlzConfig defaultConfig = new GimmeOneSeedPlzConfig();

            defaultConfig.PatchVanillaItemAxeOnBlockBrokenWith = true;
            defaultConfig.PatchToolworksCollectibleBehaviorFellingOnBlockBrokenWith = true;
            defaultConfig.PatchIDGCollectibleBehaviorWoodChoppingOnBlockBrokenWith = true;
            defaultConfig.GuaranteedTreeSeedsOnFelledCount = 1;
            defaultConfig.MinRequiredBlocksBrokenOnFullFellCount = 3;
            defaultConfig.MinRequiredBlocksBrokenOnPartialFellCount = 35;
            defaultConfig.UseAvgVarDropSettings = false;
            defaultConfig.TreeSeedDropAvg = 0.5f;
            defaultConfig.TreeSeedDropVar = 6.0f;

            return defaultConfig;
        }
    }
}
