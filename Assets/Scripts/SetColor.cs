using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SetColor : MonoBehaviour
{
    [SerializeField, Min(0)] private int materialIndex;
    
    private MeshRenderer _meshRenderer;

    private void OnEnable()
    {
        ColorPicker.OnColorPicked += SetUp;
    }
    
    private void OnDisable()
    {
        ColorPicker.OnColorPicked -= SetUp;
    }

    private void SetUp(Color color)
    {
        _meshRenderer.materials[materialIndex].color = color;
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        if (materialIndex >= _meshRenderer.materials.Length)
        {
            Debug.LogError($"Material index ({materialIndex}) out of range ({_meshRenderer.materials.Length})");
            return;
        }
        
        _meshRenderer.materials[materialIndex] = new Material(_meshRenderer.materials[materialIndex]);
        SetUp(PlayerCustomSettings.CurrentColor);
    }
}