using UnityEngine;

public class TradeButton : MonoBehaviour
{
    public void Click()
    {
        InventoryController.Instance.TradingOpen((DialogController.instance.speaker as Human).inventory);
        DialogController.instance.Hide();
    }
}
