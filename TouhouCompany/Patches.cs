using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TouhouCompany
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Start")]
        static void StartRoundPatch()
        {
            try
            {
                //if (TouhouCompanyPlugin.EnableSkyBoxReplace.Value)
                //    ReplaceShip();
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

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "SpawnUnlockable")]
        private static void ReplaceUnlockable(int unlockableIndex, StartOfRound __instance)
        {
            UnlockableItem unlockableItem = __instance.unlockablesList.unlockables[unlockableIndex];
            var unlockableName = unlockableItem.unlockableName;

            __instance.SpawnedShipUnlockables.TryGetValue(unlockableIndex, out GameObject unlockableObject);
            if (unlockableObject == null) return;

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
                    animatedTrigger.boolFalseAudios[0] = TouhouCompanyPlugin.NitoriTheme;

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

        [HarmonyPrefix, HarmonyPatch(typeof(ItemDropship), "Start")]
        private static void ReplaceItemShip(ItemDropship __instance)
        {
            if (TouhouCompanyPlugin.MoriyaVisuals == null ||
                !TouhouCompanyPlugin.EnableMoriyaReplace.Value) return;

            try
            {
                __instance.transform.GetComponent<BoxCollider>().enabled = false;
                var ship = __instance.transform.Find("ItemShip");
                ship.GetComponent<MeshRenderer>().enabled = false;
                for (var index = 0; index < ship.childCount; index++)
                {
                    var child = ship.GetChild(index);
                    if (child.name.Contains("Door"))
                    {
                        child.GetComponent<MeshRenderer>().enabled = false;
                    }
                }

                TouhouCompanyPlugin.Instance.AddLog($"Hide ItemDropship model.");

                var moriyaVisual = Object.Instantiate(TouhouCompanyPlugin.MoriyaVisuals);
                moriyaVisual.transform.SetParent(ship);
                moriyaVisual.transform.localPosition = Vector3.zero;
                moriyaVisual.transform.localRotation = Quaternion.identity;
                moriyaVisual.transform.localScale = new Vector3(0.77f, 0.77f, 0.77f);

                var light = ship.Find("Point Light");
                light.localPosition = new Vector3(0.037f, 0f, 1.75f);

                TouhouCompanyPlugin.Instance.AddLog($"ItemDropship model replaced.");

                if (!TouhouCompanyPlugin.EnableNitoriTheme.Value || TouhouCompanyPlugin.NitoriTheme == null) return;
                var music = __instance.transform.Find("Music");
                var source = music.GetComponent<AudioSource>();
                source.clip = TouhouCompanyPlugin.NitoriTheme;
                source.volume = TouhouCompanyPlugin.NitoriThemeVolume.Value;
                music = music.Find("Music (1)");
                source = music.GetComponent<AudioSource>();
                source.clip = TouhouCompanyPlugin.NitoriTheme;
                source.volume = TouhouCompanyPlugin.NitoriThemeVolume.Value;

                TouhouCompanyPlugin.Instance.AddLog($"NitoriTheme replaced.");
            }
            catch (Exception e)
            {
                TouhouCompanyPlugin.Instance.AddLog($"An error occured when replacing the ItemDropship: {e}");
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(RoundManager), "LoadNewLevelWait")]
        public static void LoadNewLevelWaitPatch(RoundManager __instance)
        {
            if (__instance.currentLevel.levelID != 3 || !TouhouCompanyPlugin.EnableHakureiReplace.Value ||
                TouhouCompanyPlugin.HakureiVisuals == null) return;

            try
            {
                var sceneByName = SceneManager.GetSceneByName("CompanyBuilding");
                var rootGameObjects = sceneByName.GetRootGameObjects();

                Transform bellTrigger = null;

                foreach (var rootObj in rootGameObjects)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"Processing {rootObj.name}.");
                    switch (rootObj.name)
                    {
                        case "Systems":
                            {
                                if (TouhouCompanyPlugin.EnableSkyBoxReplace.Value && TouhouCompanyPlugin.SkyBoxVisuals != null)
                                {
                                    var renderingObject = rootObj.transform.Find("Rendering");
                                    var skyBox = Object.Instantiate(TouhouCompanyPlugin.SkyBoxVisuals, renderingObject);
                                    skyBox.transform.SetParent(renderingObject);
                                    skyBox.transform.localPosition = Vector3.zero;
                                    skyBox.transform.localRotation = Quaternion.identity;
                                    skyBox.transform.localScale = Vector3.one * 0.5f;
                                    TouhouCompanyPlugin.Instance.AddLog($"Enable SkyBox.");
                                }

                                //var cont = rootObj.transform.Find("ItemShipAnimContainer");
                                //cont.localPosition = new Vector3(11.49f, 0.49f, -89.03f);

                                break;
                            }
                        case "Cube":
                            rootObj.transform.GetComponent<MeshRenderer>().enabled = false;
                            rootObj.transform.GetComponent<BoxCollider>().enabled = false;
                            TouhouCompanyPlugin.Instance.AddLog($"Disable Cube.");
                            break;
                        case "DepositCounter":
                            rootObj.transform.Find("SpeakerBox").GetComponent<MeshRenderer>().enabled = false;
                            rootObj.transform.Find("TinyCamera (1)").GetComponent<MeshRenderer>().enabled = false;
                            var DoorAndHookAnim = rootObj.transform.Find("DoorAndHookAnim");
                            DoorAndHookAnim.GetComponent<BoxCollider>().isTrigger = true;
                            var InteractCube = DoorAndHookAnim.Find("InteractCube");
                            InteractCube.localPosition = new Vector3(53.1024f, -8.5f, 21.4483f);
                            InteractCube.localScale = new Vector3(1.1f, 1.67f, 0.82f);
                            var trigger = InteractCube.GetComponent<InteractTrigger>();
                            trigger.hoverTip = "Donate item : [LMB]";
                            var giantHook = rootObj.transform.Find("DoorAndHookAnim/GiantHook");
                            giantHook.localPosition = new Vector3(giantHook.localPosition.x, giantHook.localPosition.y, 19.5f);
                            //var light = rootObj.transform.Find("Light/Spot Light (1)");
                            //light.localPosition = new Vector3(light.localPosition.x, -0.3f, -0.3f);
                            TouhouCompanyPlugin.Instance.AddLog($"Disable DepositCounter.");
                            break;
                        case "Plane":
                            rootObj.transform.GetComponent<MeshRenderer>().enabled = false;
                            rootObj.transform.GetComponent<MeshCollider>().isTrigger = true;
                            TouhouCompanyPlugin.Instance.AddLog($"Disable Plane.");
                            break;
                        case "Environment":
                            {
                                var lightContainer = rootObj.transform.Find("LightsContainer");
                                foreach (Transform item in lightContainer)
                                {
                                    if (item.name is "LEDHangingLight (2)" or "LEDHangingLight (3)" or "LEDHangingLight (4)")
                                    {
                                        item.GetComponent<MeshRenderer>().enabled = false;
                                    }
                                }
                                TouhouCompanyPlugin.Instance.AddLog($"Disable HangingLights.");

                                var containers = rootObj.transform.Find("Map/ShippingContainers");
                                foreach (Transform item in containers)
                                {
                                    if (item.name is "ShippingContainer" or "ShippingContainer (1)" or "ShippingContainer (3)" or "ShippingContainer (4)")
                                    {
                                        Object.Destroy(item.gameObject);
                                    }
                                }
                                TouhouCompanyPlugin.Instance.AddLog($"Disable ShippingContainers.");

                                TouhouCompanyPlugin.Instance.AddLog($"Instantiating Shrine...");
                                var cube = rootObj.transform.Find("Map/CompanyPlanet/Cube.003");
                                cube.GetComponent<MeshRenderer>().enabled = false;
                                cube.GetComponent<MeshCollider>().enabled = false;
                                var hakureiVisual = Object.Instantiate(TouhouCompanyPlugin.HakureiVisuals);
                                hakureiVisual.transform.SetParent(cube);
                                hakureiVisual.transform.localPosition = Vector3.zero;
                                hakureiVisual.transform.localRotation = Quaternion.identity;
                                hakureiVisual.transform.localScale = Vector3.one;
                                TouhouCompanyPlugin.Instance.AddLog($"Enable HakureiShrine.");

                                bellTrigger = hakureiVisual.transform.Find("HakureiShrine/铃/BellTrigger");

                                var colliders = cube.Find("Colliders");
                                foreach (Transform item in colliders)
                                {
                                    if (item.name != "Cube" && item.name != "Cube (1)" && item.name != "Cube (4)")
                                    {
                                        Object.Destroy(item.gameObject);
                                    }
                                }
                                TouhouCompanyPlugin.Instance.AddLog($"Disable Colliders.");

                                break;
                            }
                        case "BellDinger":
                            var Trigger = rootObj.transform.Find("Trigger");
                            Trigger.GetComponent<BoxCollider>().isTrigger = true;
                            rootObj.transform.Find("BellDingerAnimContainer/BellTop").GetComponent<MeshRenderer>().enabled = false;
                            rootObj.transform.Find("BellDingerAnimContainer/Stand").GetComponent<MeshRenderer>().enabled = false;

                            if (bellTrigger != null)
                            {
                                Trigger.SetParent(bellTrigger.parent);
                                Trigger.localPosition = bellTrigger.localPosition;
                                Trigger.localRotation = bellTrigger.localRotation;
                                Trigger.localScale = bellTrigger.localScale;
                                var oldAnimTrigger = Trigger.GetComponent<AnimatedObjectTrigger>();
                                var newAnimTrigger = bellTrigger.GetComponent<AnimatedObjectTrigger>();
                                oldAnimTrigger.triggerAnimator = newAnimTrigger.triggerAnimator;
                                oldAnimTrigger.boolFalseAudios = newAnimTrigger.boolFalseAudios;
                                TouhouCompanyPlugin.Instance.AddLog($"BellTrigger changed.");
                            }
                            TouhouCompanyPlugin.Instance.AddLog($"Disable Bell.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                TouhouCompanyPlugin.Instance.AddLog($"An error occured when replacing the CompanyBuilding: {e}");
            }
        }
    }
}