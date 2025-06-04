using System;
using GorillaFaces.Models;
using GorillaFaces.Tools;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaFaces.Behaviours
{
    [RequireComponent(typeof(RigContainer))]
    public class GFacesPlayer : MonoBehaviour
    {
        public RigContainer Container;

        public Player PlayerRef;

        public GorillaMouthFlap MouthFlap;

        public Action<IFaceAsset> OnFaceLoaded;
        public Action OnFaceUnloaded;

        public bool IsFaceLoaded;
        public IFaceAsset CustomFace;

        private Material faceMaterial;

        private readonly ShaderHashId faceMaterialProperty = "_BaseMap_Atlas";
        private readonly ShaderHashId faceMaterialSliceProperty = "_BaseMap_AtlasSlice";
        private readonly ShaderHashId mouthMaterialProperty = "_MouthMap";

        private Texture2DArray originalFace;
        private float atlasSlice;
        private Texture2D originalMouth;

        public void Awake()
        {
            Container = GetComponent<RigContainer>();
            PlayerRef = Container.Rig.isLocal ? PhotonNetwork.LocalPlayer : Container.Creator.GetPlayerRef();

            MouthFlap = GetComponent<GorillaMouthFlap>() ?? Container.Rig.myMouthFlap;

            faceMaterial = MouthFlap.targetFaceRenderer is Renderer renderer ? renderer.material : MouthFlap.targetFace.GetComponent<Renderer>().material;
            originalFace = faceMaterial.GetTexture(faceMaterialProperty) as Texture2DArray;
            atlasSlice = faceMaterial.GetFloat(faceMaterialSliceProperty);
            originalMouth = faceMaterial.GetTexture(mouthMaterialProperty) as Texture2D;
        }

        public void LoadCustomFace(IFaceAsset newFace)
        {
            if (newFace is null)
                throw new ArgumentNullException(nameof(newFace));

            if (IsFaceLoaded)
                UnloadCustomFace();

            Logging.Info($"Loading face {newFace.Name} for player {PlayerRef.NickName}");

            IsFaceLoaded = true;
            CustomFace = newFace;

            faceMaterial.SetTexture(faceMaterialProperty, newFace.FaceTextureArray);
            faceMaterial.SetInt(faceMaterialSliceProperty, 0);
            faceMaterial.SetTexture(mouthMaterialProperty, newFace.MouthTexture);

            OnFaceLoaded?.Invoke(newFace);
        }

        public void UnloadCustomFace()
        {
            if (!IsFaceLoaded) return;

            IsFaceLoaded = false;
            CustomFace = null;

            Logging.Info($"Unloading face for player {PlayerRef.NickName}");

            faceMaterial.SetTexture(faceMaterialProperty, originalFace);
            faceMaterial.SetFloat(faceMaterialSliceProperty, atlasSlice);
            faceMaterial.SetTexture(mouthMaterialProperty, originalMouth);

            OnFaceUnloaded?.Invoke();
        }

        public void SwitchCustomFace(IFaceAsset customFace)
        {
            if (CustomFace != customFace)
                LoadCustomFace(customFace);
            else
                UnloadCustomFace();
        }
    }
}
