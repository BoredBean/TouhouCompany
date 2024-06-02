using HarmonyLib;
using System;
using UnityEngine;

namespace TouhouCompany.Patches
{
    [HarmonyPatch(typeof(ItemDropship))]
    internal class ItemDropshipPatch
    {
        [HarmonyPrefix, HarmonyPatch("Start")]
        private static void ReplaceItemShip(ItemDropship __instance)
        {
            if (TouhouCompanyPlugin.MoriyaShrinePrefeb != null &&
                TouhouCompanyPlugin.EnableMoriyaReplace.Value) try
                {
                    __instance.transform.GetComponent<BoxCollider>().enabled = false;
                    var ship = __instance.transform.Find("ItemShip");
                    ship.GetComponent<MeshRenderer>().enabled = false;
                    for (var index = 0; index < ship.childCount; index++)
                    {
                        var child = ship.GetChild(index);
                        if (child.name.Contains("Door"))
                        {
                            child.GetComponent<MeshRenderer>().enabled = false;
                        }
                    }

                    TouhouCompanyPlugin.Instance.AddLog($"Hide ItemDropship model.");

                    var moriyaVisual = UnityEngine.Object.Instantiate(TouhouCompanyPlugin.MoriyaShrinePrefeb);
                    moriyaVisual.transform.SetParent(ship);
                    moriyaVisual.transform.localPosition = Vector3.zero;
                    moriyaVisual.transform.localRotation = Quaternion.identity;
                    moriyaVisual.transform.localScale = new Vector3(0.77f, 0.77f, 0.77f);

                    var light = ship.Find("Point Light");
                    light.localPosition = new Vector3(0.037f, 0f, 1.75f);

                    TouhouCompanyPlugin.Instance.AddLog($"ItemDropship model replaced.");

                    if (TouhouCompanyPlugin.EnableNitoriTheme.Value && TouhouCompanyPlugin.NitoriTheme != null)
                    {
                        var music = __instance.transform.Find("Music");
                        var source = music.GetComponent<AudioSource>();
                        source.clip = TouhouCompanyPlugin.NitoriTheme;
                        source.volume = TouhouCompanyPlugin.NitoriThemeVolume.Value;
                        music = music.Find("Music (1)");
                        source = music.GetComponent<AudioSource>();
                        source.clip = TouhouCompanyPlugin.NitoriTheme;
                        source.volume = TouhouCompanyPlugin.NitoriThemeVolume.Value;

                        TouhouCompanyPlugin.Instance.AddLog($"NitoriTheme replaced.");
                    }
                }
                catch (Exception e)
                {
                    TouhouCompanyPlugin.Instance.AddLog($"An error occured when replacing the ItemDropship: {e}");
                }
        }
    }
}
