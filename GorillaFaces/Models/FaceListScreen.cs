using GorillaFaces.Behaviours;
using GorillaFaces.Tools;
using GorillaInfoWatch.Models;
using GorillaInfoWatch.Models.Attributes;
using GorillaInfoWatch.Models.Widgets;
using System.Collections.Generic;
using System.Linq;

namespace GorillaFaces.Models
{
    [ShowOnHomeScreen(DisplayTitle = Constants.Name)]
    public class FaceListScreen : InfoScreen
    {
        public override string Title => "Faces";
        public override string Description => (Main.Instance.Faces is List<IFaceAsset> faces && Main.Instance.LocalPlayer is GFacesPlayer localPlayer) ? $"{faces.Count} total faces - {(!localPlayer.HasLoadedFace ? "No face loaded" : $"{localPlayer.CustomFace.Name} loaded")}" : "GorillaFaces is importing custom faces from plugins";

        public override void OnScreenLoad()
        {
            if (!Main.Instance.HasFaces) Main.Instance.OnFacesLoaded += OnFacesLoaded;
        }

        public override void OnScreenUnload()
        {
            if (!Main.Instance.HasFaces) Main.Instance.OnFacesLoaded -= OnFacesLoaded;
        }

        public void OnFacesLoaded()
        {
            Main.Instance.OnFacesLoaded -= OnFacesLoaded;

            Logging.Info("Faces have been loaded!! Setting lines for screen");
            SetContent();
        }

        public override InfoContent GetContent()
        {
            if (Main.Instance is not Main main || main.Faces is not List<IFaceAsset> faces)
                return new LineBuilder("Loading faces - please wait!");

            LineBuilder lines = new();

            foreach (IFaceAsset face in faces)
            {
                lines.Add(face.Name, new Widget_PushButton(UseFace, face));
            }

            return lines;
        }

        public void UseFace(params object[] parameters)
        {
            if (parameters.ElementAtOrDefault(0) is IFaceAsset customFace)
            {
                if (Main.Instance is not Main main || main.LocalPlayer is not GFacesPlayer facePlayer)
                    return;

                facePlayer.SwitchCustomFace(customFace);
                SetContent();
            }
        }
    }
}
