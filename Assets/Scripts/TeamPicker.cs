using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TeamPicker : MonoBehaviour
{
    [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;
    [SerializeField] private TeamPickerItem teamPickerItemPrefab;
    [SerializeField] private UnityEvent teamPickerItemEvents;
    [SerializeField] private bool useTextureDownloaderSingleton;
    [SerializeField] private TextureDownloader textureDownloader;

    public static event Action<Texture2D> OnTeamPicked;
    
    private readonly Dictionary<Texture2D, Texture2D> _teamTextures = new();

    private const string CarTextureNameFormat = "{0}_car";
    private readonly string _carTextureEmptyFormatString = string.Format(CarTextureNameFormat, "");

    private Image _image;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        
        if (useTextureDownloaderSingleton) textureDownloader = TextureDownloaderSingleton.Instance;
    }
    
    private void OnEnable()
    {
        InstantiateTeams();
        textureDownloader.OnTexturesLoaded += OnTexturesLoaded;
    }

    private void OnDisable()
    {
        DestroyTeams();
        textureDownloader.OnTexturesLoaded -= OnTexturesLoaded;
    }

    private void OnTexturesLoaded()
    {
        DestroyTeams();
        InstantiateTeams();
    }

    private void UpdateTextures()
    {
        var allTextures = textureDownloader.Textures;
        var teamTextures = allTextures.Where(t => !t.name.EndsWith(_carTextureEmptyFormatString, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var t in teamTextures)
        {
            var carTextureName = string.Format(CarTextureNameFormat, t.name);
            var carTexture = allTextures.FirstOrDefault(e => e.name.Equals(carTextureName, StringComparison.OrdinalIgnoreCase));

            if (!carTexture)
            {
                Debug.LogWarning($"Could not find car texture for {t.name}. The name should be {carTextureName}");
                continue;
            }
         
            // Debug.Log($"_teamTextures[{t.name}] = {carTexture.name}");
            _teamTextures[t] = carTexture;
        }
    }
    
    private void InstantiateTeams()
    {
        UpdateTextures();

        if (_teamTextures.Count == 0)
        {
            _image.enabled = false;
            return;
        }
        
        _image.enabled = true;
        
        var counter = 0;
        
        foreach (var (teamTexture, carTexture) in _teamTextures)
        {
            var item = Instantiate(teamPickerItemPrefab, layoutGroup.transform);

            item.SetUp(() =>
            {
                PickTeam(carTexture);
                teamPickerItemEvents?.Invoke();
            }, teamTexture);
            
            if (counter == 0) PlayerCustomSettings.SetCurrentTexture(carTexture, _teamTextures.Values.ToList());
            counter++;
        }
    }

    private void DestroyTeams()
    {
        foreach (Transform t in layoutGroup.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void PickTeam(Texture2D texture)
    {
        PlayerCustomSettings.SetCurrentTexture(texture, _teamTextures.Values.ToList());
        OnTeamPicked?.Invoke(texture);
    }
}