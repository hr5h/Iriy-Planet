using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class QuestFinal : MonoBehaviour
{
    //Выбранный вариант концовки
    public static Ending ending = null;
    //Все варианты концовок
    public Dictionary<string, Ending> endings = new Dictionary<string, Ending>();

    public static bool final;
    public static QuestFinal instance;
    public static string questName;
    public Human player;
    public Human pilot;
    public GameObject finalWindow;
    public GameObject finalTip;
    public bool pilotInArea;

    public FinalAnswerButton finalAnswer1;
    public FinalAnswerButton finalAnswer2;

    private static List<AsyncOperationHandle<IList<ScriptableObject>>> addressableHandlers = new List<AsyncOperationHandle<IList<ScriptableObject>>>();
    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Корабль отремонтирован, пора решить, что делать дальше");
        gameObject.SetActive(true);
    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
        Destroy(gameObject);
    }
    public void Fail() //Квест провален
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    //TODO перенести логику загрузки ассетов в AddressableLoader
    private void LoadEndings(string label, Dictionary<string, Ending> dict)
    {
        List<ScriptableObject> result = new List<ScriptableObject>();
        addressableHandlers.Add(AddressableTools.AddressableLoader.LoadAsset(label, result));
        foreach (var x in result)
        {
            if (!endings.ContainsKey(x.name.ToLower()))
                endings.Add(x.name.ToLower(), x as Ending);
        }
    }
    private void ReleaseEndings()
    {
        foreach (var handler in addressableHandlers)
        {
            Addressables.Release(handler);
        }
        addressableHandlers.Clear();
    }
    private void ToEnding(string Name)
    {
        Name = Name.ToLower();
        ending = endings[Name];
        //TODO использовать SceneLoader
        SceneManager.LoadScene("_Source/Scenes/EndingScene");

    }
    private void Awake()
    {
        LoadEndings("EndingData", endings);
        final = false;
        instance = this;
        questName = "Финал";
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        ReleaseEndings();
    }
    public void ShowFinalWindow()
    {
        final = true;
        finalTip.SetActive(false);
        Time.timeScale = 0f;
        if (pilot && pilotInArea)
        {
            finalAnswer1.MakeClickable();
            finalAnswer2.MakeClickable();
        }
        else
        {
            finalAnswer1.MakeNoClickable();
            finalAnswer2.MakeNoClickable();
        }
        finalWindow.SetActive(true);
    }
    public void HideFinalWindow()
    {
        final = false;
        finalTip.SetActive(true);
        Time.timeScale = 1f;
        finalWindow.SetActive(false);
    }
    public void MakeAChoice(int option)
    {
        bool playerHasProtector = Command.HasItem(player.inventory, "protector");
        bool pilotHasProtector = pilot != null && Command.HasItem(pilot.inventory, "protector");
        switch (option)
        {
            case 0:
                {
                    if (!playerHasProtector)
                        ToEnding("ending0.0");
                    else
                        ToEnding("ending0.1");
                }
                break;
            case 1:
                {
                    if (!playerHasProtector && !pilotHasProtector)
                        ToEnding("ending1.0");
                    else if (!pilotHasProtector)
                        ToEnding("ending1.1");
                    else
                        ToEnding("ending1.2");
                }
                break;
            case 2:
                {
                    if (!pilotHasProtector)
                        ToEnding("ending2.0");
                    else
                        ToEnding("ending2.1");
                }
                break;
            case 3:
                {
                    ToEnding("ending3.0");
                }
                break;
        }
    }
    //private void Start()
    //{
    //    //Тут подписки на разные события и активация объектов
    //}
}
