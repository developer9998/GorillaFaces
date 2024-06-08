using UnityEngine;

namespace GorillaFaces.Models
{
    public class GorillaFace
    {
        public string Name, Path;

        public Texture2D[] MouthArray = new Texture2D[11];
        public Texture2D[] EyeArray = new Texture2D[5];

        public Texture2D Base, MouthSheet, EyeSheet;
    }
}
