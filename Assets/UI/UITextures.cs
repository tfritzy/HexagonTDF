using System.Collections.Generic;
using UnityEngine;

public enum TextureType
{
    Vignette,
};

public static class UITextures
{
    private static Dictionary<TextureType, Sprite> _uiTextures = new Dictionary<TextureType, Sprite>();
    public static Sprite GetTexture(TextureType texture)
    {
        if (!_uiTextures.ContainsKey(texture))
        {
            _uiTextures[texture] = Resources.Load<Sprite>($"Textures/{texture}");
        }

        return _uiTextures[texture];
    }
}