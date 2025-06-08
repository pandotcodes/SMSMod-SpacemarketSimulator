using HarmonyLib;
using TurnTheGameOn.SimpleTrafficSystem;

namespace SpacemarketSimulator.Patches
{
    [HarmonyPatch(typeof(AITrafficLight), "EnableGreenLight")] public static class AITrafficLight_EnableGreenLight_Patch { public static void Postfix(AITrafficLight __instance) => __instance.DisableAllLights(); }
}
