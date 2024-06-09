using BepInEx;
using BepInEx.Logging;
using GorillaExtensions;
using GorillaFaces.Behaviours;
using GorillaFaces.Models;
using GorillaFaces.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GorillaFaces
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly AssetLoader _assetLoader;
        private readonly FaceConstructor _faceConstructor;

        private List<GorillaFace> _constructedFaces = [];

        public Plugin()
        {
            _assetLoader = new AssetLoader();
            _faceConstructor = new FaceConstructor();

            Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, Constants.GUID);
            GorillaTagger.OnPlayerSpawned(Initialize);
        }

        private async void Initialize()
        {
            try
            {
                _constructedFaces = await _faceConstructor.GetFaces(Path.GetDirectoryName(typeof(Plugin).Assembly.Location));

                GorillaFace face = _constructedFaces.FirstOrDefault(face => face.Name == PlayerPrefs.GetString("GorillaFace", "Default")) ?? _constructedFaces.GetRandomItem();
                Material material = await _assetLoader.LoadAsset<Material>("Face Material");

                material.SetTexture("_Base", face.Base);
                material.SetTexture("_Eye", face.EyeSheet);
                material.SetTexture("_Mouth", face.MouthSheet);

                PhysicalFace.Face = face;
                PhysicalFace.Material = material;

                Events.ApplyFace += OnFaceApplied;

                GorillaTagger.Instance.offlineVRRig.headMesh.transform.Find("gorillaface").GetComponent<MeshRenderer>().material = new Material(PhysicalFace.Material);
            }
            catch (Exception ex)
            {
                Log(ex.ToString(), LogLevel.Error);
            }
        }

        private void OnFaceApplied(GorillaFace face)
        {
            PhysicalFace.Face = face;

            PlayerPrefs.SetString("GorillaFace", face.Name);
            PhysicalFace.Material.SetTexture("_Base", PhysicalFace.Face.Base);
            PhysicalFace.Material.SetTexture("_Eye", PhysicalFace.Face.EyeSheet);
            PhysicalFace.Material.SetTexture("_Mouth", PhysicalFace.Face.MouthSheet);
        }

        private void Log(object message, LogLevel level = LogLevel.Info) => Logger.Log(level, message);
    }
}
