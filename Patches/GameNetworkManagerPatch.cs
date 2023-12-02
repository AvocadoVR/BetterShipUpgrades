using System;
using System.Collections.Generic;
using BetterShipUpgrades.Networking;
using BetterShipUpgrades.Type;
using HarmonyLib;
using UnityEngine;

namespace BetterShipUpgrades.Patches
{
	[HarmonyPatch]
	public class GameNetworkManagerPatch
	{
		[HarmonyPatch(typeof(GameNetworkManager), "SaveGameValues"), HarmonyPostfix]
		static void SaveGameValues(GameNetworkManager __instance)
		{
			if (!__instance.isHostingGame)
			{
				return;
			}

			if (!ES3.KeyExists("FileGameVers", __instance.currentSaveFileName))
			{
				ES3.Save("FileGameVers", __instance.gameVersionNum, __instance.currentSaveFileName);
			}

			if (!StartOfRound.Instance.inShipPhase)
			{
				return;
			}

			try
			{
				TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
				if (timeOfDay != null)
				{
					ES3.Save("QuotaFulfilled", timeOfDay.quotaFulfilled, __instance.currentSaveFileName);
					ES3.Save("QuotasPassed", timeOfDay.timesFulfilledQuota, __instance.currentSaveFileName);
					ES3.Save("ProfitQuota", timeOfDay.profitQuota, __instance.currentSaveFileName);
				}

				ES3.Save("CurrentPlanetID", StartOfRound.Instance.currentLevelID, __instance.currentSaveFileName);
				Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
				if (terminal != null)
				{
					// terminal.groupCredits
					ES3.Save("GroupCredits", 10000, __instance.currentSaveFileName);
					if (terminal.unlockedStoryLogs.Count > 0)
					{
						ES3.Save("StoryLogs", terminal.unlockedStoryLogs.ToArray(), __instance.currentSaveFileName);
					}

					if (terminal.scannedEnemyIDs.Count > 0)
					{
						ES3.Save("EnemyScans", terminal.scannedEnemyIDs.ToArray(), __instance.currentSaveFileName);
					}
				}

				StartOfRound startOfRound = UnityEngine.Object.FindObjectOfType<StartOfRound>();
				if (startOfRound != null)
				{
					List<int> list = new List<int>();
					for (int i = 0; i < startOfRound.unlockablesList.unlockables.Count; i++)
					{
						if (startOfRound.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer ||
						    startOfRound.unlockablesList.unlockables[i].hasBeenMoved ||
						    startOfRound.unlockablesList.unlockables[i].inStorage)
						{
							list.Add(i);
						}

						if (startOfRound.unlockablesList.unlockables[i].IsPlaceable)
						{
							if (startOfRound.unlockablesList.unlockables[i].canBeStored)
							{
								ES3.Save(
									"ShipUnlockStored_" + startOfRound.unlockablesList.unlockables[i].unlockableName,
									startOfRound.unlockablesList.unlockables[i].inStorage,
									__instance.currentSaveFileName);
							}

							if (startOfRound.unlockablesList.unlockables[i].hasBeenMoved)
							{
								ES3.Save(
									"ShipUnlockMoved_" + startOfRound.unlockablesList.unlockables[i].unlockableName,
									startOfRound.unlockablesList.unlockables[i].hasBeenMoved,
									__instance.currentSaveFileName);
								ES3.Save("ShipUnlockPos_" + startOfRound.unlockablesList.unlockables[i].unlockableName,
									startOfRound.unlockablesList.unlockables[i].placedPosition,
									__instance.currentSaveFileName);
								ES3.Save("ShipUnlockRot_" + startOfRound.unlockablesList.unlockables[i].unlockableName,
									startOfRound.unlockablesList.unlockables[i].placedRotation,
									__instance.currentSaveFileName);
							}
						}
					}

					if (list.Count > 0)
					{
						ES3.Save("UnlockedShipObjects", list.ToArray(), __instance.currentSaveFileName);
					}

					ES3.Save("DeadlineTime", (int)Mathf.Clamp(timeOfDay.timeUntilDeadline, 0f, 99999f),
						__instance.currentSaveFileName);
					ES3.Save("RandomSeed", startOfRound.randomMapSeed, __instance.currentSaveFileName);
					ES3.Save("Stats_DaysSpent", startOfRound.gameStats.daysSpent, __instance.currentSaveFileName);
					ES3.Save("Stats_Deaths", startOfRound.gameStats.deaths, __instance.currentSaveFileName);
					ES3.Save("Stats_ValueCollected", startOfRound.gameStats.scrapValueCollected,
						__instance.currentSaveFileName);
					ES3.Save("Stats_StepsTaken", startOfRound.gameStats.allStepsTaken, __instance.currentSaveFileName);
				}
				
				SaveItemsInShip(__instance);
			}
			catch (Exception arg)
			{
				Debug.LogError($"Error while trying to save game values when disconnecting as host: {arg}");
			}
		}

