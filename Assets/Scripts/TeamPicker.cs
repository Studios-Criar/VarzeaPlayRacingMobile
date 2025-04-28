using System.Collections.Generic;
using System.Linq;
using Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TeamPicker : MonoBehaviour
{
    [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;
    [SerializeField] private TeamPickerItem teamPickerItemPrefab;
    [SerializeField] private UnityEvent teamPickerItemEvents;
    
    public static event System.Action<TeamPickerItem> OnTeamPicked;
    
    private List<TeamPickerItem> _teamPickerItems = new();
    
    private void OnEnable()
    {
        InstantiateTeams();
        TextureDownloader.Instance.OnTexturesLoaded += UpdateUI;
    }

    private void OnDisable()
    {
        DestroyTeams();
        TextureDownloader.Instance.OnTexturesLoaded -= UpdateUI;
    }

    private void UpdateUI()
    {
        DestroyTeams();
        InstantiateTeams();
    }
    
    private void InstantiateTeams()
    {
        var counter = 0;
        
        foreach (var t in TextureDownloader.Instance.Textures)
        {
            var item = Instantiate(teamPickerItemPrefab, layoutGroup.transform);
            item.SetUp(() =>
            {
                PickTeam(item);
                teamPickerItemEvents?.Invoke();
            }, t);

            _teamPickerItems.Add(item);
            if (counter == 0) StaticCustomSettings.CurrentTeam = item;
            counter++;
        }
    }

    private void DestroyTeams()
    {
        foreach (Transform t in layoutGroup.transform)
        {
            Destroy(t.gameObject);
        }
        
        _teamPickerItems.Clear();
    }

    private void PickTeam(TeamPickerItem teamPickerItem)
    {
        StaticCustomSettings.CurrentTeam = teamPickerItem;
        OnTeamPicked?.Invoke(StaticCustomSettings.CurrentTeam);
        StaticCustomSettings.AvailableTeams = _teamPickerItems.Where(t => t != StaticCustomSettings.CurrentTeam).ToList();
    }
}