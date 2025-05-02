using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CarMaterialController : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField, Min(0)] private int materialIndex;

    private MeshRenderer _meshRenderer;
    private Texture2D _currentTexture;

    private void OnEnable()
    {
        ColorPicker.OnColorPicked += UpdateColor;
        TeamPicker.OnTeamPicked += UpdateTexture;
    }

    private void OnDisable()
    {
        ColorPicker.OnColorPicked -= UpdateColor;
        TeamPicker.OnTeamPicked -= UpdateTexture;
    }
    
    private void OnDestroy()
    {
        PlayerCustomSettings.ReleaseTexture(_currentTexture);
    }
    
    private void UpdateColor(Color color)
    {
        if (materialIndex >= _meshRenderer.materials.Length)
        {
            Debug.LogError($"Material index ({materialIndex}) out of range ({_meshRenderer.materials.Length})");
            return;
        }
        
        _meshRenderer.materials[materialIndex].color = color;
    }

    private void UpdateTexture(Texture2D texture)
    {
        _currentTexture = texture;
        
        if (materialIndex >= _meshRenderer.materials.Length)
        {
            Debug.LogError($"Material index ({materialIndex}) out of range ({_meshRenderer.materials.Length})");
            return;
        }
        
        _meshRenderer.materials[materialIndex].mainTexture = _currentTexture;
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _currentTexture = isPlayer ? PlayerCustomSettings.CurrentTexture : PlayerCustomSettings.GetRandomTexture();
        if (_currentTexture) UpdateTexture(_currentTexture);
        if (isPlayer) UpdateColor(PlayerCustomSettings.CurrentColor);
    }
}
