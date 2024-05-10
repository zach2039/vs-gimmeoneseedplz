using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimmeOneSeedPlz
{
    public class GimmeOneSeedPlzConfig
    {
        public static GimmeOneSeedPlzConfig Loaded { get; set; } = new GimmeOneSeedPlzConfig();

        public bool PatchVanillaItemAxeOnBlockBrokenWith { get; set; } = true;

        public bool PatchToolworksCollectibleBehaviorFellingOnBlockBrokenWith { get; set; } = true;

        public int GuaranteedTreeSeedsOnFelledCount { get; set; } = 1;

        public int MinRequiredBlocksBrokenOnFullFellCount { get; set; } = 3;

        public int MinRequiredBlocksBrokenOnPartialFellCount { get; set; } = 35;
    }
}
