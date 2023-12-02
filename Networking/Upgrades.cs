using System.Collections.Generic;
using System.Linq;
using BetterShipUpgrades.Type;
using Unity.Netcode;

namespace BetterShipUpgrades.Networking
{ 
    public class Upgrades : NetworkBehaviour
    {
        private Upgrades Instance;

        public static readonly string[] _romanNumerals = new[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "(MAXED)" };
        
        public static List<UpgradeData> shipUpgrades = new List<UpgradeData>();

        public static UpgradeData _teleporter;
        public static UpgradeData _inverseTeleporter;

        public static void RegisterUpgrades()
        {
            if ("placeholder" != "for now")
            {
                var teleporter = new UpgradeData()
                {
                    upgradeName = "Teleporter",
                    hasUpgrade = false,
                    tier = 1,
                    maxTier = 3
                };

                var inverseTeleporter = new UpgradeData()
                {
                    upgradeName = "Inverse Teleporter",
                    hasUpgrade = false,
                    tier = 1,
                    maxTier = 2
                };

            
                shipUpgrades.Add(teleporter);
                shipUpgrades.Add(inverseTeleporter);
                _teleporter = shipUpgrades[0];
                _inverseTeleporter = shipUpgrades[1];
            }
            else
            {

            }
        }
        
        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
    }
}