		[HarmonyPatch(typeof(GameNetworkManager), "SaveItemsInShip"), HarmonyPostfix]
		static void SaveItemsInShip(GameNetworkManager __instance)
		{
			GrabbableObject[] array = UnityEngine.Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			if (array == null || array.Length == 0)
			{
				ES3.DeleteKey("shipGrabbableItemIDs", __instance.currentSaveFileName);
				ES3.DeleteKey("shipGrabbableItemPos", __instance.currentSaveFileName);
				ES3.DeleteKey("shipScrapValues", __instance.currentSaveFileName);
				ES3.DeleteKey("shipItemSaveData", __instance.currentSaveFileName);
				return;
			}

			List<int> list = new List<int>();
			List<Vector3> list2 = new List<Vector3>();
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			int num = 0;
			for (int i = 0; i < array.Length && i <= StartOfRound.Instance.maxShipItemCapacity; i++)
			{
				if (!StartOfRound.Instance.allItemsList.itemsList.Contains(array[i].itemProperties))
				{
					continue;
				}

				if (array[i].itemProperties.spawnPrefab == null)
				{
					Debug.LogError("Item '" + array[i].itemProperties.itemName + "' has no spawn prefab set!");
				}
				else
				{
					if (array[i].itemUsedUp)
					{
						continue;
					}

					for (int j = 0; j < StartOfRound.Instance.allItemsList.itemsList.Count; j++)
					{
						if (StartOfRound.Instance.allItemsList.itemsList[j] == array[i].itemProperties)
						{
							list.Add(j);
							list2.Add(array[i].transform.position);
							break;
						}
					}

					if (array[i].itemProperties.isScrap)
					{
						list3.Add(array[i].scrapValue);
					}

					if (array[i].itemProperties.saveItemVariable)
					{
						try
						{
							num = array[i].GetItemDataToSave();
						}
						catch
						{
							Debug.LogError(
								$"An error occured while getting item data to save for item type: {array[i].itemProperties}; gameobject '{array[i].gameObject.name}'");
						}

						list4.Add(num);
						Debug.Log($"Saved data for item type: {array[i].itemProperties.itemName} - {num}");
					}
				}
			}

			if (list.Count <= 0)
			{
				Debug.Log("Got no ship grabbable items to save.");
				return;
			}

			ES3.Save("shipGrabbableItemPos", list2.ToArray(), __instance.currentSaveFileName);
			ES3.Save("shipGrabbableItemIDs", list.ToArray(), __instance.currentSaveFileName);
			if (list3.Count > 0)
			{
				ES3.Save("shipScrapValues", list3.ToArray(), __instance.currentSaveFileName);
			}
			else
			{
				ES3.DeleteKey("shipScrapValues", __instance.currentSaveFileName);
			}

			if (list4.Count > 0)
			{
				ES3.Save("shipItemSaveData", list4.ToArray(), __instance.currentSaveFileName);
			}
			else
			{
				ES3.DeleteKey("shipItemSaveData", __instance.currentSaveFileName);
			}
		}
	}
}