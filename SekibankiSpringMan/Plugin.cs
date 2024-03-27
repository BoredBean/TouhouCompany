using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SekibankiSpringMan
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public static GameObject sekiVisuals;
        public static GameObject sekiHeadVisuals;

        internal static ConfigEntry<bool> EnableDeadHeadReplace;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");

            EnableDeadHeadReplace = Config.Bind("General", "EnableDeadHeadReplacement", true, "Replace the spring/coil on the dead body to a head of Sekibanki.");


            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            try
            {
                var bundle = AssetUtils.LoadAssetBundleFromResources("sekibanki", typeof(SpringManPatch).Assembly);
                sekiVisuals = bundle.LoadAsset<GameObject>("Sekibanki.prefab");
                sekiHeadVisuals = bundle.LoadAsset<GameObject>("SekibankiHead.prefab");
                var componentsInChildren = sekiVisuals.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in componentsInChildren)
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

        public void AddLog(string info)
        {
            Logger.LogInfo(info);
        }
    }

    [HarmonyPatch(typeof(DeadBodyInfo), "Start")]
    internal class BodySpringPatch
    {
        private static void Postfix(DeadBodyInfo __instance)
        {
            if (__instance is not DeadBodyInfo || Plugin.sekiHeadVisuals == null ||
                !Plugin.EnableDeadHeadReplace.Value) return;
            foreach (var part in __instance.bodyParts)
            {
                if (!part.name.Contains("Spring")) continue;
                var springContainer = part.transform.Find("spine.001/spine.002/spine.003/spine.004/SpringContainer");
                var spring = springContainer.Find("Spring.001").GetComponent<SkinnedMeshRenderer>();
                var springManMetaRig = springContainer.Find("SpringMetarig");
                if (spring != null) spring.enabled = false;
                var sekiHeadVisual = Object.Instantiate<GameObject>(Plugin.sekiHeadVisuals);
                sekiHeadVisual.transform.SetParent(springContainer);
                sekiHeadVisual.transform.localPosition = Vector3.zero;
                sekiHeadVisual.transform.localRotation = Quaternion.identity;
                sekiHeadVisual.transform.localScale = Vector3.one;
                var sekiMesh = sekiHeadVisual.transform.Find("Head/Body");
                var sekiMetarig = sekiHeadVisual.transform.Find("Head/SpringMetarig");
                sekiMetarig.SetParent(springManMetaRig.parent, true);
                sekiMetarig.transform.localScale = springManMetaRig.transform.localScale;
                sekiMetarig.transform.localRotation = springManMetaRig.transform.localRotation;
                sekiMetarig.transform.localPosition = springManMetaRig.transform.localPosition;
                var component = sekiMesh.GetComponent<SkinnedMeshRenderer>();
                component.rootBone = sekiMetarig;
                sekiMetarig.name = "SpringMetarig";
                Plugin.Instance.AddLog($"Spring model changed");
            }
        }
    }

    [HarmonyPatch(typeof(EnemyAI), "Start")]
    internal class SpringManPatch
    {
        private static void Postfix(EnemyAI __instance)
        {
            if (__instance is not SpringManAI || Plugin.sekiVisuals == null) return;

            var coilHeadAnimator = __instance.GetComponentInChildren<Animator>();
            var originalController = coilHeadAnimator.runtimeAnimatorController;

            var springManModel = __instance.transform.Find("SpringManModel");
            var springManBody = (springManModel?.Find("Body"))?.GetComponent<SkinnedMeshRenderer>();
            if (springManBody != null) springManBody.enabled = false;
            var springManHead = (springManModel?.Find("Head"))?.GetComponent<MeshRenderer>();
            var springManMetaRig = springManModel?.Find("AnimContainer").Find("metarig");
            if (springManMetaRig == null) return;
            springManMetaRig.name = "old-metarig";
            if (springManHead != null) springManHead.enabled = false;
            var sekiVisual = Object.Instantiate<GameObject>(Plugin.sekiVisuals);
            sekiVisual.transform.SetParent(springManModel);
            sekiVisual.transform.localPosition = Vector3.zero;
            sekiVisual.transform.localRotation = Quaternion.identity;
            sekiVisual.transform.localScale = Vector3.one;
            var sekiMesh = sekiVisual.transform.Find("SekibankiModel/Body");
            var sekiMetarig = sekiVisual.transform.Find("SekibankiModel/metarig");
            sekiMetarig.SetParent(springManMetaRig.parent, true);
            sekiMetarig.transform.localScale = springManMetaRig.transform.localScale;
            sekiMetarig.transform.localRotation = springManMetaRig.transform.localRotation;
            sekiMetarig.transform.localPosition = springManMetaRig.transform.localPosition;
            var component = sekiMesh.GetComponent<SkinnedMeshRenderer>();
            component.rootBone = sekiMetarig;
            sekiMetarig.name = "metarig";

            Plugin.Instance.AddLog($"SpringMan model changed");

            /* This reset the state of the animator? I have no idea. It just works. */
            coilHeadAnimator.runtimeAnimatorController = new AnimatorOverrideController(originalController);

            Plugin.Instance.AddLog($"The state of the animator reset");
        }
    }
}