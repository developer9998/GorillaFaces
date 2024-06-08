using UnityEngine;

namespace GorillaFaces.Extensions
{
    public static class TextureEx
    {
        // https://stackoverflow.com/a/40854227
        public static Texture2D WriteTexture(this Texture2D baseTex, Texture2D newTex, Vector2 coordinate)
        {
            int startX = 0;
            int startY = baseTex.height - newTex.height;

            for (int x = startX; x < baseTex.width; x++)
            {
                if (x < 0 || x >= newTex.width) continue;
                for (int y = startY; y < baseTex.height; y++)
                {
                    if (y < 0) continue;

                    Color bgColor = baseTex.GetPixel(x - (int)coordinate.x, y - (int)coordinate.y);
                    Color wmColor = newTex.GetPixel(x - startX, y - startY);

                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a);

                    baseTex.SetPixel(x + (int)coordinate.x, y - (int)coordinate.y, final_color);
                }
            }

            return baseTex;
        }
    }
}
