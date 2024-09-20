using UnityEngine;

public class Coin : Interactable
{
    [SerializeField] private int coinID;

    protected override void Start()
    {
        base.Start();
        PopupActive = false;
    }

    protected override void Interact()
    {
        GameAssets.g.PlayerMoneySystem.AddCoin(coinID);
        AudioManager.g.Play("Coin");
        DestoryInteractable();
    }


}
