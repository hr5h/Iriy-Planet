using TMPro;
using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    public int ind;
    public float height;
    public TextMeshProUGUI textMesh;
    public RectTransform rect;

    public void Create(Answer a)
    {
        textMesh.text = a.text;
        textMesh.ForceMeshUpdate();
        height = textMesh.GetRenderedValues()[1];
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
    }
    public void Click()
    {
        DialogController.instance.dialog.ToAnswer(ind);
    }
}
