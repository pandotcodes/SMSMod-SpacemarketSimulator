using HarmonyLib;
using TurnTheGameOn.SimpleTrafficSystem;

namespace SpacemarketSimulator.Patches
{
    [HarmonyPatch(typeof(AITrafficLight), "EnableRedLight")] public static class AITrafficLight_EnableRedLight_Patch { public static void Postfix(AITrafficLight __instance) => __instance.DisableAllLights(); }
}
