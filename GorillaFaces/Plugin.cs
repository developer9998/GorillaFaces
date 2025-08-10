using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GorillaFaces.Behaviours;
using GorillaFaces.Behaviours.Networking;
using GorillaFaces.Tools;
using HarmonyLib;
using UnityEngine;
using GorillaFaces.Models;
using GorillaInfoWatch.Models.Attributes;

[assembly: InfoWatchCompatible]

namespace GorillaFaces
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource MainLogSource;

        public static ConfigFile MainConfig;

        public void Awake()
        {
            MainLogSource = Logger;
            MainConfig = Config;

            Configuration.CurrentFace = MainConfig.Bind(Constants.Name, "Current Face", "Gorilla Face", "The name of the face currently used by the player.");
            Configuration.DefaultFaceType = MainConfig.Bind(Constants.Name, "Default Face Type", EDefaultFaceType.Random, "The method used for handling assigning a face to a player without the mod.");
            Configuration.DefaultFaceName = MainConfig.Bind(Constants.Name, "Default Face Name", "Gorilla Face", "The name of the defined default face. Default Face Type must be set to Assigned.");

            Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, Constants.GUID);
            GorillaTagger.OnPlayerSpawned(() => new GameObject(Constants.Name, typeof(Main), typeof(NetworkHandler)));
        }
    }
}