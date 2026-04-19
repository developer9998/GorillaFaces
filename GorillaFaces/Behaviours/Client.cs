using GorillaFaces.Extensions;
using GorillaFaces.Models;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace GorillaFaces.Behaviours;

[RequireComponent(typeof(RigContainer)), DisallowMultipleComponent]
public class Client : MonoBehaviour
{
    public RigContainer Container;

    public Player Creator;

    public GorillaMouthFlap MouthFlap;

    public Action<IFaceAsset> OnFaceLoaded;
    public Action OnFaceUnloaded;

    public bool HasLoadedFace;
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
        Creator = Container.Rig.isLocal ? PhotonNetwork.LocalPlayer : Container.Creator.GetPlayerRef();
        MouthFlap = Container.Rig.GetMouthFlap() ?? GetComponent<GorillaMouthFlap>();

        faceMaterial = MouthFlap.targetFace.GetComponent<Renderer>().material;
        originalFace = faceMaterial.GetTexture(faceMaterialProperty) as Texture2DArray;
        atlasSlice = faceMaterial.GetFloat(faceMaterialSliceProperty);
        originalMouth = faceMaterial.GetTexture(mouthMaterialProperty) as Texture2D;
    }

    public void LoadCustomFace(IFaceAsset newFace)
    {
        if (newFace is null)
            throw new ArgumentNullException(nameof(newFace));

        if (HasLoadedFace) UnloadCustomFace();

        HasLoadedFace = true;
        CustomFace = newFace;

        faceMaterial.SetTexture(faceMaterialProperty, newFace.FaceMap);
        faceMaterial.SetInt(faceMaterialSliceProperty, 0);
        faceMaterial.SetTexture(mouthMaterialProperty, newFace.MouthMap);

        OnFaceLoaded?.Invoke(newFace);
    }

    public void UnloadCustomFace()
    {
        if (!HasLoadedFace) return;

        HasLoadedFace = false;
        CustomFace = null;

        faceMaterial.SetTexture(faceMaterialProperty, originalFace);
        faceMaterial.SetFloat(faceMaterialSliceProperty, atlasSlice);
        faceMaterial.SetTexture(mouthMaterialProperty, originalMouth);

        OnFaceUnloaded?.Invoke();
    }

    public void SwitchCustomFace(IFaceAsset customFace)
    {
        if (CustomFace != customFace) LoadCustomFace(customFace);
        else UnloadCustomFace();
    }
}
