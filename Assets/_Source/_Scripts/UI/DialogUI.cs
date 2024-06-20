using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public static DialogUI instance;
    public TextMeshProUGUI textMesh;
    public TextMeshProUGUI speakerName;
    public RectTransform textRect;
    public RectTransform answersRect;
    public List<AnswerButton> answers = new List<AnswerButton>();
    public GameObject ButtonPref;
    public ScrollRect scrollRect;
    public float aHeight;
    public int maxSize;

    [SerializeField] private ScrollRect _textScroll;
    [SerializeField] private ScrollRect _answerScroll;

    public Canvas _canvas;
    public void Clear() //Очистить диалоговое окно
    {
        textMesh.text = "";
        textMesh.ForceMeshUpdate();
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textMesh.GetRenderedValues()[1]);
        ClearAnswers();
    }
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        instance = this;
    }
    private void Start()
    {
        DialogController.instance.Closed.AddListener(Clear);
    }
    public void Open()
    {
        _canvas.enabled = true;
        _textScroll.enabled = true;
        _answerScroll.enabled = true;
    }
    public void Close()
    {
        _canvas.enabled = false;
        _textScroll.enabled = false;
        _answerScroll.enabled = false;
    }
    public void CreateButton(Answer answer, int ind) //Создание кнопки для ответа
    {
        var bt = Instantiate(ButtonPref, answersRect.transform).GetComponent<AnswerButton>();
        bt.Create(answer);
        bt.transform.localPosition += new Vector3(0, -aHeight);
        bt.ind = ind;
        aHeight += bt.height + 4;
        answersRect.sizeDelta = new Vector2(answersRect.sizeDelta.x, aHeight);
        answers.Add(bt);
    }
    public void ClearAnswers() //Очистить поле ответов
    {
        for (int i = 0; i < answers.Count; i++)
        {
            Destroy(answers[i].gameObject);
        }
        answers.Clear();
        aHeight = 0;
        answersRect.sizeDelta = new Vector2(answersRect.sizeDelta.x, aHeight);
    }
    public void PrintNode(string str, bool isAnswer = false) //Вывести реплику в окно
    {
        //TODO удаление старого текста, чтобы не захламлять память. Учесть атрибут <сolor>
        if (!isAnswer)
        {
            var tmp = DialogController.instance.speaker.MyName + ":\n" + str + " \n";
            //while (textMesh.text.Length + tmp.Length + 2 > maxSize)
            //{
            //    textMesh.text = textMesh.text.Remove(0, str.ToString().IndexOf(" \n") + 2);
            //}
            textMesh.text += tmp;
            HistoryController.instance.Add(DialogController.instance.speaker.MyName + ":\n" + str);
        }
        else
        {
            var tmp = "<color=green>" + WorldController.Instance.playerControl.human.MyName + ":\n" + str + "</color> \n";
            //while (textMesh.text.Length + tmp.Length + 2 > maxSize)
            //{
            //    textMesh.text = textMesh.text.Remove(0, str.ToString().IndexOf(" \n") + 2);
            //}
            textMesh.text += tmp;
            HistoryController.instance.Add(WorldController.Instance.playerControl.human.MyName + ":\n" + str);
        }
        textMesh.ForceMeshUpdate();
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textMesh.GetRenderedValues()[1]);
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 0);
    }
}