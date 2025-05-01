using LlockhamIndustries.Decals;
using UnityEngine;

public class ProjectionController : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private ProjectionRenderer[] projectionRenderers;

    private Texture2D _currentTeam;

    private void OnEnable()
    {
        TeamPicker.OnTeamPicked += OnTeamPicked;
    }
    
    private void OnDisable()
    {
        TeamPicker.OnTeamPicked -= OnTeamPicked;
    }

    private void OnDestroy()
    {
        PlayerCustomSettings.ReleaseTeam(_currentTeam);
    }

    private void Awake()
    {
        foreach (var projectionRenderer in projectionRenderers)
        {
            projectionRenderer.Projection = ScriptableObject.CreateInstance<Metallic>();
            projectionRenderer.Projection.TransparencyType = TransparencyType.Blend;
        }

        _currentTeam = !isPlayer ? PlayerCustomSettings.GetRandomTeam() : PlayerCustomSettings.CurrentTeam;
        UpdateProjection(_currentTeam);
    }

    private void Update()
    {
        foreach (var projectionRenderer in projectionRenderers)
        {
            projectionRenderer.enabled = ((Metallic)projectionRenderer.Projection).albedo.Texture;
        }
    }

    private void OnTeamPicked(Texture2D texture)
    {
        _currentTeam = texture;
        UpdateProjection(texture);
    }

    private void UpdateProjection(Texture2D texture)
    {
        foreach (var projectionRenderer in projectionRenderers)
        {
            var projection = (Metallic)projectionRenderer.Projection;

            projection.albedo.Texture = texture;
            projection.albedo.Color = Color.white;

            projectionRenderer.ChangeProjection();
            // projectionRenderer.UpdateProperties();
        }
    }
}
