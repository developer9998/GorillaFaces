using HarmonyLib;

namespace GorillaFaces.Patches;

[HarmonyPatch(typeof(GorillaMouthFlap), "SliceUpdate")]
public class MouthPatch
{
	public static bool Prefix()
	{
		return false;
	}
}
