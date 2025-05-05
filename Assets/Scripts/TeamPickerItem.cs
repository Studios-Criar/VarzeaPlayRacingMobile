using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Util;

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
        image.sprite = texture.ToSprite();
    }
}