using System.Collections.Generic;
using System.Linq;
using GorillaFaces.Behaviours;
using GorillaFaces.Tools;
using GorillaInfoWatch.Models;
using GorillaInfoWatch.Models.Attributes;
using GorillaInfoWatch.Models.Widgets;

namespace GorillaFaces.Models
{
    [ShowOnHomeScreen(DisplayTitle = Constants.Name)]
    public class FaceListScreen : Screen
    {
        public override string Title => "Faces";
        public override string Description => (Singleton<Main>.Instance is Main main && main.Faces is List<IFaceAsset> faces && main.LocalPlayer is GFacesPlayer localPlayer) ? $"{faces.Count} total faces - {(!localPlayer.IsFaceLoaded ? "No face loaded" : $"{localPlayer.CustomFace.Name} loaded")}" : "GorillaFaces is importing custom faces from plugins";

        public override void OnShow()
        {
            base.OnShow();

            if (!Singleton<Main>.Instance.HasFaces)
                Singleton<Main>.Instance.OnFacesLoaded += OnFacesLoaded;
        }

        public override void OnClose()
        {
            base.OnClose();

            if (!Singleton<Main>.Instance.HasFaces)
                Singleton<Main>.Instance.OnFacesLoaded -= OnFacesLoaded;
        }

        public void OnFacesLoaded()
        {
            Singleton<Main>.Instance.OnFacesLoaded -= OnFacesLoaded;

            Logging.Info("Faces have been loaded!! Setting lines for screen");
            SetContent();
        }

        public override ScreenLines GetContent()
        {
            if (Singleton<Main>.Instance is not Main main || main.Faces is not List<IFaceAsset> faces)
                return new LineBuilder("Loading faces - please wait!");

            LineBuilder lines = new();

            foreach(IFaceAsset face in faces)
            {
                lines.Add(face.Name, new Widget_PushButton(UseFace, face));
            }

            return lines;
        }

        public void UseFace(params object[] parameters)
        {
            if (parameters.ElementAtOrDefault(0) is IFaceAsset customFace)
            {
                if (Singleton<Main>.Instance is not Main main || main.LocalPlayer is not GFacesPlayer facePlayer)
                    return;

                facePlayer.SwitchCustomFace(customFace);
                SetContent();
            }
        }
    }
}
