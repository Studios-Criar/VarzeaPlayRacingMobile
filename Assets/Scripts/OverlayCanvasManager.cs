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
    
    public void Open(string message, bool dismissible = true)
    {
        messageText.text = message.ToUpper();
        overlayCanvas.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(dismissible);
    }

    public void Open(string message, System.Action onClose)
    {
        Open(message);
        
        _confirmButtonAction = () =>
        {
            Debug.Log("Confirm");
            onClose?.Invoke();
            Close();
        };
    }

    public void Close()
    {
        overlayCanvas.gameObject.SetActive(false);
    }
}
