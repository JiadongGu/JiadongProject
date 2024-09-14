using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class RetailCustomers : MonoBehaviour
{
    public int baseTraffic = 1000;
    [ReadOnly] public int currentTraffic;
    public int trafficLevel = 1;                             // 1: Low Traffic, 2: Medium Traffic, 3: High Traffic
    public Vector2 updateRange = new Vector2(1, 3);
    public bool allowSelling;

    [HorizontalLine]

    public RetailRenter renter;
    public RetailInventory inventory;
    public RetailBusiness business;

    [HorizontalLine]

    Image allowSellingImage;
    Image allowSellingIconImage;
    public Button allowSellingButton;
    public Color pausedColor;
    public Color resumeColor;
    public Sprite playIcon;
    public Sprite resumeIcon;


    void Start()
    {
        StartCoroutine(SellUpdateRoutine());

        allowSellingImage = allowSellingButton.GetComponent<Image>();
        allowSellingIconImage = allowSellingButton.transform.GetChild(0).GetComponent<Image>();
        allowSellingButton.onClick.AddListener(() =>
        {
            allowSelling = !allowSelling;
        });
    }

    void Update()
    {
        allowSellingImage.color = allowSelling? resumeColor : pausedColor;
        allowSellingIconImage.sprite = allowSelling? resumeIcon : playIcon;
    }

    IEnumerator SellUpdateRoutine()
    {
        while (true)
        {
            // If I dont have a building yet
            if (renter.IOwnRetailBuilding() == false)
            {
                yield return null;
                continue;
            }
            // If there isn't even any items in my inventory
            if (inventory.myStockItems.Count == 0)
            {
                yield return null;
                continue;
            }

            if (allowSelling == false)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(Random.Range(updateRange.x, updateRange.y));

            trafficLevel = GetTrafficLevel(business.currentDateTime);

            // Calculate the current traffic (amount of people inside my building) based on the current trafficLevel and location premium
            currentTraffic = CalculateCurrentTraffic(renter.myRetailBuilding.locationPremium, trafficLevel);

            // Go through each person in the building and calculate if they are willing to purchase
            int randomPeopleBuying = Random.Range(0, currentTraffic / 4);
            for (int i = 0; i < randomPeopleBuying; i++)
            {
                // Simulate customer interest in an item
                int indexStock = Random.Range(0, inventory.myStockItems.Count);
                RetailInventory.InventoryStock selectedStock = null;
                if (indexStock <= inventory.myStockItems.Count - 1)
                    selectedStock = inventory.myStockItems[indexStock];
                else
                    continue;

                float purchaseProbability = CalculatePurchaseProbability(selectedStock);

                // Determine if this customer will make a purchase
                if (Random.value < purchaseProbability && selectedStock.quantity > 0)
                {
                    int quantity = Mathf.Min(selectedStock.quantity, Random.Range(1,5));
                    business.SellItem(selectedStock, quantity);

                    print($"Customer bought x{quantity} of {selectedStock.itemData.itemName}. PP: {purchaseProbability}");
                }
            }


            yield return null;
        }
    }

    int CalculateCurrentTraffic(float locationPremium, int trafficLevel)
    {
        int scaledTraffic = (int)(baseTraffic * locationPremium * trafficLevel * 0.5);
        return scaledTraffic;
    }

    float CalculatePurchaseProbability(RetailInventory.InventoryStock stock)
    {
        // Calculate the markup percentage
        float markupPercentage = (stock.sellPrice - stock.cogsPrice) / stock.cogsPrice;

        // Calculate price attractiveness based on how close the selling price is to COGS
        // Assuming customers perceive lower markups as more attractive deals
        float priceAttractiveness = 1.0f - markupPercentage; // More attractive if markup is lower

        // Clamp the price attractiveness to ensure it remains within realistic bounds
        float clampedProbability = Mathf.Clamp(priceAttractiveness, 0.01f, 0.9f);

        return clampedProbability; // Return the probability, clamped between 10% and 90%
    }

    int GetTrafficLevel(System.DateTime currentTime)
    {
        int hour = currentTime.Hour;

        if (hour >= 7 && hour < 10) // 7 AM to 10 AM
            return 2;
        else if (hour >= 10 && hour < 15) // 10 AM to 3 PM
            return 3;
        else if (hour >= 15 && hour < 19) // 3 PM to 7 PM
            return 1;
        else
            return 0; // No traffic outside of 7 AM to 7 PM
    }
}
