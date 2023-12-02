using BepInEx;
using BepInEx.Logging;
using BetterShipUpgrades.Networking;
using BetterShipUpgrades.Patches;
using HarmonyLib;
using LethalAPI.TerminalCommands.Models;
using PluginInfo = BetterShipUpgrades.PluginInfo;

namespace BetterShipUpgrades
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.modName, PluginInfo.modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        
        private readonly Harmony harmony = new Harmony(PluginInfo.GUID);
        public static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.GUID);

        private TerminalModRegistry commands;

        void Awake()
        {
            Instance = this;
            mls.LogInfo("BetterShipUpgrades Loaded!");

            commands = TerminalRegistry.RegisterFrom(new CommandsPatch());
            
            Upgrades.RegisterUpgrades();
            
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
            harmony.PatchAll(typeof(ShipTeleporterPatch));
        }
    }
}