using ExitGames.Client.Photon;
using GorillaFaces.Models;
using GorillaFaces.Tools;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace GorillaFaces.Behaviours.Networking
{
    [RequireComponent(typeof(RigContainer)), DisallowMultipleComponent]
    public class NetworkedPlayer : MonoBehaviour
    {
        public VRRig Rig;
        public NetPlayer Owner;
        public Player PlayerRef;

        public bool HasGorillaFaces;

        private GFacesPlayer facePlayer;

        private bool hasFallbackFaces;
        private IFaceAsset randomFace, userIdBasedFace;

        private bool _initialize;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialize) return;
            _initialize = true;

            if (!TryGetComponent(out facePlayer)) facePlayer = gameObject.AddComponent<GFacesPlayer>();

            PlayerRef = (Owner is PunNetPlayer punNetPlayer) ? punNetPlayer.PlayerRef : PhotonNetwork.CurrentRoom.GetPlayer(Owner.ActorNumber);

            if (!HasGorillaFaces)
            {
                TryFallbackFace();
                CheckProperties();
            }
        }

        public void CheckProperties()
        {
            if (PlayerRef is null) return;
            NetworkHandler.Instance.OnPlayerPropertiesUpdate(PlayerRef, PlayerRef.CustomProperties);
        }

        public void LoadFallbackFaces()
        {
            if (hasFallbackFaces || Main.Instance.Faces is not List<IFaceAsset> faces)
                return;

            Random random = new();
            randomFace = faces[random.Next(0, faces.Count)];

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(Owner.UserId));
                int seed = BitConverter.ToInt32(hash, 0);
                random = new Random(seed);
                userIdBasedFace = faces[random.Next(0, faces.Count)];
            }

            hasFallbackFaces = true;
        }

        public void TryFallbackFace()
        {
            if (HasGorillaFaces || Main.Instance.Faces is not List<IFaceAsset> faces || Main.Instance.LocalPlayer is not GFacesPlayer localPlayer)
                return;

            if (!hasFallbackFaces) LoadFallbackFaces();

            if (hasFallbackFaces)
            {
                switch (Configuration.DefaultFaceType.Value)
                {
                    case EDefaultFaceType.Random:
                        facePlayer.LoadCustomFace(randomFace);
                        break;
                    case EDefaultFaceType.RandomSeed:
                        facePlayer.LoadCustomFace(userIdBasedFace);
                        break;
                    case EDefaultFaceType.Matching:
                        if (localPlayer.HasLoadedFace) facePlayer.LoadCustomFace(localPlayer.CustomFace);
                        else facePlayer.UnloadCustomFace();
                        break;
                    case EDefaultFaceType.Assigned:
                        if (faces.Find(face => face.Name == Configuration.DefaultFaceName.Value) is IFaceAsset assignedFace)
                            facePlayer.LoadCustomFace(assignedFace);
                        else
                            facePlayer.UnloadCustomFace();
                        break;
                }
            }

        }

        public void OnDestroy()
        {
            if (HasGorillaFaces || facePlayer.HasLoadedFace)
            {
                HasGorillaFaces = false;
                facePlayer.UnloadCustomFace();
            }
        }

        public void OnPlayerPropertyChanged(Hashtable properties)
        {
            if (!_initialize) Initialize();

            if (properties.TryGetValue("CustomFace", out object obj) && obj is string faceName)
            {
                if (Main.Instance.GetFace(faceName) is IFaceAsset customFace) facePlayer.LoadCustomFace(customFace);
                else facePlayer.UnloadCustomFace();
            }
        }
    }
}
