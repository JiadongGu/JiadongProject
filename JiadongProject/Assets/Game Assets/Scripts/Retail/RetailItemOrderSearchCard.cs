using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailItemOrderSearchCard : MonoBehaviour
{
    [ReadOnly] public RetailInventory.InventoryStock stock;
    [ReadOnly] public float totalPrice;

    [HorizontalLine]

    public Image icon;
    public TMP_Text nameText;
    public TMP_Text cogsPriceText;
    public TMP_Text totalPriceText;
    public Button orderButton;
    public Button subtractButton;
    public Button addButton;
    public TMP_InputField quantityField;
    

    Action OnQuantityChange;

    public void Init(RetailInventory.InventoryStock stock)
    {
        this.stock = stock;

        nameText.text = stock?.itemData?.itemName;
        icon.sprite = stock?.itemData?.icon;
        SetQuantity(stock.reorderAmount);

        cogsPriceText.text = "COGS: $" + stock?.cogsPrice.ToString("F2");
        UpdateTotalPrice();

        quantityField.onValueChanged.RemoveAllListeners();
        subtractButton.onClick.RemoveAllListeners();
        addButton.onClick.RemoveAllListeners();

        quantityField.onValueChanged.AddListener(OnQuantityFieldChangedCallback);
        subtractButton.onClick.AddListener(() => ChangeQuantity(-1));
        addButton.onClick.AddListener(() => ChangeQuantity(1));

        OnQuantityChange += UpdateTotalPrice;
        orderButton.onClick.AddListener(Order);
    }

    void OnQuantityFieldChangedCallback(string newValue)
    {
        if (int.TryParse(newValue, out int quantity))
        {
            SetQuantity(quantity);
        }
    }

    void Order()
    {
        if(GetOrderAmount() <= 0)
        {
            print($"You have to order at least 1 of {stock.itemData.itemName} to start the order!");
            return;
        }
            
        if(MoneyManager.Instance.money >= totalPrice)
            RetailOrdering.Instance.ReorderItem(stock.itemData, GetOrderAmount());
        else
            print($"You do not have enough money to order {GetOrderAmount()}x of {stock.itemData.itemName}!");
    }

    void UpdateTotalPrice()
    {
        totalPrice = stock.reorderAmount * stock.cogsPrice;
        totalPriceText.text = "Total: $" + totalPrice.ToString("F2");
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

        OnQuantityChange?.Invoke();
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

    int GetOrderAmount()
    {
        return stock.reorderAmount;
    }
}
