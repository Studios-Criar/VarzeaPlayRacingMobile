using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    [RequireComponent(typeof(NetworkingManager))]
    public class TextureDownloader : MonoBehaviour
    {
        [SerializeField, Tooltip("Texture directory URL")]
        private string baseUrl;
        
        [SerializeField, Tooltip("This key must be unique. No other texture downloader should have the same key")]
        private string key;
        
        private string TextureCacheDirectoryPath => Path.Combine(Application.persistentDataPath, "Cache", key);
        private string AssetsFilePath => Path.Combine(TextureCacheDirectoryPath, $"{key} Assets.json");

        public event Action OnTexturesLoaded;
        
        public List<Texture2D> Textures => new(_textures.Values);
        
        private Dictionary<Uri, Texture2D> _textures;
        private Dictionary<Uri, CachedTexture> _textureCache;

        private NetworkingManager _networkingManager;
    
        private void Start()
        {
            _networkingManager = GetComponent<NetworkingManager>();
            
            // Debug.Log($"Assets path: {AssetsPath}");
            Directory.CreateDirectory(TextureCacheDirectoryPath);

            try
            {
                var content = File.ReadAllText(AssetsFilePath);
                _textureCache = JsonConvert.DeserializeObject<Dictionary<Uri, CachedTexture>>(content);
            }
            catch (FileNotFoundException)
            {
                Debug.LogWarning("Texture cache file not found");
                _textureCache = new Dictionary<Uri, CachedTexture>();
            }
            catch (Exception e)
            {
                OverlayCanvasManager.Instance.Open($"An error occurred trying to read from cache: {e.Message}", true, Load);
                _textureCache = new Dictionary<Uri, CachedTexture>();
                Debug.LogException(e);
                return;
            }
            
            Load();
        }

        [ContextMenu("Load")]
        public async void Load()
        {
            OverlayCanvasManager.Instance.Open("Checking internet connection...", false);
            
            var request = await _networkingManager.Get("https://google.com", null);
            
            if (request.error == null)
            {
                LoadTexturesOnline();
                return;
            }
            
            OverlayCanvasManager.Instance.Open("No internet connection. Using cache.", true, LoadTexturesOffline);
        }

        private async Task<List<Uri>> ListDirectory(Uri url, string[] extensions = null)
        {
            var request = await _networkingManager.Get(url, null);

            if (request.error != null)
            {
                throw new Exception(request.error);
            }
            
            return NetworkingManager.ParseApacheDirectoryIndex(url, request.downloadHandler.text, extensions);
        }
    
        private async Task<Texture2D> DownloadTexture(Uri uri, bool useCache = true)
        {
            var headers = new Dictionary<string, string>();
        
            if (useCache && _textureCache.TryGetValue(uri, out var asset))
            {
                headers["If-None-Match"] = asset.etag;
                // Debug.Log($"Using cache. ETag: {asset.etag}");
            }

            var request = await _networkingManager.GetTexture(uri, headers);
        
            // Debug.Log($"Response Code: {request.responseCode}");

            // Cache hit
            if (request.responseCode == 304)
            {
                return null;
            }
        
            if (request.error != null)
            {
                throw new Exception($"(Request) {request.error}\n" + 
                                    $"(DownloadHandler){request.downloadHandler.error}");
            }

            var texture = DownloadHandlerTexture.GetContent(request);
            var etag = request.GetResponseHeader("etag");
        
            SaveTextureToCache(uri, etag);

            return texture;
        }

        private void SaveTextureToCache(Uri uri, string etag)
        {
            var path = GetTexturePath(uri);
        
            _textureCache[uri] = new CachedTexture
            {
                Uri = uri,
                path = path,
                name = Path.GetFileNameWithoutExtension(uri.LocalPath),
                etag = etag
            };
        
            // Debug.Log($"Caching asset: {_textureCache[uri].path}");
        }

        private async Task<Texture2D> LoadTextureFromCache(Uri uri)
        {
            if (!_textureCache.TryGetValue(uri, out var cachedTexture))
            {
                Debug.LogError($"Could not find {uri} in cache");
                return null;
            }
            
            var data = await File.ReadAllBytesAsync(cachedTexture.path);
            
            var localTexture = new Texture2D(1, 1)
            {
                name = Path.GetFileNameWithoutExtension(uri.LocalPath)
            };

            if (localTexture.LoadImage(data, false)) return localTexture;
            
            Debug.LogError($"Could not load {uri}");
            return null;
        }
        
        private void ClearCache(IList<Uri> fileList)
        {
            var deleteList = _textureCache.Where(f => !fileList.Contains(f.Key)).ToList();
            
            foreach (var (uri, cachedTexture) in deleteList)
            {
                try
                {
                    Debug.Log($"Deleting {cachedTexture.name} ({uri})");
                    File.Delete(cachedTexture.path);
                    _textureCache.Remove(uri);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private async void LoadTexturesOffline()
        {
            _textures = new Dictionary<Uri, Texture2D>();
            
            foreach (var uri in _textureCache.Keys)
            {
                var textureName = Path.GetFileNameWithoutExtension(uri.LocalPath);
                OverlayCanvasManager.Instance.Open($"Loading texture from cache: {textureName}", false);
                var texture = await LoadTextureFromCache(uri);
                if (texture) _textures[uri] = texture;
            }
            
            OnTexturesLoaded?.Invoke();
            OverlayCanvasManager.Instance.Close();
        }

        private async void LoadTexturesOnline()
        {
            _textures = new Dictionary<Uri, Texture2D>();
            
            List<Uri> fileList;

            try
            {
                // var extensions = new[] { ".png", ".jpg", ".jpeg" };
                // fileList = await ListDirectory(new Uri(baseUrl), extensions);
                fileList = await ListDirectory(new Uri(baseUrl));
            }
            catch (Exception e)
            {
                OverlayCanvasManager.Instance.Open("Failed to retrieve textures online. Using cache.", true, LoadTexturesOffline);
                Debug.LogException(e);
                return;
            }

            ClearCache(fileList);
        
            foreach (var uri in fileList)
            {
                var textureName = Path.GetFileNameWithoutExtension(uri.LocalPath);
                OverlayCanvasManager.Instance.Open($"Downloading texture: {textureName}", false);
                
                Texture2D remoteTexture;

                try
                {
                    remoteTexture = await DownloadTexture(uri);
                }
                catch (Exception e)
                {
                    OverlayCanvasManager.Instance.Open($"Error: {e.Message}");
                    Debug.LogException(e);
                    continue;
                }

                if (remoteTexture == null)
                {
                    // Debug.Log($"Hit cache: {uri}");
                    _textures[uri] = await LoadTextureFromCache(uri);
                    continue;
                }
            
                _textures[uri] = remoteTexture;
                remoteTexture.name = textureName;
                var path = GetTexturePath(uri);
                await File.WriteAllBytesAsync(path, remoteTexture.EncodeToPNG());
            }

            OnTexturesLoaded?.Invoke();
            await File.WriteAllTextAsync(AssetsFilePath, JsonConvert.SerializeObject(_textureCache, Formatting.Indented));
            OverlayCanvasManager.Instance.Close();
        }
        
        private string GetTexturePath(Uri uri)
        {
            var path = Path.Combine(TextureCacheDirectoryPath, Path.GetFileName(uri.LocalPath));
            return Path.ChangeExtension(path, "png");
        }
    }
}
