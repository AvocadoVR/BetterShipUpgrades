﻿using BetterShipUpgrades.Networking;
using LethalAPI.TerminalCommands.Attributes;
using Upgrades = BetterShipUpgrades.Networking.Upgrades;

namespace BetterShipUpgrades.Patches
{
    public class CommandsPatch
    {
        [TerminalCommand("upgrades", true), CommandInfo("Upgrade your ship!")]
        public string Upgrade()
        {
            string upgrades = "";
            for (int i = 0; i < Upgrades.shipUpgrades.Count; i++)
            {
                if (Upgrades.shipUpgrades[i].hasUpgrade && Upgrades.shipUpgrades[i].tier.HasValue)
                {
                    int tierValue = Upgrades.shipUpgrades[i].tier.Value;
                    upgrades += " " + Upgrades.shipUpgrades[i].upgradeName + " " + $"{Upgrades._romanNumerals[tierValue + 1]}";
                }
                else
                {
                    upgrades += " " + Upgrades.shipUpgrades[i].upgradeName + " " + $"{Upgrades._romanNumerals[Upgrades._romanNumerals.Length]}";
                }
            }
    
            return upgrades;
        }

    }
}