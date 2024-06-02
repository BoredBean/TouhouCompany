using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TouhouCompany
{
    public class KoumakanManager : NetworkBehaviour
    {
        public static Dictionary<string, GameObject> Objects;

        private void Awake()
        {
            TouhouCompanyPlugin.Instance.AddLog("Awaking KoumakanManager");
            Objects = [];
            DontDestroyOnLoad(gameObject);
            TouhouCompanyPlugin.Instance.AddLog("KoumakanManager is awake");
        }

        public void Destroy()
        {
            Objects.Clear();
        }

        [ServerRpc]
        public void SpawnKoumakanServerRpc()
        {
            TouhouCompanyPlugin.Instance.AddLog("Running SpawnKoumakanServerRpc");
            EnableKoumakanClientRpc();
        }

        [ClientRpc]
        public void EnableKoumakanClientRpc()
        {
            TouhouCompanyPlugin.Instance.AddLog("Running RearrangeSceneClientRpc");

            Scene sceneByName = SceneManager.GetSceneByName("Level4March");
            GameObject[] rootGameObjects = sceneByName.GetRootGameObjects();

            try
            {
                foreach (var rootObj in rootGameObjects)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"Processing {rootObj.name} on Level4March.");
                    switch (rootObj.name)
                    {
                        case "Environment":
                            {
                                var roofDome = rootObj.transform.Find("Map/RoofDome");
                                roofDome.GetComponent<MeshRenderer>().enabled = false;
                                roofDome.GetComponent<MeshCollider>().enabled = false;
                                var koumakanVisual = Instantiate(TouhouCompanyPlugin.KoumakanPrefeb);
                                koumakanVisual.transform.SetParent(roofDome);
                                koumakanVisual.transform.localPosition = Vector3.zero;
                                koumakanVisual.transform.localRotation = Quaternion.identity;
                                koumakanVisual.transform.localScale = Vector3.one;

                                TouhouCompanyPlugin.Instance.AddLog($"Enable Koumakan.");

                                var hangingLight13 = rootObj.transform.Find("HangingLight (13)");
                                var hangingLight14 = rootObj.transform.Find("HangingLight (14)");
                                Destroy(hangingLight13.gameObject);
                                Destroy(hangingLight14.gameObject);
                                TouhouCompanyPlugin.Instance.AddLog($"Disable HangingLights.");
                                var steelDoorFake = rootObj.transform.Find("SteelDoorFake");
                                var steelDoorFake1 = rootObj.transform.Find("SteelDoorFake (1)");
                                var doorFrame = rootObj.transform.Find("DoorFrame (1)");
                                Destroy(steelDoorFake.gameObject);
                                Destroy(steelDoorFake1.gameObject);
                                Destroy(doorFrame.gameObject);
                                TouhouCompanyPlugin.Instance.AddLog($"Disable SteelDoor.");

                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                TouhouCompanyPlugin.Instance.AddLog($"An error occured when replacing Level4MarchBuilding: {e}");
            }
        }
    }
}
