using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private bool randomColor;
    [SerializeField, Range(0f, 1f)] private float initialHue;
    [SerializeField, Range(0f, 1f)] private float saturation = 1f;
    [SerializeField, Range(0f, 1f)] private float value = 1f;
    
    public static event System.Action<Color> OnColorPicked;

    private void OnEnable()
    {
        var hue = randomColor ? Random.Range(0f, 1f) : initialHue;
        PickColor(hue);
        slider.value = hue;
    }

    public void PickColor(float hue)
    {
        var color = Color.HSVToRGB(hue, saturation, value);
        StaticCustomSettings.CurrentColor = color;
        OnColorPicked?.Invoke(color);
    }
}