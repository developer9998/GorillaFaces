using BepInEx.Configuration;
using GorillaFaces.Models;

namespace GorillaFaces.Tools
{
    public class Configuration
    {
        public static ConfigEntry<string> CurrentFace;

        public static ConfigEntry<EDefaultFaceType> DefaultFaceType;

        public static ConfigEntry<string> DefaultFaceName;
    }
}
