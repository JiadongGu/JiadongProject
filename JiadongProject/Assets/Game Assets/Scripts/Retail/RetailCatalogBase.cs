using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public abstract class RetailCatalogElement : MonoBehaviour
{
    [ReadOnly] public RetailInventory.InventoryStock stock;
    public virtual void Init(RetailInventory.InventoryStock stock)
    {
        this.stock = stock;
    }
}

public abstract class RetailCatalogBase : MonoBehaviour
{
    public RetailInventory inventory;

    [HorizontalLine]

    public TMP_InputField searchField;
    public TMP_Dropdown supplyChainDropdown;
    public TMP_Text resultText;
    public GameObject noneText;

    [HorizontalLine]

    [ReadOnly] public List<RetailCatalogElement> spawnedCatalogElements = new List<RetailCatalogElement>();
    [ReadOnly] public List<RetailItem> filteredItems;
    Coroutine filterViewRoutine;

    void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        searchField.onValueChanged.AddListener(OnSearchFieldChangeCallback);
        supplyChainDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        OnDropdownValueChanged(0);
    }

    public void UpdateResult()
    {
        resultText.text = $"Result: {ShowingContentAmount()} items";
    }

    public void PopulateDropdownCompanies()
    {
        List<string> chainNames = new List<string> { "All" };
        chainNames.AddRange(inventory.allSupplyChains.Select(chain => chain.chainName).ToList());

        List<TMP_Dropdown.OptionData> options = chainNames
            .Select(name => new TMP_Dropdown.OptionData(name))
            .ToList();

        supplyChainDropdown.ClearOptions();
        supplyChainDropdown.AddOptions(options);
    }

    public void OnDropdownValueChanged(int dropdownIndex)
    {
        if (dropdownIndex == 0) // "All" option selected
        {
            // Get all retail items from all supply chains
            filteredItems = inventory.allSupplyChains
                .SelectMany(chain => chain.GetAllRetailItems())
                .ToList();
        }
        else
        {
            // Specific supply chain selected
            SupplyChain selectedChain = inventory.allSupplyChains[dropdownIndex - 1];
            filteredItems = selectedChain.GetAllRetailItems();
        }

        OnSearchFieldChangeCallback(searchField.text);
    }

    void OnSearchFieldChangeCallback(string value)
    {
        if (filterViewRoutine != null) StopCoroutine(filterViewRoutine);
        filterViewRoutine = StartCoroutine(FilterContentViewRoutine(value));
    }

    IEnumerator FilterContentViewRoutine(string filterValue)
    {
        yield return new WaitForSeconds(0.7f);
        foreach (var element in spawnedCatalogElements)
        {
            element.gameObject.SetActive(filteredItems.Contains(element.stock.itemData) &&
                                        element.stock.itemData.itemName.Contains(filterValue, System.StringComparison.OrdinalIgnoreCase));
        }
        UpdateNoneText();
        UpdateResult();
    }

    public int ShowingContentAmount()
    {
        return spawnedCatalogElements.Where(x => x.gameObject.activeSelf).Count();
    }

    public virtual void UpdateNoneText()
    {
        noneText.SetActive(ShowingContentAmount() == 0);
    }
}
