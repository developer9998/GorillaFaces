using System;
using UnityEngine;

namespace GorillaFaces.Extensions;

internal static class TextureExtensions
{
    public static Texture2D Clone(this Texture2D source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        RenderTexture renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        Graphics.Blit(source, renderTexture);

        RenderTexture active = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D texture = new(source.width, source.height, TextureFormat.RGBA32, false)
        {
            name = source.name,
            filterMode = source.filterMode
        };
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = active;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture;
    }

    public static Texture2D Overlay(this Texture2D source, Texture2D overlay, Vector2 pivot, bool overridePixels = true)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (!source.isReadable)
            throw new ArgumentException("Texture source is not readable", nameof(source));

        if (overlay == null)
            throw new ArgumentNullException(nameof(overlay));

        if (!overlay.isReadable)
            throw new ArgumentException("Texture overlay is not readable", nameof(overlay));

        int posX = (int)pivot.x;
        int posY = (int)(pivot.y + overlay.height);

        // Logging.Info($"{overlay.name} overriding {source.name} at {posX}x{posY} ({pivot.x}, {pivot.y})");

        for (int x = 0; x < overlay.width; x++)
        {
            for (int y = 0; y < overlay.height; y++)
            {
                int destX = posX + x;
                int destY = posY + y;

                if (destX < 0 || destX >= source.width || destY < 0 || destY >= source.height)
                    continue;

                Color overlayColor = overlay.GetPixel(x, y);
                Color sourceColor = source.GetPixel(destX, destY);

                source.SetPixel(destX, destY, overridePixels ? overlayColor : Color.Lerp(sourceColor, overlayColor, overlayColor.a));
            }
        }

        source.Apply();
        return source;
    }

    public static Texture2D Resize(this Texture2D source, float scaleFactor)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (scaleFactor <= 0)
            throw new ArgumentOutOfRangeException(nameof(scaleFactor), "ScaleFactor must be a number larger than zero");

        return source.Resize(new Vector2(Mathf.RoundToInt(source.width * scaleFactor), Mathf.RoundToInt(source.height * scaleFactor)));
    }

    public static Texture2D Resize(this Texture2D source, Vector2 size)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (!source.isReadable)
            throw new ArgumentException("Texture source is not readable", nameof(source));

        if (size.x <= 0 || size.y <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size coordinates must be a number larger than zero");

        int width = (int)size.x;
        int height = (int)size.y;

        Texture2D texture = new(width, height, source.format, false)
        {
            name = source.name,
            filterMode = source.filterMode
        };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int targetX = Mathf.FloorToInt((float)x / width * source.width);
                int targetY = Mathf.FloorToInt((float)y / height * source.height);

                Color color = source.GetPixel(targetX, targetY);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }
}