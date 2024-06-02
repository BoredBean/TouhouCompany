using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TouhouCompany
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class TouhouCompanyPlugin : BaseUnityPlugin
    {
        public static TouhouCompanyPlugin Instance;

        public static GameObject YukuriPrefeb;
        public static AudioClip YukuriAudio;
        internal static ConfigEntry<bool> EnableYukuriReplace;
        internal static ConfigEntry<bool> EnableYukuriAudio;
        internal static ConfigEntry<float> YukuriAudioVolume;

        public static GameObject ShanghaiPrefeb;
        internal static ConfigEntry<bool> EnableShanghaiReplace;

        public static GameObject MoriyaShrinePrefeb;
        public static AudioClip NitoriTheme;
        internal static ConfigEntry<bool> EnableMoriyaReplace;
        internal static ConfigEntry<bool> EnableNitoriTheme;
        internal static ConfigEntry<float> NitoriThemeVolume;

        public static GameObject HakureiShrinePrefeb;
        public static GameObject NewspaperPrefeb;
        public static GameObject NotePrefeb;
        internal static ConfigEntry<bool> EnableHakureiReplace;
        public static GameObject SkyBoxPrefeb;
        internal static ConfigEntry<bool> EnableSkyBoxReplace;

        public static GameObject KoumakanPrefeb;
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

            NetcodeWeaver();
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
                    YukuriPrefeb = bundle.LoadAsset<GameObject>("YukuriUnlockableContainer.prefab");
                    Logger.LogInfo($"Load Yukuri: {YukuriPrefeb != null}");

                    if (EnableYukuriAudio.Value)
                    {
                        YukuriAudio = bundle.LoadAsset<AudioClip>("yukuri.ogg");
                        Logger.LogInfo($"Load YukuriAudio: {YukuriAudio != null}");
                    }
                }

                if (EnableShanghaiReplace.Value)
                {
                    ShanghaiPrefeb = bundle.LoadAsset<GameObject>("上海人形Container.prefab");
                    Logger.LogInfo($"Load Shanghai: {ShanghaiPrefeb != null}");
                }

                if (EnableMoriyaReplace.Value)
                {
                    MoriyaShrinePrefeb = bundle.LoadAsset<GameObject>("守矢.prefab");
                    Logger.LogInfo($"Load Moriya: {MoriyaShrinePrefeb != null}");

                    if (EnableNitoriTheme.Value)
                    {
                        NitoriTheme = bundle.LoadAsset<AudioClip>("Nitori.mp3");
                        Logger.LogInfo($"Load NitoriTheme: {NitoriTheme != null}");
                    }
                }

                if (EnableHakureiReplace.Value)
                {
                    HakureiShrinePrefeb = bundle.LoadAsset<GameObject>("Shrine.prefab");
                    HakureiShrinePrefeb.AddComponent<ShrineBuilding>();
                    NewspaperPrefeb = bundle.LoadAsset<GameObject>("Newspaper.prefab");
                    NotePrefeb = bundle.LoadAsset<GameObject>("Note.prefab");
                    Logger.LogInfo($"Load Hakurei: {HakureiShrinePrefeb != null}, " +
                        $"Newspaper: {NewspaperPrefeb != null}, " +
                        $"Note: {NotePrefeb != null}");
                    if (EnableSkyBoxReplace.Value)
                    {
                        SkyBoxPrefeb = bundle.LoadAsset<GameObject>("SkyBox.prefab");
                        Logger.LogInfo($"Load SkyBox: {SkyBoxPrefeb != null}");
                    }
                }

                if (EnableKoumakanReplace.Value)
                {
                    KoumakanPrefeb = bundle.LoadAsset<GameObject>("Koumakan.prefab");
                    Logger.LogInfo($"Load Koumakan: {KoumakanPrefeb != null}");
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

        private static void NetcodeWeaver()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false).Length != 0)
                        method.Invoke(null, null);
                }
            }
        }
    }
}