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
    public static TeamPickerItem CurrentTeam { get; private set; }
    
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
        
        foreach (var t in TextureDownloader.Instance.TeamTextures)
        {
            var item = Instantiate(teamPickerItemPrefab, layoutGroup.transform);
            item.SetUp(() =>
            {
                PickTeam(item);
                teamPickerItemEvents?.Invoke();
            }, t);

            if (counter == 0) CurrentTeam = item;
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

    private void PickTeam(TeamPickerItem teamPickerItem)
    {
        CurrentTeam = teamPickerItem;
        OnTeamPicked?.Invoke(CurrentTeam);
    }
}