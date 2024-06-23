using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TouhouCompany
{
    public class ShrineManager : NetworkBehaviour
    {
        public static Dictionary<string, GameObject> Objects;
        public static GameObject newspaper;
        public static GameObject note;
        public static Animator sukimaAnimator;

        private void Awake()
        {
            TouhouCompanyPlugin.Instance.AddLog("Awaking ShrineManager");
            Objects = [];
            newspaper = null;
            note = null;
            sukimaAnimator = null;
            DontDestroyOnLoad(gameObject);
            TouhouCompanyPlugin.Instance.AddLog("ShrineManager is awake");
        }

        public void Destroy()
        {
            if (!IsServer) return;
            DespawnShrineServerRpc();

            newspaper?.GetComponent<NetworkObject>().Despawn(true);
            note?.GetComponent<NetworkObject>().Despawn(true);
        }

        [ServerRpc]
        public void SpawnShrineServerRpc()
        {
            TouhouCompanyPlugin.Instance.AddLog("Running SpawnShrineServerRpc");
            Objects.Add("Shrine", Spawn(TouhouCompanyPlugin.HakureiShrinePrefeb, new Vector3(-28f, -1.94983721f, -96.4449615f), Quaternion.Euler(270f, 90f, 0f), new Vector3(1.75130033f, 1.75130022f, 1.7513001f)));
            if (newspaper == null) newspaper = Spawn(TouhouCompanyPlugin.NewspaperPrefeb, new Vector3(-42.6040001f, -0.858900011f, -34.6570015f), Quaternion.Euler(0f, 90f, 180f), new Vector3(0.365308851f, 0.365308613f, 0.365308642f));
            if (note == null) note = Spawn(TouhouCompanyPlugin.NotePrefeb, new Vector3(-37.2468671f, 0.435433865f, -36.8849983f), Quaternion.Euler(-3f, 270f, 0f), new Vector3(0.182594687f, 0.912972927f, 0.182594657f));
            EnableShrineCompanyClientRpc(Objects["Shrine"].GetComponent<NetworkObject>().NetworkObjectId);
        }

        public static GameObject Spawn(GameObject prefeb, Vector3 location, Quaternion rotation, Vector3 scale)
        {
            TouhouCompanyPlugin.Instance.AddLog($"Instantiating {prefeb.name}...");
            GameObject gameObject = Instantiate(prefeb, location, rotation);
            gameObject.transform.localScale = scale;

            NetworkObject component = gameObject.GetComponent<NetworkObject>();
            if (component != null)
            {
                component.Spawn();
                TouhouCompanyPlugin.Instance.AddLog($"{gameObject.name} NetworkObject spawned.");
            }
            else
            {
                TouhouCompanyPlugin.Instance.AddLog(gameObject.name + "NetworkObject could not be spawned.");
            }

            return gameObject;
        }

        [ClientRpc]
        public void EnableShrineCompanyClientRpc(ulong shrineId)
        {
            TouhouCompanyPlugin.Instance.AddLog("Running EnableShrineCompanyClientRpc");

            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(shrineId, out NetworkObject shrineObject);
            shrineObject.transform.GetComponent<Animator>().enabled = false;

            Scene sceneByName = SceneManager.GetSceneByName("CompanyBuilding");
            GameObject[] rootGameObjects = sceneByName.GetRootGameObjects();

            try
            {
                foreach (var gameObject in rootGameObjects)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"Processing {gameObject.name} on CompanyBuilding.");
                    switch (gameObject.name)
                    {
                        case "Systems":
                            {
                                if (TouhouCompanyPlugin.EnableSkyBoxReplace.Value && TouhouCompanyPlugin.SkyBoxPrefeb != null)
                                {
                                    var renderingObject = gameObject.transform.Find("Rendering");
                                    var outsideSkybox = Instantiate(TouhouCompanyPlugin.SkyBoxPrefeb, renderingObject);
                                    TouhouCompanyPlugin.Instance.AddLog($"Init SkyBox.");
                                }

                                //var cont = rootObj.transform.Find("ItemShipAnimContainer");
                                //cont.localPosition = new Vector3(11.49f, 0.49f, -89.03f);

                                break;
                            }
                        case "CompanyMonstersAnims":
                            {
                                var bezierCurve = gameObject.transform.Find("TentacleAnimContainer/GrossTentacle/BezierCurve");
                                var bezierCurve2 = gameObject.transform.Find("TentacleAnimContainer/GrossTentacle2/BezierCurve");
                                //var collider = rootObj.transform.Find("TentacleAnimContainer/GrossTentacle/Armature/Bone/Bone.001/Bone.002/Bone.003/Collider (2)");
                                bezierCurve.GetComponent<SkinnedMeshRenderer>().enabled = false;
                                bezierCurve2.GetComponent<SkinnedMeshRenderer>().enabled = false;

                                TouhouCompanyPlugin.Instance.AddLog($"Disable Tentacles.");

                                break;
                            }
                        case "Cube":
                            Destroy(gameObject);
                            TouhouCompanyPlugin.Instance.AddLog($"Destroy Cube.");
                            break;
                        case "DepositCounter":
                            Destroy(gameObject.transform.Find("SpeakerBox").gameObject);
                            Destroy(gameObject.transform.Find("TinyCamera (1)").gameObject);

                            /* DoorAndHookAnim is a NetworkObject */
                            var DoorAndHookAnim = gameObject.transform.Find("DoorAndHookAnim");
                            DoorAndHookAnim.GetComponent<BoxCollider>().isTrigger = true;
                            var DepositCounterDoor = DoorAndHookAnim.Find("DepositCounterDoor (1)");
                            DepositCounterDoor.GetComponent<MeshRenderer>().enabled = false;
                            var InteractCube = DoorAndHookAnim.Find("InteractCube");
                            InteractCube.localPosition = new Vector3(53.0629997f, -8.74699974f, 21.4619999f);
                            InteractCube.localScale = new Vector3(0.930000007f, 1.39999998f, 0.75f);
                            var trigger = InteractCube.GetComponent<InteractTrigger>();
                            trigger.hoverTip = "Donate item : [LMB]";
                            //var giantHook = gameObject.transform.Find("DoorAndHookAnim/GiantHook");
                            //giantHook.localPosition = new Vector3(giantHook.localPosition.x, giantHook.localPosition.y, 19.5f);
                            /* DoorAndHookAnim is a NetworkObject */
                            sukimaAnimator = shrineObject?.transform.Find("HakureiShrine/Armature/Book殿_Parent").GetComponent<Animator>();

                            var speakerAudio = gameObject.transform.Find("Audios/SpeakerAudio").GetComponent<AudioSource>();
                            //speakerAudio.outputAudioMixerGroup = TouhouCompanyPlugin.TuneMixer?.FindMatchingGroups($"Tune19")[0];

                            TouhouCompanyPlugin.Instance.AddLog($"Disable DepositCounter.");
                            break;
                        case "Plane":
                            Destroy(gameObject);
                            TouhouCompanyPlugin.Instance.AddLog($"Destroy Plane.");
                            break;
                        case "Environment":
                            {
                                var containers = gameObject.transform.Find("Map/ShippingContainers");
                                foreach (Transform item in containers)
                                {
                                    if (item.name is "ShippingContainer" or "ShippingContainer (1)" or "ShippingContainer (3)" or "ShippingContainer (4)")
                                    {
                                        Destroy(item.gameObject);
                                    }
                                }
                                TouhouCompanyPlugin.Instance.AddLog($"Destroy ShippingContainers.");

                                var cube = gameObject.transform.Find("Map/CompanyPlanet/Cube.003");
                                Destroy(cube.gameObject);
                                TouhouCompanyPlugin.Instance.AddLog($"Destroy Cube.003.");

                                var lightContainer = gameObject.transform.Find("LightsContainer");
                                foreach (Transform item in lightContainer)
                                {
                                    if (item.name is "LEDHangingLight (2)" or "LEDHangingLight (3)" or "LEDHangingLight (4)")
                                    {
                                        Destroy(item.gameObject);
                                    }
                                }
                                TouhouCompanyPlugin.Instance.AddLog($"Destroy HangingLights.");

                                break;
                            }
                        case "BellDinger":
                            gameObject.transform.Find("BellDingerAnimContainer/BellTop").GetComponent<MeshRenderer>().enabled = false;
                            gameObject.transform.Find("BellDingerAnimContainer/Stand").GetComponent<MeshRenderer>().enabled = false;

                            /* Trigger is a NetworkObject */
                            var Trigger = gameObject.transform.Find("Trigger");
                            Trigger.GetComponent<BoxCollider>().isTrigger = true;

                            var bellTrigger = shrineObject?.transform.Find("HakureiShrine/铃/BellTrigger");
                            if (bellTrigger != null)
                            {
                                Trigger.localPosition = new Vector3(2.304908f, 1.9572053f, -1.78813553f);
                                Trigger.localRotation = Quaternion.Euler(270f, 270f, 0f);
                                Trigger.localScale = new Vector3(0.175130054f, 0.201399535f, 3.94042516f);
                                var oldAnimTrigger = Trigger.GetComponent<AnimatedObjectTrigger>();
                                var newAnimTrigger = bellTrigger.GetComponent<AnimatedObjectTrigger>();
                                oldAnimTrigger.triggerAnimator = newAnimTrigger.triggerAnimator;
                                oldAnimTrigger.boolFalseAudios = newAnimTrigger.boolFalseAudios;
                                TouhouCompanyPlugin.Instance.AddLog($"BellTrigger changed.");
                            }
                            /* Trigger is a NetworkObject */

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


        [ServerRpc]
        public void DespawnShrineServerRpc()
        {
            foreach (KeyValuePair<string, GameObject> @object in Objects)
            {
                @object.Value?.GetComponent<NetworkObject>().Despawn(true);
            }
            Objects.Clear();
        }
    }
}
