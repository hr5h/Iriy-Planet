using TMPro;
using UnityEngine;

public class TradingUI : MonoBehaviour
{
    public Human player;
    public Human other;

    public TextMeshProUGUI playerMoney;
    public TextMeshProUGUI otherMoney;
    public TextMeshProUGUI salePrice;
    public TextMeshProUGUI purchasePrice;

    private void UpdatePlayerMoney()
    {
        playerMoney.text = "Деньги: " + player.money;
    }
    private void UpdateOtherMoney()
    {
        otherMoney.text = other.MyName + ": " + other.money;
    }
    private void UpdatePurchasePrice()
    {
        purchasePrice.text = "Покупка: " + InventoryController.Instance.purchasePrice;
    }
    private void UpdateSalePrice()
    {
        salePrice.text = "Продажа: " + InventoryController.Instance.salePrice;
    }
    public void RefreshText()
    {
        UpdatePlayerMoney();
        UpdateOtherMoney();
        UpdatePurchasePrice();
        UpdateSalePrice();
    }
    public void Awake()
    {
        InventoryController.Instance.OnChangedSalePrice.AddListener(UpdateSalePrice);
        InventoryController.Instance.OnChangedPurchasePrice.AddListener(UpdatePurchasePrice);
        player.OnChangedMoney.AddListener(UpdatePlayerMoney);
    }
    public void ChangeOther(Human human)
    {
        if (other != null)
        {
            other.OnChangedMoney.RemoveListener(UpdateOtherMoney);
        }

        other = human;

        if (other != null)
        {
            other.OnChangedMoney.AddListener(UpdateOtherMoney);
        }
    }
}
