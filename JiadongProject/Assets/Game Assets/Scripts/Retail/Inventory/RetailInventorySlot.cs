using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailInventorySlot : RetailCatalogElement
{
    public Image icon;
    public TMP_Text countText;
    public Button button;

    public override void Init(RetailInventory.InventoryStock stock)
    {
        base.Init(stock);
        
        button.onClick.RemoveAllListeners();
        icon.sprite = stock?.itemData?.icon;
        countText.text = stock?.quantity + "";

        button.onClick.AddListener(() =>
        {
            RetailInfoPanel.Instance.UpdateInfo(stock);
        });
    }
}
