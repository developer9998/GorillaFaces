using HarmonyLib;

namespace GorillaFaces.Patches
{
    [HarmonyPatch(typeof(GorillaMouthFlap), "InvokeUpdate")]
    public class MouthPatch
    {
        public static bool Prefix() => false;
    }
}
