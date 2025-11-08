using GorillaFaces.Behaviours.Networking;
using GorillaFaces.Models;
using GorillaFaces.Tools;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GorillaFaces.Behaviours
{
    internal class Main : Singleton<Main>
    {
        public bool HasFaces => Faces != null;

        public FaceLoader Loader;

        public List<IFaceAsset> Faces;

        public GFacesPlayer LocalPlayer;

        public Action OnFacesLoaded;

        private Dictionary<string, IFaceAsset> dictNameToFace;

        public override async void Initialize()
        {
            DontDestroyOnLoad(gameObject);

            VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;

            Loader = new FaceLoader(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), new FaceConfig(offlineVRRig.myMouthFlap, offlineVRRig.myEyeExpressions));

            Faces = await Loader.GetAllFaces();

            dictNameToFace = Faces
                .DistinctBy(face => face.Name)
                .ToDictionary(face => face.Name, face => face);

            LocalPlayer = offlineVRRig.gameObject.AddComponent<GFacesPlayer>();
            LocalPlayer.OnFaceLoaded += (IFaceAsset currentFace) =>
            {
                NetworkHandler.Instance.SetProperty("CustomFace", currentFace.Name);
                Configuration.CurrentFace.Value = currentFace.Name;
            };
            LocalPlayer.OnFaceUnloaded += () =>
            {
                NetworkHandler.Instance.SetProperty("CustomFace", string.Empty);
                Configuration.CurrentFace.Value = string.Empty;
            };
            if (GetFace(Configuration.CurrentFace.Value) is IFaceAsset currentFace)
            {
                LocalPlayer.LoadCustomFace(currentFace);
            }

            CheckProperties();
            Configuration.DefaultFaceType.SettingChanged += (object sender, EventArgs args) =>
            {
                CheckProperties();
            };

            OnFacesLoaded?.Invoke();
        }

        public void CheckProperties()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (var netPlayer in NetworkSystem.Instance.PlayerListOthers)
                {
                    if (netPlayer is PunNetPlayer punNetPlayer && punNetPlayer.PlayerRef is Player player)
                    {
                        NetworkHandler.Instance.OnPlayerPropertiesUpdate(player, player.CustomProperties);
                    }
                }
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
