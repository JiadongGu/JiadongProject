using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class RetailOrdering : Singleton<RetailOrdering>
{
    public RetailInventory inventory;
    public List<StatusItem> statusItems = new List<StatusItem>();

    [HorizontalLine]

    public GameObject capacityWarningPanel;
    public ObjectPooler orderCardPool;
    public GameObject noneText;

    public System.Action OnStatusChange;

    void Start()
    {
        OnStatusChange += () =>
        {
            noneText.SetActive(statusItems.Count == 0);
        };
    }

    public void OrderItem(RetailItem retailItem, int quantity)
    {
        if(inventory.capacity + quantity > RetailRenter.Instance.myRetailBuilding.capacity)
        {
            print($"Cannot order {retailItem.itemName} x{quantity} because you're at capacity!");
            capacityWarningPanel.SetActive(true);
            return;
        }

        RetailInventory.InventoryStock findStock = inventory.GetStockByItem(retailItem);
        float shippingCost = findStock.shippingCost;
        float totalCost = (quantity * findStock.cogsPrice) + shippingCost;

        if(MoneyManager.Instance.money < totalCost)
        {
            print($"You do not have enough money to order x{quantity} + {shippingCost.ToString("F2")} shipping cost of {retailItem.itemName}!");
            return;
        }

        MoneyManager.Instance.ChangeMoney(-totalCost);
        RetailBusiness.Instance.AddToMoneySpentOnOrdersToday(totalCost);
        StartCoroutine(ReorderRoutine(retailItem, quantity, findStock.cogsPrice));
    }

    void AddNewOrderCard(StatusItem newItem)
    {
        statusItems.Add(newItem);

        RetailOrderCard orderCard = orderCardPool.GrabFromPool(Vector3.zero, Quaternion.identity).GetComponent<RetailOrderCard>();
        orderCard.Init(newItem);

        OnStatusChange?.Invoke();
    }

    void RemoveOrderCard(string id)
    {
        var findOrder = statusItems.Find(item => item.id == id);

        if (findOrder != null)
        {
            RetailOrderCard cardToRemove = orderCardPool.allObjects
                .Where(card => card.gameObject.activeSelf)
                .Select(card => card.GetComponent<RetailOrderCard>())
                .FirstOrDefault(item => item != null && item.statusItem.id == findOrder.id);
            
            if(cardToRemove != null)
            {
                cardToRemove.statusItem = null;
                orderCardPool.InsertToPool(cardToRemove.gameObject);
            }
            statusItems.Remove(findOrder);

            OnStatusChange?.Invoke();
        }
        else
        {
            print($"Order {id} cannot be found!");
        }
    }

    IEnumerator ReorderRoutine(RetailItem retailItem, int quantity, float cogsPrice)    // Reorder Coroutine
    {
        StatusItem newOrderItem = new StatusItem(retailItem, quantity);
        newOrderItem.cogsPrice = cogsPrice;
        newOrderItem.shippingCost = inventory.GetStockByItem(retailItem).shippingCost;

        AddNewOrderCard(newOrderItem);

        print($"Started ordering x{quantity} {retailItem.itemName}");

        while (newOrderItem.etaTime > 0)
        {
            newOrderItem.etaTime -= Time.deltaTime;
            yield return null;
        }

        print($"Order x{quantity} {retailItem.itemName} received!");

        RemoveOrderCard(newOrderItem.id);

        inventory.AddItemToInventory(retailItem, quantity);
    }

    [Button]
    public void TestReorder()
    {
        RetailItem randomItem = inventory.GetRandomSupplyChain().GetRandomRetailItem();
        OrderItem(randomItem, 1);
    }

    [System.Serializable]
    public class StatusItem
    {
        public RetailItem item;
        public string id;
        public int quantity;
        public float etaTime;
        [AllowNesting][ReadOnly] public float cogsPrice;
        [AllowNesting][ReadOnly] public float shippingCost;

        public StatusItem(RetailItem retailItem, int quantity)
        {
            item = retailItem;
            id = GenerateID();
            this.quantity = quantity;

            etaTime = item.orderTimeSeconds;
        }

        string GenerateID()
        {
            System.Text.StringBuilder newID = new System.Text.StringBuilder();
            System.Random random = new System.Random();

            for (int i = 0; i < 12; i++)
            {
                int digit = random.Next(0, 10);
                newID.Append(digit);
            }

            return newID.ToString();
        }
    }
}
