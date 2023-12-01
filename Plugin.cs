using BepInEx;
using BepInEx.Logging;
using BetterShipUpgrades.Patches;
using HarmonyLib;
using PluginInfo = BetterShipUpgrades.PluginInfo;

namespace BetterShipUpgrades
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.modName, PluginInfo.modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        
        private readonly Harmony harmony = new Harmony(PluginInfo.GUID);
        public static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.GUID);

        void Awake()
        {
            Instance = this;
            mls.LogInfo("BetterShipUpgrades Loaded!");
            
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
        }
    }
}