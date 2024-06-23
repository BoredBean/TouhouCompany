using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TouhouCompany.Patches
{
    [HarmonyPatch]
    internal class OtherPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(DepositItemsDesk), "OpenShutDoor")]
        public static void AngryMeshFix(DepositItemsDesk __instance, bool open)
        {
            if (TouhouCompanyPlugin.EnableHakureiReplace.Value && TouhouCompanyPlugin.HakureiShrinePrefeb != null) try
                {
                    ShrineManager.sukimaAnimator?.SetBool("doorOpen", open);
                }
                catch (Exception e)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"An error occured when enable Sukima: {e}");
                }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DepositItemsDesk), "Attack")]
        public static void AngryMeshFix(DepositItemsDesk __instance)
        {
            if (TouhouCompanyPlugin.EnableHakureiReplace.Value && TouhouCompanyPlugin.HakureiShrinePrefeb != null) try
                {
                    var sceneByName = SceneManager.GetSceneByName("CompanyBuilding");
                    var rootGameObjects = sceneByName.GetRootGameObjects();

                    foreach (var rootObj in rootGameObjects)
                    {
                        TouhouCompanyPlugin.Instance.AddLog($"Processing {rootObj.name}.");
                        switch (rootObj.name)
                        {
                            case "CompanyMonstersAnims":
                                {
                                    var bezierCurve = rootObj.transform.Find("TentacleAnimContainer/GrossTentacle/BezierCurve");
                                    var bezierCurve2 = rootObj.transform.Find("TentacleAnimContainer/GrossTentacle2/BezierCurve");
                                    //var collider = rootObj.transform.Find("TentacleAnimContainer/GrossTentacle/Armature/Bone/Bone.001/Bone.002/Bone.003/Collider (2)");
                                    bezierCurve.GetComponent<SkinnedMeshRenderer>().enabled = true;
                                    bezierCurve2.GetComponent<SkinnedMeshRenderer>().enabled = true;

                                    TouhouCompanyPlugin.Instance.AddLog($"Enable Tentacles.");

                                    break;
                                }
                        }
                    }
                }
                catch (Exception e)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"An error occured when enable CompanyMonster: {e}");
                }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerControllerB), "SetObjectAsNoLongerHeld")]
        public static bool TestNoLongerHeld(bool droppedInElevator,
                                            bool droppedInShipRoom,
                                            Vector3 targetFloorPosition,
                                            GrabbableObject dropObject,
                                            int floorYRot,
                                            PlayerControllerB __instance)
        {
            if (dropObject.name != "Note(Clone)" || dropObject.name != "Newspaper(Clone)") return true;
            TouhouCompanyPlugin.Instance.AddLog($"TestNoLongerHeld: {dropObject.name}.");
            for (int index = 0; index < __instance.ItemSlots.Length; ++index)
            {
                if (__instance.ItemSlots[index] == dropObject)
                    __instance.ItemSlots[index] = null;
            }
            dropObject.heldByPlayerOnServer = false;
            dropObject.parentObject = null;
            if (__instance.IsServer)
            {
                if (droppedInElevator)
                    dropObject.transform.SetParent(__instance.playersManager.elevatorTransform, true);
                else
                    dropObject.transform.SetParent(__instance.playersManager.propsContainer, true);
            }
            __instance.SetItemInElevator(droppedInShipRoom, droppedInElevator, dropObject);
            dropObject.EnablePhysics(true);
            dropObject.EnableItemMeshes(true);
            dropObject.transform.localScale = dropObject.originalScale;
            dropObject.isHeld = false;
            dropObject.isPocketed = false;
            dropObject.fallTime = 0.0f;
            if (dropObject.transform.parent != null) dropObject.startFallingPosition = dropObject.transform.parent.InverseTransformPoint(dropObject.transform.position);
            dropObject.targetFloorPosition = targetFloorPosition;
            dropObject.floorYRot = floorYRot;
            __instance.twoHanded = false;
            __instance.twoHandedAnimation = false;
            __instance.carryWeight -= Mathf.Clamp(dropObject.itemProperties.weight - 1f, 0.0f, 10f);
            __instance.isHoldingObject = false;

            var targetType = __instance.GetType();
            var hasThrownObject = targetType.GetField("hasThrownObject", BindingFlags.NonPublic | BindingFlags.Instance);

            if (hasThrownObject != null)
            {
                hasThrownObject.SetValue(__instance, true);
            }
            TouhouCompanyPlugin.Instance.AddLog($"Replaced method TestNoLongerHeld().");
            return false;
        }
    }
}