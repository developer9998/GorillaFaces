using HarmonyLib;

namespace GorillaFaces.Extensions;

internal static class RigExtensions
{
    public static GorillaMouthFlap GetMouthFlap(this VRRig rig)
    {
        Traverse traverse = Traverse.Create(rig);
        Traverse field = traverse.Field("myMouthFlap");
        return field.GetValue<GorillaMouthFlap>();
    }

    public static GorillaEyeExpressions GetEyeExpressions(this VRRig rig)
    {
        Traverse traverse = Traverse.Create(rig);
        Traverse field = traverse.Field("myEyeExpressions");
        return field.GetValue<GorillaEyeExpressions>();
    }
}
