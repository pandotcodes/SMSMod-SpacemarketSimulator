using HarmonyLib;
using System.Linq;

namespace SpacemarketSimulator.Patches
{
    [HarmonyPatch(typeof(NPCTrafficManager), "SpawnNPC")]
    public static class NPCTrafficManager_SpawnNPC_Patch
    {
        public static void Postfix(NPCTrafficManager __instance)
        {
            WaypointNavigator npc = __instance.m_ActiveNPCs.Last();

            npc.gameObject.SetActive(false);
        }
    }
}
