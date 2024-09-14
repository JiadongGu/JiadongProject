using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailRenter : Singleton<RetailRenter>
{
    [ReadOnly] public RetailBuilding viewingRetailBuilding;
    [ReadOnly] public RetailBuilding myRetailBuilding;

    [HorizontalLine]

    public TMP_Text baseRentText;
    public TMP_Text sizeFactorText;
    public TMP_Text locationPremiumText;
    public TMP_Text capacityFactorText;
    public TMP_Text totalCapacityText;
    public TMP_Text totalRent;
    public Button rentButton;

    [HorizontalLine]
    public int retailMenu;
    public int storeHubPanel;

    void Start()
    {
        rentButton.onClick.AddListener(() =>
        {
            if (viewingRetailBuilding != null)
            {
                if(IOwnRetailBuilding() == false) RentBuilding(viewingRetailBuilding);
                else ShowStoreHub();
            }
            else
            {
                if(IOwnRetailBuilding())
                {
                    ShowStoreHub();
                    return;
                }
                print("Something went wrong... You are renting a building you cannot see.");
            }
        });
    }

    public void UpdateRentInfoPreview(RetailBuilding building)
    {
        viewingRetailBuilding = building;

        baseRentText.text = "Base Rent: $" + building.baseRent;
        sizeFactorText.text = "Size Factor: x" + building.sizeFactor;
        locationPremiumText.text = "Location Premium: x" + building.locationPremium;
        capacityFactorText.text = "Capacity Factor: x" + building.capacityFactor;
        totalCapacityText.text = "Total Capacity: " + building.capacity;
        totalRent.text = "Total Rent: $" + building.totalRent;
    }

    void RentBuilding(RetailBuilding building)
    {
        if (building == null) return;

        if (MoneyManager.Instance.money - building.totalRent >= 0)
        {
            MoneyManager.Instance.ChangeMoney(-building.totalRent);
            myRetailBuilding = building;
            viewingRetailBuilding = null;
            ShowStoreHub();
            RetailBusiness.Instance.UpdateTodayInfoTexts();
            rentButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "VIEW";
        }
        else
        {
            print("You dont have enough money to rent this!");
        }
    }

    public void ShowStoreHub()
    {
        MenusManager.Instance.ShowAllCanvas(false);
        MenusManager.Instance.ShowPanelFromMenu(retailMenu, storeHubPanel, true);
    }

    public bool IOwnRetailBuilding() => myRetailBuilding != null;
}
