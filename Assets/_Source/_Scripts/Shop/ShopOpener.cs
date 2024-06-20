using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopOpener : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnCanInterract;
    [HideInInspector] public UnityEvent OnDontCanInterract;

    public GameObject shopButton;
    private bool button = false;

    private List<Character> characters = new List<Character>();
    private List<DropItems> dropItems = new List<DropItems>();
    public Character character;
    public DropItems drop;
    public int countStash;

    public Human player;
    public PlayerControl playerController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && button)
            Open();
        Follow();
    }

    public void ActionButton()
    {
        Open();
    }

    public void Open()
    {
        if (character)
        {
            if (!DialogController.instance.dialog)
            {
                character.dialog.StartDialog();
                playerController.showDialog = true;
            }
            else
            {
                if (InventoryController.Instance.trading)
                {
                    InventoryController.Instance.TradingClose();
                    DialogController.instance.Show();
                    playerController.showDialog = true;
                }
                else
                {
                    DialogController.instance.Close();
                    playerController.showDialog = false;
                }
            }
            WorldController.Instance.playerControl.OnWindowChanged?.Invoke();
            return;
        }
        if (drop)
        {
            if (!InventoryController.Instance.exchange)
            {
                InventoryController.Instance.ExchangeOpen(drop.inventory);
                if (drop.firstTime)
                {
                    drop.firstTime = false;
                    Command.GiveMoney(player, drop.money);
                    Command.ShowMessage("Получено денег:", drop.money.ToString(), Color.white, 1, false);
                }
                if (drop.isStash)
                {
                    drop.isStash = false;
                    countStash--;
                    Command.ShowMessage("Вы нашли:", (3 - countStash).ToString() + " из 3 тайников", Color.white, 2, true);
                }
                if (countStash == 0)
                {
                    Command.ShowMessage("Поздравляем,", "Вы нашли все тайники!", Color.white, 2, true);
                }
            }
            else
            {
                InventoryController.Instance.ExchangeClose();
            }
            WorldController.Instance.playerControl.OnWindowChanged?.Invoke();
        }
        //if (!InventoryController.instance.trading)
        //{
        //    InventoryController.instance.TradingOpen(inventory);
        //    shopButton.SetActive(false);
        //}
        //else
        //{
        //    InventoryController.instance.TradingClose();
        //    shopButton.SetActive(true);
        //}
    }

    private void FindTarget(bool isDrop = false)
    {
        if (!isDrop)
        {
            if (characters.Count > 0)
            {
                character = characters[characters.Count - 1];
                drop = null;
                button = true;
                shopButton.SetActive(true);
                return;
            }
            if (dropItems.Count > 0)
            {
                drop = dropItems[dropItems.Count - 1];
                character = null;
                button = true;
                shopButton.SetActive(true);
                return;
            }
        }
        else
        {
            if (dropItems.Count > 0)
            {
                drop = dropItems[dropItems.Count - 1];
                character = null;
                button = true;
                shopButton.SetActive(true);
                return;
            }
            if (characters.Count > 0)
            {
                character = characters[characters.Count - 1];
                drop = null;
                button = true;
                shopButton.SetActive(true);
                return;
            }
        }

        shopButton.SetActive(false);
        character = null;
        drop = null;
        button = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Character obj))
        {
            if (!obj.canSpeak)
                return;
            characters.Add(obj);
            OnCanInterract?.Invoke();
            FindTarget();
        }
        else
        {
            if (collision.gameObject.TryGetComponent(out DropItems obj2))
            {
                dropItems.Add(obj2);
                OnCanInterract?.Invoke();
                FindTarget(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Character obj))
        {
            characters.Remove(obj);
            if (characters.Count == 0 && dropItems.Count == 0)
            {
                OnDontCanInterract?.Invoke();
            }
            if (obj == character)
            {
                if (DialogController.instance.speaker == obj)
                {
                    DialogController.instance.Close();
                    if (InventoryController.Instance.inventory)
                    {
                        InventoryController.Instance.TradingClose();
                    }
                }
                WorldController.Instance.playerControl.OnWindowChanged?.Invoke();
                FindTarget();
            }
        }
        else
        {
            if (collision.gameObject.TryGetComponent(out DropItems obj2))
            {
                dropItems.Remove(obj2);
                if (obj2 == drop)
                {
                    if (InventoryController.Instance.exchange)
                    {
                        InventoryController.Instance.ExchangeClose();
                    }
                    WorldController.Instance.playerControl.OnWindowChanged?.Invoke();
                    FindTarget(true);
                }
            }
        }
        //if (collision.tag == "Human")
        //{
        //    shopButton.SetActive(false);
        //    button = false;
        //    isHuman = false;
        //    if (InventoryController.instance.trading)
        //    {
        //        InventoryController.instance.TradingClose();
        //    }
        //}
    }

    private void Follow()
    {
        if (character)
        {
            if (!character.canSpeak)
            {
                shopButton.SetActive(false);
                character = null;
                drop = null;
                button = false;
                return;
            }
            shopButton.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + 30, character.transform.position.z);
        }
        else
        if (drop)
            shopButton.transform.position = new Vector3(drop.transform.position.x, drop.transform.position.y + 30, drop.transform.position.z);
    }

}
