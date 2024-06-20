using TMPro;
using UnityEngine;

public class RepairProgress : MonoBehaviour
{
    private TextMeshProUGUI tMesh;

    public void UpdateText()
    {
        tMesh.text = "Прогресс ремонта:\n" + QuestShipRepair.instance.progress.ToString() + "%";
    }
    private void Start()
    {
        QuestShipRepair.instance.ProgressChanged.AddListener(UpdateText);
        tMesh = GetComponentInChildren<TextMeshProUGUI>();
    }
}
