using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class RetailInventory : MonoBehaviour
{
    public List<SupplyChain> allSupplyChains = new List<SupplyChain>();
    public List<InventoryStock> allStockItems = new List<InventoryStock>();


    [Button]
    public void AddRandomItemToStock()
    {
        if(allSupplyChains.Count == 0) return;

        SupplyChain randomSupplyChain = allSupplyChains[Random.Range(0, allSupplyChains.Count)];
        RetailItem randomRetailItem = randomSupplyChain.GetRandomRetailItem();

        AddItemToInventory(randomRetailItem);
    }

    public bool AddItemToInventory(RetailItem retailItem)
    {
        foreach (var item in allStockItems) // Look through current inventory, and find the existin stock, if found, quantity++
        {
            if(item.itemData.name == retailItem.name)
            {
                item.quantity++;
                return false;    // Returning false means we added to an existing inventory stock
            }
        }

        // If not found new retail item, then add a new inventory stock
        InventoryStock newStock = new InventoryStock();
        newStock.itemData = retailItem;

        newStock.quantity = 1;
        allStockItems.Add(newStock);
        
        return true;
    }

    public void ReorderItem(RetailItem retailItem, int quantity)
    {
        StartCoroutine(ReorderRoutine(retailItem, quantity));
    }

    IEnumerator ReorderRoutine(RetailItem retailItem, int quantity)    // Reorder Coroutine
    {
        print($"Started ordering x{quantity} {retailItem.itemName}");
        yield return new WaitForSeconds(retailItem.orderTimeSeconds);
        print("DONE");
        for (int i = 0; i < quantity; i++)
        {
            AddItemToInventory(retailItem);
        }
    }

    // [Button]
    // public void TestReorder()
    // {
    //     RetailItem randomItem = GetRandomRetailItem();
    //     ReorderItem(randomItem, 4);
    // }

    [System.Serializable]
    public class InventoryStock
    {
        public RetailItem itemData;
        public float sellPrice;
        public int quantity;
        public int reorderLevel;
    }
}


