using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TeamPickerItem : MonoBehaviour
{
    [SerializeField] private Image image;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    public void SetUp(UnityAction onClick, Texture2D texture)
    {
        _button.onClick.AddListener(onClick);
        image.sprite = CreateSprite(texture);
    }

    private static Sprite CreateSprite(Texture2D texture2D)
    {
        var pivot = new Vector2(.5f, .5f);
        var rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
        return Sprite.Create(texture2D, rect, pivot, 100f);
    }
}