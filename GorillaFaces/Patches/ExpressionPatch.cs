using HarmonyLib;

namespace GorillaFaces.Patches
{
    [HarmonyPatch(typeof(GorillaEyeExpressions), "InvokeUpdate")]
    public class ExpressionPatch
    {
        public static bool Prefix() => false;
    }
}
