using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GorillaFaces.Tools;

namespace GorillaFaces.Models
{
    public class FaceLoader(string basePath, FaceConfig faceConfig)
    {
        public string BasePath = basePath;

        public FaceConfig FaceConfig = faceConfig;

        public async Task<List<IFaceAsset>> GetAllFaces()
        {
            List<IFaceAsset> faces = [];

            var files = Directory.GetFiles(BasePath, "*.gorillaface", SearchOption.AllDirectories).ToList();

            faces.AddRange(await LoadFaces<GorillaFace>(files));

            return faces;
        }

        public async Task<List<T>> LoadFaces<T>(List<string> files) where T : IFaceAsset
        {
            List<T> faces = [];

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                try
                {
                    var asset = Activator.CreateInstance<T>();
                    var face = await asset.Construct(file, FaceConfig);

                    if (face == null)
                    {
                        File.Move(file, $"{file}.broken");
                        continue;
                    }

                    faces.Add(asset);
                }
                catch (Exception ex)
                {
                    Logging.Fatal($"Error constructing face: {file}");
                    Logging.Error(ex);
                    File.Move(file, string.Concat(file, ".broken"));
                }
            }

            return faces;
        }
    }
}
