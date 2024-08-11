using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class RetailOrdering : MonoBehaviour
{
    public RetailInventory inventory;
    public List<StatusItem> statusItems = new List<StatusItem>();

    public void ReorderItem(RetailItem retailItem, int quantity)
    {
        RetailInventory.InventoryStock findStock = inventory.allAvailableItems.Find(item => item.itemData.itemName == retailItem.itemName);
        MoneyManager.Instance.ChangeMoney(-findStock.cogsPrice);
        StartCoroutine(ReorderRoutine(retailItem, quantity, findStock.cogsPrice));
    }

    IEnumerator ReorderRoutine(RetailItem retailItem, int quantity, float cogsPrice)    // Reorder Coroutine
    {
        StatusItem newOrderItem = new StatusItem(retailItem, quantity);
        newOrderItem.cogsPrice = cogsPrice;
        statusItems.Add(newOrderItem);

        print($"Started ordering x{quantity} {retailItem.itemName}");

        while(newOrderItem.etaTime > 0)
        {
            newOrderItem.etaTime -= Time.deltaTime;
            yield return null;
        }
        
        print($"Order x{quantity} {retailItem.itemName} received!");
        statusItems.Remove(newOrderItem);
        for (int i = 0; i < quantity; i++)
        {
            inventory.AddItemToInventory(retailItem, cogsPrice);
        }
    }

    [Button]
    public void TestReorder()
    {
        RetailItem randomItem = inventory.GetRandomSupplyChain().GetRandomRetailItem();
        ReorderItem(randomItem, 1);
    }

    [System.Serializable]
    public class StatusItem
    {
        public RetailItem item;
        public int quantity;
        public float etaTime;
        [AllowNesting][ReadOnly] public float cogsPrice;

        public StatusItem(RetailItem retailItem, int quantity)
        {
            item = retailItem;
            this.quantity = quantity;

            etaTime = item.orderTimeSeconds;
        }
    }
}
