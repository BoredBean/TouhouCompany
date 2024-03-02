using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KeishiSpringMan
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static GameObject Visuals;

        public static AudioClip Audio;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            try
            {
                AssetBundle bundle = AssetUtils.LoadAssetBundleFromResources("keishi", typeof(SpringManPatch).Assembly);
                Visuals = bundle.LoadAsset<GameObject>("Keishi.prefab");
                Renderer[] componentsInChildren = Visuals.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in componentsInChildren)
                {
                    renderer.gameObject.layer = LayerMask.NameToLayer("Enemies");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(("Couldn't load asset bundle: " + ex.Message));
                return;
            }
        }
    }

    [HarmonyPatch(typeof(EnemyAI), "Start")]
    internal class SpringManPatch
    {
        private static void Postfix(EnemyAI __instance)
        {
            if (__instance is SpringManAI && !(Plugin.Visuals == null))
            {
                Transform springManModel = __instance.transform.Find("SpringManModel");
                SkinnedMeshRenderer springManBody = (springManModel?.Find("Body"))?.GetComponent<SkinnedMeshRenderer>();
                if (springManBody != null) springManBody.enabled = false;
                MeshRenderer springManHead = (springManModel?.Find("Head"))?.GetComponent<MeshRenderer>();
                Transform springManMetaRig = springManModel?.Find("AnimContainer").Find("metarig");
                if (springManMetaRig != null)
                {
                    springManMetaRig.name = "old-metarig";
                    if (springManHead != null) springManHead.enabled = false;
                    GameObject keishiVisual = Object.Instantiate<GameObject>(Plugin.Visuals);
                    keishiVisual.transform.SetParent(springManModel);
                    keishiVisual.transform.localPosition = Vector3.zero;
                    keishiVisual.transform.localRotation = Quaternion.identity;
                    keishiVisual.transform.localScale = Vector3.one*0.8f;
                    Transform keishiMesh = keishiVisual.transform.Find("KeishiModel/Body");
                    Transform keishiMetarig = keishiVisual.transform.Find("KeishiModel/metarig");
                    keishiMetarig.SetParent(springManMetaRig.parent, true);
                    keishiMetarig.transform.localScale = springManMetaRig.transform.localScale;
                    keishiMetarig.transform.localRotation = springManMetaRig.transform.localRotation;
                    keishiMetarig.transform.localPosition = springManMetaRig.transform.localPosition;
                    SkinnedMeshRenderer component = keishiMesh.GetComponent<SkinnedMeshRenderer>();
                    component.rootBone = keishiMetarig;
                    keishiMetarig.name = "metarig";
                }
            }
        }
    }
}