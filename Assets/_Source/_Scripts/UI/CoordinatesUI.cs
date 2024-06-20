using TMPro;
using UnityEngine;

public class CoordinatesUI : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI textMesh;
    public Transform player = null;
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = "Квадрат (0; 0)";
    }
}
