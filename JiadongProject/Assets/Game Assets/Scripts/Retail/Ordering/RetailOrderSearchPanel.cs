using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetailOrderSearchPanel : RetailCatalogBase
{
    public Transform contentParent;
    public RetailCatalogElement searchCardPrefab;

    public override void Init() {
        
        inventory.OnAvailableItemsRefreshed += PopulateElements;
        inventory.OnAvailableItemsRefreshed += UpdateResult;
        inventory.OnAvailableItemsRefreshed += PopulateDropdownCompanies;

        base.Init();
    }

    void PopulateElements()
    {
        foreach (Transform c in contentParent)
        {
            Destroy(c.gameObject);
        }
        foreach (var stock in inventory.allAvailableItems)
        {
            
            RetailCatalogElement newElement = Instantiate(searchCardPrefab, contentParent);
            newElement.Init(stock);
            spawnedCatalogElements.Add(newElement);
        }
    }
}
