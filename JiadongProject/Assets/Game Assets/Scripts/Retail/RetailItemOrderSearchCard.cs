using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailItemOrderSearchCard : MonoBehaviour
{
    [ReadOnly] public RetailInventory.InventoryStock stock;
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text cogsPriceText;
    public TMP_Text totalPriceText;
    public Button orderButton;
    public Button subtractButton;
    public Button addButton;
    public TMP_InputField quantityField;

    public void Init(RetailInventory.InventoryStock stock)
    {
        this.stock = stock;

        nameText.text = stock?.itemData?.itemName;
        icon.sprite = stock?.itemData?.icon;
        SetQuantity(stock.reorderAmount);

        cogsPriceText.text = "COGS: $" + stock?.cogsPrice.ToString("F2");
        totalPriceText.text = "Total: $" + (stock?.reorderAmount * stock?.cogsPrice)?.ToString("F2");

        quantityField.onValueChanged.RemoveAllListeners();
        subtractButton.onClick.RemoveAllListeners();
        addButton.onClick.RemoveAllListeners();

        quantityField.onValueChanged.AddListener(OnQuantityChanged);
        subtractButton.onClick.AddListener(() => ChangeQuantity(-1));
        addButton.onClick.AddListener(() => ChangeQuantity(1));
    }

    void OnQuantityChanged(string newValue)
    {
        if (int.TryParse(newValue, out int quantity))
        {
            SetQuantity(quantity);
        }
    }

    void SetQuantity(int newAmount)
    {
        if (newAmount < 0)
        {
            print("You cannot set the quantity lower than 0.");
            return;
        }
        stock.reorderAmount = newAmount;
        quantityField.text = stock?.reorderAmount + "";
    }

    void ChangeQuantity(int amount)
    {
        if (stock.reorderAmount + amount < 0)
        {
            print("You cannot change the quantity lower than 0.");
            return;
        }
        SetQuantity(stock.reorderAmount + amount);
    }
}
