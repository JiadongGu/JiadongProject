using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    public float money;
    public TMP_Text moneyText;

    void Start()
    {
        UpdateMoneyText();
    }

    public void ChangeMoney(float amount)
    {
        money += amount;

        if (money <= 0)
        {
            money = 0;
        }

        UpdateMoneyText();
    }

    [Button]
    public void TestAddMoney()
    {
        ChangeMoney(100);
    }

    void UpdateMoneyText()
    {
        moneyText.text = "$" + money.ToString("F2");
    }
}
