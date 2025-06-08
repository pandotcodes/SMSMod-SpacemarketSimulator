using HarmonyLib;

namespace SpacemarketSimulator.Patches
{
    [HarmonyPatch(typeof(SectionManager), "UpgradeStore")]
    public static class SectionManager_UpgradeStore_Patch
    {
        public static void Postfix()
        {
            //Plugin.DisableStandardEnvironment();
        }
    }
}
