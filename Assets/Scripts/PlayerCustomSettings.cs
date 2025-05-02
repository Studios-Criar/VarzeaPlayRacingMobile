using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlayerCustomSettings
{
    public static Texture2D CurrentTexture { get; private set; }

    public static Color CurrentColor
    {
        get => _currentColor;
        set
        {
            _currentColor = value;
            Debug.Log($"Current color: {value}");
        }
    }

    private static Color _currentColor = Color.white;
    private static List<Texture2D> _availableTextures = new();

    public static void SetCurrentTexture(Texture2D team, IList<Texture2D> textures)
    {
        CurrentTexture = team;
        _availableTextures = textures.Where(t => t != CurrentTexture).ToList();
        Debug.Log($"Current texture: {team.name}");
    }
    
    public static Texture2D GetRandomTexture()
    {
        if (_availableTextures.Count == 0) return null;
        var pickedTexture = _availableTextures[Random.Range(0, _availableTextures.Count)];
        // Debug.Log($"Reserving texture {pickedTexture.name}");
        _availableTextures.Remove(pickedTexture);
        return pickedTexture;
    }

    public static void ReleaseTexture(Texture2D texture)
    {
        // Debug.Log($"Releasing texture {texture.name}");
        _availableTextures.Add(texture);
    }
}