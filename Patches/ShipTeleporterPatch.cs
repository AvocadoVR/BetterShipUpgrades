using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BetterShipUpgrades.Networking;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterShipUpgrades.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    public class ShipTeleporterPatch
    {
        private static readonly MethodInfo dropAllHeldItemsMethod = typeof(PlayerControllerB).GetMethod("DropAllHeldItems", BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo tierMethod = typeof(ShipTeleporterPatch).GetMethod("Tier", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly CodeMatch[] teleporterPatchIlMatch = new CodeMatch[] {
            new CodeMatch(i => i.IsLdarg(0)),
            new CodeMatch(i => i.opcode == OpCodes.Ldfld),
            new CodeMatch(i => i.LoadsConstant(1)),
            new CodeMatch(i => i.LoadsConstant(0)),
            new CodeMatch(i => i.Calls(dropAllHeldItemsMethod))
        };
        
        [HarmonyTranspiler, HarmonyPatch("beamUpPlayer", MethodType.Enumerator)]
        public static IEnumerable<CodeInstruction> beamUpPlayer(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher codeMatcher = new CodeMatcher(instructions);
            
            codeMatcher.End();
            codeMatcher.MatchStartBackwards(teleporterPatchIlMatch);
            codeMatcher.Advance(2);
            codeMatcher.RemoveInstructionsWithOffsets(0, 2);
            codeMatcher.Insert(new CodeInstruction(OpCodes.Callvirt, tierMethod));

            return codeMatcher.Instructions();
        }


        private static void Tier(PlayerControllerB player)
        {
            bool itemsFall = true;
            bool disconnecting = false;
            
            switch (Upgrades._teleporter.tier)
            {
                case 1: // Default Logic
                    Plugin.mls.LogError("Tier1");
                    for (int i = 0; i < player.ItemSlots.Length; i++)
                    {
                        GrabbableObject grabbableObject = player.ItemSlots[i];
                        if (!(grabbableObject != null))
                        {
                            continue;
                        }
                        
                        if (itemsFall)
                        {
                            grabbableObject.parentObject = null;
                            grabbableObject.heldByPlayerOnServer = false;
                            if (player.isInElevator)
                            {
                                grabbableObject.transform.SetParent(player.playersManager.elevatorTransform, worldPositionStays: true);
                            }
                            else
                            {
                                grabbableObject.transform.SetParent(player.playersManager.propsContainer, worldPositionStays: true);
                            }
                            player.SetItemInElevator(player.isInHangarShipRoom, player.isInElevator, grabbableObject);
                            grabbableObject.EnablePhysics(enable: true);
                            grabbableObject.EnableItemMeshes(enable: true);
                            grabbableObject.transform.localScale = grabbableObject.originalScale;
                            grabbableObject.isHeld = false;
                            grabbableObject.isPocketed = false;
                            grabbableObject.startFallingPosition = grabbableObject.transform.parent.InverseTransformPoint(grabbableObject.transform.position);
                            grabbableObject.FallToGround(randomizePosition: true);
                            grabbableObject.fallTime = UnityEngine.Random.Range(-0.3f, 0.05f);
                            if (player.IsOwner)
                            {
                                grabbableObject.DiscardItemOnClient();
                            }
                            else if (!grabbableObject.itemProperties.syncDiscardFunction)
                            {
                                grabbableObject.playerHeldBy = null;
                            }
                        }
                        if (player.IsOwner && !disconnecting)
                        {
                            HUDManager.Instance.holdingTwoHandedItem.enabled = false;
                            HUDManager.Instance.itemSlotIcons[i].enabled = false;
                            HUDManager.Instance.ClearControlTips();
                            player.activatingItem = false;
                        }
                        player.ItemSlots[i] = null;
                    }
                    if (player.isHoldingObject)
                    {
                        player.isHoldingObject = false;
                        if (player.currentlyHeldObjectServer != null)
                        {
                            MethodInfo methodInfo = player.GetType().GetMethod("SetSpecialGrabAnimationBool", BindingFlags.NonPublic | BindingFlags.Instance);
                            methodInfo.Invoke(player, new object[] { false, player.currentlyHeldObjectServer });
                        }
                        player.playerBodyAnimator.SetBool("cancelHolding", value: true);
                        player.playerBodyAnimator.SetTrigger("Throw");
                    }
                    player.twoHanded = false;
                    player.carryWeight = 1f;
                    player.currentlyHeldObjectServer = null;
                    break;
                case 2:
                    Plugin.mls.LogError("Tier2");
                    for (int i = 0; i < player.ItemSlots.Length; i++)
                    {
                        GrabbableObject grabbableObject = player.ItemSlots[i];
                        if (!(grabbableObject != null))
                        {
                            continue;
                        }

                        if (itemsFall)
                        {
                            grabbableObject.parentObject = null;
                            grabbableObject.heldByPlayerOnServer = false;
                            if (player.isInElevator)
                            {
                                grabbableObject.transform.SetParent(player.playersManager.elevatorTransform, worldPositionStays: true);
                            }
                            else
                            {
                                grabbableObject.transform.SetParent(player.playersManager.propsContainer, worldPositionStays: true);
                            }
                            player.SetItemInElevator(player.isInHangarShipRoom, player.isInElevator, grabbableObject);
                            grabbableObject.EnablePhysics(enable: true);
                            grabbableObject.EnableItemMeshes(enable: true);
                            grabbableObject.transform.localScale = grabbableObject.originalScale;
                            grabbableObject.isHeld = false;
                            grabbableObject.isPocketed = false;
                            grabbableObject.startFallingPosition = grabbableObject.transform.parent.InverseTransformPoint(grabbableObject.transform.position);
                            grabbableObject.FallToGround(randomizePosition: true);
                            grabbableObject.fallTime = UnityEngine.Random.Range(-0.3f, 0.05f);
                            if (player.IsOwner)
                            {
                                grabbableObject.DiscardItemOnClient();
                            }
                            else if (!grabbableObject.itemProperties.syncDiscardFunction)
                            {
                                grabbableObject.playerHeldBy = null;
                            }
                        }
                        if (player.IsOwner && !disconnecting)
                        {
                            HUDManager.Instance.holdingTwoHandedItem.enabled = false;
                            HUDManager.Instance.itemSlotIcons[i].enabled = false;
                            HUDManager.Instance.ClearControlTips();
                            player.activatingItem = false;
                        }
                        player.ItemSlots[i] = null;
                    }
                    if (player.isHoldingObject)
                    {
                        player.isHoldingObject = false;
                        if (player.currentlyHeldObjectServer != null)
                        {
                            MethodInfo methodInfo = player.GetType().GetMethod("SetSpecialGrabAnimationBool", BindingFlags.NonPublic | BindingFlags.Instance);
                            methodInfo.Invoke(player, new object[] { false, player.currentlyHeldObjectServer });
                        }
                        player.playerBodyAnimator.SetBool("cancelHolding", value: true);
                        player.playerBodyAnimator.SetTrigger("Throw");
                    }
                    player.twoHanded = false;
                    player.carryWeight = 1f;
                    player.currentlyHeldObjectServer = null;
                    break;
                case 3:
                    Plugin.mls.LogError("Tier3");
                    for (int i = 0; i < player.ItemSlots.Length; i++)
                    {
                        GrabbableObject grabbableObject = player.ItemSlots[i];
                        if (!(grabbableObject != null))
                        {
                            continue;
                        }

                        if (itemsFall)
                        {
                            grabbableObject.parentObject = null;
                            grabbableObject.heldByPlayerOnServer = false;
                            if (player.isInElevator)
                            {
                                grabbableObject.transform.SetParent(player.playersManager.elevatorTransform, worldPositionStays: true);
                            }
                            else
                            {
                                grabbableObject.transform.SetParent(player.playersManager.propsContainer, worldPositionStays: true);
                            }
                            player.SetItemInElevator(player.isInHangarShipRoom, player.isInElevator, grabbableObject);
                            grabbableObject.EnablePhysics(enable: true);
                            grabbableObject.EnableItemMeshes(enable: true);
                            grabbableObject.transform.localScale = grabbableObject.originalScale;
                            grabbableObject.isHeld = false;
                            grabbableObject.isPocketed = false;
                            grabbableObject.startFallingPosition = grabbableObject.transform.parent.InverseTransformPoint(grabbableObject.transform.position);
                            grabbableObject.FallToGround(randomizePosition: true);
                            grabbableObject.fallTime = UnityEngine.Random.Range(-0.3f, 0.05f);
                            if (player.IsOwner)
                            {
                                grabbableObject.DiscardItemOnClient();
                            }
                            else if (!grabbableObject.itemProperties.syncDiscardFunction)
                            {
                                grabbableObject.playerHeldBy = null;
                            }
                        }
                        if (player.IsOwner && !disconnecting)
                        {
                            HUDManager.Instance.holdingTwoHandedItem.enabled = false;
                            HUDManager.Instance.itemSlotIcons[i].enabled = false;
                            HUDManager.Instance.ClearControlTips();
                            player.activatingItem = false;
                        }
                        player.ItemSlots[i] = null;
                    }
                    if (player.isHoldingObject)
                    {
                        player.isHoldingObject = false;
                        if (player.currentlyHeldObjectServer != null)
                        {
                            MethodInfo methodInfo = player.GetType().GetMethod("SetSpecialGrabAnimationBool", BindingFlags.NonPublic | BindingFlags.Instance);
                            methodInfo.Invoke(player, new object[] { false, player.currentlyHeldObjectServer });
                        }
                        player.playerBodyAnimator.SetBool("cancelHolding", value: true);
                        player.playerBodyAnimator.SetTrigger("Throw");
                    }
                    player.twoHanded = false;
                    player.carryWeight = 1f;
                    player.currentlyHeldObjectServer = null;
                    break;
            } 
        }
    }
}