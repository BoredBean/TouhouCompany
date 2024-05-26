using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TouhouCompany
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class TouhouCompanyPlugin : BaseUnityPlugin
    {
        public static TouhouCompanyPlugin Instance;

        public static GameObject YukuriVisuals;
        public static AudioClip YukuriAudio;
        internal static ConfigEntry<bool> EnableYukuriReplace;
        internal static ConfigEntry<bool> EnableYukuriAudio;
        internal static ConfigEntry<float> YukuriAudioVolume;

        public static GameObject ShanghaiVisuals;
        internal static ConfigEntry<bool> EnableShanghaiReplace;

        public static GameObject MoriyaVisuals;
        public static AudioClip NitoriTheme;
        internal static ConfigEntry<bool> EnableMoriyaReplace;
        internal static ConfigEntry<bool> EnableNitoriTheme;
        internal static ConfigEntry<float> NitoriThemeVolume;

        public static GameObject HakureiVisuals;
        internal static ConfigEntry<bool> EnableHakureiReplace;
        public static GameObject SkyBoxVisuals;
        internal static ConfigEntry<bool> EnableSkyBoxReplace;

        public static GameObject KoumakanVisuals;
        internal static ConfigEntry<bool> EnableKoumakanReplace;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("""  ______ ____   __  __ __  __ ____   __  __""");
            Logger.LogInfo(""" /_  __// __ \ / / / // / / // __ \ / / / /""");
            Logger.LogInfo("""  / /  / / / // / / // /_/ // / / // / / / """);
            Logger.LogInfo(""" / /  / /_/ // /_/ // __  // /_/ // /_/ /  """);
            Logger.LogInfo("""/_/   \____/ \____//_/ /_/ \____/ \____/   """);
            Logger.LogInfo("");

            EnableYukuriReplace = Config.Bind("1.Pumpkin", "EnableYukuriReplace", true,
                "Replace the model of Jack'o Lantern to YukuriReimu.");
            EnableYukuriAudio = Config.Bind("1.Pumpkin", "EnableYukuriAudio", true,
                """Replace the audio to "Yukuri".""");
            YukuriAudioVolume = Config.Bind("1.Pumpkin", "YukuriAudioVolume(0.0-1.0)", 0.3f,
                """Config the volume of "Yukuri".""");

            EnableShanghaiReplace = Config.Bind("2.PJMan", "EnableShanghaiReplace", true,
                "Replace the PJMan with 上海人形.");

            EnableMoriyaReplace = Config.Bind("3.ItemShip", "EnableMoriyaReplace", true,
                "Replace the Item Ship with Moriya Shrine.");
            EnableNitoriTheme = Config.Bind("3.ItemShip", "EnableNitoriTheme", true,
                """Replace the default music to Nitori's theme music.""");
            NitoriThemeVolume = Config.Bind("3.ItemShip", "NitoriThemeVolume(0.0-1.0)", 1f,
                """Config the volume of Nitori's theme music.""");

            EnableHakureiReplace = Config.Bind("4.CompanyBuilding", "EnableHakureiReplace", true,
                "Replace the Company Building with Hakurei Shrine.");
            EnableSkyBoxReplace = Config.Bind("4.CompanyBuilding", "EnableSkyBoxReplace", true,
                "Replace the SkyBox.");

            EnableKoumakanReplace = Config.Bind("5.March", "EnableKoumakanReplace", true,
                "Replace the March Building with Koumakan.");

            Logger.LogInfo("Patching all functions.");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            Logger.LogInfo("All systems ready, begin loading assets.");

            try
            {
                var dllFolderPath = Path.GetDirectoryName(Info.Location);
                if (dllFolderPath == null) return;
                var assetBundleFilePath = Path.Combine(dllFolderPath, "shipassets");
                var bundle = AssetBundle.LoadFromFile(assetBundleFilePath);

                Logger.LogInfo($"Loading asset bundle.");
                if (bundle == null) return;

                if (EnableYukuriReplace.Value)
                {
                    YukuriVisuals = bundle.LoadAsset<GameObject>("YukuriUnlockableContainer.prefab");
                    Logger.LogInfo($"Load Yukuri: {YukuriVisuals != null}");

                    if (EnableYukuriAudio.Value)
                    {
                        YukuriAudio = bundle.LoadAsset<AudioClip>("yukuri.ogg");
                        Logger.LogInfo($"Load YukuriAudio: {YukuriAudio != null}");
                    }
                }

                if (EnableShanghaiReplace.Value)
                {
                    ShanghaiVisuals = bundle.LoadAsset<GameObject>("上海人形Container.prefab");
                    Logger.LogInfo($"Load Shanghai: {ShanghaiVisuals != null}");
                }

                if (EnableMoriyaReplace.Value)
                {
                    MoriyaVisuals = bundle.LoadAsset<GameObject>("守矢.prefab");
                    Logger.LogInfo($"Load Moriya: {MoriyaVisuals != null}");

                    if (EnableNitoriTheme.Value)
                    {
                        NitoriTheme = bundle.LoadAsset<AudioClip>("Nitori.mp3");
                        Logger.LogInfo($"Load NitoriTheme: {NitoriTheme != null}");
                    }
                }

                if (EnableHakureiReplace.Value)
                {
                    HakureiVisuals = bundle.LoadAsset<GameObject>("Shrine.prefab");
                    Logger.LogInfo($"Load Hakurei: {HakureiVisuals != null}");
                    if (EnableSkyBoxReplace.Value)
                    {
                        SkyBoxVisuals = bundle.LoadAsset<GameObject>("SkyBox.prefab");
                        Logger.LogInfo($"Load SkyBox: {SkyBoxVisuals != null}");
                    }
                }

                if (EnableKoumakanReplace.Value)
                {
                    KoumakanVisuals = bundle.LoadAsset<GameObject>("Koumakan.prefab");
                    Logger.LogInfo($"Load Koumakan: {KoumakanVisuals != null}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to load assetbundle: " + ex.Message);
            }
        }

        private void Start()
        {
            try
            {
                Logger.LogInfo($"Start is good.");
            }
            catch (Exception ex)
            {
                Logger.LogError("Start not good." + ex.Message);
            }
        }

        public void AddLog(string info)
        {
            Logger.LogInfo(info);
        }
    }
}