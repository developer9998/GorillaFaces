using GorillaFaces.Behaviours.Networking;
using HarmonyLib;
using UnityEngine;

namespace GorillaFaces.Patches
{
    [HarmonyPatch(typeof(RigContainer), nameof(RigContainer.OnDisable))]
    public class RigDisablePatch
    {
        [HarmonyWrapSafe]
        public static void Postfix(RigContainer __instance)
        {
            if (__instance.TryGetComponent(out NetworkedPlayer networkedPlayer))
            {
                Object.Destroy(networkedPlayer);
            }
        }
    }
}
