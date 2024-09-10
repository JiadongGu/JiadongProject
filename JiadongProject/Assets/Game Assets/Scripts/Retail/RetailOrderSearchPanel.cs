using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class RetailOrderSearchPanel : MonoBehaviour
{
    public RetailInventory inventory;
    public Transform searchCardParent;
    public RetailItemOrderSearchCard searchCardPrefab;

    [HorizontalLine]

    public TMP_InputField searchField;
    public TMP_Dropdown supplyChainDropdown;
    public TMP_Text resultText;

    [HorizontalLine]

    [ReadOnly] public List<RetailItemOrderSearchCard> spawnedSearchCards = new List<RetailItemOrderSearchCard>();
    [ReadOnly] public List<RetailItem> filteredRetailItems;
    Coroutine filterViewRoutine;

    void Awake()
    {
        inventory.OnAvailableItemsRefreshed += PopulateSearchCards;
        inventory.OnAvailableItemsRefreshed += UpdateResult;
        inventory.OnAvailableItemsRefreshed += PopulateDropdownCompanies;
        searchField.onValueChanged.AddListener(OnSearchFieldChangeCallback);
        supplyChainDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        OnDropdownValueChanged(0);
    }

    void UpdateResult()
    {
        resultText.text = $"Result: {spawnedSearchCards.Where(x => x.gameObject.activeSelf).Count()} items";
    }

    void PopulateSearchCards()
    {
        foreach (Transform c in searchCardParent)
        {
            Destroy(c.gameObject);
        }

        foreach (var stock in inventory.allAvailableItems)
        {
            RetailItemOrderSearchCard newCard = Instantiate(searchCardPrefab, searchCardParent);
            newCard.Init(stock);
            spawnedSearchCards.Add(newCard);
        }
    }

    void PopulateDropdownCompanies()
    {
        List<string> chainNames = new List<string> { "All" };
        chainNames.AddRange(inventory.allSupplyChains.Select(chain => chain.chainName).ToList());

        List<TMP_Dropdown.OptionData> options = chainNames
            .Select(name => new TMP_Dropdown.OptionData(name))
            .ToList();

        supplyChainDropdown.ClearOptions();
        supplyChainDropdown.AddOptions(options);
    }

    void OnDropdownValueChanged(int dropdownIndex)
    {
        if (dropdownIndex == 0) // "All" option selected
        {
            // Get all retail items from all supply chains
            filteredRetailItems = inventory.allSupplyChains
                .SelectMany(chain => chain.GetAllRetailItems())
                .ToList();
        }
        else
        {
            // Specific supply chain selected
            SupplyChain selectedChain = inventory.allSupplyChains[dropdownIndex - 1];
            filteredRetailItems = selectedChain.GetAllRetailItems();
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
        foreach (var cards in spawnedSearchCards)
        {
            cards.gameObject.SetActive(filteredRetailItems.Contains(cards.stock.itemData) && 
                                        cards.stock.itemData.itemName.Contains(filterValue, System.StringComparison.OrdinalIgnoreCase));
        }
        UpdateResult();
    }
}
