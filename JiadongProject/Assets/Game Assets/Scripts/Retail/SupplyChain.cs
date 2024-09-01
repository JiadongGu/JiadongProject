using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu()]
public class SupplyChain : ScriptableObject
{
    public string chainName;
    [ShowAssetPreview] public Sprite chainIcon;

    [HorizontalLine]

    public List<SupplyCompany> companies = new List<SupplyCompany>();

    public RetailItem GetRandomRetailItem()
    {
        SupplyCompany randomCompany = companies[Random.Range(0, companies.Count)];
        RetailItem retailItem = randomCompany.retailItems[Random.Range(0, randomCompany.retailItems.Count)];

        return retailItem;
    }

    [System.Serializable]
    public class SupplyCompany
    {
        public string companyName;
        [ShowAssetPreview] public Sprite companyIcon;
        [Expandable] public List<RetailItem> retailItems = new List<RetailItem>();
    }
}
