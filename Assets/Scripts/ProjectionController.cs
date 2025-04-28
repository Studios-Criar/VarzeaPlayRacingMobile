using System;
using LlockhamIndustries.Decals;
using UnityEngine;

[RequireComponent(typeof(ProjectionRenderer))]
public class ProjectionController : MonoBehaviour
{
    private ProjectionRenderer _projectionRenderer;

    private void OnEnable()
    {
        TeamPicker.OnTeamPicked += UpdateProjection;
    }
    
    private void OnDisable()
    {
        TeamPicker.OnTeamPicked -= UpdateProjection;
    }
    
    private void Awake()
    {
        _projectionRenderer = GetComponent<ProjectionRenderer>();
        _projectionRenderer.Projection = ScriptableObject.CreateInstance<Metallic>();
        _projectionRenderer.Projection.TransparencyType = TransparencyType.Blend;
        
        if (TeamPicker.CurrentTeam) UpdateProjection(TeamPicker.CurrentTeam);
    }

    private void Update()
    {
        _projectionRenderer.enabled = ((Metallic) _projectionRenderer.Projection).albedo.Texture;
    }

    private void UpdateProjection(TeamPickerItem teamPickerItem)
    {
        var projection = (Metallic) _projectionRenderer.Projection;
        
        projection.albedo.Texture = teamPickerItem.Texture;
        projection.albedo.Color = Color.white;
        
        _projectionRenderer.ChangeProjection();
        _projectionRenderer.UpdateProjection();
        _projectionRenderer.UpdateProperties();
    }
}
