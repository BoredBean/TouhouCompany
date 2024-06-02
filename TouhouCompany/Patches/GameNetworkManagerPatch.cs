using HarmonyLib;
using Unity.Netcode;

namespace TouhouCompany.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(GameNetworkManager __instance)
        {
            TouhouCompanyPlugin.Instance.AddLog("Adding network prefab");
            NetworkManager.Singleton.AddNetworkPrefab(TouhouCompanyPlugin.HakureiShrinePrefeb);
            NetworkManager.Singleton.AddNetworkPrefab(TouhouCompanyPlugin.NewspaperPrefeb);
            NetworkManager.Singleton.AddNetworkPrefab(TouhouCompanyPlugin.NotePrefeb);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void StartDisconnectPatch()
        {
            TouhouCompanyPlugin.Instance.AddLog("Player disconnected.");
            RoundManagerPatch.ShrineManager.Destroy();
            RoundManagerPatch.KoumakanManager.Destroy();
        }
    }
}
