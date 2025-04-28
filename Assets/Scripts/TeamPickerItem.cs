using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TeamPickerItem : MonoBehaviour
{
    [SerializeField] private Image image;

    private Button _button;
    
    public Texture2D Texture { get; private set; }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    public void SetUp(UnityAction onClick, Texture2D texture2D)
    {
        Texture = texture2D;
        _button.onClick.AddListener(onClick);
        image.sprite = CreateSprite(Texture);
    }

    private static Sprite CreateSprite(Texture2D texture2D)
    {
        var pivot = new Vector2(.5f, .5f);
        var rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
        const float pixelsPerUnit = 100f;
        
        return Sprite.Create(texture2D, rect, pivot, pixelsPerUnit);
    }
}