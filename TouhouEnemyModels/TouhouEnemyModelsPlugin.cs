using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Utils;
using LCSoundTool;
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

        public static AudioClip LostAudio;
        internal static ConfigEntry<bool> EnableDeathAudio;
        internal static ConfigEntry<float> DeathAudioVolume;

        public static GameObject sekiVisuals;
        public static GameObject sekiHeadVisuals;
        public static AudioClip SekibankiTheme;
        internal static ConfigEntry<bool> EnableCoilHeadReplace;
        internal static ConfigEntry<bool> EnableBodyCoilReplace;
        internal static ConfigEntry<bool> EnableSekibankiTheme;
        internal static ConfigEntry<float> SekibankiSFXVolume;

        public static GameObject satoriVisuals;
        public static AudioClip SatoriTheme;
        internal static ConfigEntry<bool> EnableNutcrackerReplace;
        internal static ConfigEntry<bool> EnableSatoriTheme;
        internal static ConfigEntry<float> SatoriSFXVolume;

        public static GameObject suikaVisuals;
        public static AudioClip SuikaTheme;
        public static AudioClip[] SuikaAudios = new AudioClip[4];
        internal static ConfigEntry<bool> EnableForestGiantReplace;
        internal static ConfigEntry<bool> EnableSuikaTheme;
        internal static ConfigEntry<float> SuikaThemeVolume;
        internal static ConfigEntry<bool> EnableSuikaVoice;
        internal static ConfigEntry<float> SuikaVoiceVolume;

        public static GameObject marisaVisuals;
        public static AudioClip MarisaTheme;
        public static AudioClip[] MarisaAudios = new AudioClip[8];
        internal static ConfigEntry<bool> EnableHoarderBugReplace;
        internal static ConfigEntry<bool> EnableMarisaTheme;
        internal static ConfigEntry<float> MarisaSFXVolume;
        internal static ConfigEntry<bool> EnableMarisaVoice;
        internal static ConfigEntry<float> MarisaVoiceVolume;

        public static GameObject utsuhoVisuals;
        public static GameObject utsuhoNestSpawnVisuals;
        public static AudioClip UtsuhoTheme;
        internal static ConfigEntry<bool> EnableRadMechReplace;
        internal static ConfigEntry<bool> EnableUtsuhoTheme;
        internal static ConfigEntry<float> UtsuhoSFXVolume;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");

            EnableDeathAudio = Config.Bind("General", "EnableDeathAudio", false,
                "Play a death audio when an enemy is killed.");
            DeathAudioVolume = Config.Bind("General", "DeathAudioVolume(0.0-1.0)", 0.5f,
                "Config the volume of the death audio.");

            EnableCoilHeadReplace = Config.Bind("CoilHead", "EnableCoilHead", true,
                "Replace the model of Coil-Head to Sekibanki.");
            EnableBodyCoilReplace = Config.Bind("CoilHead", "EnableDeadBodyCoil", true,
                "Replace the coil on the dead body to a head of Sekibanki.");
            EnableSekibankiTheme = Config.Bind("CoilHead", "EnableSekibankiTheme", true,
                "Replace the step audio to Sekibanki's theme music.");
            SekibankiSFXVolume = Config.Bind("CoilHead", "SekibankiSoundVolume(0.0-1.0)", 0.5f,
                "Config the volume of Sekibanki's sound effect including the theme music.");

            EnableNutcrackerReplace = Config.Bind("Nutcracker", "EnableNutcracker", true,
                "Replace the model of Nutcracker to NutSatori.");
            EnableSatoriTheme = Config.Bind("Nutcracker", "EnableSatoriTheme", true,
                "Replace the angry audio to Satori's theme music.");
            SatoriSFXVolume = Config.Bind("Nutcracker", "SatoriSoundVolume(0.0-1.0)", 0.5f,
                "Config the volume of Satori's sound effect including the theme music.");

            EnableForestGiantReplace = Config.Bind("ForestGiant", "EnableForestGiant", true,
                "Replace the model of ForestGiant to SuikaGiant.");
            EnableSuikaTheme = Config.Bind("ForestGiant", "EnableSuikaTheme", true,
                "Play Suika's theme music.");
            SuikaThemeVolume = Config.Bind("ForestGiant", "SuikaThemeVolume(0.0-1.0)", 0.5f,
                "Config the volume of Suika's theme music.");
            EnableSuikaVoice = Config.Bind("ForestGiant", "EnableSuikaVoice", true,
                "Suika has something to say.");
            SuikaVoiceVolume = Config.Bind("ForestGiant", "SuikaVoiceVolume(0.0-1.0)", 1f,
                "Config the volume of Suika's voice.");

            EnableHoarderBugReplace = Config.Bind("HoarderBug", "EnableHoarderBug", true,
                "Replace the model of HoarderBug to HoarderMarisa.");
            EnableMarisaTheme = Config.Bind("HoarderBug", "EnableMarisaTheme", true,
                "Play Marisa's theme music.");
            MarisaSFXVolume = Config.Bind("HoarderBug", "MarisaSoundVolume(0.0-1.0)", 0.5f,
                "Config the volume of Marisa's sound effect including the theme music.");
            EnableMarisaVoice = Config.Bind("HoarderBug", "EnableMarisaVoice", true,
                "Marisa has something to say.");
            MarisaVoiceVolume = Config.Bind("HoarderBug", "MarisaVoiceVolume(0.0-1.0)", 1f,
                "Config the volume of Marisa's voice.");

            EnableRadMechReplace = Config.Bind("RadMech", "EnableRadMech", true,
                "Replace the model of RadMech to UtsuhoMech.");
            EnableUtsuhoTheme = Config.Bind("RadMech", "EnableUtsuhoTheme", true,
                "Play Utsuho's theme music.");
            UtsuhoSFXVolume = Config.Bind("RadMech", "UtsuhoSoundVolume(0.0-1.0)", 0.5f,
                "Config the volume of Utsuho's sound effect including the theme music.");


            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void Start()
        {
            try
            {
                var bundle =
                    AssetUtils.LoadAssetBundleFromResources("touhouenemies", typeof(TouhouEnemiesPlugin).Assembly);
                Logger.LogInfo($"Loading asset bundle.");
                if (bundle == null) return;

                //var allAssets = bundle.LoadAllAssets();
                //foreach (var asset in allAssets)
                //{
                //    Logger.LogInfo(asset.name);
                //}

                if (EnableDeathAudio.Value)
                {
                    LostAudio = bundle.LoadAsset<AudioClip>("se_pldead00.mp3");
                    Logger.LogInfo($"Load death audio.");
                }

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
                        //if (SatoriTheme != null) SoundTool.ReplaceAudioClip("NutcrackerAngry", SatoriTheme);
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
                        SuikaTheme = bundle.LoadAsset<AudioClip>("SuikaTheme.mp3");
                        Logger.LogInfo($"Load SuikaTheme: {SuikaTheme != null}");
                    }

                    if (EnableSuikaVoice.Value)
                    {
                        SuikaAudios[0] = bundle.LoadAsset<AudioClip>("SuikaAngry.ogg");
                        SuikaAudios[1] = bundle.LoadAsset<AudioClip>("SuikaDead.ogg");
                        SuikaAudios[2] = bundle.LoadAsset<AudioClip>("SuikaHappy.ogg");
                        SuikaAudios[3] = bundle.LoadAsset<AudioClip>("SuikaWarn.ogg");
                        SoundTool.ReplaceAudioClip("StunGiant", SuikaAudios[0]);
                        SoundTool.ReplaceAudioClip("ForestGiantDie", SuikaAudios[1]);
                        SoundTool.ReplaceAudioClip("FGiantEatPlayerSFX", SuikaAudios[3]);
                        Logger.LogInfo($"Load SuikaAudio: 0: {SuikaAudios[0] != null}, " +
                                       $"1: {SuikaAudios[1] != null}, 2: {SuikaAudios[2] != null}, 3: {SuikaAudios[3] != null}");
                    }
                }

                if (EnableHoarderBugReplace.Value)
                {
                    marisaVisuals = bundle.LoadAsset<GameObject>("HoarderMarisa.prefab");
                    componentsInChildren.AddRange(marisaVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    Logger.LogInfo($"Load HoarderMarisa: {marisaVisuals != null}");

                    if (EnableMarisaTheme.Value)
                    {
                        MarisaTheme = bundle.LoadAsset<AudioClip>("MarisaTheme.mp3");
                        if (MarisaTheme != null) SoundTool.ReplaceAudioClip("Fly", MarisaTheme);
                        Logger.LogInfo($"Load MarisaTheme: {MarisaTheme != null}");
                    }

                    if (EnableMarisaVoice.Value)
                    {
                        MarisaAudios[0] = bundle.LoadAsset<AudioClip>("MarisaAngry.ogg");
                        MarisaAudios[1] = bundle.LoadAsset<AudioClip>("MarisaAttack.ogg");
                        MarisaAudios[2] = bundle.LoadAsset<AudioClip>("MarisaDead.ogg");
                        MarisaAudios[3] = bundle.LoadAsset<AudioClip>("MarisaGreeting.ogg");
                        MarisaAudios[4] = bundle.LoadAsset<AudioClip>("MarisaHappy.ogg");
                        MarisaAudios[5] = bundle.LoadAsset<AudioClip>("MarisaHurt.ogg");
                        MarisaAudios[6] = bundle.LoadAsset<AudioClip>("MarisaMove.ogg");
                        MarisaAudios[7] = bundle.LoadAsset<AudioClip>("MarisaWarn.ogg");
                        //if (MarisaAudios[0] != null) SoundTool.ReplaceAudioClip("Chitter1", MarisaAudios[0]);
                        //if (MarisaAudios[3] != null) SoundTool.ReplaceAudioClip("Chitter2", MarisaAudios[3]);
                        //if (MarisaAudios[7] != null) SoundTool.ReplaceAudioClip("Chitter3", MarisaAudios[7]);
                        //if (MarisaAudios[0] != null) SoundTool.ReplaceAudioClip("AngryScreech", MarisaAudios[0]);
                        //if (MarisaAudios[7] != null) SoundTool.ReplaceAudioClip("AngryScreech2", MarisaAudios[7]);
                        if (MarisaAudios[1] != null) SoundTool.ReplaceAudioClip("HoarderBugCry", MarisaAudios[1]);

                        Logger.LogInfo($"Load MarisaAudio: 0: {MarisaAudios[0] != null}, " +
                                       $"1: {MarisaAudios[1] != null}, 2: {MarisaAudios[2] != null}, " +
                                       $"3: {MarisaAudios[3] != null}, 4: {MarisaAudios[4] != null}, " +
                                       $"5: {MarisaAudios[5] != null}, 6: {MarisaAudios[6] != null}, " +
                                       $"7: {MarisaAudios[7] != null}");
                    }
                }

                if (EnableRadMechReplace.Value)
                {
                    utsuhoVisuals = bundle.LoadAsset<GameObject>("UtsuhoMech.prefab");
                    utsuhoNestSpawnVisuals = bundle.LoadAsset<GameObject>("UtsuhoMechNestSpawnObject.prefab");
                    componentsInChildren.AddRange(utsuhoVisuals.GetComponentsInChildren<Renderer>(true).ToList());
                    componentsInChildren.AddRange(utsuhoNestSpawnVisuals.GetComponentsInChildren<Renderer>(true)
                        .ToList());
                    Logger.LogInfo(
                        $"Load UtsuhoMech: {utsuhoVisuals != null}, NestSpawnObject: {utsuhoNestSpawnVisuals != null}");

                    if (EnableUtsuhoTheme.Value)
                    {
                        UtsuhoTheme = bundle.LoadAsset<AudioClip>("UtsuhoTheme.mp3");
                        if (UtsuhoTheme != null) SoundTool.ReplaceAudioClip("LRADAlarm3", UtsuhoTheme);
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
                Logger.LogError("Failed to load assetbundle: " + ex.Message);
            }
        }

        public void AddLog(string info)
        {
            Logger.LogInfo(info);
        }
    }
}