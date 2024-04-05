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

        public static GameObject utsuhoVisuals;
        internal static ConfigEntry<bool> EnableRadMechReplace;
        internal static ConfigEntry<bool> EnableUtsuhoTheme;
        public static AudioClip UtsuhoTheme;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");

            EnableCoilHeadReplace = Config.Bind("CoilHead", "EnableCoilHead", true, "Replace the model of Coil-Head to Sekibanki.");
            EnableBodyCoilReplace = Config.Bind("CoilHead", "EnableDeadBodyCoil", true, "Replace the coil on the dead body to a head of Sekibanki.");
            EnableSekibankiTheme = Config.Bind("CoilHead", "EnableSekibankiTheme", true, "Replace the step audio to Sekibanki theme music.");
            EnableNutcrackerReplace = Config.Bind("Nutcracker", "EnableNutcracker", true, "Replace the model of Nutcracker to NutSatori.");
            EnableSatoriTheme = Config.Bind("Nutcracker", "EnableSatoriTheme", true, "Replace the angry audio to Satori theme music.");
            EnableForestGiantReplace = Config.Bind("ForestGiant", "EnableForestGiant", true, "Replace the model of ForestGiant to NutSatori.");
            EnableSuikaTheme = Config.Bind("ForestGiant", "EnableSuikaTheme", true, "Play Suika theme music.");
            EnableRadMechReplace = Config.Bind("RadMech", "EnableRadMech", true, "Replace the model of RadMech to UtsuhoMech.");
            EnableUtsuhoTheme = Config.Bind("RadMech", "EnableUtsuhoTheme", true, "Play Utsuho theme music.");


            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            try
            {
                var bundle = AssetUtils.LoadAssetBundleFromResources("touhouenemies", typeof(TouhouEnemiesPlugin).Assembly);
                var componentsInChildren = new List<Renderer>();
                if (EnableCoilHeadReplace.Value)
                {
                    sekiVisuals = bundle.LoadAsset<GameObject>("Sekibanki.prefab");
                    componentsInChildren.AddRange(sekiVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load Sekibanki: {sekiVisuals != null}");
                    SekibankiTheme = bundle.LoadAsset<AudioClip>("SekibankiTheme.mp3");
                    Logger.LogInfo($"Load SekibankiTheme: {SekibankiTheme != null}");
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
                    SatoriTheme = bundle.LoadAsset<AudioClip>("SatoriTheme.mp3");
                    Logger.LogInfo($"Load SatoriTheme: {SatoriTheme != null}");
                }
                if (EnableForestGiantReplace.Value)
                {
                    suikaVisuals = bundle.LoadAsset<GameObject>("SuikaGiant.prefab");
                    componentsInChildren.AddRange(suikaVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load SuikaGiant: {suikaVisuals != null}");
                    SuikaTheme = bundle.LoadAsset<AudioClip>("SuikaTheme.mp3");
                    Logger.LogInfo($"Load SuikaTheme: {SuikaTheme != null}");
                }
                if (EnableRadMechReplace.Value)
                {
                    utsuhoVisuals = bundle.LoadAsset<GameObject>("UtsuhoMech.prefab");
                    componentsInChildren.AddRange(utsuhoVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load UtsuhoMech: {utsuhoVisuals != null}");
                    UtsuhoTheme = bundle.LoadAsset<AudioClip>("UtsuhoTheme.mp3");
                    Logger.LogInfo($"Load UtsuhoTheme: {UtsuhoTheme != null}");
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