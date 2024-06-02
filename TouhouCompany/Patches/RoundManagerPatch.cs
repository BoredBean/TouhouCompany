using HarmonyLib;
using Unity.Netcode;

namespace TouhouCompany.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch : NetworkBehaviour
    {
        public static ShrineManager ShrineManager;
        public static KoumakanManager KoumakanManager;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(RoundManager __instance)
        {
            TouhouCompanyPlugin.Instance.AddLog("RoundManagerPatch has awoken");
            ShrineManager = __instance.gameObject.AddComponent<ShrineManager>();
            KoumakanManager = __instance.gameObject.AddComponent<KoumakanManager>();
        }

        [HarmonyPatch("LoadNewLevelWait")]
        [HarmonyPrefix]
        public static void LoadNewLevelWaitPatch(RoundManager __instance)
        {
            TouhouCompanyPlugin.Instance.AddLog($"Loading level {__instance.currentLevel.levelID}...");

            if (__instance.currentLevel.levelID == 3 && TouhouCompanyPlugin.EnableHakureiReplace.Value &&
            TouhouCompanyPlugin.HakureiShrinePrefeb != null)
            {
                TouhouCompanyPlugin.Instance.AddLog("Spawning shrine objects");
                ShrineManager.SpawnShrineServerRpc();
            }

            if (__instance.currentLevel.levelID == 4 && TouhouCompanyPlugin.EnableKoumakanReplace.Value &&
                TouhouCompanyPlugin.KoumakanPrefeb != null)
            {
                TouhouCompanyPlugin.Instance.AddLog("Spawning Koumakan objects");
                KoumakanManager.SpawnKoumakanServerRpc();
            }
        }

        [HarmonyPatch("DespawnPropsAtEndOfRound")]
        [HarmonyPostfix]
        public static void DespawnPropsAtEndOfRoundPatch(RoundManager __instance)
        {
            TouhouCompanyPlugin.Instance.AddLog($"End level {__instance.currentLevel.levelID}...");

            if (__instance.currentLevel.levelID == 3 && TouhouCompanyPlugin.EnableHakureiReplace.Value &&
            TouhouCompanyPlugin.HakureiShrinePrefeb != null)
            {
                TouhouCompanyPlugin.Instance.AddLog("Despawning shrine objects");
                if (ShrineManager.IsServer)
                {
                    ShrineManager.DespawnShrineServerRpc();
                }
            }
        }
    }
}
