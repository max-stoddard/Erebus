using UnityEngine;
using UnityEngine.UI;

public class PlayerMoneySystem : MonoBehaviour
{
    [SerializeField] private int StartingMoney;
    private int CurrentMoney;
    public int TotalMoney; // Also the score
    [SerializeField] private Text MoneyText;
    public bool[] Coins { get; private set; }
    [SerializeField] private bool DebugCoins;

    [SerializeField] private bool DebugMoney;

    private void Start()
    {
        CurrentMoney = DebugMoney ? 1000000000 : StartingMoney;
        TotalMoney = StartingMoney;
        UpdateUI();
        Coins = new bool[4];
        if (DebugCoins)
        {
            Coins = new bool[4] { true, true, true, true };
        }
    }

    public bool CanBuy(int cost)
    {
        return CurrentMoney - cost >= 0;
    }

    public void ChangeMoney(int money) // Cost can be negative or positive: if negative money deducted and if positive money added
    {
        if (money > 0)
        {
            CurrentMoney += money;
            TotalMoney += money;
        }
        else if (money == 0)
        {
            Debug.LogWarning($"$0 Attempted to add; is this correct?");
        }
        else
        {
            if (!CanBuy(money))
            {
                Debug.LogError($"Have ${CurrentMoney} and tried to deduct ${money}");
            }
            else
            {
                CurrentMoney += money;
            }
        }
        UpdateUI();
    }


    public void UpdateUI()
    {
        GameAssets.g.UIMethods.ChangeMoneyText(CurrentMoney);
        GameAssets.g.UIMethods.ChangeScoreText(TotalMoney);
    }

    public void AddCoin(int ID)
    {
        if (ID < 0 || ID > 3)
        {
            Debug.LogError("CoinID out of range");
            return;
        }
        Coins[ID] = true;
    }

}
