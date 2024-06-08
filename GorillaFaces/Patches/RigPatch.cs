using GorillaFaces.Behaviours;
using HarmonyLib;

namespace GorillaFaces.Patches
{
    [HarmonyPatch(typeof(VRRig), "SharedStart")]
    public class RigPatch
    {
        [HarmonyWrapSafe]
        public static void Postfix(VRRig __instance)
        {
            if (__instance.GetComponent<PhysicalFace>()) return;
            __instance.gameObject.AddComponent<PhysicalFace>();
        }
    }
}
