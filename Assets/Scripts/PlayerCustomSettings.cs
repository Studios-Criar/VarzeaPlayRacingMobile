using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlayerCustomSettings
{
    public static Texture2D CurrentTeam { get; private set; }

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
    private static List<Texture2D> _availableTeams = new();

    public static void SetCurrentTeam(Texture2D team, IList<Texture2D> textures)
    {
        CurrentTeam = team;
        _availableTeams = textures.Where(t => t != CurrentTeam).ToList();
        Debug.Log($"New current team: {team.name}");
    }
    
    public static Texture2D GetRandomTeam()
    {
        if (_availableTeams.Count == 0) return null;
        var pickedTeam = _availableTeams[Random.Range(0, _availableTeams.Count)];
        // Debug.Log($"Reserving team {pickedTeam.name}");
        _availableTeams.Remove(pickedTeam);
        return pickedTeam;
    }

    public static void ReleaseTeam(Texture2D team)
    {
        // Debug.Log($"Releasing team {team.name}");
        _availableTeams.Add(team);
    }
}