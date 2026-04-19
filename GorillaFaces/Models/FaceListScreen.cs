using GorillaFaces.Behaviours;
using GorillaInfoWatch.Models;
using GorillaInfoWatch.Models.Attributes;
using GorillaInfoWatch.Models.Widgets;
using System.Collections.Generic;
using System.Linq;

namespace GorillaFaces.Models
{
    [ShowOnHomeScreen(DisplayTitle = "GorillaFaces")]
    public class FaceListScreen : InfoScreen
    {
        public override string Title => "Faces";
        public override string Description => (Core.Instance.Faces is List<IFaceAsset> faces && Core.Instance.LocalPlayer is Client localPlayer) ? $"{faces.Count} total faces - {(!localPlayer.HasLoadedFace ? "No face loaded" : $"{localPlayer.CustomFace.Name} loaded")}" : "GorillaFaces is importing custom faces from plugins";

        public override void OnScreenLoad()
        {
            if (!Core.Instance.HasFaces) Core.Instance.OnFacesLoaded += OnFacesLoaded;
        }

        public override void OnScreenUnload()
        {
            if (!Core.Instance.HasFaces) Core.Instance.OnFacesLoaded -= OnFacesLoaded;
        }

        public void OnFacesLoaded()
        {
            Core.Instance.OnFacesLoaded -= OnFacesLoaded;
            SetContent();
        }

        public override InfoContent GetContent()
        {
            if (Core.Instance is not Core main || main.Faces is not List<IFaceAsset> faces)
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
                if (Core.Instance is not Core main || main.LocalPlayer is not Client facePlayer)
                    return;

                facePlayer.SwitchCustomFace(customFace);
                SetContent();
            }
        }
    }
}
