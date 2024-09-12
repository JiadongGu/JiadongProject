using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu()]
public class SupplyChain : ScriptableObject
{
    public string chainName;
    [ShowAssetPreview] public Sprite chainIcon;

    [HorizontalLine]

    public List<SupplyCompany> companies = new List<SupplyCompany>();

    [System.Serializable]
    public class SupplyCompany
    {
        public string companyName;
        [ShowAssetPreview] public Sprite companyIcon;
        public int shippingCost = 5;
        [Expandable] public List<RetailItem> retailItems = new List<RetailItem>();
    }

    public RetailItem GetRandomRetailItem()
    {
        SupplyCompany randomCompany = companies[Random.Range(0, companies.Count)];
        RetailItem retailItem = randomCompany.retailItems[Random.Range(0, randomCompany.retailItems.Count)];

        return retailItem;
    }

    public bool DoesRetailItemExist(RetailItem item)
    {
        return companies.Any(company => company.retailItems.Contains(item)); 
    }

    public List<RetailItem> GetAllRetailItems()
    {
        return companies.SelectMany(company => company.retailItems).ToList();
    }
}