namespace BetterShipUpgrades.Type
{
    public class UpgradeData
    {
        public string upgradeName { get; set;  }
        public bool hasUpgrade { get; set; }
        public int? tier { get; set; }
        public int? maxTier { get; set; }
    }
}