using NaughtyAttributes;
using UnityEngine;

public class RetailBusiness : MonoBehaviour
{
    public RetailInventory inventory;

    [Button]
    public void TestSellRandomItem()
    {
        if(inventory.allStockItems.Count == 0) return;
        
        RetailInventory.InventoryStock stockToSell = inventory.allStockItems[Random.Range(0, inventory.allStockItems.Count )];
        SellItem(stockToSell, 2);
    }

    public void SellItem(RetailInventory.InventoryStock stockItem, int amount)
    {
        if(inventory.RemoveItemFromInventory(stockItem.itemData, amount))   // "Hey customer, we have enough of this item to sell to you :)"
        {
            float totalPrice = -stockItem.sellPrice * amount;
            MoneyManager.Instance.ChangeMoney(totalPrice);
            
            print($"Sold x{amount} of {stockItem.itemData.itemName} for ${totalPrice*-1}.");
        }
        else
        {
            print($"Failed to sell x{amount} of {stockItem.itemData.itemName} because you have {stockItem.quantity} in stock but you want to buy {amount}.");
        }
    }
}
