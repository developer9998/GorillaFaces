using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces.Models
{
    public interface IFaceAsset
    {
        public string Location { get; }
        public string Name { get; }
        public Texture2DArray FaceMap { get; }
        public Texture2D MouthMap { get; }
        Task<IFaceAsset> Construct(string filePath, FaceParameters faceConfig);
    }
}
