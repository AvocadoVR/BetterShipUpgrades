using Unity.Netcode;

namespace BetterShipUpgrades.Networking
{
    public class UpgradesData : NetworkBehaviour
    {
        public static UpgradesData Instance;

        public static int teleportTier;
        public static int inverseTeleporterTier;

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameNetworkManager __instance = GameNetworkManager.Instance;
            
            teleportTier = ES3.Load("teleportTier", __instance.currentSaveFileName, 1);
            inverseTeleporterTier = ES3.Load("inverseTeleportTier", __instance.currentSaveFileName, 1);
        }

        void Start()
        {
            Plugin.mls.LogError($"Tiers" + " " + $"{teleportTier}" + " " + $"{inverseTeleporterTier}");
        }
    }
}