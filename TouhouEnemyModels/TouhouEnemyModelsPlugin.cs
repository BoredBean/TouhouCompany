using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Utils;
using LCSoundTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TouhouEnemyModels
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    public class TouhouEnemiesPlugin : BaseUnityPlugin
    {
        public static TouhouEnemiesPlugin Instance;
        public static bool SoundToolGood = false;

        public static AudioClip LostAudio;
        internal static ConfigEntry<bool> EnableDeathAudio;
        internal static ConfigEntry<float> DeathAudioVolume;

        public static GameObject SekiVisuals;
        public static GameObject SekiHeadVisuals;
        public static AudioClip SekibankiTheme;
        internal static ConfigEntry<bool> EnableCoilHeadReplace;
        internal static ConfigEntry<bool> EnableBodyCoilReplace;
        internal static ConfigEntry<bool> EnableSekibankiTheme;
        internal static ConfigEntry<float> SekibankiSFXVolume;

        public static GameObject SatoriVisuals;
        public static AudioClip SatoriTheme;
        internal static ConfigEntry<bool> EnableNutcrackerReplace;
        internal static ConfigEntry<bool> EnableSatoriTheme;
        internal static ConfigEntry<float> SatoriSFXVolume;

        public static GameObject SuikaVisuals;
        public static AudioClip SuikaTheme;
        public static AudioClip[] SuikaAudios = new AudioClip[4];
        internal static ConfigEntry<bool> EnableForestGiantReplace;
        internal static ConfigEntry<bool> EnableSuikaTheme;
        internal static ConfigEntry<float> SuikaThemeVolume;
        internal static ConfigEntry<bool> EnableSuikaVoice;
        internal static ConfigEntry<float> SuikaVoiceVolume;

        public static GameObject MarisaVisuals;
        public static GameObject BugVisuals;
        public static AudioClip MarisaTheme;
        public static AudioClip[] MarisaAudios = new AudioClip[8];
        public static AudioClip[] BugAudios = new AudioClip[6];
        internal static ConfigEntry<bool> EnableHoarderBugReplace;
        internal static ConfigEntry<int> BugSpawnRate;
        internal static ConfigEntry<bool> EnableMarisaTheme;
        internal static ConfigEntry<float> MarisaSFXVolume;
        internal static ConfigEntry<bool> EnableMarisaVoice;
        internal static ConfigEntry<bool> EnableBugVoice;
        internal static ConfigEntry<float> MarisaVoiceVolume;

        public static GameObject UtsuhoVisuals;
        public static GameObject UtsuhoNestSpawnVisuals;
        public static AudioClip UtsuhoTheme;
        internal static ConfigEntry<bool> EnableRadMechReplace;
        internal static ConfigEntry<bool> EnableUtsuhoTheme;
        internal static ConfigEntry<float> UtsuhoSFXVolume;

        public static GameObject YuyukoVisuals;
        public static AudioClip YuyukoTheme;
        internal static ConfigEntry<bool> EnableSandWormReplace;
        internal static ConfigEntry<bool> EnableYuyukoTheme;
        internal static ConfigEntry<float> YuyukoSFXVolume;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("""  ______ ____   __  __ __  __ ____   __  __""");
            Logger.LogInfo(""" /_  __// __ \ / / / // / / // __ \ / / / /""");
            Logger.LogInfo("""  / /  / / / // / / // /_/ // / / // / / / """);
            Logger.LogInfo(""" / /  / /_/ // /_/ // __  // /_/ // /_/ /  """);
            Logger.LogInfo("""/_/   \____/ \____//_/ /_/ \____/ \____/   """);
            Logger.LogInfo("");

            EnableDeathAudio = Config.Bind("0.General", "EnableDeathAudio", false,
                "Play a death audio when an enemy is killed.");
            DeathAudioVolume = Config.Bind("0.General", "DeathAudioVolume(0.0-1.0)", 0.3f,
                "Config the volume of the death audio.");

            EnableCoilHeadReplace = Config.Bind("1.CoilHead", "EnableCoilHead", true,
                "Replace the model of Coil-Head to Sekibanki.");
            EnableBodyCoilReplace = Config.Bind("1.CoilHead", "EnableDeadBodyCoil", true,
                "Replace the coil on the dead body to a head of Sekibanki.");
            EnableSekibankiTheme = Config.Bind("1.CoilHead", "EnableSekibankiTheme", true,
                "Replace the step audio to Sekibanki's theme music.");
            SekibankiSFXVolume = Config.Bind("1.CoilHead", "SekibankiSoundVolume(0.0-1.0)", 0.3f,
                "Config the volume of Sekibanki's sound effect including the theme music.");

            EnableNutcrackerReplace = Config.Bind("2.Nutcracker", "EnableNutcracker", true,
                "Replace the model of Nutcracker to NutSatori.");
            EnableSatoriTheme = Config.Bind("2.Nutcracker", "EnableSatoriTheme", true,
                "Replace the angry audio to Satori's theme music.");
            SatoriSFXVolume = Config.Bind("2.Nutcracker", "SatoriSoundVolume(0.0-1.0)", 0.3f,
                "** THIS NOT WORK! ** Config the volume of Satori's sound effect including the theme music.");

            EnableForestGiantReplace = Config.Bind("3.ForestGiant", "EnableForestGiant", true,
                "Replace the model of ForestGiant to SuikaGiant.");
            EnableSuikaTheme = Config.Bind("3.ForestGiant", "EnableSuikaTheme", true,
                "Play Suika's theme music.");
            SuikaThemeVolume = Config.Bind("3.ForestGiant", "SuikaThemeVolume(0.0-1.0)", 0.3f,
                "Config the volume of Suika's theme music.");
            EnableSuikaVoice = Config.Bind("3.ForestGiant", "EnableSuikaVoice", true,
                "Suika has something to say.");
            SuikaVoiceVolume = Config.Bind("3.ForestGiant", "SuikaVoiceVolume(0.0-1.0)", 1f,
                "Config the volume of Suika's voice.");

            EnableHoarderBugReplace = Config.Bind("4.HoarderBug", "EnableHoarderBug", true,
                "Replace the model of HoarderBug to HoarderMarisa.");
            BugSpawnRate = Config.Bind("4.HoarderBug", "BugSpawnRate(0-10)", 5,
                "How likely Marisa will be spawn as a bug. HoarderMarisa(0) -> KirisameBug(10) No Sync!");
            EnableMarisaTheme = Config.Bind("4.HoarderBug", "EnableMarisaTheme", true,
                "Play Marisa's theme music. Not for the bug.");
            MarisaSFXVolume = Config.Bind("4.HoarderBug", "MarisaSoundVolume(0.0-1.0)", 0.3f,
                "Config the volume of Marisa's sound effect including the theme music.");
            EnableMarisaVoice = Config.Bind("4.HoarderBug", "EnableMarisaVoice", true,
                "Marisa has something to say.");
            EnableBugVoice = Config.Bind("4.HoarderBug", "EnableBugVoice", true,
                "It sounds like a Zerg Queen, but only for the bug.");
            MarisaVoiceVolume = Config.Bind("4.HoarderBug", "MarisaVoiceVolume(0.0-1.0)", 1f,
                "Config the volume of Marisa's voice.");

            EnableRadMechReplace = Config.Bind("5.RadMech", "EnableRadMech", false,
                "Replace the model of RadMech to UtsuhoMech.");
            EnableUtsuhoTheme = Config.Bind("5.RadMech", "EnableUtsuhoTheme", true,
                "Play Utsuho's theme music.");
            UtsuhoSFXVolume = Config.Bind("5.RadMech", "UtsuhoSoundVolume(0.0-1.0)", 0.3f,
                "** THIS NOT WORK! ** Config the volume of Utsuho's sound effect including the theme music.");

            EnableSandWormReplace = Config.Bind("6.SandWorm", "EnableSandWorm", true,
                "Replace the model of SandWorm to Yuyuko.");
            EnableYuyukoTheme = Config.Bind("6.SandWorm", "EnableYuyukoTheme", true,
                "Play Yuyuko's theme music.");
            YuyukoSFXVolume = Config.Bind("6.SandWorm", "YuyukoSoundVolume(0.0-1.0)", 0.3f,
                "Config the volume of Yuyuko's sound effect including the theme music.");

            Logger.LogInfo("Patching all functions.");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            Logger.LogInfo("All systems ready, begin loading assets.");

            try
            {
                var dllFolderPath = Path.GetDirectoryName(Info.Location);
                if (dllFolderPath == null) return;
                var assetBundleFilePath = Path.Combine(dllFolderPath, "touhouenemies");
                var bundle = AssetBundle.LoadFromFile(assetBundleFilePath);

                Logger.LogInfo($"Loading asset bundle.");
                if (bundle == null) return;

                if (EnableDeathAudio.Value)
                {
                    LostAudio = bundle.LoadAsset<AudioClip>("se_pldead00.mp3");
                    Logger.LogInfo($"Load death audio.");
                }

                var componentsInChildren = new List<Renderer>();
                if (EnableCoilHeadReplace.Value)
                {
                    SekiVisuals = bundle.LoadAsset<GameObject>("Sekibanki.prefab");
                    componentsInChildren.AddRange(SekiVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load Sekibanki: {SekiVisuals != null}");

                    if (EnableSekibankiTheme.Value)
                    {
                        SekibankiTheme = bundle.LoadAsset<AudioClip>("SekibankiTheme.mp3");
                        Logger.LogInfo($"Load SekibankiTheme: {SekibankiTheme != null}");
                    }

                    if (EnableBodyCoilReplace.Value)
                    {
                        SekiHeadVisuals = bundle.LoadAsset<GameObject>("SekibankiHead.prefab");
                        Logger.LogInfo($"Load SekibankiHead: {SekiHeadVisuals != null}");
                    }
                }

                if (EnableNutcrackerReplace.Value)
                {
                    SatoriVisuals = bundle.LoadAsset<GameObject>("NutSatori.prefab");
                    componentsInChildren.AddRange(SatoriVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load NutSatori: {SatoriVisuals != null}");

                    if (EnableSatoriTheme.Value)
                    {
                        SatoriTheme = bundle.LoadAsset<AudioClip>("SatoriTheme.mp3");
                        Logger.LogInfo($"Load SatoriTheme: {SatoriTheme != null}");
                    }
                }

                if (EnableForestGiantReplace.Value)
                {
                    SuikaVisuals = bundle.LoadAsset<GameObject>("SuikaGiant.prefab");
                    componentsInChildren.AddRange(SuikaVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load SuikaGiant: {SuikaVisuals != null}");

                    if (EnableSuikaTheme.Value)
                    {
                        SuikaTheme = bundle.LoadAsset<AudioClip>("SuikaTheme.mp3");
                        Logger.LogInfo($"Load SuikaTheme: {SuikaTheme != null}");
                    }

                    if (EnableSuikaVoice.Value)
                    {
                        SuikaAudios[0] = bundle.LoadAsset<AudioClip>("SuikaAngry.ogg");
                        SuikaAudios[1] = bundle.LoadAsset<AudioClip>("SuikaLost.ogg");
                        SuikaAudios[2] = bundle.LoadAsset<AudioClip>("SuikaHappy.ogg");
                        SuikaAudios[3] = bundle.LoadAsset<AudioClip>("SuikaWarn.ogg");
                        Logger.LogInfo($"Load SuikaAudio: 0: {SuikaAudios[0] != null}, " +
                                       $"1: {SuikaAudios[1] != null}, 2: {SuikaAudios[2] != null}, 3: {SuikaAudios[3] != null}");
                    }
                }

                if (EnableHoarderBugReplace.Value)
                {
                    MarisaVisuals = bundle.LoadAsset<GameObject>("HoarderMarisa.prefab");
                    BugVisuals = bundle.LoadAsset<GameObject>("KirisameBug.prefab");
                    componentsInChildren.AddRange(MarisaVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    componentsInChildren.AddRange(BugVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load HoarderMarisa: {MarisaVisuals != null}");

                    if (EnableMarisaTheme.Value)
                    {
                        MarisaTheme = bundle.LoadAsset<AudioClip>("MarisaTheme.mp3");
                        Logger.LogInfo($"Load MarisaTheme: {MarisaTheme != null}");
                    }

                    if (EnableMarisaVoice.Value)
                    {
                        MarisaAudios[0] = bundle.LoadAsset<AudioClip>("MarisaAngry.ogg");
                        MarisaAudios[1] = bundle.LoadAsset<AudioClip>("MarisaAttack.ogg");
                        MarisaAudios[2] = bundle.LoadAsset<AudioClip>("MarisaLost.ogg");
                        MarisaAudios[3] = bundle.LoadAsset<AudioClip>("MarisaGreeting.ogg");
                        MarisaAudios[4] = bundle.LoadAsset<AudioClip>("MarisaHappy.ogg");
                        MarisaAudios[5] = bundle.LoadAsset<AudioClip>("MarisaHurt.ogg");
                        MarisaAudios[6] = bundle.LoadAsset<AudioClip>("MarisaMove.ogg");
                        MarisaAudios[7] = bundle.LoadAsset<AudioClip>("MarisaWarn.ogg");

                        Logger.LogInfo($"Load MarisaAudio: 0: {MarisaAudios[0] != null}, " +
                                       $"1: {MarisaAudios[1] != null}, 2: {MarisaAudios[2] != null}, " +
                                       $"3: {MarisaAudios[3] != null}, 4: {MarisaAudios[4] != null}, " +
                                       $"5: {MarisaAudios[5] != null}, 6: {MarisaAudios[6] != null}, " +
                                       $"7: {MarisaAudios[7] != null}");
                    }

                    if (EnableBugVoice.Value)
                    {
                        BugAudios[0] = bundle.LoadAsset<AudioClip>("BugAngry.ogg");
                        BugAudios[1] = bundle.LoadAsset<AudioClip>("BugAttack.ogg");
                        BugAudios[2] = bundle.LoadAsset<AudioClip>("BugLost.ogg");
                        BugAudios[3] = bundle.LoadAsset<AudioClip>("BugGreeting.ogg");
                        BugAudios[4] = bundle.LoadAsset<AudioClip>("BugHappy.ogg");
                        BugAudios[5] = bundle.LoadAsset<AudioClip>("BugWarn.ogg");

                        Logger.LogInfo($"Load MarisaAudio: 0: {BugAudios[0] != null}, " +
                                       $"1: {BugAudios[1] != null}, 2: {BugAudios[2] != null}, " +
                                       $"3: {BugAudios[3] != null}, 4: {BugAudios[4] != null}, " +
                                       $"5: {BugAudios[5] != null}");
                    }
                }

                if (EnableRadMechReplace.Value)
                {
                    UtsuhoVisuals = bundle.LoadAsset<GameObject>("UtsuhoMech.prefab");
                    UtsuhoNestSpawnVisuals = bundle.LoadAsset<GameObject>("UtsuhoMechNestSpawnObject.prefab");
                    componentsInChildren.AddRange(UtsuhoVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    componentsInChildren.AddRange(UtsuhoNestSpawnVisuals.GetComponentsInChildren<Renderer>(true)
                        .ToList());
                    Logger.LogInfo(
                        $"Load UtsuhoMech: {UtsuhoVisuals != null}, NestSpawnObject: {UtsuhoNestSpawnVisuals != null}");

                    if (EnableUtsuhoTheme.Value)
                    {
                        UtsuhoTheme = bundle.LoadAsset<AudioClip>("UtsuhoTheme.mp3");
                        Logger.LogInfo($"Load UtsuhoTheme: {UtsuhoTheme != null}");
                    }
                }

                if (EnableSandWormReplace.Value)
                {
                    YuyukoVisuals = bundle.LoadAsset<GameObject>("EaterYuyuko.prefab");
                    componentsInChildren.AddRange(YuyukoVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo(
                        $"Load EaterYuyuko: {YuyukoVisuals != null}");

                    if (EnableYuyukoTheme.Value)
                    {
                        YuyukoTheme = bundle.LoadAsset<AudioClip>("YuyukoTheme.mp3");
                        Logger.LogInfo($"Load YuyukoTheme: {YuyukoTheme != null}");
                    }
                }

                foreach (var renderer in componentsInChildren)
                {
                    renderer.gameObject.layer = LayerMask.NameToLayer("Enemies");
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
                if (SuikaAudios[1] != null) SoundTool.ReplaceAudioClip("StunGiant", SuikaAudios[1]);
                if (SuikaAudios[3] != null) SoundTool.ReplaceAudioClip("FGiantEatPlayerSFX", SuikaAudios[3]);
                SoundToolGood = true;
                Logger.LogInfo($"SoundTool is good.");
            }
            catch (Exception ex)
            {
                Logger.LogError("SoundTool not good." + ex.Message);
            }
        }

        public void AddLog(string info)
        {
            Logger.LogInfo(info);
        }
    }
}