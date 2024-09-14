using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailInventory : RetailCatalogBase
{
    [Expandable] public List<SupplyChain> allSupplyChains = new List<SupplyChain>();
    [ReadOnly] public List<InventoryStock> allAvailableItems = new List<InventoryStock>();
    [ReadOnly] public int capacity;
    public List<InventoryStock> myStockItems = new List<InventoryStock>();

    [HorizontalLine]

    public TMP_Text capacityText;
    public Transform slotsParent;
    public RetailCatalogElement invSlotPrefab;

    public System.Action<InventoryStock> OnStockAdded;
    public System.Action<InventoryStock> OnStockUpdated;
    public System.Action<InventoryStock> OnStockRemoved;
    public System.Action OnAvailableItemsRefreshed;

    RetailInfoPanel infoPanel;
    RetailRenter renter;

    public override void Init()
    {
        inventory.OnAvailableItemsRefreshed += UpdateResult;
        inventory.OnAvailableItemsRefreshed += PopulateDropdownCompanies;

        base.Init();
    }

    void Start()
    {
        PopulateAvailableItems();
        renter = RetailRenter.Instance;
        infoPanel = RetailInfoPanel.Instance;

        OnStockAdded += (stock) =>
        {
            RetailCatalogElement newElement = Instantiate(invSlotPrefab, slotsParent);
            newElement.Init(stock);
            spawnedCatalogElements.Add(newElement);

            UpdateNoneText();
        };
        OnStockUpdated += (stock) =>
        {
            RetailInventorySlot invSlot = (RetailInventorySlot)spawnedCatalogElements.Find(x => x.stock.itemData == stock.itemData);
            invSlot.Init(stock);
        };
        OnStockRemoved += (stock) =>
        {
            RetailInventorySlot invSlot = (RetailInventorySlot)spawnedCatalogElements.Find(x => x.stock.itemData == stock.itemData);
            int i = spawnedCatalogElements.FindIndex(x => x.stock.itemData == stock.itemData);
            Destroy(spawnedCatalogElements[i].gameObject);
            spawnedCatalogElements.RemoveAt(i);


            if (infoPanel.inventoryStock?.itemData == stock.itemData)
            {
                infoPanel.UpdateInfo(null);
            }

            UpdateNoneText();
        };

        UpdateNoneText();
    }

    void Update()
    {
        UpdateCapacityText();
    }

    void PopulateAvailableItems()
    {
        foreach (var chain in allSupplyChains)
        {
            foreach (var company in chain.companies)
            {
                foreach (var item in company.retailItems)
                {
                    float randCOGS = (int)Random.Range(item.cogsRange.x, item.cogsRange.y);
                    InventoryStock newStockitem = new InventoryStock
                    {
                        itemData = item,
                        cogsPrice = randCOGS,
                        shippingCost = company.shippingCost,
                        sellPrice = randCOGS
                    };

                    allAvailableItems.Add(newStockitem);
                }
            }
        }

        OnAvailableItemsRefreshed?.Invoke();
    }

    public override void UpdateNoneText()
    {
        noneText.SetActive(spawnedCatalogElements.Count == 0);
    }

    void UpdateCapacityText()
    {
        if (renter.IOwnRetailBuilding())
            capacityText.text = $"Items: {capacity}/{renter.myRetailBuilding.capacity}";
    }

    [Button]
    public void AddRandomItemToStock()
    {
        if (allSupplyChains.Count == 0) return;

        SupplyChain randomSupplyChain = allSupplyChains[Random.Range(0, allSupplyChains.Count)];
        RetailItem randomRetailItem = randomSupplyChain.GetRandomRetailItem();

        AddItemToInventory(randomRetailItem);
    }

    public bool AddItemToInventory(RetailItem retailItem, int amount)
    {
        if (capacity + amount >= renter.myRetailBuilding.capacity)
        {
            print("Cannot add more items, you are at capacity!");
            return false;
        }
        for (int i = 0; i < amount; i++)
        {
            AddItemToInventory(retailItem);
        }
        return true;
    }

    public bool AddItemToInventory(RetailItem retailItem)
    {
        if (renter.IOwnRetailBuilding())
        {
            if (capacity >= renter.myRetailBuilding.capacity)
            {
                print("Cannot add more items, you are at capacity!");
                return false;
            }
        }
        foreach (var item in myStockItems) // Look through current inventory, and find the existing item, if found, quantity++
        {
            if (item.itemData.name == retailItem.name)
            {
                item.quantity++;
                capacity++;
                RetailBusiness.Instance.AddToTodayStockHistory(item, sold: 0);
                OnStockUpdated?.Invoke(item);
                return true;
            }
        }

        // If not found new retail item, then add a new inventory stock
        InventoryStock newStock = allAvailableItems.FirstOrDefault(item => item.itemData == retailItem);
        newStock.quantity = 1;
        capacity++;
        myStockItems.Add(newStock);

        RetailBusiness.Instance.AddToStockHistory(newStock);
        RetailBusiness.Instance.AddToTodayStockHistory(newStock, sold: 0);

        OnStockAdded?.Invoke(newStock);
        return true;
    }

    public bool RemoveItemFromInventory(RetailItem retailItem, int amount)
    {
        foreach (var item in myStockItems) // Look through current inventory, and find the existing item
        {
            if (item.itemData.name == retailItem.name)
            {
                if (QuantityAvailable(retailItem, amount))   // Do we have enough items in our stock to sell this
                {
                    item.quantity -= amount;

                    if (item.quantity == 0)
                    {
                        OnStockRemoved?.Invoke(item);
                        myStockItems.Remove(item);
                        return true;    // We successfully removed the item
                    }
                    OnStockUpdated?.Invoke(item);
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
        InventoryStock stockItem = myStockItems.Find(x => x.itemData.itemName == item.itemName);
        return stockItem != null && stockItem.quantity >= quantity;
    }

    public SupplyChain GetRandomSupplyChain()
    {
        return allSupplyChains[Random.Range(0, allSupplyChains.Count)];
    }

    public InventoryStock GetStockByItem(RetailItem item)
    {
        return allAvailableItems.Find(x => x.itemData.itemName == item.itemName);
    }

    [System.Serializable]
    public class InventoryStock
    {
        public RetailItem itemData;
        public float cogsPrice;
        public float shippingCost;
        public float sellPrice;
        public int quantity;
        public int sold;
        public int reorderLevel;
        public int reorderAmount;

        public void SetProperties(InventoryStock stock, bool excludeSold = true)
        {
            itemData = stock.itemData;
            cogsPrice = stock.cogsPrice;
            shippingCost = stock.shippingCost;
            sellPrice = stock.sellPrice;
            quantity = stock.quantity;
            if (excludeSold == false) sold = stock.sold;
            reorderLevel = stock.reorderLevel;
            reorderAmount = stock.reorderAmount;
        }
    }
}


