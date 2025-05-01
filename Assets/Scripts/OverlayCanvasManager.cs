using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayCanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas overlayCanvas;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;

    private System.Action _confirmButtonAction;

    public static OverlayCanvasManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        confirmButton.onClick.AddListener(() => _confirmButtonAction?.Invoke());
    }
    
    public void Open(string message, bool dismissible = true, System.Action onClose = null)
    {
        messageText.text = message;
        overlayCanvas.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(dismissible);

        _confirmButtonAction = () =>
        {
            onClose?.Invoke();
            Close();
        };
    }

    public void Close()
    {
        overlayCanvas.gameObject.SetActive(false);
    }
}
