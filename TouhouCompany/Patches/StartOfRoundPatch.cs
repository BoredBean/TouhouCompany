using HarmonyLib;
using System;
using UnityEngine;

namespace TouhouCompany.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPostfix, HarmonyPatch("Start")]
        static void StartRoundPatch()
        {
            try
            {
                if (TouhouCompanyPlugin.EnableSkyBoxReplace.Value && TouhouCompanyPlugin.SkyBoxPrefeb != null)
                {
                    //ShipReplacer.ReplaceShip();
                    //HideSpaceProps();
                    //HideMiscMeshes();
                }
            }
            catch (Exception e)
            {
                TouhouCompanyPlugin.Instance.AddLog($"An error occured when replacing the SkyBox: {e}");
            }
        }

        //public static void ReplaceShip()
        //{
        //    try
        //    {

        //        if (newShipInside != null && vanillaShipInside != null)
        //        {
        //            //ShipWindowPlugin.Log.LogInfo($"Calling ReplaceShip when ship was already replaced! Restoring original...");
        //            ObjectReplacer.Restore(vanillaShipInside);
        //        }

        //        vanillaShipInside = FindOrThrow("Environment/HangarShip/ShipInside");
        //        string shipName = GetShipAssetName();

        //        //ShipWindowPlugin.Log.LogInfo($"Replacing ship with {shipName}");

        //        GameObject newShipPrefab = ShipWindowPlugin.mainAssetBundle.LoadAsset<GameObject>
        //            ($"Assets/LethalCompany/Mods/ShipWindow/Ships/{shipName}.prefab");

        //        if (newShipPrefab == null) throw new Exception($"Could not load requested ship replacement! {shipName}");
        //        AddWindowScripts(newShipPrefab);
        //        ReplaceGlassMaterial(newShipPrefab);

        //        newShipInside = ObjectReplacer.Replace(vanillaShipInside, newShipPrefab);

        //        StartOfRound.Instance.StartCoroutine(WaitAndCheckSwitch());

        //    }
        //    catch (Exception e)
        //    {
        //        TouhouCompanyPlugin.Instance.AddLog($"Failed to replace ShipInside! \n{e}");
        //    }
        //}
    }
}
