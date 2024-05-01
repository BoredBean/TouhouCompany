using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TouhouCompany
{
    [HarmonyPatch]
    internal class Patches
    {
        static List<string> _replacedUnlockable = [];

        [HarmonyPatch(typeof(StartOfRound), "SpawnUnlockable")]
        [HarmonyPostfix]
        private static void ReplaceUnlockable(int unlockableIndex, StartOfRound __instance)
        {
            UnlockableItem unlockableItem = __instance.unlockablesList.unlockables[unlockableIndex];
            var unlockableName = unlockableItem.unlockableName;
            if(_replacedUnlockable.Contains(unlockableName)) return;

            __instance.SpawnedShipUnlockables.TryGetValue(unlockableIndex, out GameObject unlockableObject);
            if(unlockableObject == null)return;

            if (unlockableName == "JackOLantern" && TouhouCompanyPlugin.YukuriVisuals != null &&
                TouhouCompanyPlugin.EnableYukuriReplace.Value)
            {
                try
                {
                    var pumpkinModel = unlockableObject.transform.Find("PumpkinMesh");
                    var pumpkinMesh = pumpkinModel?.GetComponent<MeshRenderer>();
                    if (pumpkinMesh == null) return;
                    pumpkinMesh.enabled = false;
                    var collider = unlockableObject.transform.Find("PlacementCollider");
                    var placeable = collider.GetComponent<PlaceableShipObject>();
                    var trigger = unlockableObject.transform.Find("HitPumpkinTrigger");
                    var animatedTrigger = trigger.GetComponent<AnimatedObjectTrigger>();
                    var yukuriVisual = Object.Instantiate(TouhouCompanyPlugin.YukuriVisuals);
                    var yukuri = yukuriVisual.transform.Find("Yukuri");
                    yukuri.SetParent(unlockableObject.transform);
                    yukuri.transform.localPosition = pumpkinMesh.transform.localPosition;
                    yukuri.transform.localRotation = pumpkinMesh.transform.localRotation;
                    yukuri.transform.localScale = Vector3.one;
                    placeable.mainMesh = yukuri.GetComponent<MeshFilter>();
                    animatedTrigger.boolFalseAudios[0] = TouhouCompanyPlugin.YukuriAudio;

                    TouhouCompanyPlugin.Instance.AddLog($"Replaced the Pumpkin.");
                }
                catch
                {
                    TouhouCompanyPlugin.Instance.AddLog($"Failed to replace the Pumpkin.");
                }
            }
            else if (unlockableName == "Plushie pajama man" && TouhouCompanyPlugin.ShanghaiVisuals != null &&
                TouhouCompanyPlugin.EnableShanghaiReplace.Value)
            {
                try
                {
                    var plushieModel = unlockableObject.transform.Find("PJManPlush");
                    var plushieMesh = plushieModel?.GetComponent<MeshRenderer>();
                    if (plushieMesh == null) return;
                    plushieMesh.enabled = false;
                    var collider = unlockableObject.transform.Find("PlacementCollider");
                    var placeable = collider.GetComponent<PlaceableShipObject>();
                    var trigger = unlockableObject.transform.Find("TouchTrigger");
                    var animatedTrigger = trigger.GetComponent<AnimatedObjectTrigger>();
                    var shanghaiVisual = Object.Instantiate(TouhouCompanyPlugin.ShanghaiVisuals);
                    var shanghai = shanghaiVisual.transform.Find("PJManPlush");
                    shanghai.SetParent(unlockableObject.transform);
                    shanghai.transform.localPosition = plushieMesh.transform.localPosition;
                    shanghai.transform.localRotation = plushieMesh.transform.localRotation;
                    shanghai.transform.localScale = plushieMesh.transform.localScale;
                    placeable.mainMesh = shanghai.GetComponent<MeshFilter>();
                    animatedTrigger.triggerAnimator = shanghai.GetComponent<Animator>();

                    TouhouCompanyPlugin.Instance.AddLog($"Replaced the Plushie.");
                }
                catch
                {
                    TouhouCompanyPlugin.Instance.AddLog($"Failed to replace the Plushie.");
                }
            }
        }
    }
}