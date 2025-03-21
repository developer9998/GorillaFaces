using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaFaces.Extensions;
using GorillaFaces.Models;
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
			if (!File.Exists(filePath))
			{
				return null;
			}
			GorillaFace Face = new GorillaFace
			{
				Path = filePath,
				Name = Path.GetFileNameWithoutExtension(filePath)
			};
			using ZipArchive archive = ZipFile.OpenRead(filePath);
			Array.Sort(archive.Entries.ToArray(), (ZipArchiveEntry x, ZipArchiveEntry y) => string.Compare(x.Name, y.Name));
			foreach (ZipArchiveEntry entry in archive.Entries)
			{
				string entryName = Path.GetFileNameWithoutExtension(entry.Name);
				if (!entry.Name.EndsWith(".png"))
				{
					continue;
				}
				using MemoryStream memoryStream = new MemoryStream();
				await entry.Open().CopyToAsync(memoryStream);
				byte[] bytes = memoryStream.ToArray();
				Texture2D texture = new Texture2D(64, 65, (TextureFormat)3, false)
				{
					filterMode = (FilterMode)0,
					name = Path.GetFileNameWithoutExtension(entry.Name)
				};
				ImageConversion.LoadImage(texture, bytes);
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
			Texture2D eyeSheet = new Texture2D(256, 130, (TextureFormat)4, true)
			{
				name = "Eye Sheet",
				filterMode = (FilterMode)0
			};
			eyeSheet.SetPixels(Enumerable.Repeat<Color>(Color.clear, 33280).ToArray());
			for (int i = 0; i < Face.EyeArray.Length; i++)
			{
				Texture2D eyeImage = Face.EyeArray[i];
				eyeSheet.WriteTexture(eyeImage, new Vector2((float)(i % 4 * 64), (float)(i / 4 * -65)));
			}
			eyeSheet.Apply();
			Face.EyeSheet = eyeSheet;
			Texture2D mouthSheet = new Texture2D(256, 195, (TextureFormat)4, true)
			{
				name = "Mouth Sheet",
				filterMode = (FilterMode)0
			};
			mouthSheet.SetPixels(Enumerable.Repeat<Color>(Color.clear, 49920).ToArray());
			for (int j = 0; j < Face.MouthArray.Length; j++)
			{
				Texture2D mouthImage = Face.MouthArray[j];
				mouthSheet.WriteTexture(mouthImage, new Vector2((float)(j % 4 * 64), (float)(j / 4 * 65)));
			}
			mouthSheet.Apply();
			Face.MouthSheet = mouthSheet;
			return Face;
		}
	}
}
