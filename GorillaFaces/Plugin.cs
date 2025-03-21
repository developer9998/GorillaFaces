using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using GorillaExtensions;
using GorillaFaces.Behaviours;
using GorillaFaces.Models;
using GorillaFaces.Tools;
using HarmonyLib;
using UnityEngine;

namespace GorillaFaces
{
	[BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
	public class Plugin : BaseUnityPlugin
	{
		public static Plugin Instance;

		public List<GorillaFace> Faces = [];

		private AssetLoader asset_loader;

		private FaceConstructor face_constructor;

		public void Awake()
		{
			Instance = this;

			asset_loader = new AssetLoader();
			face_constructor = new FaceConstructor();

			Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, Constants.GUID);
			GorillaTagger.OnPlayerSpawned(Initialize);
		}

		private async void Initialize()
		{
			try
			{
				Faces = await face_constructor.GetFaces(Path.GetDirectoryName(typeof(Plugin).Assembly.Location));
				GorillaFace face = Faces.FirstOrDefault(face => face.Name == PlayerPrefs.GetString("GorillaFace", "Default")) ?? GTExt.GetRandomItem(Faces);
				
				Material material = await asset_loader.LoadAsset<Material>("Face Material");
				material.SetTexture("_Base", (Texture)(object)face.Base);
				material.SetTexture("_Eye", (Texture)(object)face.EyeSheet);
				material.SetTexture("_Mouth", (Texture)(object)face.MouthSheet);
				
				PhysicalFace.Face = face;
				PhysicalFace.Material = material;

				Events.ApplyFace += OnFaceApplied;
				GorillaTagger.Instance.offlineVRRig.headMesh.transform.Find("gorillaface").GetComponent<MeshRenderer>().material = new Material(PhysicalFace.Material);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void OnFaceApplied(GorillaFace face)
		{
			PhysicalFace.Face = face;
			PlayerPrefs.SetString("GorillaFace", face.Name);
			PhysicalFace.Material.SetTexture("_Base", (Texture)(object)PhysicalFace.Face.Base);
			PhysicalFace.Material.SetTexture("_Eye", (Texture)(object)PhysicalFace.Face.EyeSheet);
			PhysicalFace.Material.SetTexture("_Mouth", (Texture)(object)PhysicalFace.Face.MouthSheet);
		}
	}
}