using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class RetailHistoryBar : MonoBehaviour
{
    [ReadOnly] public RetailBusiness.RetailHistory retailHistory;
    public TMP_Text dayText;
    public TMP_Text startMoneyText;
    public TMP_Text endMoneyText;
    public TMP_Text salesText;
    public TMP_Text grossText;
    public TMP_Text netText;

    public void Init(RetailBusiness.RetailHistory history)
    {
        retailHistory = history;
        if(history != null)
        {
            dayText.text = retailHistory.day + "";
            startMoneyText.text = "$" + retailHistory.startMoney.ToString("F2");
            endMoneyText.text = "$" + retailHistory.endMoney.ToString("F2");
            salesText.text = "$" + retailHistory.salesRevenue.ToString("F2");
            grossText.text = "$" + retailHistory.grossProfit.ToString("F2");
            netText.text = "$" + retailHistory.netProfit.ToString("F2");
        }
    }
}
