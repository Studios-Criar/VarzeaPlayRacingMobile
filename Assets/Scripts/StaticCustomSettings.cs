using System.Collections.Generic;
using UnityEngine;

public static class StaticCustomSettings
{
    public static TeamPickerItem CurrentTeam
    {
        get => _currentTeam;
        set
        {
            _currentTeam = value;
            Debug.Log($"New current team: {value.Texture.name}");
        }
    }
    
    private static TeamPickerItem _currentTeam;

    public static Color CurrentColor
    {
        get => _currentColor;
        set
        {
            _currentColor = value;
            Debug.Log($"New current color: {value}");
        }
    }

    private static Color _currentColor;
    
    public static List<TeamPickerItem> AvailableTeams { get; set; } = new();
    
    public static TeamPickerItem GetRandomTeam()
    {
        var pickedTeam = AvailableTeams[Random.Range(0, StaticCustomSettings.AvailableTeams.Count)];
        Debug.Log($"Reserving team {pickedTeam.Texture.name}");
        AvailableTeams.Remove(pickedTeam);
        return pickedTeam;
    }

    public static void ReleaseTeam(TeamPickerItem team)
    {
        Debug.Log($"Releasing team {team.Texture.name}");
        AvailableTeams.Add(team);
    }
}