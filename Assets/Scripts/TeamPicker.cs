using Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TeamPicker : MonoBehaviour
{
    [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;
    [SerializeField] private TeamPickerItem teamPickerItemPrefab;
    [SerializeField] private UnityEvent teamPickerItemEvents;
    [SerializeField] private TextureDownloader textureDownloader;
    
    public static event System.Action<Texture2D> OnTeamPicked;
    
    private void OnEnable()
    {
        InstantiateTeams();
        textureDownloader.OnTexturesLoaded += UpdateUI;
    }

    private void OnDisable()
    {
        DestroyTeams();
        textureDownloader.OnTexturesLoaded -= UpdateUI;
    }

    private void UpdateUI()
    {
        DestroyTeams();
        InstantiateTeams();
    }
    
    private void InstantiateTeams()
    {
        var counter = 0;
        
        foreach (var t in textureDownloader.Textures)
        {
            var item = Instantiate(teamPickerItemPrefab, layoutGroup.transform);
            item.SetUp(() =>
            {
                PickTeam(t);
                teamPickerItemEvents?.Invoke();
            }, t);
            
            if (counter == 0) PlayerCustomSettings.SetCurrentTeam(t, textureDownloader.Textures);
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
        PlayerCustomSettings.SetCurrentTeam(texture, textureDownloader.Textures);
        // StaticCustomSettings.AvailableTeams = TextureDownloader.Instance.Textures.Where(t => t != StaticCustomSettings.CurrentTeam).ToList();
        OnTeamPicked?.Invoke(PlayerCustomSettings.CurrentTeam);
    }
}