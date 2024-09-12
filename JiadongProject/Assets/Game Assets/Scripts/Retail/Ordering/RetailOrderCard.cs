using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailOrderCard : MonoBehaviour
{
    [ReadOnly] public RetailOrdering.StatusItem statusItem;
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text idText;
    public TMP_Text quantityText;
    public TMP_Text etaText;
    public TMP_Text priceText;

    public void Init(RetailOrdering.StatusItem statusItem)
    {
        this.statusItem = statusItem;

        nameText.text = statusItem?.item?.itemName;
        idText.text = "ID: " + statusItem?.id;
        icon.sprite = statusItem?.item?.icon;
        quantityText.text = "Quantity: " + statusItem?.quantity;

        float? totalPrice = (statusItem?.cogsPrice * statusItem?.quantity) + statusItem?.shippingCost;
        priceText.text = "Price: $" + totalPrice?.ToString("F2");
    }

    void Update()
    {
        if(statusItem != null)
            etaText.text = "ETA: " + GetCurrentETAString();
    }

    public string GetCurrentETAString()
    {
        float seconds = statusItem?.etaTime ?? 0;
        int minutes = Mathf.FloorToInt(seconds / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);

        string timeFormatted = $"{minutes} min to {remainingSeconds} sec";
        return timeFormatted;
    }
}
