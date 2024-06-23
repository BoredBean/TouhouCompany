using HarmonyLib;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TouhouCompany.Patches
{
    [HarmonyPatch]
    internal class InitializeGamePatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(InitializeGame), "Start")]
        public static void startInitializeGame(InitializeGame __instance)
        {
            if (TouhouCompanyPlugin.EnableHakureiReplace.Value && TouhouCompanyPlugin.HakureiShrinePrefeb != null) try
                {
                    __instance.playColdOpenCinematic = true;
                }
                catch (Exception e)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"An error occured when startInitializeGame: {e}");
                }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ColdOpenCinematicCutscene), "Start")]
        public static void startColdOpenCinematic(ColdOpenCinematicCutscene __instance)
        {
            if (TouhouCompanyPlugin.EnableHakureiReplace.Value && TouhouCompanyPlugin.HakureiShrinePrefeb != null) try
                {
                    Scene sceneByName = SceneManager.GetSceneByName("ColdOpen1");
                    GameObject[] rootGameObjects = sceneByName.GetRootGameObjects();

                    foreach (var gameObject in rootGameObjects)
                    {
                        TouhouCompanyPlugin.Instance.AddLog($"Processing {gameObject.name} on ColdOpen1.");
                        switch (gameObject.name)
                        {
                            case var name when
                            //name == "SuitManSpriteContainer" ||
                            name == "ColdOpenMonitorRoom" ||
                            name == "DoorFrame" ||
                            name == "ComputerChairAndShelf":
                                {
                                    gameObject.SetActive(false);
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {gameObject.name}.");

                                    break;
                                }
                            case "Systems":
                                {
                                    if (TouhouCompanyPlugin.EnableSkyBoxReplace.Value && TouhouCompanyPlugin.SkyBoxPrefeb != null)
                                    {
                                        var renderingObject = gameObject.transform.Find("Rendering");
                                        var outsideSkybox = Object.Instantiate(TouhouCompanyPlugin.SkyBoxPrefeb, renderingObject);
                                        TouhouCompanyPlugin.Instance.AddLog($"Init SkyBox.");
                                    }

                                    break;
                                }
                            case "Plane":
                                {
                                    gameObject.transform.localPosition = new Vector3(-20.4247322f, 10.964f, -2.05800009f);
                                    TouhouCompanyPlugin.Instance.AddLog($"Set {gameObject.name}.");
                                    break;
                                }
                            case "ColdOpen1AnimContainer":
                                {
                                    var findObject = gameObject.transform.Find("Cube.005");
                                    findObject.GetComponent<MeshRenderer>().enabled = false;
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("MiscProps3/CurtainRod (2)");
                                    findObject.localPosition = new Vector3(-15.71f, 7.64701f, -23.8593235f);
                                    TouhouCompanyPlugin.Instance.AddLog($"Set {findObject.name}.");
                                    findObject = gameObject.transform.Find("MiscProps3/Cylinder.001 (1)");
                                    findObject.localPosition = new Vector3(-13.6400003f, 2.52699995f, -8.00623226f);
                                    TouhouCompanyPlugin.Instance.AddLog($"Set {findObject.name}.");
                                    findObject = gameObject.transform.Find("MiscProps3/FancyDesk");
                                    findObject.GetComponent<MeshRenderer>().enabled = false;
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("MiscProps3/PottedPlant");
                                    findObject.GetComponent<MeshRenderer>().enabled = false;
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("MiscProps3/Cube.002");
                                    findObject.gameObject.SetActive(false);
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("Door");
                                    findObject.gameObject.SetActive(false);
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("Lamp");
                                    findObject.gameObject.SetActive(false);
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("Remote");
                                    findObject.GetComponent<MeshRenderer>().enabled = false;
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("ShipModels2b");
                                    findObject.gameObject.SetActive(false);
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("Desk");
                                    findObject.GetComponent<MeshRenderer>().enabled = false;
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");
                                    findObject = gameObject.transform.Find("DeskAddonComponent");
                                    findObject.GetComponent<MeshRenderer>().enabled = false;
                                    TouhouCompanyPlugin.Instance.AddLog($"Disable {findObject.name}.");

                                    var shrine = Object.Instantiate(TouhouCompanyPlugin.HakureiShrinePrefeb);
                                    Object.Destroy(shrine.GetComponent<NetworkObject>());
                                    Object.Destroy(shrine.GetComponent<ShrineBuilding>());
                                    var occludeAudio = shrine.transform.Find("HakureiShrine/Body/RecordPlayerContainer/Audio").GetComponentInChildren<OccludeAudio>();
                                    Object.Destroy(occludeAudio);
                                    occludeAudio = shrine.transform.Find("HakureiShrine/Body/TelevisionContainer/TVAudio").GetComponentInChildren<OccludeAudio>();
                                    Object.Destroy(occludeAudio);
                                    occludeAudio = shrine.transform.Find("HakureiShrine/Armature/土Interval_Parent/Toilet/Cube").GetComponentInChildren<OccludeAudio>();
                                    Object.Destroy(occludeAudio);
                                    shrine.transform.position = new Vector3(4.1f, 7.19865322f, -85.7f);
                                    shrine.transform.rotation = Quaternion.Euler(270, 90, 0);
                                    shrine.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                                    TouhouCompanyPlugin.Instance.AddLog($"Enable Shrine.");

                                    break;
                                }
                        }
                    }
                }
                catch (Exception e)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"An error occured when startColdOpenCinematic: {e}");
                }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ColdOpenCinematicCutscene), "Update")]
        public static void updateColdOpenCinematic(ColdOpenCinematicCutscene __instance)
        {
            if (TouhouCompanyPlugin.EnableHakureiReplace.Value && TouhouCompanyPlugin.HakureiShrinePrefeb != null) try
                {
                    if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    {
                        __instance.EndColdOpenCutscene();
                        return;
                    }
                }
                catch (Exception e)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"An error occured when updateColdOpenCinematic: {e}");
                }
        }
    }
}