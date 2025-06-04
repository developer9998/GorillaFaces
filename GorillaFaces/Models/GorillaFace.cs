using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaFaces.Extensions;
using GorillaFaces.Tools;
using UnityEngine;

namespace GorillaFaces.Models
{
    public class GorillaFace : IFaceAsset
    {
        public string FilePath { get; private set; }

        public string Name { get; private set; }

        public Texture2DArray FaceTextureArray { get; private set; }
        public Texture2D MouthTexture { get; private set; }

        private readonly Texture2D[] FaceTexArray = new Texture2D[5];

        private readonly Texture2D[] MouthTexArray = new Texture2D[10];

        private Texture2D NoMicTex;

        private Texture2D rawFaceTexture;

        private readonly CultureInfo cultureInfo = new("en-US");

        public async Task<IFaceAsset> Construct(string filePath, FaceConfig faceConfig)
        {
            Logging.Info($"GorillaFace constructing from file: {filePath}");

            FilePath = filePath;
            Name = Path.GetFileNameWithoutExtension(filePath);

            using ZipArchive archive = ZipFile.OpenRead(filePath);
            Array.Sort([.. archive.Entries], (ZipArchiveEntry x, ZipArchiveEntry y) => string.Compare(x.Name, y.Name));

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string entryNameNoEx = Path.GetFileNameWithoutExtension(entry.Name);

                Logging.Info($"Processing entry: {entryNameNoEx}");

                if (entry.Name.EndsWith(".png"))
                {
                    Logging.Info($"Loading as texture");

                    using MemoryStream memoryStream = new();
                    await entry.Open().CopyToAsync(memoryStream);
                    byte[] bytes = memoryStream.ToArray();

                    Texture2D texture = new(64, 64, TextureFormat.RGBA32, false)
                    {
                        filterMode = FilterMode.Point,
                        name = entryNameNoEx
                    };

                    ImageConversion.LoadImage(texture, bytes);
                    texture.Apply();

                    if (entryNameNoEx.StartsWith("nomic"))
                    {
                        Logging.Info("NoMic texture");

                        NoMicTex = texture;
                        continue;
                    }

                    int index = int.Parse(Regex.Match(entryNameNoEx, @"\d+").Value, cultureInfo);

                    if (entryNameNoEx.StartsWith("face"))
                    {
                        Logging.Info($"Face texture {index - 1}");
                        FaceTexArray[index - 1] = texture;
                    }
                    else if (entryNameNoEx.StartsWith("mouth"))
                    {
                        Logging.Info($"Mouth texture {index}");
                        MouthTexArray[index] = texture;
                    }

                    continue;
                }
            }

            rawFaceTexture = new(256, 128, TextureFormat.RGBA32, true)
            {
                name = "BaseSheet",
                filterMode = FilterMode.Point
            };

            rawFaceTexture.SetPixels([.. Enumerable.Repeat(Color.clear, rawFaceTexture.width * rawFaceTexture.height)]);
            for (int i = 0; i < FaceTexArray.Length; i++)
            {
                Texture2D baseImage = FaceTexArray[i];
                rawFaceTexture.Overlay(baseImage, new Vector2(i % 4 * baseImage.width, i / 4 * -baseImage.height));
            }

            rawFaceTexture.Apply();

            Texture2DArray array = new(rawFaceTexture.width, rawFaceTexture.height, 1, rawFaceTexture.format, false, false)
            {
                name = "FaceTextureArray",
                filterMode = FilterMode.Point
            };
            array.SetPixels(rawFaceTexture.GetPixels(), 0, 0);
            array.Apply();

            FaceTextureArray = array;

            Texture2D mouthSheet = faceConfig.MouthTexture.Clone();
            mouthSheet.name = "MouthSheet";

            float scaleFactor = 2;

            /*
            Texture2D mouthSheet = new(256, 256, TextureFormat.RGBA32, true)
            {
                name = "MouthSheet",
                filterMode = FilterMode.Point
            };

            mouthSheet.SetPixels([.. Enumerable.Repeat(Color.clear, 256 * 256)]);

            mouthSheet.Apply();
            */

            for (int i = 0; i < MouthTexArray.Length; i++)
            {
                Texture2D mouthImage = MouthTexArray[i].Resize(scaleFactor);
                if (faceConfig.MouthCoordinates.TryGetValue(mouthImage.name, out Vector2 position))
                {
                    mouthSheet.Overlay(mouthImage, new Vector2(mouthSheet.width * position.x, mouthSheet.height * position.y));
                    continue;
                }
                mouthSheet.Overlay(mouthImage, new Vector2(i % 4 * mouthImage.width, i / 4 * mouthImage.height));
            }

            if (faceConfig.MouthCoordinates.TryGetValue("nomic", out Vector2 noMicPosition))
            {
                mouthSheet.Overlay(NoMicTex.Resize(scaleFactor), new Vector2(mouthSheet.width * noMicPosition.x, mouthSheet.height * noMicPosition.y));
            }

            mouthSheet.Apply();
            MouthTexture = mouthSheet;

            return this;
        }
    }
}