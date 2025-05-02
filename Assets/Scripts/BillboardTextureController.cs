using Networking;
using UnityEngine;

public class BillboardTextureController : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    [SerializeField] private TextureDownloader textureDownloader;
    [SerializeField, Min(10f)] private float updateTime = 30f;
    
    private static readonly int EmissionMapShaderNameId = Shader.PropertyToID("_EmissionMap");

    private void OnEnable()
    {
        textureDownloader.OnTexturesLoaded += OnTexturesLoaded;
    }
    
    private void OnDisable()
    {
        textureDownloader.OnTexturesLoaded -= OnTexturesLoaded;
    }

    private void OnTexturesLoaded()
    {
        CancelInvoke();
        InvokeRepeating(nameof(UpdateTexture), 0, updateTime);
    }

    private void UpdateTexture()
    {
        foreach (var m in materials)
        {
            var texture = GetRandomTexture();
            m.mainTexture = texture;
            m.SetTexture(EmissionMapShaderNameId, texture);
        }
    }

    private Texture2D GetRandomTexture()
    {
        var index = Random.Range(0, textureDownloader.Textures.Count);
        return textureDownloader.Textures.Count == 0 ? null : textureDownloader.Textures[index];
    }
}
