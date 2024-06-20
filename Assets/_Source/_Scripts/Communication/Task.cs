using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Task : MonoBehaviour
{
    public string title;
    public string description;
    public float height;
    public TextMeshProUGUI titleMesh;
    public TextMeshProUGUI descriptionMesh;
    public RectTransform background;

    public UnityEvent<Task> OnTaskCreated; ///Событие выдачи задания

    public void Create(string t, string d)
    {
        title = t;
        description = d;

        titleMesh.text = title;
        descriptionMesh.text = description;

        titleMesh.ForceMeshUpdate();
        descriptionMesh.ForceMeshUpdate();

        height = titleMesh.GetRenderedValues()[1] + descriptionMesh.GetRenderedValues()[1];

        background.sizeDelta = new Vector2(background.sizeDelta.x, height - 4);
        descriptionMesh.transform.localPosition = titleMesh.transform.localPosition - new Vector3(-descriptionMesh.transform.localPosition.x, titleMesh.GetRenderedValues()[1]);
    }
}
