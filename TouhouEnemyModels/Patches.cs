using HarmonyLib;
using System.Collections.Generic;
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
            if (__instance is null || TouhouEnemiesPlugin.sekiHeadVisuals == null ||
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
                    var sekiHeadVisual = Object.Instantiate(TouhouEnemiesPlugin.sekiHeadVisuals);
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
                case SpringManAI ai when TouhouEnemiesPlugin.sekiVisuals != null &&
                                         TouhouEnemiesPlugin.EnableCoilHeadReplace.Value:
                    {
                        try
                        {
                            var coilHeadAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = coilHeadAnimator.runtimeAnimatorController;

                            var springManModel = ai.transform.Find("SpringManModel");
                            var springManBody = (springManModel?.Find("Body"))?.GetComponent<SkinnedMeshRenderer>();
                            if (springManBody != null) springManBody.enabled = false;
                            var springManHead = (springManModel?.Find("Head"))?.GetComponent<MeshRenderer>();
                            if (springManHead != null) springManHead.enabled = false;
                            springManModel.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                "Sekibanki";
                            var springManMetaRig = springManModel?.Find("AnimContainer").Find("metarig");
                            if (springManMetaRig == null) return;
                            springManMetaRig.name = "old-metarig";
                            var sekiVisual = Object.Instantiate(TouhouEnemiesPlugin.sekiVisuals);
                            sekiVisual.transform.SetParent(ai.transform);
                            sekiVisual.transform.localPosition = Vector3.zero;
                            sekiVisual.transform.localRotation = Quaternion.identity;
                            sekiVisual.transform.localScale = Vector3.one;
                            var sekiModel = sekiVisual.transform.Find("SekibankiModel");
                            var sekiBody = sekiModel.transform.Find("Body");
                            var sekiHead = sekiModel.transform.Find("Head");
                            var sekiMetarig = sekiModel.transform.Find("metarig");
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

                            ai.mainCollider = sekiModel.GetComponent<Collider>();

                            TouhouEnemiesPlugin.Instance.AddLog($"Collider: {ai.mainCollider != null}");

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
                case NutcrackerEnemyAI ai when TouhouEnemiesPlugin.satoriVisuals != null &&
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
                            var satoriVisual = Object.Instantiate(TouhouEnemiesPlugin.satoriVisuals);
                            satoriVisual.transform.SetParent(ai.transform);
                            satoriVisual.transform.localPosition = Vector3.zero;
                            satoriVisual.transform.localRotation = Quaternion.identity;
                            satoriVisual.transform.localScale = Vector3.one;
                            var satoriMesh = satoriVisual.transform.Find("MeshContainer/LOD0");
                            var satoriMetarig = satoriVisual.transform.Find("MeshContainer/AnimContainer/metarig");
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
                case ForestGiantAI ai when TouhouEnemiesPlugin.suikaVisuals != null &&
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
                            var suikaVisual = Object.Instantiate(TouhouEnemiesPlugin.suikaVisuals);
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
                case HoarderBugAI ai when TouhouEnemiesPlugin.marisaVisuals != null &&
                                          TouhouEnemiesPlugin.EnableHoarderBugReplace.Value:
                    {
                        try
                        {
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
                            ai.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>().headerText =
                                "HoarderMarisa";
                            var hoarderBugArmature = hoarderBugModel?.Find("AnimContainer").Find("Armature");
                            if (hoarderBugArmature == null) return;
                            hoarderBugArmature.name = "old-Armature";
                            var marisaVisual = Object.Instantiate(TouhouEnemiesPlugin.marisaVisuals);
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

                            var marisaAnimator = marisaVisual.transform.Find("HoarderBugModel").GetComponent<Animator>();
                            ai.creatureAnimator = marisaAnimator;

                            TouhouEnemiesPlugin.Instance.AddLog($"The animator is replaced.");
                        }
                        catch
                        {
                            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace HoarderBug.");
                        }

                        break;
                    }
                //case RadMechAI ai when TouhouEnemiesPlugin.utsuhoVisuals != null &&
                //                       TouhouEnemiesPlugin.EnableRadMechReplace.Value:
                //    {
                //        try
                //        {
                //            var radMechMesh = ai.transform.Find("MeshContainer");
                //            var body = radMechMesh?.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
                //            if (body != null) body.enabled = false;
                //            radMechMesh.Find("ScanNode (1)").gameObject.GetComponent<ScanNodeProperties>().headerText = "Utsuho";
                //            var radMechMetaRig = radMechMesh?.Find("AnimContainer").Find("metarig");
                //            if (radMechMetaRig == null) return;
                //            radMechMetaRig.name = "old-metarig";

                //            var utsuhoVisual = Object.Instantiate(TouhouEnemiesPlugin.utsuhoVisuals);
                //            utsuhoVisual.transform.SetParent(radMechMesh);
                //            utsuhoVisual.transform.localPosition = Vector3.zero;
                //            utsuhoVisual.transform.localRotation = Quaternion.identity;
                //            utsuhoVisual.transform.localScale = Vector3.one;

                //            var utsuhoMesh = utsuhoVisual.transform.Find("MeshContainer/Body");
                //            var utsuhoMetarig = utsuhoVisual.transform.Find("MeshContainer/AnimContainer/metarig");
                //            utsuhoMetarig.SetParent(radMechMetaRig.parent, true);
                //            utsuhoMetarig.transform.localScale = radMechMetaRig.transform.localScale;
                //            utsuhoMetarig.transform.localRotation = radMechMetaRig.transform.localRotation;
                //            utsuhoMetarig.transform.localPosition = radMechMetaRig.transform.localPosition;
                //            var component = utsuhoMesh.GetComponent<SkinnedMeshRenderer>();
                //            TouhouEnemiesPlugin.Instance.AddLog($"mesh.");
                //            component.rootBone = utsuhoMetarig;
                //            utsuhoMetarig.name = "metarig";

                //            TouhouEnemiesPlugin.Instance.AddLog($"RadMech model changed.");

                //            ai.ownCollider = utsuhoVisual.GetComponent<Collider>();

                //            ai.torsoContainer = utsuhoMetarig.transform.Find("TorsoContainer");

                //            ai.torsoDefaultRotation.SetParent(utsuhoMetarig);
                //            ai.torsoDefaultRotation.localScale = ai.torsoContainer.localScale;
                //            ai.torsoDefaultRotation.localRotation = ai.torsoContainer.localRotation;
                //            ai.torsoDefaultRotation.localPosition = ai.torsoContainer.localPosition;

                //            var legLeft2 = utsuhoMetarig.transform.Find("LegLeft/LegLeft2");
                //            ai.leftFootPoint.SetParent(legLeft2);
                //            var leftFootPoint = legLeft2.transform.Find("LeftFootPoint");
                //            ai.leftFootPoint.localScale = leftFootPoint.localScale;
                //            ai.leftFootPoint.localRotation = leftFootPoint.localRotation;
                //            ai.leftFootPoint.localPosition = leftFootPoint.localPosition;

                //            var legRight2 = utsuhoMetarig.transform.Find("LegRight/LegRight2");
                //            ai.rightFootPoint.SetParent(legRight2);
                //            var rightFootPoint = legRight2.transform.Find("RightFootPoint");
                //            ai.rightFootPoint.localScale = rightFootPoint.localScale;
                //            ai.rightFootPoint.localRotation = rightFootPoint.localRotation;
                //            ai.rightFootPoint.localPosition = rightFootPoint.localPosition;

                //            var leftUpperArm = ai.torsoContainer.transform.Find("Torso/LeftUpperArm");
                //            ai.gunArm = leftUpperArm.transform.Find("GunArmContainer");
                //            ai.defaultArmRotation.SetParent(leftUpperArm);
                //            ai.defaultArmRotation.localScale = ai.gunArm.localScale;
                //            ai.defaultArmRotation.localRotation = ai.gunArm.localRotation;
                //            ai.defaultArmRotation.localPosition = ai.gunArm.localPosition;

                //            ai.gunPoint.SetParent(ai.gunArm);
                //            var gunPoint = ai.gunArm.transform.Find("GunPoint");
                //            ai.gunPoint.localScale = gunPoint.localScale;
                //            ai.gunPoint.localRotation = gunPoint.localRotation;
                //            ai.gunPoint.localPosition = gunPoint.localPosition;

                //            var rightLowerArm = ai.torsoContainer.transform.Find("Torso/RightUpperArm/RightLowerarm");
                //            ai.holdPlayerPoint.SetParent(rightLowerArm);
                //            var holdPlayerPoint = rightLowerArm.transform.Find("GrabPlayerPoint");
                //            ai.holdPlayerPoint.localScale = holdPlayerPoint.localScale;
                //            ai.holdPlayerPoint.localRotation = holdPlayerPoint.localRotation;
                //            ai.holdPlayerPoint.localPosition = holdPlayerPoint.localPosition;

                //            TouhouEnemiesPlugin.Instance.AddLog($"Collider: {ai.ownCollider != null}, " +
                //                                                $"Torso: {ai.torsoContainer != null}, " +
                //                                                $"TorsoRotation: {ai.torsoDefaultRotation != null}, " +
                //                                                $"LeftFoot: {ai.leftFootPoint != null}, " +
                //                                                $"RightFoot: {ai.rightFootPoint != null}, " +
                //                                                $"GunArm: {ai.gunArm != null}, " +
                //                                                $"GunArmRotation: {ai.defaultArmRotation != null}, " +
                //                                                $"GunPoint: {ai.gunPoint != null}, " +
                //                                                $"HoldPoint: {ai.holdPlayerPoint != null}");

                //            var utsuhoAnimator = utsuhoVisual.transform.Find("MeshContainer/AnimContainer").GetComponent<Animator>();
                //            ai.creatureAnimator = utsuhoAnimator;

                //            TouhouEnemiesPlugin.Instance.AddLog($"The animator is replaced.");
                //        }
                //        catch
                //        {
                //            TouhouEnemiesPlugin.Instance.AddLog($"Failed to replace RadMech.");
                //        }

                //        break;
                //    }
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
                __instance.creatureSFX.volume = 0.75f;
                //__instance.creatureSFX.loop = true;
            }

            if (__instance.creatureSFX.isPlaying)
            {
                __instance.creatureSFX.UnPause();
            }
            else
            {
                __instance.creatureSFX.Play();
            }

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
            TouhouEnemiesPlugin.Instance.AddLog($"Satori theme Loaded.");
        }

        [HarmonyPatch(typeof(EnemyAI), "SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        [ClientRpc]
        public static void ReplaceSuikaTheme(int stateIndex, EnemyAI __instance)
        {
            if (__instance is not ForestGiantAI || !TouhouEnemiesPlugin.EnableForestGiantReplace.Value ||
                !TouhouEnemiesPlugin.EnableSuikaTheme.Value ||
                TouhouEnemiesPlugin.SuikaTheme == null) return;
            switch (stateIndex)
            {
                case 0:
                    __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[0]);
                    WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[0],
                        0.4f);
                    __instance.creatureSFX.Stop();
                    TouhouEnemiesPlugin.Instance.AddLog($"Stop Suika theme.");
                    break;
                case 1:
                    {
                        if (__instance.creatureSFX.clip != TouhouEnemiesPlugin.SuikaTheme)
                        {
                            __instance.creatureSFX.clip = TouhouEnemiesPlugin.SuikaTheme;
                            __instance.creatureSFX.volume = 0.5f;
                            __instance.creatureSFX.loop = true;
                            __instance.creatureSFX.spatialBlend = 1;
                        }

                        __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[2]);
                        WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[2],
                            0.4f);
                        if (__instance.creatureSFX.isPlaying)
                        {
                            __instance.creatureSFX.UnPause();
                        }
                        else
                        {
                            __instance.creatureSFX.Play();
                        }

                        TouhouEnemiesPlugin.Instance.AddLog($"Playing Suika theme.");
                        WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.SuikaTheme, 0.3f);
                        break;
                    }
                case 2:
                    __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[0]);
                    WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[0],
                        0.4f);
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
            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.SuikaAudios[3]);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[3], 0.4f);
        }

        [HarmonyPatch(typeof(ForestGiantAI), "KillEnemy")]
        [HarmonyPrefix]
        public static void SuikaLost(ForestGiantAI __instance)
        {
            if (TouhouEnemiesPlugin.SuikaAudios != null && __instance.giantCry != TouhouEnemiesPlugin.SuikaAudios[1])
            {
                __instance.giantCry = TouhouEnemiesPlugin.SuikaAudios[1];
            }

            __instance.creatureSFX.Stop();
            __instance.creatureSFX.PlayOneShot(TouhouEnemiesPlugin.LostAudio, 0.5f);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.LostAudio, 0.2f);
            var suikaMeshRenderer = __instance.transform.Find("SuikaGiant(Clone)/FGiantModelContainer/BodyLOD0").GetComponent<SkinnedMeshRenderer>();
            suikaMeshRenderer.SetBlendShapeWeight(3, 100);
            suikaMeshRenderer.SetBlendShapeWeight(30, 100);
            TouhouEnemiesPlugin.Instance.AddLog($"Suika lost.");
        }

        [HarmonyPatch(typeof(HoarderBugAI), "Start")]
        [HarmonyPostfix]
        public static void ReplaceMarisaTheme(HoarderBugAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableHoarderBugReplace.Value ||
                !TouhouEnemiesPlugin.EnableMarisaTheme.Value ||
                TouhouEnemiesPlugin.MarisaTheme == null) return;
            __instance.chitterSFX[0] = TouhouEnemiesPlugin.MarisaAudios[0];
            __instance.chitterSFX[1] = TouhouEnemiesPlugin.MarisaAudios[3];
            __instance.chitterSFX[2] = TouhouEnemiesPlugin.MarisaAudios[7];
            __instance.angryScreechSFX[0] = TouhouEnemiesPlugin.MarisaAudios[0];
            __instance.angryScreechSFX[1] = TouhouEnemiesPlugin.MarisaAudios[7];
            __instance.angryVoiceSFX = TouhouEnemiesPlugin.MarisaAudios[1];
            __instance.bugFlySFX = TouhouEnemiesPlugin.MarisaTheme;
            __instance.creatureVoice.loop = false;
            TouhouEnemiesPlugin.Instance.AddLog($"Marisa theme Loaded.");
        }

        [HarmonyPatch(typeof(HoarderBugAI), "GrabItem")]
        [HarmonyPostfix]
        public static void MarisaGrab(HoarderBugAI __instance)
        {
            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.MarisaAudios[4]);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.MarisaAudios[4], 0.4f);
        }

        [HarmonyPatch(typeof(HoarderBugAI), "HitEnemy")]
        [HarmonyPostfix]
        public static void MarisaHit(HoarderBugAI __instance)
        {
            if (__instance.isEnemyDead) return;
            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.MarisaAudios[5]);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[5], 0.4f);
        }

        [HarmonyPatch(typeof(HoarderBugAI), "KillEnemy")]
        [HarmonyPrefix]
        public static void MarisaLost(HoarderBugAI __instance, ref float ___velX, ref float ___velZ)
        {
            __instance.creatureVoice.PlayOneShot(TouhouEnemiesPlugin.MarisaAudios[2]);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureVoice, TouhouEnemiesPlugin.SuikaAudios[2], 0.4f);
            __instance.creatureSFX.PlayOneShot(TouhouEnemiesPlugin.LostAudio, 0.5f);
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.LostAudio, 0.2f);
        }

        //[HarmonyPatch(typeof(RadMechAI), "Start")]
        //[HarmonyPostfix]
        //public static void ReplaceUtsuhoTheme(RadMechAI __instance)
        //{
        //    if (!TouhouEnemiesPlugin.EnableRadMechReplace.Value || !TouhouEnemiesPlugin.EnableUtsuhoTheme.Value ||
        //        TouhouEnemiesPlugin.UtsuhoTheme == null) return;
        //    __instance.LocalLRADAudio.clip = TouhouEnemiesPlugin.UtsuhoTheme;
        //    TouhouEnemiesPlugin.Instance.AddLog($"Utsuho theme Loaded.");
        //}

        #endregion
    }
}