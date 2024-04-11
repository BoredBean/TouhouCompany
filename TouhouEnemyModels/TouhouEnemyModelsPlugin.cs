using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TouhouEnemyModels
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class TouhouEnemiesPlugin : BaseUnityPlugin
    {
        public static TouhouEnemiesPlugin Instance;

        public static AudioClip DeadAudio;

        public static GameObject sekiVisuals;
        public static GameObject sekiHeadVisuals;
        internal static ConfigEntry<bool> EnableCoilHeadReplace;
        internal static ConfigEntry<bool> EnableBodyCoilReplace;
        internal static ConfigEntry<bool> EnableSekibankiTheme;
        public static AudioClip SekibankiTheme;

        public static GameObject satoriVisuals;
        internal static ConfigEntry<bool> EnableNutcrackerReplace;
        internal static ConfigEntry<bool> EnableSatoriTheme;
        public static AudioClip SatoriTheme;

        public static GameObject suikaVisuals;
        internal static ConfigEntry<bool> EnableForestGiantReplace;
        internal static ConfigEntry<bool> EnableSuikaTheme;
        public static AudioClip SuikaTheme;
        public static AudioClip[] SuikaAudios = new AudioClip[4];

        public static GameObject marisaVisuals;
        internal static ConfigEntry<bool> EnableHoarderBugReplace;
        internal static ConfigEntry<bool> EnableMarisaTheme;
        public static AudioClip MarisaTheme;
        public static AudioClip[] MarisaAudios = new AudioClip[8];

        public static GameObject utsuhoVisuals;
        internal static ConfigEntry<bool> EnableRadMechReplace;
        internal static ConfigEntry<bool> EnableUtsuhoTheme;
        public static AudioClip UtsuhoTheme;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");

            EnableCoilHeadReplace = Config.Bind("CoilHead", "EnableCoilHead", true,
                "Replace the model of Coil-Head to Sekibanki.");
            EnableBodyCoilReplace = Config.Bind("CoilHead", "EnableDeadBodyCoil", true,
                "Replace the coil on the dead body to a head of Sekibanki.");
            EnableSekibankiTheme = Config.Bind("CoilHead", "EnableSekibankiTheme", true,
                "Replace the step audio to Sekibanki theme music.");
            EnableNutcrackerReplace = Config.Bind("Nutcracker", "EnableNutcracker", true,
                "Replace the model of Nutcracker to NutSatori.");
            EnableSatoriTheme = Config.Bind("Nutcracker", "EnableSatoriTheme", true,
                "Replace the angry audio to Satori theme music.");
            EnableForestGiantReplace = Config.Bind("ForestGiant", "EnableForestGiant", true,
                "Replace the model of ForestGiant to NutSatori.");
            EnableSuikaTheme = Config.Bind("ForestGiant", "EnableSuikaTheme", true, "Play Suika theme music.");
            EnableHoarderBugReplace = Config.Bind("HoarderBug", "EnableHoarderBug", true,
                "Replace the model of HoarderBug to MarisaBug.");
            EnableMarisaTheme = Config.Bind("HoarderBug", "EnableMarisaTheme", true, "Play Marisa theme music.");
            EnableRadMechReplace = Config.Bind("RadMech", "EnableRadMech", true,
                "Replace the model of RadMech to UtsuhoMech.");
            EnableUtsuhoTheme = Config.Bind("RadMech", "EnableUtsuhoTheme", true, "Play Utsuho theme music.");


            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            try
            {
                var bundle =
                    AssetUtils.LoadAssetBundleFromResources("touhouenemies", typeof(TouhouEnemiesPlugin).Assembly);
                DeadAudio = bundle.LoadAsset<AudioClip>("se_pldead00.mp3");
                var componentsInChildren = new List<Renderer>();
                if (EnableCoilHeadReplace.Value)
                {
                    sekiVisuals = bundle.LoadAsset<GameObject>("Sekibanki.prefab");
                    componentsInChildren.AddRange(sekiVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load Sekibanki: {sekiVisuals != null}");

                    if (EnableSekibankiTheme.Value)
                    {
                        SekibankiTheme = bundle.LoadAsset<AudioClip>("SekibankiTheme.mp3");
                        Logger.LogInfo($"Load SekibankiTheme: {SekibankiTheme != null}");
                    }

                    if (EnableBodyCoilReplace.Value)
                    {
                        sekiHeadVisuals = bundle.LoadAsset<GameObject>("SekibankiHead.prefab");
                        Logger.LogInfo($"Load SekibankiHead: {sekiHeadVisuals != null}");
                    }
                }

                if (EnableNutcrackerReplace.Value)
                {
                    satoriVisuals = bundle.LoadAsset<GameObject>("NutSatori.prefab");
                    componentsInChildren.AddRange(satoriVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load NutSatori: {satoriVisuals != null}");

                    if (EnableSatoriTheme.Value)
                    {
                        SatoriTheme = bundle.LoadAsset<AudioClip>("SatoriTheme.mp3");
                        Logger.LogInfo($"Load SatoriTheme: {SatoriTheme != null}");
                    }

                }

                if (EnableForestGiantReplace.Value)
                {
                    suikaVisuals = bundle.LoadAsset<GameObject>("SuikaGiant.prefab");
                    componentsInChildren.AddRange(suikaVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load SuikaGiant: {suikaVisuals != null}");

                    if (EnableSuikaTheme.Value)
                    {
                        SuikaAudios[0] = bundle.LoadAsset<AudioClip>("SuikaAngry.ogg");
                        SuikaAudios[1] = bundle.LoadAsset<AudioClip>("SuikaDead.ogg");
                        SuikaAudios[2] = bundle.LoadAsset<AudioClip>("SuikaHappy.ogg");
                        SuikaAudios[3] = bundle.LoadAsset<AudioClip>("SuikaWarn.ogg");
                        SuikaTheme = bundle.LoadAsset<AudioClip>("SuikaTheme.mp3");
                        Logger.LogInfo($"Load SuikaTheme: {SuikaTheme != null}, SuikaAudio: {SuikaAudios[0] != null}");
                    }
                }

                if (EnableHoarderBugReplace.Value)
                {
                    marisaVisuals = bundle.LoadAsset<GameObject>("MarisaBug.prefab");
                    componentsInChildren.AddRange(marisaVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load MarisaBug: {marisaVisuals != null}");

                    if (EnableMarisaTheme.Value)
                    {
                        MarisaAudios[0] = bundle.LoadAsset<AudioClip>("MarisaAngry.ogg");
                        MarisaAudios[1] = bundle.LoadAsset<AudioClip>("MarisaAttack.ogg");
                        MarisaAudios[2] = bundle.LoadAsset<AudioClip>("MarisaDead.ogg");
                        MarisaAudios[3] = bundle.LoadAsset<AudioClip>("MarisaGreeting.ogg");
                        MarisaAudios[4] = bundle.LoadAsset<AudioClip>("MarisaHappy.ogg");
                        MarisaAudios[5] = bundle.LoadAsset<AudioClip>("MarisaHurt.ogg");
                        MarisaAudios[6] = bundle.LoadAsset<AudioClip>("MarisaMove.ogg");
                        MarisaAudios[7] = bundle.LoadAsset<AudioClip>("MarisaWarn.ogg");
                        MarisaTheme = bundle.LoadAsset<AudioClip>("MarisaTheme.mp3");
                        Logger.LogInfo($"Load MarisaTheme: {MarisaTheme != null}, MarisaAudio: {MarisaAudios[0] != null}");
                    }
                }

                if (EnableRadMechReplace.Value)
                {
                    utsuhoVisuals = bundle.LoadAsset<GameObject>("UtsuhoMech.prefab");
                    componentsInChildren.AddRange(utsuhoVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load UtsuhoMech: {utsuhoVisuals != null}");

                    if (EnableUtsuhoTheme.Value)
                    {
                        UtsuhoTheme = bundle.LoadAsset<AudioClip>("UtsuhoTheme.mp3");
                        Logger.LogInfo($"Load UtsuhoTheme: {UtsuhoTheme != null}");
                    }
                }

                foreach (var renderer in componentsInChildren)
                {
                    renderer.gameObject.layer = LayerMask.NameToLayer("Enemies");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Couldn't load asset bundle: " + ex.Message);
            }
        }

        public void AddLog(string info)
        {
            Logger.LogInfo(info);
        }
    }
}