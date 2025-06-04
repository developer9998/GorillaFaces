using System.ComponentModel;

namespace GorillaFaces.Models
{
    public enum EDefaultFaceType
    {
        [Description("No custom faces are loaded, instead using the base game face")]
        None,
        [Description("An entirely random custom face is loaded")]
        Random,
        [Description("A seed-based (using Player ID) random custom face is loaded")]
        RandomSeed,
        [Description("A custom face used by the local player is loaded")]
        Matching,
        [Description("A specified custom face is loaded")]
        Assigned
    }
}
