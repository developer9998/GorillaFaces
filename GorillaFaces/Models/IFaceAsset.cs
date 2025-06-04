using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces.Models
{
    public interface IFaceAsset
    {
        public string FilePath { get; }
        public string Name { get; }

        public Texture2DArray FaceTextureArray { get; }
        public Texture2D MouthTexture { get; }

        Task<IFaceAsset> Construct(string filePath, FaceConfig faceConfig);
    }
}
