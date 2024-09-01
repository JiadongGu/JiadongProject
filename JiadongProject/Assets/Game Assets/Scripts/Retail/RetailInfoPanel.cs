using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailInfoPanel : Singleton<RetailInfoPanel>
{
    [ReadOnly] public RetailInventory.InventoryStock inventoryStock;
    public TMP_Text itemNameText;
    public TMP_Text countText;
    public Image iconImage;

    void Start()
    {
        UpdateInfo(null);
    }

    public void UpdateInfo(RetailInventory.InventoryStock inventoryStock)
    {
        itemNameText.text = inventoryStock == null? "Select An Item To View Info" : inventoryStock.itemData?.itemName;
        countText.text = $"Quantity: " + (inventoryStock == null? 0 : inventoryStock.quantity);
        iconImage.sprite = inventoryStock?.itemData?.icon;
    }
}
