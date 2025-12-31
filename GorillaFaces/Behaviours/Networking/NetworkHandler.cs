using ExitGames.Client.Photon;
using GorillaFaces.Tools;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GorillaFaces.Behaviours.Networking
{
    internal class NetworkHandler : MonoBehaviourPunCallbacks
    {
        public static NetworkHandler Instance { get; private set; }

        public Action<NetPlayer, Dictionary<string, object>> OnPlayerPropertyChanged;

        private readonly Dictionary<string, object> _properties = [];
        private bool _setProperties;
        private float _setPropertyTimer;

        public void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            if (NetworkSystem.Instance && NetworkSystem.Instance is NetworkSystemPUN)
            {
                SetProperty("Version", Constants.Version);
                return;
            }

            enabled = false;
        }

        public void Update()
        {
            _setPropertyTimer -= Time.deltaTime;

            if (_setProperties && _properties.Count > 0 && _setPropertyTimer <= 0)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new()
                {
                    {
                        Constants.CustomProperty,
                        _properties
                    }
                });

                _setProperties = false;
                _setPropertyTimer = Constants.NetworkSetInterval;
            }
        }

        public void SetProperty(string key, object value)
        {
            if (_properties.ContainsKey(key)) _properties[key] = value;
            else _properties.Add(key, value);

            _setProperties = true;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            NetPlayer netPlayer = NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber);

            if (netPlayer.IsLocal || !VRRigCache.rigsInUse.TryGetValue(netPlayer, out RigContainer playerRig) || !playerRig.TryGetComponent(out NetworkedPlayer networkedPlayer))
                return;

            if (changedProps.TryGetValue(Constants.CustomProperty, out object property) && property is Dictionary<string, object> dictionary)
            {
                networkedPlayer.HasGorillaFaces = true;

                Logging.Info($"Recieved properties from {netPlayer.NickName}: {string.Join(", ", dictionary.Select(prop => $"[{prop.Key}: {prop.Value}]"))}");
                OnPlayerPropertyChanged?.Invoke(netPlayer, dictionary);

                return;
            }
        }
    }
}
