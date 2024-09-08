using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetailOrderSearchPanel : MonoBehaviour
{
    public RetailInventory inventory;
    public ObjectPooler searchCardPool;

    [HorizontalLine]

    public TMP_InputField searchField;
    public Button searchButton;
    public TMP_Dropdown supplyChainDropdown;
    public TMP_Text resultText;

    void Awake()
    {
        inventory.OnAvailableItemsRefreshed += UpdateResult;
    }

    void UpdateResult()
    {
        resultText.text = $"Result: {inventory.allAvailableItems.Count} items";
    }
}
