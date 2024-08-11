using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class RetailInventory : MonoBehaviour
{
    [Expandable] public List<SupplyChain> allSupplyChains = new List<SupplyChain>();
    [ReadOnly] public List<InventoryStock> allAvailableItems = new List<InventoryStock>();
    public List<InventoryStock> allStockItems = new List<InventoryStock>();

    void Start()
    {
        PopulateAvailableItems();
    }

    void PopulateAvailableItems()
    {
        foreach (var chain in allSupplyChains)
        {
            foreach (var company in chain.companies)
            {
                foreach (var item in company.retailItems)
                {
                    InventoryStock newStockitem = new InventoryStock();
                    newStockitem.itemData = item;
                    newStockitem.cogsPrice = (int)Random.Range(item.cogsRange.x, item.cogsRange.y);

                    allAvailableItems.Add(newStockitem);
                }
            }
        }
    }


    [Button]
    public void AddRandomItemToStock()
    {
        if (allSupplyChains.Count == 0) return;

        SupplyChain randomSupplyChain = allSupplyChains[Random.Range(0, allSupplyChains.Count)];
        RetailItem randomRetailItem = randomSupplyChain.GetRandomRetailItem();

        AddItemToInventory(randomRetailItem);
    }

    public void AddItemToInventory(RetailItem retailItem, float cogsPrice = 0)
    {
        foreach (var item in allStockItems) // Look through current inventory, and find the existing item, if found, quantity++
        {
            if (item.itemData.name == retailItem.name)
            {
                item.quantity++;
                return;
            }
        }

        // If not found new retail item, then add a new inventory stock
        InventoryStock newStock = new InventoryStock();
        newStock.itemData = retailItem;
        newStock.quantity = 1;
        newStock.cogsPrice = cogsPrice;
        allStockItems.Add(newStock);
    }

    public bool RemoveItemFromInventory(RetailItem retailItem, int amount)
    {
        foreach (var item in allStockItems) // Look through current inventory, and find the existing item
        {
            if (item.itemData.name == retailItem.name)
            {
                if(QuantityAvailable(retailItem, amount))   // Do we have enough items in our stock to sell this
                {
                    item.quantity -= amount;
                    if(item.quantity == 0) allStockItems.Remove(item);

                    return true;    // We successfully removed the item
                }
                else
                {
                    return false; // We don't have enough items in stock to sell that amount
                }
            }
        }

        return false;   // We couldn't find the item
    }

    public bool QuantityAvailable(RetailItem item, int quantity)
    {
        InventoryStock stockItem = allStockItems.Find(x => x.itemData.itemName == item.itemName);
        return stockItem != null && stockItem.quantity >= quantity;
    }

    public SupplyChain GetRandomSupplyChain()
    {
        return allSupplyChains[Random.Range(0, allSupplyChains.Count)];
    }

    [System.Serializable]
    public class InventoryStock
    {
        public RetailItem itemData;
        public float cogsPrice;
        public float sellPrice;
        public int quantity;
        public int reorderLevel;
        public int reorderAmount;
    }
}


