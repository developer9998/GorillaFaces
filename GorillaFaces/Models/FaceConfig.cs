using GorillaFaces.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GorillaFaces.Models
{
    public class FaceConfig
    {
        public Vector2 FaceSize;

        public Texture2D MouthTexture;

        public Dictionary<string, Vector2> MouthCoordinates;

        public FaceConfig(GorillaMouthFlap mouthFlap, GorillaEyeExpressions eyeExpressions)
        {
            Material material = mouthFlap.targetFace.GetComponent<Renderer>().material;

            Vector4 vector = material.GetVector(eyeExpressions._BaseMap_ST);
            FaceSize = new(vector.x, vector.y);

            MouthTexture = (material.GetTexture(mouthFlap._MouthMap) as Texture2D).Clone();

            MouthCoordinates = [];
            for (int i = 0; i < mouthFlap.mouthFlapLevels.Length; i++)
            {
                MouthCoordinates.TryAdd($"mouth{i}", mouthFlap.mouthFlapLevels[i].faces.First());
            }
            MouthCoordinates.TryAdd("nomic", mouthFlap.noMicFace.faces.First());
        }
    }
}
