using HarmonyLib;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TouhouCompany.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        public static GameObject vanillaShip;
        public static GameObject newShip;

        [HarmonyPostfix, HarmonyPatch("Start")]
        static void StartRoundPatch()
        {
            try
            {
                if (!TouhouCompanyPlugin.EnableHangarShipReplace.Value || TouhouCompanyPlugin.HangarShipPrefeb == null || newShip != null)
                    return;
                vanillaShip = GameObject.Find("Environment/HangarShip");
                newShip = Object.Instantiate(TouhouCompanyPlugin.HangarShipPrefeb, vanillaShip.transform);
                //newShip.transform.localPosition = new Vector3(11.1f, 10.15f, -7.22f);
                newShip.transform.localPosition = new Vector3(11.1f, 32.6f, -7.22f);
                newShip.transform.localRotation = Quaternion.Euler(0, 90f, 0);
                newShip.transform.localScale = Vector3.one * 3f;
            }
            catch (Exception e)
            {
                TouhouCompanyPlugin.Instance.AddLog($"An error occured when replacing the HangarShip: {e}");
            }
        }
    }
}
