using GorillaFaces.Behaviours.Networking;
using GorillaFaces.Models;
using GorillaFaces.Tools;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces.Behaviours
{
    internal class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }

        public bool HasFaces => Faces != null;

        public FaceLoader Loader;

        public List<IFaceAsset> Faces;

        public GFacesPlayer LocalPlayer;

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

            Loader = new FaceLoader(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), new FaceParameters(localRig.myMouthFlap, localRig.myEyeExpressions));

            Faces = await Loader.GetAllFaces();

            dictNameToFace = Faces
                .DistinctBy(face => face.Name)
                .ToDictionary(face => face.Name, face => face);

            LocalPlayer = localRig.gameObject.AddComponent<GFacesPlayer>();
            LocalPlayer.OnFaceLoaded += currentFace =>
            {
                NetworkHandler.Instance.SetProperty("CustomFace", currentFace.Name);
                Configuration.CurrentFace.Value = currentFace.Name;
                ForEachNetworkedPlayer(player => player.TryFallbackFace());
            };
            LocalPlayer.OnFaceUnloaded += () =>
            {
                NetworkHandler.Instance.SetProperty("CustomFace", string.Empty);
                Configuration.CurrentFace.Value = string.Empty;
                ForEachNetworkedPlayer(player => player.TryFallbackFace());
            };

            if (GetFace(Configuration.CurrentFace.Value) is IFaceAsset currentFace)
            {
                LocalPlayer.LoadCustomFace(currentFace);
            }

            ForEachNetworkedPlayer(player => player.CheckProperties());

            Configuration.DefaultFaceType.SettingChanged += (_, _) =>
            {
                ForEachNetworkedPlayer(player => player.TryFallbackFace());
            };

            OnFacesLoaded?.Invoke();
        }

        private void ForEachNetworkedPlayer(Action<NetworkedPlayer> action)
        {
            if (!NetworkSystem.Instance.InRoom || !VRRigCache.isInitialized) return;

            foreach (RigContainer playerRig in VRRigCache.rigsInUse.Values)
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

        public async void RequestContent(Action onYieldResponse, Action<FaceContent> onContentRecieved)
        {
            if (HasFaces)
                goto SendContent;

            onYieldResponse?.Invoke();
            while (!HasFaces)
                await Task.Yield();

            SendContent:
            onContentRecieved?.Invoke(new FaceContent()
            {
                Faces = Faces,
                LocalPlayer = LocalPlayer
            });
        }
    }
}
