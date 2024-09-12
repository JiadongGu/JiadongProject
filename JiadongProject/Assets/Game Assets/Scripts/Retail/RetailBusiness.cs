using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class RetailBusiness : Singleton<RetailBusiness>
{
    public RetailInventory inventory;
    [ReadOnly] public List<RetailInventory.InventoryStock> stockHistory = new List<RetailInventory.InventoryStock>();

    [Button]
    public void TestSellRandomItem()
    {
        if(inventory.myStockItems.Count == 0) return;
        
        RetailInventory.InventoryStock stockToSell = inventory.myStockItems[Random.Range(0, inventory.myStockItems.Count )];
        SellItem(stockToSell, 1);
    }

    public void SellItem(RetailInventory.InventoryStock stockItem, int amount)
    {
        if(inventory.RemoveItemFromInventory(stockItem.itemData, amount))   // "Hey customer, we have enough of this item to sell to you :)"
        {
            float totalPrice = stockItem.sellPrice * amount;
            stockHistory.Find(x=>x.itemData == stockItem.itemData).sold += amount;
            MoneyManager.Instance.ChangeMoney(totalPrice);
            
            print($"Sold x{amount} of {stockItem.itemData.itemName} for ${totalPrice}.");
        }
        else
        {
            print($"Failed to sell x{amount} of {stockItem.itemData.itemName} because you have {stockItem.quantity} in stock but you want to buy {amount}.");
        }
    }

    public void AddToStockHistory(RetailInventory.InventoryStock stock)
    {
        foreach (var s in stockHistory) // Look through current history, and find the existing stock, if found, exit
        {
            if (s.itemData == stock.itemData)
            {
                s.SetProperties(stock);
                return;
            }
        }

        // If not found stock, then add reference to history
        stockHistory.Add(stock);
    }
}
