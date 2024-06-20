using TMPro;
using UnityEngine;

public class FinalAnswerButton : MonoBehaviour
{
    public int index;
    public bool clickable;
    public TextMeshProUGUI tMesh;
    private void Awake()
    {
        tMesh = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void MakeClickable()
    {
        clickable = true;
        tMesh.alpha = 1f;
    }
    public void MakeNoClickable()
    {
        clickable = false;
        tMesh.alpha = 0f;
    }
    public void ButtonClick()
    {
        if (clickable)
            QuestFinal.instance.MakeAChoice(index);
    }
}
