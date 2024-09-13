using NaughtyAttributes;

public class MoneyManager : Singleton<MoneyManager>
{
    public float money;

    public void ChangeMoney(float amount)
    {
        money += amount;

        if (money <= 0)
        {
            money = 0;
        }
    }

    [Button]
    public void TestAddMoney()
    {
        ChangeMoney(100);
    }
}
