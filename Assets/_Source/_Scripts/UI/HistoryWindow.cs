using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryWindow : MonoBehaviour
{
    public RectTransform rect;
    public TextMeshProUGUI textMesh;

    private void Start()
    {
        HistoryController.instance.OnTextUpdate.AddListener(TextUpdate);
        TextUpdate();
    }
    public void TextUpdate()
    {
        //Logger.Debug(HistoryController.instance.text.ToString());
        textMesh.text = HistoryController.instance.text.ToString();
        textMesh.ForceMeshUpdate();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, textMesh.GetRenderedValues()[1]);
    }
}
