using UnityEngine;
using UnityEngine.UI;

public class DragElement : MonoBehaviour
{
    public Image rend;
    private void Awake()
    {
        rend = GetComponent<Image>();
    }
}
