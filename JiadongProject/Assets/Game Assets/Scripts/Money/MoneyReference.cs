using TMPro;
using UnityEngine;

public class MoneyReference : MonoBehaviour
{
    public TMP_Text moneyText;

    void Update()
    {
        moneyText.text = $"${MoneyManager.Instance.money:F2}";
    }
}
