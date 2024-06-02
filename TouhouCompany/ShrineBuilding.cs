using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

#nullable enable
namespace TouhouCompany
{
    internal class ShrineBuilding: NetworkBehaviour
    {
        private Fog fog;
        private float lowFogWeight = 500f;
        private float highFogWeight = 0.0f;
        private float targetFogWeight = 0.0f;
        private float fogFadeVelocity = 0.0f;
        private float fogFadeTime = 0.5f;

        private void Awake()
        {
            Scene sceneByName = SceneManager.GetSceneByName("CompanyBuilding");
            foreach (GameObject rootGameObject in sceneByName.GetRootGameObjects())
            {
                if (rootGameObject.name == "Environment")
                {
                    Transform transform = rootGameObject.transform.Find("Lighting/BrightDay/Sun/SunAnimContainer/Sky and Fog Global Volume");
                    if ((bool)transform)
                    {
                        TouhouCompanyPlugin.Instance.AddLog("Found fog gameobject, getting volume component");
                        fog = (Fog)transform.GetComponent<Volume>().profile.components.Find(x => x.GetType() == typeof(Fog));
                        highFogWeight = fog.meanFreePath.value;
                        targetFogWeight = highFogWeight;
                    }
                    else
                        TouhouCompanyPlugin.Instance.AddLog("Could not find fog gameobject");
                }
            }
        }

        private void Update()
        {
            fog.meanFreePath.value = Mathf.SmoothDamp(fog.meanFreePath.value, targetFogWeight, ref fogFadeVelocity, fogFadeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerControllerB component;
            if (!other.gameObject.TryGetComponent(out component) || !(component == GameNetworkManager.Instance.localPlayerController))
                return;
            targetFogWeight = lowFogWeight;
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerControllerB component;
            if (!other.gameObject.TryGetComponent(out component) || !(component == GameNetworkManager.Instance.localPlayerController))
                return;
            targetFogWeight = highFogWeight;
        }
    }
}
