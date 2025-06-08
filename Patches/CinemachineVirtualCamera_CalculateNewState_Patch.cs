using Cinemachine;
using HarmonyLib;

namespace SpacemarketSimulator.Patches
{
    [HarmonyPatch(typeof(CinemachineVirtualCamera), "CalculateNewState")]
    public static class CinemachineVirtualCamera_CalculateNewState_Patch
    {
        public static void Postfix(ref CameraState __result)
        {
            __result.Lens.FarClipPlane = 1500;
        }
    }
}
