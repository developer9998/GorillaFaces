using BepInEx;
using BepInEx.Configuration;
using GorillaFaces.Behaviours;
using GorillaFaces.Behaviours.Networking;
using GorillaFaces.Models;
using GorillaInfoWatch.Models.Attributes;
using GorillaLibrary;
using UnityEngine;

[assembly: InfoWatchCompatible]

namespace GorillaFaces;

[BepInPlugin("dev.gorillafaces", "GorillaFaces", "1.0.3"), BepInDependency("dev.gorillalibrary")]
internal class Plugin : BaseUnityPlugin
{
    public static ConfigEntry<string> CurrentFace;

    public static ConfigEntry<FaceAssignment> DefaultFaceType;

    public static ConfigEntry<string> DefaultFaceName;

    public void Awake()
    {
        CurrentFace = Config.Bind("Local Player Appearance", "Current Face", "Gorilla Face", "The name of the face currently used by the player.");
        DefaultFaceType = Config.Bind("Room Player Appearance", "Default Face Type", FaceAssignment.Random, "The method used for handling assigning a face to a player without the mod.");
        DefaultFaceName = Config.Bind("Room Player Appearance", "Default Face Name", "Gorilla Face", "The name of the defined default face. Default Face Type must be set to Assigned.");

        Events.Core.OnGameInitialized += () =>
        {
            DontDestroyOnLoad(new GameObject("GorillaFaces", typeof(Core), typeof(NetworkHandler)));
        };

        Events.Rig.OnRigAdded += (rig, player) =>
        {
            if (!rig.GetComponent<NetworkedPlayer>())
            {
                NetworkedPlayer networkedPlayer = rig.gameObject.AddComponent<NetworkedPlayer>();
                networkedPlayer.Rig = rig;
                networkedPlayer.Owner = player;
            }
        };

        Events.Rig.OnRigRemoved += (rig) =>
        {
            if (rig.TryGetComponent(out NetworkedPlayer networkedPlayer))
            {
                Destroy(networkedPlayer);
            }
        };
    }
}