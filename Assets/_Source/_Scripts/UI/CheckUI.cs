using TMPro;
using UnityEngine;

public class CheckUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void ChangeText(string newText)
    {
        text.text = newText;
    }

}
