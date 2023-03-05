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
            Debug.Log("Path" + $"Textures/{texture}");
            _uiTextures[texture] = Resources.Load<Sprite>($"Textures/{texture}");
        }

        Debug.Log("Getting texture" + _uiTextures[texture]);

        return _uiTextures[texture];
    }
}