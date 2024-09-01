using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailInventorySlot : MonoBehaviour
{
    [ReadOnly] public RetailInventory.InventoryStock stockItem;
    public Image icon;
    public TMP_Text countText;
    public Button button;

    public void Init(RetailInventory.InventoryStock stockItem)
    {
        button.onClick.RemoveAllListeners();

        this.stockItem = stockItem;
        icon.sprite = stockItem?.itemData?.icon;
        countText.text = stockItem?.quantity + "";

        button.onClick.AddListener(() =>
        {
            RetailInfoPanel.Instance.UpdateInfo(stockItem);
        });
    }
}
