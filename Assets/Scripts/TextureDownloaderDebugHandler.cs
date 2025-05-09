using System;
using UnityEngine;

[RequireComponent(typeof(Network.TextureDownloader))]
public class TextureDownloaderDebugHandler : MonoBehaviour
{
    private Network.TextureDownloader _textureDownloader;

    private void Awake()
    {
        _textureDownloader = GetComponent<Network.TextureDownloader>();
    }
        
    private void OnEnable()
    {
        _textureDownloader.OnLog += OnLog;
        _textureDownloader.OnError += OnError;
        _textureDownloader.OnTexturesLoaded += OnFinish;
    }

    private void OnDisable()
    {
        _textureDownloader.OnLog -= OnLog;
        _textureDownloader.OnError -= OnError;
        _textureDownloader.OnTexturesLoaded -= OnFinish;
    }

    private static void OnLog(string message)
    {
        Debug.Log(message);
    }
        
    private static void OnError(Network.TextureDownloader.Error error)
    {
        switch (error.ErrorType)
        {
            case Network.TextureDownloader.ErrorType.NetworkUnavailable:
            case Network.TextureDownloader.ErrorType.ListingFailed:
            case Network.TextureDownloader.ErrorType.DownloadFailed:
            case Network.TextureDownloader.ErrorType.CacheFileError:
            case Network.TextureDownloader.ErrorType.CachedTextureNotFound:
            case Network.TextureDownloader.ErrorType.ReadFromCacheFailed:
                Debug.LogError($"({error.ErrorType}): {error.Message}");
                if (error.Exception != null) Debug.LogException(error.Exception);
                break;
                
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
        
    private static void OnFinish()
    {
        Debug.Log("Textures loaded");
    }
}