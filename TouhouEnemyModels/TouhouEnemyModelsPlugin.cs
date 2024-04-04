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
        public static GameObject satoriVisuals;

        internal static ConfigEntry<bool> EnableCoilHeadReplace;
        internal static ConfigEntry<bool> EnableBodyCoilReplace;
        internal static ConfigEntry<bool> EnableSekibankiTheme;
        public static AudioClip SekibankiTheme;

        internal static ConfigEntry<bool> EnableNutcrackerReplace;
        internal static ConfigEntry<bool> EnableSatoriTheme;
        public static AudioClip SatoriTheme;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");

            EnableCoilHeadReplace = Config.Bind("CoilHead", "EnableCoilHead", true, "Replace the model of Coil-Head to Sekibanki.");
            EnableBodyCoilReplace = Config.Bind("CoilHead", "EnableDeadBodyCoil", true, "Replace the coil on the dead body to a head of Sekibanki.");
            EnableSekibankiTheme = Config.Bind("CoilHead", "EnableSekibankiTheme", true, "Replace the step audio to Sekibanki theme music.");
            EnableNutcrackerReplace = Config.Bind("Nutcracker", "EnableNutcracker", true, "Replace the model of Nutcracker to NutSatori.");
            EnableSatoriTheme = Config.Bind("Nutcracker", "EnableSatoriTheme", true, "Replace the angry audio to Satori theme music.");


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