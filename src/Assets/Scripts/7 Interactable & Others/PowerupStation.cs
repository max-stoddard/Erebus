using UnityEngine;

public class PowerupStation : Interactable
{
    [SerializeField] private int PowerupID;
    private PowerupManager.Powerup Powerup;
    [SerializeField] private SpriteRenderer[] PotionSprites;


    protected override void Start()
    {
        Powerup = GameAssets.g.PowerupManager.GetPowerup(PowerupID);
        base.Start();
        CreatePopup();
        foreach (PowerupStation ps in FindObjectsOfType<PowerupStation>())
        {
            if (ps.PowerupID == PowerupID && ps.gameObject != gameObject)
            {
                Debug.LogError($"Duplication of one type of PowerupStation: {gameObject} & {ps.gameObject}");
            }
        }
        if (!GameAssets.g.PowerupManager.IsInRange(PowerupID))
        {
            Debug.LogError($"No powerup of index {PowerupID} exists");
        }

        CurrentCost = Powerup.price;
        foreach (SpriteRenderer s in PotionSprites)
        {
            s.sprite = Powerup.sprite;
        }
    }

    protected override void Interact()
    {
        SpendMoneyCurrentCost(); // OBJECTIVE 22.4
        GameAssets.g.PowerupManager.PowerupActive(PowerupID);
        InRadius = false;
        PopupActive = false;
        DisableInteractable();
    }

    protected override void CreatePopup()
    {
        base.CreatePopup();

        bps.ChangeImage(Powerup.sprite);
        bps.ChangeMainMessage($"BUY {Powerup.name}");
        bps.ChangeSideMessage("");
        bps.ChangePrice(Powerup.price);
    }
}
