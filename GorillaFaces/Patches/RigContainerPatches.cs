using GorillaFaces.Behaviours.Networking;
using HarmonyLib;
using UnityEngine;

namespace GorillaFaces.Patches
{
    [HarmonyPatch(typeof(RigContainer)), HarmonyWrapSafe]
    internal class RigContainerPatches
    {
        [HarmonyPatch(nameof(RigContainer.Creator), MethodType.Setter), HarmonyPostfix]
        public static void RigCreationPatch(RigContainer __instance, NetPlayer value)
        {
            if (!__instance.GetComponent<NetworkedPlayer>())
            {
                NetworkedPlayer networkedPlayer = __instance.gameObject.AddComponent<NetworkedPlayer>();
                networkedPlayer.Rig = __instance.Rig;
                networkedPlayer.Owner = value;
            }
        }

        [HarmonyPatch(nameof(RigContainer.OnDisable)), HarmonyPostfix]
        public static void RigRemovalPatch(RigContainer __instance)
        {
            if (__instance.TryGetComponent(out NetworkedPlayer networkedPlayer))
            {
                Object.Destroy(networkedPlayer);
            }
        }
    }
}
