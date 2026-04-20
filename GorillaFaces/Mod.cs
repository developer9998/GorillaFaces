using GorillaFaces;
using GorillaFaces.Behaviours;
using GorillaFaces.Behaviours.Networking;
using GorillaFaces.Models;
using GorillaInfoWatch.Models.Attributes;
using GorillaLibrary;
using GorillaLibrary.Extensions;
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
        CurrentFace = category.CreateSimpleEntry("Current Face", "Gorilla Face", "The face given to the local player");
        DefaultFaceType = category.CreateSimpleEntry("Face Assignment", FaceAssignment.Random, "The means of assigning faces to other players");
        DefaultFaceName = category.CreateSimpleEntry("Face Override", "Gorilla Face", "The face given to other players with the Static assignment");

        Events.Core.OnGameInitialized.Subscribe(() =>
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