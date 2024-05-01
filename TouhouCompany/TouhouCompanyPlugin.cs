using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
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

            Logger.LogInfo("Patching all functions.");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            Logger.LogInfo("All systems ready, begin loading assets.");

            try
            {
                var bundle =
                    AssetUtils.LoadAssetBundleFromResources("shipassets", typeof(TouhouCompanyPlugin).Assembly);
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
                    Logger.LogInfo($"Load 上海: {ShanghaiVisuals != null}");
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