using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace TouhouEnemyModels
{
    [HarmonyPatch]
    internal class Patches
    {
        #region ModelReplace

        [HarmonyPatch(typeof(DeadBodyInfo), "Start")]
        [HarmonyPostfix]
        private static void ReplaceDeadBodySpringModel(DeadBodyInfo __instance)
        {
            if (__instance is null || TouhouEnemiesPlugin.SekiHeadVisuals == null ||
                !TouhouEnemiesPlugin.EnableCoilHeadReplace.Value ||
                !TouhouEnemiesPlugin.EnableBodyCoilReplace.Value) return;
            try
            {
                foreach (var part in __instance.bodyParts)
                {
                    if (!part.name.Contains("Spring")) continue;
                    var springContainer =
                        part.transform.Find("spine.001/spine.002/spine.003/spine.004/SpringContainer");
                    var spring = springContainer.Find("Spring.001").GetComponent<SkinnedMeshRenderer>();
                    var springManMetaRig = springContainer.Find("SpringMetarig");
                    if (spring != null) spring.enabled = false;
                    var sekiHeadVisual = Object.Instantiate(TouhouEnemiesPlugin.SekiHeadVisuals);
                    sekiHeadVisual.transform.SetParent(springContainer);
                    sekiHeadVisual.transform.localPosition = Vector3.zero;
                    sekiHeadVisual.transform.localRotation = Quaternion.identity;
                    sekiHeadVisual.transform.localScale = Vector3.one;
                    var sekiMesh = sekiHeadVisual.transform.Find("Head/Body");
                    var sekiMetarig = sekiHeadVisual.transform.Find("Head/Armature");
                    sekiMetarig.SetParent(springManMetaRig.parent, true);
                    sekiMetarig.transform.localScale = springManMetaRig.transform.localScale;
                    sekiMetarig.transform.localRotation = springManMetaRig.transform.localRotation;
                    sekiMetarig.transform.localPosition = springManMetaRig.transform.localPosition;
                    var component = sekiMesh.GetComponent<SkinnedMeshRenderer>();
                    component.rootBone = sekiMetarig;
                    sekiMetarig.name = "SpringMetarig";
                    TouhouEnemiesPlugin.Instance.AddLog($"Spring model changed.");
                }
            }
            catch
            {
                TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace the spring.");
            }
        }

        [HarmonyPatch(typeof(EnemyAI), "Start")]
        [HarmonyPrefix]
        private static void ReplaceEnemyModels(EnemyAI __instance)
        {
            switch (__instance)
            {
                case SpringManAI ai when TouhouEnemiesPlugin.SekiVisuals != null &&
                                         TouhouEnemiesPlugin.EnableCoilHeadReplace.Value:
                    {
                        try
                        {
                            var coilHeadAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = coilHeadAnimator.runtimeAnimatorController;

                            var springManModel = ai.transform.Find("SpringManModel");
                            var springManBody = springManModel?.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
                            if (springManBody != null) springManBody.enabled = false;
                            var springManHead = springManModel?.Find("Head")?.GetComponent<MeshRenderer>();
                            if (springManHead != null) springManHead.enabled = false;
                            springManModel.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                "Coilbanki";
                            var springManMetaRig = springManModel.Find("AnimContainer").Find("metarig");
                            if (springManMetaRig == null) return;
                            springManMetaRig.name = "old-metarig";
                            var sekiVisual = Object.Instantiate(TouhouEnemiesPlugin.SekiVisuals);
                            sekiVisual.transform.SetParent(ai.transform);
                            sekiVisual.transform.localPosition = Vector3.zero;
                            sekiVisual.transform.localRotation = Quaternion.identity;
                            sekiVisual.transform.localScale = Vector3.one;
                            var sekiModel = sekiVisual.transform.Find("SekibankiModel");
                            var sekiBody = sekiModel.transform.Find("Body");
                            var sekiHead = sekiModel.transform.Find("Head");
                            var sekiMetarig = sekiModel.transform.Find("AnimContainer");
                            sekiMetarig.SetParent(springManMetaRig.parent, true);
                            sekiMetarig.transform.localScale = springManMetaRig.transform.localScale;
                            sekiMetarig.transform.localRotation = springManMetaRig.transform.localRotation;
                            sekiMetarig.transform.localPosition = springManMetaRig.transform.localPosition;
                            var component = sekiBody.GetComponent<SkinnedMeshRenderer>();
                            component.rootBone = sekiMetarig.transform.Find("spine");
                            component = sekiHead.GetComponent<SkinnedMeshRenderer>();
                            component.rootBone = sekiMetarig.transform.Find("spine");
                            sekiMetarig.name = "metarig";

                            TouhouEnemiesPlugin.Instance.AddLog($"SpringMan model changed.");

                            //ai.mainCollider = sekiModel.GetComponent<Collider>();

                            //TouhouEnemiesPlugin.Instance.AddLog($"Collider: {ai.mainCollider != null}");

                            /* This is reset. the state of the animator? I have no idea. It just works. */
                            coilHeadAnimator.runtimeAnimatorController =
                                new AnimatorOverrideController(originalController);

                            TouhouEnemiesPlugin.Instance.AddLog($"The state of the animator reset.");
                        }
                        catch
                        {
                            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace SpringMan.");
                        }

                        break;
                    }
                case NutcrackerEnemyAI ai when TouhouEnemiesPlugin.SatoriVisuals != null &&
                                               TouhouEnemiesPlugin.EnableNutcrackerReplace.Value:
                    {
                        try
                        {
                            var nutcrackerAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = nutcrackerAnimator.runtimeAnimatorController;

                            var nutcrackerModel = ai.transform.Find("MeshContainer");
                            var nutcrackerLod0 = nutcrackerModel?.Find("LOD0")?.GetComponent<SkinnedMeshRenderer>();
                            var nutcrackerLod1 = nutcrackerModel?.Find("LOD1")?.GetComponent<SkinnedMeshRenderer>();
                            var nutcrackerTestShotgun = nutcrackerModel
                                ?.Find(
                                    "AnimContainer/metarig/spinecontainer/GunAndArmsContainer/GunPoint/GunPointWithOffset/TestShotgun")
                                ?.GetComponent<MeshRenderer>();
                            var nutcrackerGunBarrel = nutcrackerModel
                                ?.Find(
                                    "AnimContainer/metarig/spinecontainer/GunAndArmsContainer/GunPoint/GunPointWithOffset/TestShotgun/GunBarrel")
                                ?.GetComponent<MeshRenderer>();
                            if (nutcrackerLod0 != null) nutcrackerLod0.enabled = false;
                            if (nutcrackerLod1 != null) nutcrackerLod1.enabled = false;
                            if (nutcrackerTestShotgun != null) nutcrackerTestShotgun.enabled = false;
                            if (nutcrackerGunBarrel != null) nutcrackerGunBarrel.enabled = false;
                            ai.transform.Find("ScanNode (1)").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                "NutSatori";
                            var nutcrackerMetaRig = nutcrackerModel?.Find("AnimContainer").Find("metarig");
                            if (nutcrackerMetaRig == null) return;
                            nutcrackerMetaRig.name = "old-metarig";
                            var satoriVisual = Object.Instantiate(TouhouEnemiesPlugin.SatoriVisuals);
                            satoriVisual.transform.SetParent(ai.transform);
                            satoriVisual.transform.localPosition = Vector3.zero;
                            satoriVisual.transform.localRotation = Quaternion.identity;
                            satoriVisual.transform.localScale = Vector3.one;
                            var satoriMesh = satoriVisual.transform.Find("MeshContainer/LOD0");
                            var satoriMetarig = satoriVisual.transform.Find("MeshContainer/Armature/metarig");
                            satoriMetarig.SetParent(nutcrackerMetaRig.parent, true);
                            satoriMetarig.transform.localScale = nutcrackerMetaRig.transform.localScale;
                            satoriMetarig.transform.localRotation = nutcrackerMetaRig.transform.localRotation;
                            satoriMetarig.transform.localPosition = nutcrackerMetaRig.transform.localPosition;
                            var component = satoriMesh.GetComponent<SkinnedMeshRenderer>();
                            component.rootBone = satoriMetarig.transform.Find("spinecontainer/spine");
                            satoriMetarig.name = "metarig";

                            TouhouEnemiesPlugin.Instance.AddLog($"Nutcracker model changed.");

                            ai.torsoContainer = satoriMetarig.transform.Find("spinecontainer");

                            var point = ai.torsoContainer.transform.Find("GunAndArmsContainer/GunPoint");
                            ai.gunPoint.SetParent(point);
                            var gunPoint = point.transform.Find("GunPointWithOffset");
                            ai.gunPoint.localScale = gunPoint.localScale;
                            ai.gunPoint.localRotation = gunPoint.localRotation;
                            ai.gunPoint.localPosition = gunPoint.localPosition;

                            TouhouEnemiesPlugin.Instance.AddLog($"Torso: {ai.torsoContainer != null}, " +
                                                                $"GunPoint: {ai.gunPoint != null}");

                            /* This reset the state of the animator? I have no idea. It just works. */
                            nutcrackerAnimator.runtimeAnimatorController =
                                new AnimatorOverrideController(originalController);

                            TouhouEnemiesPlugin.Instance.AddLog($"The state of the animator is reset.");
                        }
                        catch
                        {
                            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace Nutcracker.");
                        }

                        break;
                    }
                case ForestGiantAI ai when TouhouEnemiesPlugin.SuikaVisuals != null &&
                                           TouhouEnemiesPlugin.EnableForestGiantReplace.Value:
                    {
                        try
                        {
                            var forestGiantAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = forestGiantAnimator.runtimeAnimatorController;

                            var forestGiantModel = ai.transform.Find("FGiantModelContainer");
                            var bodyLod0 = forestGiantModel?.Find("BodyLOD0")?.GetComponent<SkinnedMeshRenderer>();
                            var bodyLod1 = forestGiantModel?.Find("BodyLOD1")?.GetComponent<SkinnedMeshRenderer>();
                            var bodyLod2 = forestGiantModel?.Find("BodyLOD2")?.GetComponent<SkinnedMeshRenderer>();
                            if (bodyLod0 != null) bodyLod0.enabled = false;
                            if (bodyLod1 != null) bodyLod1.enabled = false;
                            if (bodyLod2 != null) bodyLod2.enabled = false;
                            ai.transform.Find("ScanNode (1)").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                "SuikaGiant";
                            var forestGiantMetaRig = forestGiantModel?.Find("AnimContainer").Find("metarig");
                            if (forestGiantMetaRig == null) return;
                            forestGiantMetaRig.name = "old-metarig";
                            var suikaVisual = Object.Instantiate(TouhouEnemiesPlugin.SuikaVisuals);
                            suikaVisual.transform.SetParent(ai.transform);
                            suikaVisual.transform.localPosition = Vector3.zero;
                            suikaVisual.transform.localRotation = Quaternion.identity;
                            suikaVisual.transform.localScale = Vector3.one;
                            var suikaMesh = suikaVisual.transform.Find("FGiantModelContainer/BodyLOD0");
                            var suikaMetarig = suikaVisual.transform.Find("FGiantModelContainer/metarig");
                            suikaMetarig.SetParent(forestGiantMetaRig.parent, true);
                            suikaMetarig.transform.localScale = forestGiantMetaRig.transform.localScale;
                            suikaMetarig.transform.localRotation = forestGiantMetaRig.transform.localRotation;
                            suikaMetarig.transform.localPosition = forestGiantMetaRig.transform.localPosition;
                            var suikaMeshRenderer = suikaMesh.GetComponent<SkinnedMeshRenderer>();
                            suikaMeshRenderer.rootBone = suikaMetarig.transform.Find("spine");
                            suikaMetarig.name = "metarig";

                            TouhouEnemiesPlugin.Instance.AddLog($"ForestGiant model changed.");

                            var model = suikaVisual.transform.Find("FGiantModelContainer");
                            ai.centerPosition.SetParent(model);
                            var centerPosition = model.transform.Find("CenterPosition");
                            ai.centerPosition.localScale = centerPosition.localScale;
                            ai.centerPosition.localRotation = centerPosition.localRotation;
                            ai.centerPosition.localPosition = centerPosition.localPosition;

                            ai.handBone =
                                suikaMetarig.transform.Find("spine/spine.003/shoulder.R/upper_arm.R/forearm.R/hand.R");

                            ai.holdPlayerPoint.SetParent(ai.handBone);
                            var holdPlayerPoint = ai.handBone.transform.Find("PlayerPoint");
                            ai.holdPlayerPoint.localScale = holdPlayerPoint.localScale;
                            ai.holdPlayerPoint.localRotation = holdPlayerPoint.localRotation;
                            ai.holdPlayerPoint.localPosition = holdPlayerPoint.localPosition;

                            TouhouEnemiesPlugin.Instance.AddLog($"CenterPos: {ai.centerPosition != null}, " +
                                                                $"HandBone: {ai.handBone != null}, " +
                                                                $"HoldPoint: {ai.holdPlayerPoint != null}");

                            ai.reachForPlayerRig.data.root =
                                suikaMetarig.transform.Find("spine/spine.003/shoulder.R/upper_arm.R");
                            ai.reachForPlayerRig.data.mid =
                                suikaMetarig.transform.Find("spine/spine.003/shoulder.R/upper_arm.R/forearm.R");
                            ai.reachForPlayerRig.data.tip = ai.handBone;

                            /* This reset the state of the animator? I have no idea. It just works. */
                            forestGiantAnimator.runtimeAnimatorController =
                                new AnimatorOverrideController(originalController);

                            TouhouEnemiesPlugin.Instance.AddLog($"The state of the animator is reset.");
                        }
                        catch
                        {
                            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace ForestGiant.");
                        }

                        break;
                    }
                case HoarderBugAI ai when TouhouEnemiesPlugin.MarisaVisuals != null &&
                                          TouhouEnemiesPlugin.EnableHoarderBugReplace.Value:
                    {
                        try
                        {
                            var bugAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = bugAnimator.runtimeAnimatorController;

                            var hoarderBugModel = ai.transform.Find("HoarderBugModel");
                            var cube = hoarderBugModel?.Find("Cube")?.GetComponent<SkinnedMeshRenderer>();
                            var cube001 = hoarderBugModel?.Find("Cube.001")?.GetComponent<SkinnedMeshRenderer>();
                            var leftWing = hoarderBugModel?.Find("AnimContainer/Armature/Abdomen/Chest/Head/LeftWing")
                                ?.GetComponent<MeshRenderer>();
                            var rightWing = hoarderBugModel?.Find("AnimContainer/Armature/Abdomen/Chest/Head/RightWing")
                                ?.GetComponent<MeshRenderer>();
                            if (cube != null) cube.enabled = false;
                            if (cube001 != null) cube001.enabled = false;
                            if (leftWing != null) leftWing.enabled = false;
                            if (rightWing != null) rightWing.enabled = false;
                            var hoarderBugArmature = hoarderBugModel?.Find("AnimContainer").Find("Armature");
                            if (hoarderBugArmature == null) return;
                            hoarderBugArmature.name = "old-Armature";

                            var bugSpawn = Random.Range(0, 10);
                            if (TouhouEnemiesPlugin.BugSpawnRate.Value < bugSpawn + 1)
                            {
                                var hoarderBugAnimator = hoarderBugModel?.Find("AnimContainer").GetComponent<Animator>();
                                if (hoarderBugAnimator != null) hoarderBugAnimator.enabled = false;

                                ai.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                "HoarderMarisa";
                                var marisaVisual = Object.Instantiate(TouhouEnemiesPlugin.MarisaVisuals);

                                var marisaAnimator =
                                    marisaVisual.transform.Find("HoarderBugModel").GetComponent<Animator>();
                                ai.creatureAnimator = marisaAnimator;

                                marisaVisual.transform.SetParent(ai.transform);
                                marisaVisual.transform.localPosition = Vector3.zero;
                                marisaVisual.transform.localRotation = Quaternion.identity;
                                marisaVisual.transform.localScale = Vector3.one;
                                var marisaMesh = marisaVisual.transform.Find("HoarderBugModel/Cube");
                                var marisaArmature = marisaVisual.transform.Find("HoarderBugModel/Armature");
                                marisaArmature.SetParent(hoarderBugArmature.parent, true);
                                marisaArmature.transform.localScale = Vector3.one;
                                marisaArmature.transform.localRotation = hoarderBugArmature.transform.localRotation;
                                marisaArmature.transform.localPosition = hoarderBugArmature.transform.localPosition;
                                var marisaMeshRenderer = marisaMesh.GetComponent<SkinnedMeshRenderer>();
                                marisaMeshRenderer.rootBone = marisaArmature.transform.Find("Abdomen");
                                marisaArmature.name = "Armature";

                                TouhouEnemiesPlugin.Instance.AddLog($"HoarderBug model changed.");

                                var spine = marisaArmature.transform.Find("Abdomen/Spine");
                                ai.grabTarget.SetParent(spine);
                                var grabTarget = spine.transform.Find("HoldItemsTarget");
                                ai.grabTarget.localScale = grabTarget.localScale;
                                ai.grabTarget.localRotation = grabTarget.localRotation;
                                ai.grabTarget.localPosition = grabTarget.localPosition;

                                TouhouEnemiesPlugin.Instance.AddLog($"GrabTarget: {ai.grabTarget != null}");

                                TouhouEnemiesPlugin.Instance.AddLog($"The animator is replaced.");
                            }
                            else
                            {
                                ai.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                    "KirisameBug";
                                var bugVisuals = Object.Instantiate(TouhouEnemiesPlugin.BugVisuals);
                                bugVisuals.transform.SetParent(ai.transform);
                                bugVisuals.transform.localPosition = Vector3.zero;
                                bugVisuals.transform.localRotation = Quaternion.identity;
                                bugVisuals.transform.localScale = Vector3.one;
                                var bugMesh = bugVisuals.transform.Find("AnimContainer/Cube");
                                var bugArmature = bugVisuals.transform.Find("AnimContainer/Armature");
                                bugArmature.SetParent(hoarderBugArmature.parent, true);
                                bugArmature.transform.localScale = Vector3.one;
                                bugArmature.transform.localRotation = hoarderBugArmature.transform.localRotation;
                                bugArmature.transform.localPosition = hoarderBugArmature.transform.localPosition;
                                var bugMeshRenderer = bugMesh.GetComponent<SkinnedMeshRenderer>();
                                bugMeshRenderer.rootBone = bugArmature.transform.Find("Abdomen");
                                bugArmature.name = "Armature";

                                TouhouEnemiesPlugin.Instance.AddLog($"HoarderBug model changed.");

                                var spine = bugArmature.transform.Find("Abdomen/Spine");
                                ai.grabTarget.SetParent(spine);
                                var grabTarget = spine.transform.Find("HoldItemsTarget");
                                ai.grabTarget.localScale = grabTarget.localScale;
                                ai.grabTarget.localRotation = grabTarget.localRotation;
                                ai.grabTarget.localPosition = grabTarget.localPosition;

                                TouhouEnemiesPlugin.Instance.AddLog($"GrabTarget: {ai.grabTarget != null}");

                                bugAnimator.runtimeAnimatorController =
                                    new AnimatorOverrideController(originalController);

                                TouhouEnemiesPlugin.Instance.AddLog($"The animator is replaced.");
                            }
                        }
                        catch
                        {
                            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace HoarderBug.");
                        }

                        break;
                    }
                case SandWormAI ai when TouhouEnemiesPlugin.YuyukoVisuals != null &&
                                        TouhouEnemiesPlugin.EnableSandWormReplace.Value:
                    {
                        try
                        {
                            var sandWormAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = sandWormAnimator.runtimeAnimatorController;

                            var meshContainer = ai.transform.Find("MeshContainer");
                            var renderer = meshContainer?.Find("Renderer")?.GetComponent<SkinnedMeshRenderer>();
                            if (renderer != null) renderer.enabled = false;
                            var sandWormBone = meshContainer?.Find("Armature").Find("Bone");
                            if (sandWormBone == null) return;
                            sandWormBone.name = "old-Bone";
                            var yuyukoVisual = Object.Instantiate(TouhouEnemiesPlugin.YuyukoVisuals);
                            yuyukoVisual.transform.SetParent(ai.transform);
                            yuyukoVisual.transform.localPosition = Vector3.zero;
                            yuyukoVisual.transform.localRotation = Quaternion.identity;
                            yuyukoVisual.transform.localScale = Vector3.one;
                            var yuyukoMesh = yuyukoVisual.transform.Find("MeshContainer");
                            var yuyukoRenderer = yuyukoMesh.transform.Find("Renderer");
                            var yuyukoBone = yuyukoMesh.transform.Find("Armature").Find("Bone");
                            yuyukoBone.SetParent(sandWormBone.parent, true);
                            yuyukoBone.transform.localScale = sandWormBone.transform.localScale;
                            yuyukoBone.transform.localRotation = sandWormBone.transform.localRotation;
                            yuyukoBone.transform.localPosition = sandWormBone.transform.localPosition;

                            TouhouEnemiesPlugin.Instance.AddLog($"SandWorm model changed.");

                            var yuyukoBone002 = yuyukoBone.transform.Find("Bone.001/Bone.003/Bone.002");
                            var sandWormCollider = sandWormBone.transform.Find("Bone.001/Bone.003/Bone.002/Collider");
                            var sandWormVoice = sandWormBone.transform.Find("Bone.001/Bone.003/Bone.002/VoiceAudio");
                            var sandWormScanNode = sandWormBone.transform.Find("Bone.001/Bone.003/Bone.002/ScanNode");
                            sandWormScanNode.gameObject.GetComponent<ScanNodeProperties>().headerText = "EaterYuyuko";
                            sandWormCollider.SetParent(yuyukoBone002);
                            sandWormVoice.SetParent(yuyukoBone002);
                            sandWormScanNode.SetParent(yuyukoBone002);

                            TouhouEnemiesPlugin.Instance.AddLog($"Collider: {sandWormCollider != null}, " +
                                                                $"Voice: {sandWormVoice != null}, " +
                                                                $"ScanNode: {sandWormScanNode != null}");

                            /* This is reset. the state of the animator? I have no idea. It just works. */
                            sandWormAnimator.runtimeAnimatorController =
                                new AnimatorOverrideController(originalController);

                            TouhouEnemiesPlugin.Instance.AddLog($"The state of the animator reset.");
                        }
                        catch
                        {
                            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace SandWorm.");
                        }

                        break;
                    }
            }
        }

        #endregion

        #region AudioReplace

        [HarmonyPatch(typeof(SpringManAI), "SetAnimationGoClientRpc")]
        [HarmonyPostfix]
        [ClientRpc]
        public static void PlaySekibankiTheme(SpringManAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableCoilHeadReplace.Value || !TouhouEnemiesPlugin.EnableSekibankiTheme.Value ||
                TouhouEnemiesPlugin.SekibankiTheme == null) return;
            if (__instance.creatureSFX.clip != TouhouEnemiesPlugin.SekibankiTheme)
            {
                __instance.creatureSFX.clip = TouhouEnemiesPlugin.SekibankiTheme;
                __instance.creatureSFX.volume = TouhouEnemiesPlugin.SekibankiSFXVolume.Value;
                __instance.creatureSFX.loop = true;
            }

            if (__instance.creatureSFX.isPlaying)
            {
                __instance.creatureSFX.UnPause();
            }
            else
            {
                __instance.creatureSFX.Play();
            }

            WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.SekibankiTheme,
                TouhouEnemiesPlugin.SekibankiSFXVolume.Value / 2);

            TouhouEnemiesPlugin.Instance.AddLog($"Playing Sekibanki theme.");
        }

        [HarmonyPatch(typeof(SpringManAI), "SetAnimationStopClientRpc")]
        [HarmonyPostfix]
        [ClientRpc]
        public static void PauseSekibankiTheme(SpringManAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableCoilHeadReplace.Value || !TouhouEnemiesPlugin.EnableSekibankiTheme.Value ||
                TouhouEnemiesPlugin.SekibankiTheme == null || !__instance.creatureSFX.isPlaying) return;
            __instance.creatureSFX.Pause();
            TouhouEnemiesPlugin.Instance.AddLog($"Pause Sekibanki theme.");
        }

        [HarmonyPatch(typeof(NutcrackerEnemyAI), "Start")]
        [HarmonyPostfix]
        public static void ReplaceSatoriTheme(NutcrackerEnemyAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableNutcrackerReplace.Value || !TouhouEnemiesPlugin.EnableSatoriTheme.Value ||
                TouhouEnemiesPlugin.SatoriTheme == null) return;
            __instance.enemyBehaviourStates[2].VoiceClip = TouhouEnemiesPlugin.SatoriTheme;
            __instance.creatureSFX.volume = TouhouEnemiesPlugin.SatoriSFXVolume.Value;
            TouhouEnemiesPlugin.Instance.AddLog($"Satori theme Loaded.");
        }

        [HarmonyPatch(typeof(EnemyAI), "SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        [ClientRpc]
        public static void ReplaceSuikaTheme(int stateIndex, EnemyAI __instance)
        {
            if (__instance is not ForestGiantAI || !TouhouEnemiesPlugin.EnableForestGiantReplace.Value) return;
            if (TouhouEnemiesPlugin.EnableSuikaVoice.Value)
            {
                switch (stateIndex)
                {
                    case 0:
                        if (TouhouEnemiesPlugin.SuikaAudios[0] != null)
                        {
                            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[0],
                                TouhouEnemiesPlugin.SuikaVoiceVolume.Value);
                            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice,
                                TouhouEnemiesPlugin.SuikaAudios[0], TouhouEnemiesPlugin.SuikaVoiceVolume.Value / 2);
                        }

                        break;
                    case 1:
                        __instance.creatureVoice.volume = TouhouEnemiesPlugin.SuikaVoiceVolume.Value;
                        if (TouhouEnemiesPlugin.SuikaAudios[2] != null)
                        {
                            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[2],
                                TouhouEnemiesPlugin.SuikaVoiceVolume.Value);
                            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice,
                                TouhouEnemiesPlugin.SuikaAudios[2],
                                TouhouEnemiesPlugin.SuikaVoiceVolume.Value / 2);
                        }

                        break;
                    case 2:
                        if (TouhouEnemiesPlugin.SuikaAudios[1] != null && !TouhouEnemiesPlugin.SoundToolGood)
                        {
                            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[1],
                                TouhouEnemiesPlugin.SuikaVoiceVolume.Value);
                            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice,
                                TouhouEnemiesPlugin.SuikaAudios[1],
                                TouhouEnemiesPlugin.SuikaVoiceVolume.Value / 2);
                        }

                        break;
                    default:
                        TouhouEnemiesPlugin.Instance.AddLog($"Unexpected state for Suika voice.");
                        break;
                }
            }

            if (!TouhouEnemiesPlugin.EnableSuikaTheme.Value || TouhouEnemiesPlugin.SuikaTheme == null) return;
            switch (stateIndex)
            {
                case 0:
                    __instance.creatureSFX.Stop();
                    TouhouEnemiesPlugin.Instance.AddLog($"Stop Suika theme.");
                    break;
                case 1:
                    {
                        if (__instance.creatureSFX.clip != TouhouEnemiesPlugin.SuikaTheme)
                        {
                            __instance.creatureSFX.clip = TouhouEnemiesPlugin.SuikaTheme;
                            __instance.creatureSFX.volume = TouhouEnemiesPlugin.SuikaThemeVolume.Value;
                            __instance.creatureSFX.loop = true;
                            __instance.creatureSFX.spatialBlend = 1;
                        }

                        if (__instance.creatureSFX.isPlaying)
                        {
                            __instance.creatureSFX.UnPause();
                        }
                        else
                        {
                            __instance.creatureSFX.Play();
                        }

                        WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.SuikaTheme,
                            TouhouEnemiesPlugin.SuikaThemeVolume.Value / 2);

                        TouhouEnemiesPlugin.Instance.AddLog($"Playing Suika theme.");
                        break;
                    }
                case 2:
                    __instance.creatureSFX.Pause();
                    TouhouEnemiesPlugin.Instance.AddLog($"Pause Suika theme.");
                    break;
                default:
                    TouhouEnemiesPlugin.Instance.AddLog($"Unexpected state for Suika theme.");
                    break;
            }
        }

        [HarmonyPatch(typeof(ForestGiantAI), "BeginEatPlayer")]
        [HarmonyPrefix]
        public static void SuikaEat(ForestGiantAI __instance)
        {
            __instance.creatureSFX.Pause();
            if (!TouhouEnemiesPlugin.EnableForestGiantReplace.Value || !TouhouEnemiesPlugin.EnableSuikaVoice.Value ||
                TouhouEnemiesPlugin.SuikaAudios[3] == null || TouhouEnemiesPlugin.SoundToolGood) return;
            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[3],
                TouhouEnemiesPlugin.SuikaVoiceVolume.Value);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[3],
                TouhouEnemiesPlugin.SuikaVoiceVolume.Value / 2);
        }

        [HarmonyPatch(typeof(ForestGiantAI), "KillEnemy")]
        [HarmonyPrefix]
        public static void SuikaLost(ForestGiantAI __instance)
        {
            __instance.creatureSFX.Stop();
            if (!TouhouEnemiesPlugin.EnableForestGiantReplace.Value) return;
            if (TouhouEnemiesPlugin.SuikaVisuals != null)
            {
                var suikaMeshRenderer = __instance.transform.Find("SuikaGiant(Clone)/FGiantModelContainer/BodyLOD0")
                    .GetComponent<SkinnedMeshRenderer>();
                suikaMeshRenderer.SetBlendShapeWeight(3, 100);
                suikaMeshRenderer.SetBlendShapeWeight(5, 100);
                TouhouEnemiesPlugin.Instance.AddLog($"Suika lost.");
            }

            if (TouhouEnemiesPlugin.EnableSuikaVoice.Value && TouhouEnemiesPlugin.SuikaAudios[1] != null &&
                __instance.giantCry != TouhouEnemiesPlugin.SuikaAudios[1])
            {
                __instance.giantCry = TouhouEnemiesPlugin.SuikaAudios[1];
            }

            if (TouhouEnemiesPlugin.EnableDeathAudio.Value && TouhouEnemiesPlugin.LostAudio != null)
            {
                __instance.creatureSFX.PlayOneShot(TouhouEnemiesPlugin.LostAudio,
                    TouhouEnemiesPlugin.DeathAudioVolume.Value);
                WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.LostAudio,
                    TouhouEnemiesPlugin.DeathAudioVolume.Value / 2);
            }
        }

        [HarmonyPatch(typeof(HoarderBugAI), "Start")]
        [HarmonyPostfix]
        public static void ReplaceMarisaTheme(HoarderBugAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableHoarderBugReplace.Value) return;

            if (__instance.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText ==
                "HoarderMarisa")
            {
                if (TouhouEnemiesPlugin.EnableMarisaVoice.Value)
                {
                    if (TouhouEnemiesPlugin.MarisaAudios[0] != null)
                        __instance.chitterSFX[0] = TouhouEnemiesPlugin.MarisaAudios[0];
                    if (TouhouEnemiesPlugin.MarisaAudios[3] != null)
                        __instance.chitterSFX[1] = TouhouEnemiesPlugin.MarisaAudios[3];
                    if (TouhouEnemiesPlugin.MarisaAudios[7] != null)
                        __instance.chitterSFX[2] = TouhouEnemiesPlugin.MarisaAudios[7];
                    if (TouhouEnemiesPlugin.MarisaAudios[0] != null)
                        __instance.angryScreechSFX[0] = TouhouEnemiesPlugin.MarisaAudios[0];
                    if (TouhouEnemiesPlugin.MarisaAudios[7] != null)
                        __instance.angryScreechSFX[1] = TouhouEnemiesPlugin.MarisaAudios[7];
                    if (TouhouEnemiesPlugin.MarisaAudios[1] != null)
                        __instance.angryVoiceSFX = TouhouEnemiesPlugin.MarisaAudios[1];
                    __instance.creatureVoice.loop = false;
                    __instance.creatureVoice.volume = TouhouEnemiesPlugin.MarisaVoiceVolume.Value;
                }

                if (!TouhouEnemiesPlugin.EnableMarisaTheme.Value ||
                    TouhouEnemiesPlugin.MarisaTheme == null) return;
                __instance.bugFlySFX = TouhouEnemiesPlugin.MarisaTheme;
                __instance.creatureSFX.volume = TouhouEnemiesPlugin.MarisaSFXVolume.Value;
            }
            else if (__instance.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText ==
                     "KirisameBug")
            {
                if (TouhouEnemiesPlugin.EnableBugVoice.Value)
                {
                    if (TouhouEnemiesPlugin.BugAudios[0] != null)
                        __instance.chitterSFX[0] = TouhouEnemiesPlugin.BugAudios[0];
                    if (TouhouEnemiesPlugin.BugAudios[3] != null)
                        __instance.chitterSFX[1] = TouhouEnemiesPlugin.BugAudios[3];
                    if (TouhouEnemiesPlugin.BugAudios[5] != null)
                        __instance.chitterSFX[2] = TouhouEnemiesPlugin.BugAudios[5];
                    if (TouhouEnemiesPlugin.BugAudios[0] != null)
                        __instance.angryScreechSFX[0] = TouhouEnemiesPlugin.BugAudios[0];
                    if (TouhouEnemiesPlugin.BugAudios[5] != null)
                        __instance.angryScreechSFX[1] = TouhouEnemiesPlugin.BugAudios[5];
                    if (TouhouEnemiesPlugin.BugAudios[1] != null)
                        __instance.angryVoiceSFX = TouhouEnemiesPlugin.BugAudios[1];
                    __instance.creatureVoice.loop = false;
                    __instance.creatureVoice.volume = TouhouEnemiesPlugin.MarisaVoiceVolume.Value;
                }
            }

            TouhouEnemiesPlugin.Instance.AddLog($"Marisa theme Loaded.");
        }

        [HarmonyPatch(typeof(HoarderBugAI), "GrabItem")]
        [HarmonyPostfix]
        public static void MarisaGrab(HoarderBugAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableHoarderBugReplace.Value) return;

            if (__instance.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText ==
                "HoarderMarisa" && TouhouEnemiesPlugin.EnableMarisaVoice.Value &&
                TouhouEnemiesPlugin.MarisaAudios[4] != null)
            {
                __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.MarisaAudios[4],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value);
                WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.MarisaAudios[4],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value / 2);
            }
            else if (__instance.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText ==
                     "KirisameBug" && TouhouEnemiesPlugin.EnableBugVoice.Value &&
                     TouhouEnemiesPlugin.BugAudios[4] != null)
            {
                __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.BugAudios[4],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value);
                WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.BugAudios[4],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value / 2);
            }
        }

        [HarmonyPatch(typeof(HoarderBugAI), "HitEnemy")]
        [HarmonyPostfix]
        public static void MarisaHit(HoarderBugAI __instance)
        {
            if (__instance.isEnemyDead || !TouhouEnemiesPlugin.EnableHoarderBugReplace.Value ||
                __instance.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText !=
                "HoarderMarisa" || !TouhouEnemiesPlugin.EnableMarisaVoice.Value ||
                TouhouEnemiesPlugin.MarisaAudios[5] == null) return;

            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.MarisaAudios[5],
                TouhouEnemiesPlugin.MarisaVoiceVolume.Value);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.MarisaAudios[5],
                TouhouEnemiesPlugin.MarisaVoiceVolume.Value / 2);
        }

        [HarmonyPatch(typeof(HoarderBugAI), "KillEnemy")]
        [HarmonyPostfix]
        public static void MarisaLost(HoarderBugAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableHoarderBugReplace.Value) return;

            if (__instance.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText ==
                "HoarderMarisa" && TouhouEnemiesPlugin.EnableMarisaVoice.Value &&
                TouhouEnemiesPlugin.MarisaAudios[2] != null)
            {
                __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.MarisaAudios[2],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value);
                WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[2],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value / 2);
            }
            else if (__instance.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText ==
                     "KirisameBug" && TouhouEnemiesPlugin.EnableBugVoice.Value &&
                     TouhouEnemiesPlugin.BugAudios[2] != null)
            {
                __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.BugAudios[2],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value);
                WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.BugAudios[2],
                    TouhouEnemiesPlugin.MarisaVoiceVolume.Value / 2);
            }

            if (TouhouEnemiesPlugin.EnableDeathAudio.Value && TouhouEnemiesPlugin.LostAudio != null)
            {
                __instance.creatureSFX.PlayOneShot(TouhouEnemiesPlugin.LostAudio,
                    TouhouEnemiesPlugin.DeathAudioVolume.Value);
                WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.LostAudio,
                    TouhouEnemiesPlugin.DeathAudioVolume.Value / 2);
            }
        }

        [HarmonyPatch(typeof(SandWormAI), "Start")]
        [HarmonyPostfix]
        public static void ReplaceYuyukoTheme(SandWormAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableSandWormReplace.Value || !TouhouEnemiesPlugin.EnableYuyukoTheme.Value ||
                TouhouEnemiesPlugin.YuyukoTheme == null) return;
            __instance.emergeFromGroundSFX = TouhouEnemiesPlugin.YuyukoTheme;
            __instance.creatureSFX.volume = TouhouEnemiesPlugin.YuyukoSFXVolume.Value;
            TouhouEnemiesPlugin.Instance.AddLog($"Yuyuko theme Loaded.");
        }

        #endregion
    }

    [HarmonyPatch]
    public static class V50Patches
    {
        static bool Prepare()
        {
            var originalMethod = typeof(RoundManager).GetMethod("SpawnNestObjectForOutsideEnemy",
                BindingFlags.Public | BindingFlags.Instance);
            return originalMethod != null;
        }

        [HarmonyPatch(typeof(EnemyAI), "Start")]
        [HarmonyPrefix]
        public static void ReplaceUtsuhoTheme(EnemyAI __instance)
        {
            try
            {
                switch (__instance)
                {
                    case RadMechAI ai when TouhouEnemiesPlugin.UtsuhoVisuals != null &&
                                           TouhouEnemiesPlugin.EnableRadMechReplace.Value:
                        try
                        {
                            var radMechMesh = ai.transform.Find("MeshContainer");
                            if(radMechMesh == null) return;
                            var radMechAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = radMechAnimator.runtimeAnimatorController;

                            var body = radMechMesh.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
                            radMechMesh.Find("ScanNode (1)").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                "OldUtsuho";
                            var radMechMetaRig = radMechMesh.Find("AnimContainer").Find("metarig");
                            if (radMechMetaRig == null) return;
                            radMechMetaRig.name = "old-metarig";

                            var utsuhoVisual = Object.Instantiate(TouhouEnemiesPlugin.UtsuhoVisuals);
                            utsuhoVisual.transform.SetParent(ai.transform);
                            utsuhoVisual.transform.localPosition = Vector3.zero;
                            utsuhoVisual.transform.localRotation = Quaternion.identity;
                            utsuhoVisual.transform.localScale = Vector3.one;

                            var armature = utsuhoVisual.transform.Find("MeshContainer/AnimContainer");
                            var utsuhoMetarig = armature.transform.Find("metarig");
                            utsuhoMetarig.SetParent(radMechMetaRig.parent, true);
                            utsuhoMetarig.transform.localScale = radMechMetaRig.transform.localScale;
                            utsuhoMetarig.transform.localRotation = radMechMetaRig.transform.localRotation;
                            utsuhoMetarig.transform.localPosition = radMechMetaRig.transform.localPosition;

                            TouhouEnemiesPlugin.Instance.AddLog($"RadMech model changed.");

                            //ai.ownCollider = utsuhoVisual.GetComponent<Collider>();

                            ai.torsoContainer = utsuhoMetarig.transform.Find("TorsoContainer");

                            ai.torsoDefaultRotation.SetParent(utsuhoMetarig);
                            var torsoDefaultRotation = utsuhoMetarig.transform.Find("TorsoDefaultRotation0");
                            ai.torsoDefaultRotation.localScale = torsoDefaultRotation.localScale;
                            ai.torsoDefaultRotation.localRotation = torsoDefaultRotation.localRotation;
                            ai.torsoDefaultRotation.localPosition = torsoDefaultRotation.localPosition;
                            var voice = radMechMetaRig.transform.Find("VoiceSFX");
                            voice.SetParent(utsuhoMetarig);
                            var lrad = radMechMetaRig.transform.Find("3DLradAudio");
                            lrad.SetParent(utsuhoMetarig);
                            var lrad2 = radMechMetaRig.transform.Find("3DLradAudio2");
                            lrad2.SetParent(utsuhoMetarig);
                            var engine = radMechMetaRig.transform.Find("EngineSFX");
                            engine.SetParent(utsuhoMetarig);
                            var creature = radMechMetaRig.transform.Find("CreatureSFX");
                            creature.SetParent(utsuhoMetarig);

                            var radTorso = radMechMetaRig.transform.Find("TorsoContainer/Torso");
                            var radEye = radTorso.transform.Find("FlyingModeEye");
                            var radLightContainer = radTorso.transform.Find("LightContainer");
                            var spotlightOn = radTorso.transform.Find("SpotlightOnAudio");
                            var torso = utsuhoMetarig.transform.Find("TorsoContainer/Torso");
                            radEye.SetParent(torso);
                            radLightContainer.SetParent(torso);
                            spotlightOn.SetParent(torso);
                            TouhouEnemiesPlugin.Instance.AddLog($"Collider: {ai.ownCollider != null}, " +
                                                                $"Torso: {ai.torsoContainer != null}, " +
                                                                $"TorsoRotation: {ai.torsoDefaultRotation != null}");

                            var legLeft2 = utsuhoMetarig.transform.Find("LegLeft/LegLeft2");
                            var radLegLeft = radMechMetaRig.transform.Find("LegLeft/LegLeft2");
                            var shockwaveLeft = radLegLeft.transform.Find("ShockwaveLeftFoot");
                            shockwaveLeft.SetParent(legLeft2);
                            var smokeTrail = radLegLeft.transform.Find("SmokeTrail");
                            smokeTrail.SetParent(legLeft2);
                            var leftLegFire = radLegLeft.transform.Find("LeftLegFireContainer");
                            leftLegFire.SetParent(legLeft2);
                            ai.leftFootPoint.localScale = leftLegFire.localScale;
                            ai.leftFootPoint.localRotation = leftLegFire.localRotation;
                            ai.leftFootPoint.localPosition = leftLegFire.localPosition;
                            TouhouEnemiesPlugin.Instance.AddLog($"LeftFoot: {ai.leftFootPoint != null}");

                            var legRight2 = utsuhoMetarig.transform.Find("LegRight/LegRight2");
                            var radLegRight = radMechMetaRig.transform.Find("LegRight/LegRight2");
                            var shockwaveRight = radLegRight.transform.Find("ShockwaveRightFoot");
                            shockwaveRight.SetParent(legRight2);
                            var smokeTrailRight = radLegRight.transform.Find("SmokeTrailRightLeg");
                            smokeTrailRight.SetParent(legRight2);
                            var rightLegFire = radLegRight.transform.Find("RightLegFireContainer");
                            rightLegFire.SetParent(legRight2);
                            ai.rightFootPoint.localScale = rightLegFire.localScale;
                            ai.rightFootPoint.localRotation = rightLegFire.localRotation;
                            ai.rightFootPoint.localPosition = rightLegFire.localPosition;
                            TouhouEnemiesPlugin.Instance.AddLog($"RightFoot: {ai.rightFootPoint != null}");

                            var leftUpperArm = torso.transform.Find("LeftUpperArm");
                            var gunArmDefaultRot = leftUpperArm.transform.Find("GunArmDefaultRot0");
                            var radLeftUpperArm = radTorso.transform.Find("LeftUpperArm");
                            var radGunArmContainer = radLeftUpperArm.transform.Find("GunArmContainer");
                            var radLeftLowerArm = radGunArmContainer.transform.Find("LeftLowerArm");
                            ai.defaultArmRotation.SetParent(leftUpperArm);
                            ai.defaultArmRotation.localScale = gunArmDefaultRot.localScale;
                            ai.defaultArmRotation.localRotation = gunArmDefaultRot.localRotation;
                            ai.defaultArmRotation.localPosition = gunArmDefaultRot.localPosition;
                            TouhouEnemiesPlugin.Instance.AddLog($"GunArmRotation: {ai.defaultArmRotation != null}");
                            var gunArmContainer = leftUpperArm.transform.Find("GunArmContainer");
                            ai.gunArm = gunArmContainer;
                            TouhouEnemiesPlugin.Instance.AddLog($"GunArm: {ai.gunArm != null}");
                            var leftLowerArm = gunArmContainer.transform.Find("LeftLowerArm");
                            var blastParticle = radLeftLowerArm.transform.Find("BlastParticle");
                            var blowtorchAudio = radLeftLowerArm.transform.Find("BlowtorchAudio");
                            var flamethrower = radLeftLowerArm.transform.Find("FlamethrowerParticle");
                            TouhouEnemiesPlugin.Instance.AddLog($"blastParticle: {blastParticle != null}");
                            TouhouEnemiesPlugin.Instance.AddLog($"blowtorchAudio: {blowtorchAudio != null}");
                            TouhouEnemiesPlugin.Instance.AddLog($"flamethrower: {flamethrower != null}");
                            blastParticle.SetParent(leftLowerArm);
                            blowtorchAudio.SetParent(leftLowerArm);
                            flamethrower.SetParent(leftLowerArm);
                            ai.gunPoint.SetParent(leftLowerArm);
                            var gunPoint = leftLowerArm.transform.Find("GunPoint0");
                            ai.gunPoint.localScale = gunPoint.localScale;
                            ai.gunPoint.localRotation = gunPoint.localRotation;
                            ai.gunPoint.localPosition = gunPoint.localPosition;
                            TouhouEnemiesPlugin.Instance.AddLog($"GunPoint: {ai.gunPoint != null}");

                            var rightLowerArm = torso.transform.Find("RightUpperArm/RightLowerarm");
                            ai.holdPlayerPoint.SetParent(rightLowerArm);
                            var holdPlayerPoint = rightLowerArm.transform.Find("GrabPlayerPoint0");
                            ai.holdPlayerPoint.localScale = holdPlayerPoint.localScale;
                            ai.holdPlayerPoint.localRotation = holdPlayerPoint.localRotation;
                            ai.holdPlayerPoint.localPosition = holdPlayerPoint.localPosition;
                            var radClawTrigger =
                                radMechMetaRig.transform.Find(
                                    "TorsoContainer/Torso/RightUpperArm/RightLowerarm/ClawTrigger");
                            radClawTrigger.SetParent(rightLowerArm);

                            if (body != null) body.enabled = false;

                            TouhouEnemiesPlugin.Instance.AddLog($"HoldPoint: {ai.holdPlayerPoint != null}");

                            var utsuhoAnimator = armature.transform.GetComponent<Animator>();
                            AnimatorOverrideController overrideController = new(originalController);

                            Dictionary<string, AnimationClip> replacementClips = [];
                            foreach (var clip in utsuhoAnimator.runtimeAnimatorController.animationClips)
                            {
                                replacementClips[clip.name] = clip;
                                //TouhouEnemiesPlugin.Instance.AddLog($"Get clip: {clip.name}");
                            }

                            utsuhoAnimator.enabled = false;
                            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = [];
                            overrideController.GetOverrides(overrides);

                            for (var i = 0; i < overrides.Count; i++)
                            {
                                var originalClip = overrides[i].Key;
                                if (!replacementClips.TryGetValue(originalClip.name, out var newClip)) continue;
                                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(originalClip, newClip);
                                //TouhouEnemiesPlugin.Instance.AddLog($"Replace clip: {newClip.name}");
                            }
                            overrideController.ApplyOverrides(overrides);
                            radMechAnimator.runtimeAnimatorController = overrideController;

                            TouhouEnemiesPlugin.Instance.AddLog($"The animator is replaced.");


                            if (!TouhouEnemiesPlugin.EnableUtsuhoTheme.Value ||
                                TouhouEnemiesPlugin.UtsuhoTheme == null) return;
                            ai.LocalLRADAudio.clip = TouhouEnemiesPlugin.UtsuhoTheme;
                            ai.creatureSFX.volume = TouhouEnemiesPlugin.UtsuhoSFXVolume.Value;
                            TouhouEnemiesPlugin.Instance.AddLog($"Utsuho theme Loaded.");
                        }
                        catch
                        {
                            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace RadMech.");
                        }

                        break;
                    case ButlerEnemyAI ai:
                        return;
                }
            }
            catch
            {
                TouhouEnemiesPlugin.Instance.AddLog($"Maybe lower than V50, ignore.");
            }
        }

        [HarmonyPatch(typeof(RoundManager), "SpawnNestObjectForOutsideEnemy")]
        [HarmonyPostfix]
        public static void ReplaceUtsuhoSpawnNest(RoundManager __instance, EnemyType enemyType)
        {
            if (enemyType.enemyName != "RadMech" || TouhouEnemiesPlugin.UtsuhoNestSpawnVisuals == null || !TouhouEnemiesPlugin.EnableRadMechReplace.Value) return;
            var radMechMesh = __instance.enemyNestSpawnObjects.Last().transform.Find("MeshContainer");
            if (radMechMesh == null) return;
            var body = radMechMesh.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
            if (body != null) body.enabled = false;
            radMechMesh.Find("ScanNode (1)").gameObject.GetComponent<ScanNodeProperties>().headerText =
                "Old-Bird Utsuho";
            var radMechMetaRig = radMechMesh?.Find("AnimContainer").Find("metarig");
            if (radMechMetaRig == null) return;
            radMechMetaRig.name = "old-metarig";

            var utsuhoVisual = Object.Instantiate(TouhouEnemiesPlugin.UtsuhoNestSpawnVisuals);
            utsuhoVisual.transform.SetParent(radMechMesh);
            utsuhoVisual.transform.localPosition = Vector3.zero;
            utsuhoVisual.transform.localRotation = Quaternion.identity;
            utsuhoVisual.transform.localScale = Vector3.one;

            var utsuhoArmature = utsuhoVisual.transform.Find("MeshContainer/Armature");
            var utsuhoMetarig = utsuhoArmature.transform.Find("metarig");
            utsuhoArmature.SetParent(radMechMesh, true);
            utsuhoMetarig.transform.localScale = radMechMetaRig.transform.localScale;
            utsuhoMetarig.transform.localRotation = radMechMetaRig.transform.localRotation;
            utsuhoMetarig.transform.localPosition = radMechMetaRig.transform.localPosition;

            TouhouEnemiesPlugin.Instance.AddLog($"RadMechNestSpawnObject model changed.");
        }
    }
}