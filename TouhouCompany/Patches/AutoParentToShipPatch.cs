using HarmonyLib;
using System;
using UnityEngine;

namespace TouhouCompany.Patches
{
    [HarmonyPatch(typeof(AutoParentToShip))]
    internal class AutoParentToShipPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void ReplaceUnlockable(AutoParentToShip __instance)
        {
            string name = __instance.transform.name;
            //TouhouCompanyPlugin.Instance.AddLog("Unlocking " + name + ".");
            if (name.Contains("PumpkinUnlockableContainer") && TouhouCompanyPlugin.YukuriPrefeb != null && TouhouCompanyPlugin.EnableYukuriReplace.Value)
            {
                try
                {
                    MeshRenderer component1 = __instance.transform.Find("PumpkinMesh")?.GetComponent<MeshRenderer>();
                    if (component1 == null)
                        return;
                    component1.enabled = false;
                    PlaceableShipObject component2 = __instance.transform.Find("PlacementCollider").GetComponent<PlaceableShipObject>();
                    AnimatedObjectTrigger component3 = __instance.transform.Find("HitPumpkinTrigger").GetComponent<AnimatedObjectTrigger>();
                    Transform transform = UnityEngine.Object.Instantiate<GameObject>(TouhouCompanyPlugin.YukuriPrefeb).transform.Find("Yukuri");
                    transform.SetParent(__instance.transform);
                    transform.transform.localPosition = component1.transform.localPosition;
                    transform.transform.localRotation = component1.transform.localRotation;
                    transform.transform.localScale = Vector3.one;
                    component2.mainMesh = transform.GetComponent<MeshFilter>();
                    component3.boolFalseAudios[0] = TouhouCompanyPlugin.YukuriAudio;
                    TouhouCompanyPlugin.Instance.AddLog("Replaced the Pumpkin.");
                }
                catch
                {
                    TouhouCompanyPlugin.Instance.AddLog("Failed to replace the Pumpkin.");
                }
            }
            else
            {
                if (!name.Contains("PlushiePJManContainer") || !(TouhouCompanyPlugin.ShanghaiPrefeb != null) || !TouhouCompanyPlugin.EnableShanghaiReplace.Value)
                    return;
                try
                {
                    MeshRenderer component4 = __instance.transform.Find("PJManPlush")?.GetComponent<MeshRenderer>();
                    if (component4 == null)
                        return;
                    component4.enabled = false;
                    PlaceableShipObject component5 = __instance.transform.Find("PlacementCollider").GetComponent<PlaceableShipObject>();
                    AnimatedObjectTrigger component6 = __instance.transform.Find("TouchTrigger").GetComponent<AnimatedObjectTrigger>();
                    Transform transform = UnityEngine.Object.Instantiate<GameObject>(TouhouCompanyPlugin.ShanghaiPrefeb).transform.Find("PJManPlush");
                    transform.SetParent(__instance.transform);
                    transform.transform.localPosition = component4.transform.localPosition;
                    transform.transform.localRotation = component4.transform.localRotation;
                    transform.transform.localScale = component4.transform.localScale;
                    component5.mainMesh = transform.GetComponent<MeshFilter>();
                    component6.triggerAnimator = transform.GetComponent<Animator>();
                    TouhouCompanyPlugin.Instance.AddLog("Replaced the Plushie.");
                }
                catch
                {
                    TouhouCompanyPlugin.Instance.AddLog("Failed to replace the Plushie.");
                }
            }
        }
    }
}
