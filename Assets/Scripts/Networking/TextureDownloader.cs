using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    [RequireComponent(typeof(NetworkingManager))]
    public class TextureDownloader : MonoBehaviour
    {
        [SerializeField] private string baseUrl = "https://varzeaplay.com.br/games/racing/teams/";
    
        public static TextureDownloader Instance { get; private set; }
        public List<Texture2D> TeamTextures => new(_teamTextures.Values);
        public event Action OnTexturesLoaded;
        
        private static string TextureCacheDirectoryPath => Path.Combine(Application.persistentDataPath, "Cache");
        private static string AssetsFilePath => Path.Combine(Application.persistentDataPath, "assets.json");

        private Dictionary<Uri, Texture2D> _teamTextures;
        private Dictionary<Uri, CachedTexture> _textureCache;

        private NetworkingManager _networkingManager;
    
        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Debug.Log($"Assets path: {AssetsPath}");
            Directory.CreateDirectory(TextureCacheDirectoryPath);

            try
            {
                var content = File.ReadAllText(AssetsFilePath);
                _textureCache = JsonConvert.DeserializeObject<Dictionary<Uri, CachedTexture>>(content);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _textureCache = new Dictionary<Uri, CachedTexture>();
            }
            
            _networkingManager = GetComponent<NetworkingManager>();
            Load();
        }

        [ContextMenu("Load")]
        public async void Load()
        {
            var request = await _networkingManager.Get("https://google.com", null);
            
            if (request.error == null)
            {
                Debug.Log("Connected to internet");
                LoadTexturesOnline();
                return;
            }
            
            Debug.LogWarning("No internet connection");
            LoadTexturesOffline();
        }

        private async Task<List<Uri>> ListDirectory(Uri url)
        {
            var request = await _networkingManager.Get(url, null);

            if (request.error != null)
            {
                throw new Exception(request.error);
            }
        
            return NetworkingManager.ParseApacheDirectoryIndex(url, request.downloadHandler.text);
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
            var path = Path.Combine(TextureCacheDirectoryPath, Path.GetFileName(uri.LocalPath));
        
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
            var localTexture = new Texture2D(1, 1);

            if (localTexture.LoadImage(data, false)) return localTexture;
            
            Debug.LogError($"Could not load {uri}");
            return null;
        }

        private async void LoadTexturesOffline()
        {
            _teamTextures = new Dictionary<Uri, Texture2D>();
            
            foreach (var uri in _textureCache.Keys)
            {
                _teamTextures[uri] = await LoadTextureFromCache(uri);
            }
            
            OnTexturesLoaded?.Invoke();
        }

        private async void LoadTexturesOnline()
        {
            _teamTextures = new Dictionary<Uri, Texture2D>();
            
            List<Uri> filesList;

            try
            {
                filesList = await ListDirectory(new Uri(baseUrl));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                LoadTexturesOffline();
                return;
            }
        
            foreach (var uri in filesList)
            {
                Texture2D remoteTexture;

                try
                {
                    remoteTexture = await DownloadTexture(uri);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    continue;
                }

                if (remoteTexture == null)
                {
                    // Debug.Log($"Hit cache: {uri}");
                    _teamTextures[uri] = await LoadTextureFromCache(uri);
                    continue;
                }
            
                _teamTextures[uri] = remoteTexture;
                remoteTexture.name = Path.GetFileNameWithoutExtension(uri.LocalPath);
                var path = Path.Combine(TextureCacheDirectoryPath, Path.GetFileName(uri.LocalPath));
                await File.WriteAllBytesAsync(path, remoteTexture.EncodeToPNG());
            }

            OnTexturesLoaded?.Invoke();
            await File.WriteAllTextAsync(AssetsFilePath, JsonConvert.SerializeObject(_textureCache, Formatting.Indented));
        }
    }
}
