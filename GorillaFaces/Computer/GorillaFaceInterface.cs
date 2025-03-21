using System.Collections.Generic;
using System.Text;
using GorillaComputer.Behaviours;
using GorillaComputer.Models;
using GorillaFaces.Models;
using UnityEngine;

[assembly: ComputerScannable]
namespace GorillaFaces.Computer
{
	[ComputerCustomScreen]
	public class GorillaFaceInterface : ComputerScreen
	{
		private int PlayerIndex;

		public override string Title => "Faces";

		public override string Summary => "Use 'W' & 'S' to navigate faces - Press 'ENTER' to use face";

		public override void OnScreenShow()
		{
			PlayerIndex = 0;
		}

		public override string GetContent()
		{
			StringBuilder stringBuilder = new();
			List<GorillaFace> faces = Plugin.Instance.Faces;
			PlayerIndex = Mathf.Clamp(PlayerIndex, 0, faces.Count - 1);
			for (int i = 0; i < faces.Count; i++)
			{
				GorillaFace gorillaFace = faces[i];
				stringBuilder.AppendLine(" " + ((i == PlayerIndex) ? ">" : "") + " " + gorillaFace.Name);
			}
			return stringBuilder.ToString();
		}

		public override void ProcessScreen(KeyBinding key)
		{
			if (key == KeyBinding.W)
			{
				PlayerIndex--;
				UpdateScreen();
			}
			else if (key == KeyBinding.S)
			{
				PlayerIndex++;
				UpdateScreen();
			}
			else if (key == KeyBinding.enter)
			{
				GorillaFace face = Plugin.Instance.Faces[PlayerIndex];
				Events.Instance.UpdateFace(face);
			}
		}
	}
}