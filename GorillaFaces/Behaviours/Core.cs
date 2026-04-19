using GorillaFaces.Behaviours.Networking;
using GorillaFaces.Extensions;
using GorillaFaces.Models;
using GorillaLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces.Behaviours;

internal class Core : MonoBehaviour
{
    public static Core Instance { get; private set; }

    public bool HasFaces => Faces != null;

    public FaceLoader Loader;

    public List<IFaceAsset> Faces;

    public Client LocalPlayer;

    public Action OnFacesLoaded;

    private Dictionary<string, IFaceAsset> dictNameToFace;

    public async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        VRRig localRig = GorillaTagger.Instance.offlineVRRig;

        Loader = new FaceLoader(Path.GetDirectoryName(typeof(Mod).Assembly.Location), new FaceParameters(localRig.GetMouthFlap(), localRig.GetEyeExpressions()));

        Faces = await Loader.GetAllFaces();

        dictNameToFace = Faces
            .DistinctBy(face => face.Name)
            .ToDictionary(face => face.Name, face => face);

        LocalPlayer = localRig.gameObject.AddComponent<Client>();
        LocalPlayer.OnFaceLoaded += currentFace =>
        {
            NetworkHandler.Instance.SetProperty("CustomFace", currentFace.Name);
            Mod.CurrentFace.Value = currentFace.Name;
            ForEachNetworkedPlayer(player => player.TryFallbackFace());
        };
        LocalPlayer.OnFaceUnloaded += () =>
        {
            NetworkHandler.Instance.SetProperty("CustomFace", string.Empty);
            Mod.CurrentFace.Value = string.Empty;
            ForEachNetworkedPlayer(player => player.TryFallbackFace());
        };

        if (GetFace(Mod.CurrentFace.Value) is IFaceAsset currentFace)
        {
            LocalPlayer.LoadCustomFace(currentFace);
        }

        ForEachNetworkedPlayer(player => player.CheckProperties());

        Mod.DefaultFaceType.OnEntryValueChanged.Subscribe((_, _) =>
        {
            ForEachNetworkedPlayer(player => player.TryFallbackFace());
        });

        OnFacesLoaded?.Invoke();
    }

    private void ForEachNetworkedPlayer(Action<NetworkedPlayer> action)
    {
        if (!NetworkSystem.Instance.InRoom || !VRRigCache.isInitialized) return;

        foreach (RigContainer playerRig in RigUtility.Rigs.Values)
        {
            if (!playerRig.TryGetComponent(out NetworkedPlayer component)) continue;
            action(component);
        }
    }

    public IFaceAsset GetFace(string displayName)
    {
        if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrWhiteSpace(displayName) && dictNameToFace.TryGetValue(displayName, out IFaceAsset face))
            return face;

        return null;
    }
}
