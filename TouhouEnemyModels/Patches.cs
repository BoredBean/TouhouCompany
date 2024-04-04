using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace TouhouEnemyModels
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPatch(typeof(DeadBodyInfo), "Start")]
        internal class BodySpringPatch
        {
            private static void Postfix(DeadBodyInfo __instance)
            {
                if (__instance is null || TouhouEnemiesPlugin.sekiHeadVisuals == null ||
                    !TouhouEnemiesPlugin.EnableCoilHeadReplace.Value || !TouhouEnemiesPlugin.EnableBodyCoilReplace.Value) return;
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
                    TouhouEnemiesPlugin.Instance.AddLog($"Spring model changed");
                }
            }
        }

        [HarmonyPatch(typeof(EnemyAI), "Start")]
        internal class EnemyAIPatch
        {
            private static void Prefix(EnemyAI __instance)
            {
                switch (__instance)
                {
                    case SpringManAI when TouhouEnemiesPlugin.sekiVisuals != null &&
                                          TouhouEnemiesPlugin.EnableCoilHeadReplace.Value:
                        {
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
                            var sekiVisual = Object.Instantiate(TouhouEnemiesPlugin.sekiVisuals);
                            sekiVisual.transform.SetParent(springManModel);
                            sekiVisual.transform.localPosition = Vector3.zero;
                            sekiVisual.transform.localRotation = Quaternion.identity;
                            sekiVisual.transform.localScale = Vector3.one;
                            var sekiMesh = sekiVisual.transform.Find("SekibankiModel/Body");
                            var sekiMetarig = sekiVisual.transform.Find("SekibankiModel/metarig");
                            sekiMetarig.SetParent(springManMetaRig.parent, true);
                            sekiMetarig.transform.localScale = springManMetaRig.transform.localScale;
                            sekiMetarig.transform.localRotation = springManMetaRig.transform.localRotation;
                            sekiMetarig.transform.localPosition = new Vector3(0, -0.27f, 0);
                            var component = sekiMesh.GetComponent<SkinnedMeshRenderer>();
                            component.rootBone = sekiMetarig;
                            sekiMetarig.name = "metarig";

                            TouhouEnemiesPlugin.Instance.AddLog($"SpringMan model changed");

                            /* This reset the state of the animator? I have no idea. It just works. */
                            coilHeadAnimator.runtimeAnimatorController = new AnimatorOverrideController(originalController);

                            TouhouEnemiesPlugin.Instance.AddLog($"The state of the animator reset");
                            break;
                        }
                    case NutcrackerEnemyAI ai when TouhouEnemiesPlugin.satoriVisuals != null &&
                                                   TouhouEnemiesPlugin.EnableNutcrackerReplace.Value:
                        {
                            var nutcrackerAnimator = ai.GetComponentInChildren<Animator>();
                            var originalController = nutcrackerAnimator.runtimeAnimatorController;

                            var nutcrackerMesh = ai.transform.Find("MeshContainer");
                            var nutcrackerLod0 = nutcrackerMesh?.Find("LOD0")?.GetComponent<SkinnedMeshRenderer>();
                            var nutcrackerLod1 = nutcrackerMesh?.Find("LOD1")?.GetComponent<SkinnedMeshRenderer>();
                            var nutcrackerTestShotgun = nutcrackerMesh?.Find("AnimContainer/metarig/spinecontainer/GunAndArmsContainer/GunPoint/GunPointWithOffset/TestShotgun")?.GetComponent<MeshRenderer>();
                            var nutcrackerGunBarrel = nutcrackerMesh?.Find("AnimContainer/metarig/spinecontainer/GunAndArmsContainer/GunPoint/GunPointWithOffset/TestShotgun/GunBarrel")?.GetComponent<MeshRenderer>();
                            if (nutcrackerLod0 != null) nutcrackerLod0.enabled = false;
                            if (nutcrackerLod1 != null) nutcrackerLod1.enabled = false;
                            if (nutcrackerTestShotgun != null) nutcrackerTestShotgun.enabled = false;
                            if (nutcrackerGunBarrel != null) nutcrackerGunBarrel.enabled = false;
                            var nutcrackerMetaRig = nutcrackerMesh?.Find("AnimContainer").Find("metarig");
                            if (nutcrackerMetaRig == null) return;
                            nutcrackerMetaRig.name = "old-metarig";
                            var satoriVisual = Object.Instantiate(TouhouEnemiesPlugin.satoriVisuals);
                            satoriVisual.transform.SetParent(nutcrackerMesh);
                            satoriVisual.transform.localPosition = Vector3.zero;
                            satoriVisual.transform.localRotation = Quaternion.identity;
                            satoriVisual.transform.localScale = Vector3.one;
                            var satoriMesh = satoriVisual.transform.Find("MeshContainer/LOD0");
                            var satoriMetarig = satoriVisual.transform.Find("MeshContainer/AnimContainer/metarig");
                            ai.torsoContainer =
                                satoriVisual.transform.Find("MeshContainer/AnimContainer/metarig/spinecontainer");
                            ai.gunPoint = satoriVisual.transform.Find("MeshContainer/AnimContainer/metarig/spinecontainer/GunAndArmsContainer/GunPoint/GunPointWithOffset");
                            satoriMetarig.SetParent(nutcrackerMetaRig.parent, true);
                            satoriMetarig.transform.localScale = nutcrackerMetaRig.transform.localScale;
                            satoriMetarig.transform.localRotation = nutcrackerMetaRig.transform.localRotation;
                            satoriMetarig.transform.localPosition = nutcrackerMetaRig.transform.localPosition;
                            var component = satoriMesh.GetComponent<SkinnedMeshRenderer>();
                            component.rootBone = satoriMetarig;
                            satoriMetarig.name = "metarig";

                            TouhouEnemiesPlugin.Instance.AddLog($"Nutcracker model changed. Torsor: {ai.torsoContainer != null}, GunPoint: {ai.gunPoint != null}");

                            /* This reset the state of the animator? I have no idea. It just works. */
                            nutcrackerAnimator.runtimeAnimatorController =
                                new AnimatorOverrideController(originalController);

                            TouhouEnemiesPlugin.Instance.AddLog($"The state of the animator reset");
                            break;
                        }
                }
            }
        }

        [HarmonyPatch(typeof(SpringManAI), "SetAnimationGoClientRpc")]
        [HarmonyPostfix]
        [ClientRpc]
        public static void PlayWalkSounds(SpringManAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableCoilHeadReplace.Value || !TouhouEnemiesPlugin.EnableSekibankiTheme.Value ||
                TouhouEnemiesPlugin.SekibankiTheme == null) return;
            if (__instance.creatureSFX.clip != TouhouEnemiesPlugin.SekibankiTheme)
            {
                __instance.creatureSFX.clip = TouhouEnemiesPlugin.SekibankiTheme;
                __instance.creatureSFX.volume = 0.75f;
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
            TouhouEnemiesPlugin.Instance.AddLog($"Playing Sekibanki theme.");
            WalkieTalkie.TransmitOneShotAudio(__instance.creatureSFX, TouhouEnemiesPlugin.SekibankiTheme, 0.4f);
        }

        [HarmonyPatch(typeof(SpringManAI), "SetAnimationStopClientRpc")]
        [HarmonyPostfix]
        [ClientRpc]
        public static void StopWalkSounds(SpringManAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableCoilHeadReplace.Value || !TouhouEnemiesPlugin.EnableSekibankiTheme.Value ||
                TouhouEnemiesPlugin.SekibankiTheme == null || !__instance.creatureSFX.isPlaying) return;
            __instance.creatureSFX.Pause();
            TouhouEnemiesPlugin.Instance.AddLog($"Stop playing Sekibanki theme.");
        }

        [HarmonyPatch(typeof(NutcrackerEnemyAI), "Start")]
        [HarmonyPostfix]
        public static void ReplaceAudio(SpringManAI __instance)
        {
            if (!TouhouEnemiesPlugin.EnableNutcrackerReplace.Value || !TouhouEnemiesPlugin.EnableSatoriTheme.Value ||
                TouhouEnemiesPlugin.SatoriTheme == null) return;
            __instance.enemyBehaviourStates[2].VoiceClip = TouhouEnemiesPlugin.SatoriTheme;
            TouhouEnemiesPlugin.Instance.AddLog($"Satori theme Loaded.");
        }
    }
}
