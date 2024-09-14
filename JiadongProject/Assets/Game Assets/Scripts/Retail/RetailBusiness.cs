using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class RetailBusiness : Singleton<RetailBusiness>
{
    public RetailInventory inventory;
    public RetailHistoryBar historyBarPrefab;
    public Transform historyBarParent;
    [ReadOnly] public List<RetailInventory.InventoryStock> stockHistories = new List<RetailInventory.InventoryStock>();
    [ReadOnly] public List<RetailInventory.InventoryStock> stockHistoriesToday = new List<RetailInventory.InventoryStock>();
    [ReadOnly] public List<RetailHistory> retailHistories = new List<RetailHistory>();

    [HorizontalLine]

    [ReadOnly] public float startMoneyToday;
    [ReadOnly] public float moneySpentTodayOnOrders;
    [ReadOnly] public int day;
    [ReadOnly] public int month;
    public float currentTime;
    [Min(0)] public float endDayTime;
    [Min(1)] public float daysPerMonth;
    [ReadOnly] public DateTime currentDateTime;

    [HorizontalLine]

    public TMP_Text dayText;
    public TMP_Text monthText;
    public TMP_Text timeText;
    public TMP_Text salesTodayText;
    public TMP_Text grossTodayText;
    public TMP_Text netTodayText;

    public TMP_Text ordersCostTodayText;
    public TMP_Text itemsSoldTodayText;

    RetailRenter renter;

    Action OnStockHistoryTodayUpdated;
    Action OnNewMonth;

    void Start()
    {
        day = 1;
        month = 1;
        renter = RetailRenter.Instance;
        startMoneyToday = MoneyManager.Instance.money;

        OnStockHistoryTodayUpdated += UpdateTodayInfoTexts;
        OnNewMonth += BillRentUtilities;
    }

    void Update()
    {
        UpdateTime();
    }

    void BillRentUtilities()
    {
        if (renter.IOwnRetailBuilding() == false)
        {
            print("Cannot be billed on an unowned building!");
            return;
        }

        float rent = renter.myRetailBuilding.totalRent;
        print($"Billed ${rent}");
        MoneyManager.Instance.ChangeMoney(-rent);
    }

    [Button]
    public void TestSellRandomItem()
    {
        if (inventory.myStockItems.Count == 0) return;

        RetailInventory.InventoryStock stockToSell = inventory.myStockItems[UnityEngine.Random.Range(0, inventory.myStockItems.Count)];
        SellItem(stockToSell, 1);
    }

    public void SellItem(RetailInventory.InventoryStock stockItem, int amount)
    {
        if (inventory.RemoveItemFromInventory(stockItem.itemData, amount))   // "Hey customer, we have enough of this item to sell to you :)"
        {
            float totalPrice = stockItem.sellPrice * amount;
            stockHistories.Find(x => x.itemData == stockItem.itemData).sold += amount;
            MoneyManager.Instance.ChangeMoney(totalPrice);

            AddToTodayStockHistory(stockItem, sold: amount);

            print($"Sold x{amount} of {stockItem.itemData.itemName} for ${totalPrice}.");
        }
        else
        {
            print($"Failed to sell x{amount} of {stockItem.itemData.itemName} because you have {stockItem.quantity} in stock but you want to buy {amount}.");
        }
    }

    public void AddToStockHistory(RetailInventory.InventoryStock stock)
    {
        foreach (var s in stockHistories) // Look through current history, and find the existing stock, if found, exit
        {
            if (s.itemData == stock.itemData)
            {
                s.SetProperties(stock);
                return;
            }
        }

        // If not found stock, then add reference to history
        stockHistories.Add(stock);
    }

    public void AddToTodayStockHistory(RetailInventory.InventoryStock stock, int sold)
    {
        foreach (var s in stockHistoriesToday) // Look through current history, and find the existing stock, if found, exit
        {
            if (s.itemData == stock.itemData)
            {
                s.sold += sold;
                s.sellPrice = stock.sellPrice;
                s.quantity = stock.quantity;
                OnStockHistoryTodayUpdated?.Invoke();
                return;
            }
        }

        RetailInventory.InventoryStock newStock = new RetailInventory.InventoryStock();
        newStock.SetProperties(stock, excludeSold: true);
        newStock.sold = sold;

        // If not found stock, then add reference to history
        stockHistoriesToday.Add(newStock);
        OnStockHistoryTodayUpdated?.Invoke();
    }

    public void AddToMoneySpentOnOrdersToday(float amount)
    {
        moneySpentTodayOnOrders += amount;
    }

    void AddCurrentDayHistory()
    {
        RetailHistory newHistory = new RetailHistory
        {
            day = this.day,
            month = this.month,
            startMoney = startMoneyToday,
            endMoney = MoneyManager.Instance.money,

            stockHistories = this.stockHistoriesToday
        };

        newHistory.salesRevenue = newHistory.GetCalculatedSalesRevenue();
        newHistory.grossProfit = newHistory.GetCalculatedGrossProfit(moneySpentTodayOnOrders);
        newHistory.netProfit = newHistory.GetCalculatedNetProfit(renter.myRetailBuilding.totalRent);

        retailHistories.Insert(0, newHistory);
        RetailHistoryBar bar = Instantiate(historyBarPrefab, historyBarParent);
        bar.transform.SetAsFirstSibling();
        bar.Init(newHistory);
    }

    void UpdateTime()
    {
        if (renter.IOwnRetailBuilding() == false) return;

        currentTime += Time.deltaTime;
        if (currentTime >= endDayTime)
        {
            AddCurrentDayHistory();
            stockHistoriesToday.Clear();
            moneySpentTodayOnOrders = 0;
            OnStockHistoryTodayUpdated?.Invoke();

            currentTime = 0;
            day++;

            if (day > daysPerMonth)
            {
                day = 1;
                month++;
                OnNewMonth?.Invoke();
            }
            startMoneyToday = MoneyManager.Instance.money;
        }

        timeText.text = SecondsToTimeString(currentTime, endDayTime);
        dayText.text = $"Day {day}";
        monthText.text = $"Month {month}";
    }

    string SecondsToTimeString(float seconds, float totalSeconds)
    {
        TimeSpan startTime = new TimeSpan(7, 0, 0); // Start time at 7 AM
        double totalGameHours = 15; // Total hours from 7 AM to 10 PM
        TimeSpan timeSpan = TimeSpan.FromHours((seconds / totalSeconds) * totalGameHours);
        currentDateTime = DateTime.Today.Add(startTime).Add(timeSpan);
        return currentDateTime.ToString("hh:mm tt");
    }

    public void UpdateTodayInfoTexts()
    {
        float salesToday = stockHistoriesToday.Sum(x => x.sellPrice * x.sold);
        float grossToday = salesToday - moneySpentTodayOnOrders;

        salesTodayText.text = $"${salesToday:F2}";
        grossTodayText.text = $"${grossToday:F2}";
        if (renter.IOwnRetailBuilding())
        {
            if (retailHistories.Count > 0)
            {
                List<RetailHistory> historyThisMonth = retailHistories.Where(x => x.month == month).ToList();

                float grossThisMonth = historyThisMonth.Sum(x => x.grossProfit);
                float netThisMonth = grossThisMonth - renter.myRetailBuilding.totalRent;
                netTodayText.text = $"${netThisMonth:F2}";
            }
            else
            {
                float netToday = grossToday - renter.myRetailBuilding.totalRent;
                netTodayText.text = $"${netToday:F2}";
            }
        }

        ordersCostTodayText.text = $"${moneySpentTodayOnOrders:F2}";
        itemsSoldTodayText.text = stockHistoriesToday.Sum(x => x.sold).ToString();
    }

    [System.Serializable]
    public class RetailHistory
    {
        public int day;
        public int month;
        public float startMoney;
        public float endMoney;
        public float salesRevenue;
        public float grossProfit;
        public float netProfit;

        public List<RetailInventory.InventoryStock> stockHistories;

        public float GetCalculatedSalesRevenue()
        {
            float totalRevenue = 0;
            foreach (var stock in stockHistories)
            {
                totalRevenue += stock.sellPrice * stock.sold;
            }
            return totalRevenue;
        }

        public float GetCalculatedGrossProfit(float ordersCostSpent)
        {
            return salesRevenue - ordersCostSpent;
        }

        public float GetCalculatedNetProfit(float rent)
        {
            return grossProfit - rent;
        }
    }
}