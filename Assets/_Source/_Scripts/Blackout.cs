using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
using Game.Sounds;

//Скрипт управления затмениями
public class Blackout : MonoBehaviour, IUpdatable
{
    public static Blackout instance;
    public WorldController controller;
    public TimeController timeController;
    public Light2D globalLight;
    public CameraParams cameraParams;

    public float timeToBlackout; //Интервал между затмениями
    public float timeToBlackoutCurrent; //Осталось до следующего затмения

    public float time; //Продолжительность затмения
    public float timeCurrent; //Осталось до конца затмения

    public float coef; //Скорость затемнения
    public float damage; //Урон для людей по окончании затмения

    public float shakePower; //Сила тряски камеры

    public UnityEvent OnBlackoutBegin;
    public UnityEvent OnBlackoutEnd;

    public Human player;
    Bonfire closestBonfire;
    Vector3 transformBonfire;

    private float volumeAudio;
    private float volumeAudio2;

    void BeginBlackout() //Начало затмения
    {
        cameraParams.EnableCameraShake();
        AudioPlayer.Instance.PlayWorldFX(controller.startBlackoutClip);
        //StartCoroutine(StartBlackoutCoroutine());
        controller.blackout = true;
        timeToBlackoutCurrent = timeToBlackout + Random.Range(-timeToBlackout / 10, timeToBlackout / 10);
        timeCurrent = time;
        controller.arrow.SetActive(true);
        //FindBonFire(out closestBonfire);
        OnBlackoutBegin?.Invoke();
    }

    public IEnumerator StartBlackoutCoroutine()
    {
        volumeAudio = controller.audioSource.volume;
        yield return Yielders.Get(5);
        controller.audioSource.volume = 0.8f;
        yield return Yielders.Get(0.5f);
        controller.audioSource.volume = 0.6f;
        yield return Yielders.Get(0.5f);
        controller.audioSource.volume = 0.4f;
        yield return Yielders.Get(0.5f);
        controller.audioSource.volume = 0.2f;
        yield return Yielders.Get(0.5f);
        controller.audioSource.Stop();
        controller.audioSource.volume = volumeAudio;
        //controller.daySoundsSource.PlayOneShot(controller.nightSoundsClip);
    }

    void EndBlackout() //Конец затмения
    {
        cameraParams.DisableCameraShake();
        AudioPlayer.Instance.StopWorldFX(controller.startBlackoutClip);
        AudioPlayer.Instance.PlayWorldFX(controller.thunderClip);
        //StartCoroutine(EndBlackoutCoroutine());
        controller.flashlight.Flash(1f, Color.white); //Создать вспышку на экране
        controller.blackout = false;
        globalLight.intensity = timeController.time;
        var humans = controller.Entities.Humans;
        for (int i = humans.Count - 1; i >= 0; i--)
        {
            if (!humans[i].inCamp)
            {
                humans[i].TakeDamage(damage, null, Damageable.DamageType.Pure);
            }
        }
        var bonfiresOld = controller.Entities.BonfiresOld;
        for (int i = bonfiresOld.Count - 1; i >= 0; i--)
        {
            bonfiresOld[i].Delete();
        }
        var bonfires = controller.Entities.Bonfires;
        for (int i = bonfires.Count - 1; i >= 0; i--)
        {
            bonfires[i].ReduceHealth();
        }
        controller.arrow.SetActive(false);
        OnBlackoutEnd?.Invoke();
        controller.DeleteStorages();
    }

    public IEnumerator EndBlackoutCoroutine()
    {
        volumeAudio2 = controller.thunderSource.volume;
        yield return Yielders.Get(3);
        for (int i = 0; i < 4; i++)
        {
            controller.thunderSource.volume -= 0.2f;
            yield return Yielders.Get(0.5f);
        }
        controller.thunderSource.Stop();
        controller.thunderSource.volume = volumeAudio2;
        //controller.audioSource.PlayOneShot(controller.finishBlackoutClip);
        volumeAudio = controller.audioSource.volume;
        yield return Yielders.Get(5);
        for (int i = 0; i < 4; i++)
        {
            controller.audioSource.volume -= 0.2f;
            yield return Yielders.Get(0.5f);
        }
        controller.audioSource.Stop();
        controller.audioSource.volume = volumeAudio;
        //controller.daySoundsSource.PlayOneShot(controller.daySoundsClip);
    }

    //private void FindBonFire(out Bonfire bonfire) // определение ближайшего костра
    //{
    //    bonfire = null;
    //    float distance = Mathf.Infinity;
    //    Vector3 position = transform.position;
    //    for (int i = 0; i < controller.bonfires.Count; i++)
    //    {
    //        Vector3 diff = controller.bonfires[i].transform.position - position;
    //        float curDistance = diff.sqrMagnitude;
    //        if (curDistance < distance)
    //        {
    //            bonfire = controller.bonfires[i];
    //            distance = curDistance;
    //        }
    //    }
    //    transformBonfire = new Vector3(closestBonfire.transform.position.x, closestBonfire.transform.position.y).normalized;
    //}

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine()
    {
        yield return null;
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
    }
    public void ManualUpdate()
    {
        if (controller.blackout)
        {
            //controller.arrow.transform.rotation = Quaternion.FromToRotation(Vector3.right, closestBonfire.transform.position - controller.arrow.transform.position); // поворот стрелки
            //controller.arrow.transform.position = new Vector3(player.transform.position.x + transformBonfire.x*700, player.transform.position.y + transformBonfire.y*700); // положение стрелки
            //if (!player.IsDead)
            //{
            //controller.arrow.transform.SetPositionAndRotation(new Vector3(player.transform.position.x + 15, player.transform.position.y + 15),
            //    Quaternion.FromToRotation(Vector3.right, closestBonfire.transform.position - controller.arrow.transform.position));
            //if (Mathf.Abs(player.transform.position.x - closestBonfire.transform.position.x) < 900 && Mathf.Abs(player.transform.position.y - closestBonfire.transform.position.y) < 900)
            //{
            //    controller.arrow.SetActive(false);
            //}
            //}
            //Нанесение урона людям
            var humans = controller.Entities.Humans;
            for (int i = humans.Count - 1; i >= 0; --i)
            {
                if (!humans[i].inCamp)
                    humans[i].TakeDamage(3 * (1 - globalLight.intensity) * Time.deltaTime, null, Damageable.DamageType.Pure);
            }
            //Уменьшение света
            if (globalLight.intensity > 0)
            {
                globalLight.intensity = Mathf.Max(0, globalLight.intensity + Random.Range(-coef, coef * 0.9f) * Time.deltaTime);
            }
            else
            {
                if (timeCurrent == 0)
                {
                    EndBlackout();
                }
                else
                {
                    timeCurrent = Mathf.Max(0, timeCurrent - Time.deltaTime);
                }
            }
        }
        else
        {
            if (timeToBlackoutCurrent == 0)
            {
                BeginBlackout();
            }
            else
            {
                timeToBlackoutCurrent = Mathf.Max(0, timeToBlackoutCurrent - Time.deltaTime);
            }
        }
    }
}
