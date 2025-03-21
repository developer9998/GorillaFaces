using HarmonyLib;

namespace GorillaFaces.Patches;

[HarmonyPatch(typeof(GorillaEyeExpressions), "SliceUpdate")]
public class ExpressionPatch
{
	public static bool Prefix()
	{
		return false;
	}
}
