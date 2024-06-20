using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dialog : MonoBehaviour
{
    public Character speaker; //Персонаж, с которым игрок общается
    public List<DialogNode> nodes;
    public int current;

    private void Start()
    {
        speaker = GetComponent<Character>();
        speaker.dialog = this;
    }
    public void StartDialog()
    {
        current = 0;
        while (nodes[current] == null)
        {
            current++;
            if (current == nodes.Count)
                break;
        }
        DialogController.instance.Open(this);
        DialogUI.instance.PrintNode(nodes[current].textNPC);
        CreateAnswers();
    }
    public void ToAnswer(int ind) //Ответить и перейти к следующей реплике
    {
        DialogUI.instance.PrintNode(nodes[current].answers[ind].text, true);
        DialogUI.instance.ClearAnswers();
        nodes[current].answers[ind].Actions?.Invoke(); //Вызвать все действия, привязанные к ответу
        var t = nodes[current].answers[ind].next;
        var node = nodes[current];
        var answer = nodes[current].answers[ind];
        if (!answer.loop)
        {
            node.answers.RemoveAt(ind);
        }
        if (answer.removeNode)
        {
            nodes[current] = null;
            //for (int i = current; i< nodes.Count; i++)
            //{
            //    for (int j = 0; j<nodes[i].answers.Count; j++)
            //    {
            //        if (nodes[i].answers[j].next>=current)
            //        {
            //            nodes[i].answers[j].next--;
            //        }
            //    }
            //}
        }
        current = t;
        if (current != -1)
        {
            DialogUI.instance.PrintNode(nodes[current].textNPC);
            CreateAnswers();
        }
        else
        {
            DialogController.instance.Close();
        }
    }
    void CreateAnswers()
    {
        for (int i = 0; i < nodes[current].answers.Count; i++)
        {
            DialogController.instance.condition = true;
            nodes[current].answers[i].Conditions?.Invoke(); //Проверить все условия вдля ответа
            if (DialogController.instance.condition)
            {
                DialogUI.instance.CreateButton(nodes[current].answers[i], i);
            }
        }
    }
}

[System.Serializable]
public class DialogNode //Реплики NPC
{
    [TextArea(3, 10)]
    public string textNPC;
    public List<Answer> answers;
}

[System.Serializable]
public class Answer //Ответы игрока
{
    [Header("Действия после ответа")]
    public UnityEvent Actions; //Действия после ответа
    [Header("Условия выдачи ответа")]
    public UnityEvent Conditions; //Условия появления ответа
    [TextArea(3, 10)]
    public string text; //Текст ответа
    public int next; //Следующий узел диалога (-1 Завершение диалога)
    [Header("Является ли ответ повторяющимся")]
    public bool loop;
    [Header("Удаляется ли реплика после ответа")]
    public bool removeNode;
}