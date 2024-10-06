using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CityClicker))]
public class RetailBuilding : MonoBehaviour
{
    [ReadOnly] public float totalRent;
    public float baseRent;
    public float sizeFactor;
    public float locationPremium;

    [Space(20)]

    public int baseCapacity = 1000;
    public float capacityFactor;
    [ReadOnly] public int capacity;

    [Space(20)]
    public Vector2 baseRentRange = new Vector2(500, 3000);
    public Vector2 sizeFactorRange = new Vector2(1, 3);
    public Vector2 capacityFactorRange = new Vector2(1, 4);
    public Vector2 locationPremiumRange = new Vector2(1, 2);

    [HorizontalLine]

    public bool generateAtStart = true;
    public int menu = 1;
    public int panel = 0;

    CityClicker cityClicker;
    RetailRenter renter;
    LookAtFollower worldPointer;


    [Button]
    public void GenerateRandomRent()
    {
        baseRent = Mathf.Round(Random.Range(baseRentRange.x, baseRentRange.y) * 100f) / 100f;
        sizeFactor = Mathf.Round(Random.Range(sizeFactorRange.x, sizeFactorRange.y) * 100f) / 100f;
        capacityFactor = Mathf.Round(Random.Range(capacityFactorRange.x, capacityFactorRange.y) * 100f) / 100f;
        locationPremium = Mathf.Round(Random.Range(locationPremiumRange.x, locationPremiumRange.y) * 100f) / 100f;

        OnValidate();
    }

    void OnValidate()
    {
        capacity = Mathf.RoundToInt(baseCapacity + (baseCapacity * capacityFactor));
        if (capacity < 0) capacity = 0;

        CalculateTotalRent();
    }

    public void Start()
    {
        worldPointer = GetComponentInChildren<LookAtFollower>();
        renter = RetailRenter.Instance;
        cityClicker = GetComponent<CityClicker>();
        cityClicker.OnClickAction += (whoGotClick) =>
        {
            if (renter.IOwnRetailBuilding() == false)
            {
                SetRetailRenterPreviewToThis();
                MenusManager.Instance.ShowPanelFromMenu(menu, panel, true);
                return;
            }

            if(whoGotClick.GetComponent<RetailBuilding>() == this && this == renter.myRetailBuilding)
                MenusManager.Instance.ShowPanelFromMenu(menu, panel, true);
        };

        if (generateAtStart) GenerateRandomRent();
    }

    void Update()
    {
        
        if(renter.IOwnRetailBuilding() == false)
        {
            cityClicker.outline.enabled = PlayerClicker.Instance.hoveringObj == cityClicker;
        }
        else
        {
            bool isThisMyBuilding = renter.myRetailBuilding == this;
            cityClicker.outline.enabled = isThisMyBuilding;

            if(worldPointer != null && worldPointer.gameObject.activeSelf)
            {
                worldPointer.gameObject.SetActive(isThisMyBuilding);
            }
        }
    }

    public void SetRetailRenterPreviewToThis()
    {
        renter.UpdateRentInfoPreview(this);
    }

    public void CalculateTotalRent()
    {
        totalRent = baseRent * sizeFactor * capacityFactor * locationPremium;
        totalRent = Mathf.Round(totalRent * 100f) / 100f;
    }
}
