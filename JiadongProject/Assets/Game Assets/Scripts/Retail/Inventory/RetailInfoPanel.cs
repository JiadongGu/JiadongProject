using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;

public class RetailInfoPanel : Singleton<RetailInfoPanel>
{
    [ReadOnly] public RetailInventory.InventoryStock inventoryStock;
    public TMP_Text itemNameText;
    public TMP_Text textInfo;
    public Image iconImage;
    public TMP_InputField sellField;

    [HorizontalLine]
    public MenusManager menusManager;
    public RetailOrderSearchPanel orderSearchPanel;
    public int menuIndex;
    public Button orderButton;

    void Start()
    {
        UpdateInfo(null);

        orderButton.onClick.AddListener(() =>
        {
            orderSearchPanel.OnDropdownValueChanged(0); // Switch to all category
            string stockItemName = inventoryStock.itemData.itemName;
            orderSearchPanel.searchField.text = stockItemName;
            orderSearchPanel.searchField.onValueChanged?.Invoke(stockItemName);

            menusManager.ShowMenuPanel(menuIndex, true);
        });

        sellField.onValueChanged.AddListener(OnSellFieldValueChangedCallback);
    }

    void Update() 
    {
        bool noItem = inventoryStock == null || inventoryStock.itemData == null;
        sellField.interactable = noItem == false;
        orderButton.interactable = noItem == false;
    
        UpdateInfoText();
    }

    void OnSellFieldValueChangedCallback(string val)
    {
        if(float.TryParse(val, out float price))
        {
            inventoryStock.sellPrice = price;
        }
    }

    public void UpdateInfo(RetailInventory.InventoryStock inventoryStock)
    {
        this.inventoryStock = inventoryStock;
        itemNameText.text = inventoryStock == null? "Select An Item To View Info" : inventoryStock.itemData?.itemName;
        iconImage.sprite = inventoryStock?.itemData?.icon;
        sellField.text = inventoryStock?.sellPrice.ToString("F2");

        UpdateInfoText();
    }
    
    public void UpdateInfoText()
    {
        string infos = $"Quantity: {(inventoryStock == null? 0 : inventoryStock.quantity)}\nSold: {(inventoryStock == null? 0 : inventoryStock.sold)}\nCOGS Price: ${(inventoryStock == null? 0 : inventoryStock.cogsPrice)}";
        textInfo.text = infos;
    }
}
