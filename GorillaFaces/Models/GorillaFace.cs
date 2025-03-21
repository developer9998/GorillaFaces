using UnityEngine;

namespace GorillaFaces.Models
{
	public class GorillaFace
	{
		public string Name;

		public string Path;

		public Texture2D[] MouthArray = (Texture2D[])(object)new Texture2D[11];

		public Texture2D[] EyeArray = (Texture2D[])(object)new Texture2D[5];

		public Texture2D Base;

		public Texture2D MouthSheet;

		public Texture2D EyeSheet;
	}
}