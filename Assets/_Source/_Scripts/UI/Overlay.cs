using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    public static bool Exists { get; private set; }
    public static Overlay Instance { get; private set; }
    public CanvasScaler canvasScaler;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            canvasScaler = GetComponent<CanvasScaler>();
            Exists = true;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Exists = false;
        }
    }
}
