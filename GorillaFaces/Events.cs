using GorillaFaces.Models;
using System;

namespace GorillaFaces
{
    public class Events
    {
        public static event Action<GorillaFace> ApplyFace;

        public virtual void UpdateFace(GorillaFace face) => ApplyFace?.Invoke(face);
    }
}
