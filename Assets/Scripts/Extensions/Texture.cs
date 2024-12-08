using UnityEngine;

namespace Extensions
{
    public static partial class Extensions
    {
        public static void Fill(this Texture2D texture, Color color)
        {
            var pixels = texture.GetPixels();
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
        }
    }
}