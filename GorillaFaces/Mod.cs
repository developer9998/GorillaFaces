using GorillaFaces;
using GorillaFaces.Behaviours;
using GorillaFaces.Behaviours.Networking;
using GorillaFaces.Models;
using GorillaInfoWatch.Models.Attributes;
using GorillaLibrary;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Mod), "GorillaFaces", "1.0.2", "dev9998")]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]
[assembly: MelonAdditionalDependencies("GorillaLibrary")]
[assembly: InfoWatchCompatible]

namespace GorillaFaces;

internal class Mod : GorillaMod
{
    public static MelonPreferences_Entry<string> CurrentFace;

    public static MelonPreferences_Entry<FaceAssignment> DefaultFaceType;

    public static MelonPreferences_Entry<string> DefaultFaceName;

    public override void OnInitializeMelon()
    {
        MelonPreferences_Category category = CreateCategory("GorillaFaces");
        CurrentFace = category.CreateEntry("Current Face", "Gorilla Face", "Current Face", "The face given to the local player", false, false, null);
        DefaultFaceType = category.CreateEntry("Face Assignment", FaceAssignment.Random, "Face Assignment", "The means of assigning faces to other players", false, false, null);
        DefaultFaceName = category.CreateEntry("Face Override", "Gorilla Face", "Face Override", "The face given to other players with the Static assignment", false, false, null);

        Events.Game.OnGameInitialized.Subscribe(() =>
        {
            Object.DontDestroyOnLoad(new GameObject("GorillaFaces", typeof(Core), typeof(NetworkHandler)));
        });

        Events.Rig.OnRigAdded.Subscribe((rig, player) =>
        {
            if (!rig.GetComponent<NetworkedPlayer>())
            {
                NetworkedPlayer networkedPlayer = rig.gameObject.AddComponent<NetworkedPlayer>();
                networkedPlayer.Rig = rig;
                networkedPlayer.Owner = player;
            }
        });

        Events.Rig.OnRigRemoved.Subscribe((rig) =>
        {
            if (rig.TryGetComponent(out NetworkedPlayer networkedPlayer))
            {
                Object.Destroy(networkedPlayer);
            }
        });
    }
}