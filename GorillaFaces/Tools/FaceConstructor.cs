using GorillaFaces.Extensions;
using GorillaFaces.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces.Tools
{
    public class FaceConstructor
    {
        public async Task<List<GorillaFace>> GetFaces(string directoryPath)
        {
            List<GorillaFace> faces = [];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            async Task SearchDirectory(string subDirectoryPath)
            {
                FileInfo[] subDirectoryFiles = new DirectoryInfo(subDirectoryPath).GetFiles("*.gorillaface");
                foreach (FileInfo subFile in subDirectoryFiles)
                {
                    faces.Add(await GetFace(subFile.FullName));
                }
            }

            await SearchDirectory(directoryPath);

            string[] pluginDirectories = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);
            foreach (string directory in pluginDirectories)
            {
                await SearchDirectory(directory);
            }

            return faces;
        }

        public async Task<GorillaFace> GetFace(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            GorillaFace Face = new()
            {
                Path = filePath,
                Name = Path.GetFileNameWithoutExtension(filePath)
            };

            using ZipArchive archive = ZipFile.OpenRead(filePath);
            Array.Sort(archive.Entries.ToArray(), (x, y) => string.Compare(x.Name, y.Name));

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string entryName = Path.GetFileNameWithoutExtension(entry.Name);
                if (entry.Name.EndsWith(".png"))
                {
                    using MemoryStream memoryStream = new();
                    await entry.Open().CopyToAsync(memoryStream);

                    byte[] bytes = memoryStream.ToArray();

                    Texture2D texture = new(64, 65, TextureFormat.RGB24, false)
                    {
                        filterMode = FilterMode.Point,
                        name = Path.GetFileNameWithoutExtension(entry.Name)
                    };
                    texture.LoadImage(bytes);
                    texture.Apply();

                    if (entry.Name.StartsWith("eye"))
                    {
                        Face.EyeArray[(int)char.GetNumericValue(entryName.Last())] = texture;
                    }
                    else if (entry.Name.StartsWith("mouth"))
                    {
                        Face.MouthArray[(int)char.GetNumericValue(entryName.Last())] = texture;
                    }
                    else if (entry.Name.StartsWith("silent"))
                    {
                        Face.MouthArray[^1] = texture;
                    }
                    else
                    {
                        Face.Base = texture;
                    }
                }
            }

            Texture2D eyeSheet = new(256, 130, TextureFormat.RGBA32, true)
            {
                name = "Eye Sheet",
                filterMode = FilterMode.Point,
            };
            eyeSheet.SetPixels(Enumerable.Repeat(Color.clear, 256 * 130).ToArray());

            for (int i = 0; i < Face.EyeArray.Length; i++)
            {
                Texture2D eyeImage = Face.EyeArray[i];
                eyeSheet.WriteTexture(eyeImage, new Vector2(i % 4 * 64, i / 4 * -65));
            }

            eyeSheet.Apply();
            Face.EyeSheet = eyeSheet;

            Texture2D mouthSheet = new(256, 195, TextureFormat.RGBA32, true)
            {
                name = "Mouth Sheet",
                filterMode = FilterMode.Point
            };
            mouthSheet.SetPixels(Enumerable.Repeat(Color.clear, 256 * 195).ToArray());

            for (int i = 0; i < Face.MouthArray.Length; i++)
            {
                Texture2D mouthImage = Face.MouthArray[i];
                mouthSheet.WriteTexture(mouthImage, new Vector2(i % 4 * 64, i / 4 * 65));
            }

            mouthSheet.Apply();
            Face.MouthSheet = mouthSheet;

            return Face;
        }
    }
}
