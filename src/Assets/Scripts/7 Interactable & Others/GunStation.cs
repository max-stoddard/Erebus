using UnityEngine;

public class GunStation : Interactable
{
    [Header("Gun")]

    [SerializeField] private int GunID;

    private bool ToBuyGun; // if true then mode is to buy gun, if not is in mode to buy ammo


    protected override void Start()
    {
        base.Start();
        ToBuyGun = !GameAssets.g.PlayerCombat.PlayerHasGunID(GunID);
        CreatePopup();
    }

    protected override void Interact()
    {
        if (ToBuyGun)
        {
            GameAssets.g.PlayerCombat.AddGun(GunID); // OBJECTIVE 24.5
            Destroy(Popup);
            ToBuyGun = false;
            SpendMoneyCurrentCost(); // OBJECTIVE 22.1
            CurrentCost = GameAssets.g.PlayerCombat.GetCurrentGun().AmmoCost();
            CreatePopup();
        }


        else if (!ToBuyGun && !GameAssets.g.PlayerCombat.IsGunAmmoFull(GunID))
        {
            GameAssets.g.PlayerCombat.RefillGunAmmo(GameAssets.g.PlayerCombat.GetSlotFromGunID(GunID)); // OBJECTIVE 24.5
            SpendMoneyCurrentCost(); // OBJECTIVE 22.3
        }
        else
        {
            GameAssets.g.UIMethods.FlashMagAmmoRed();
            GameAssets.g.UIMethods.FlashResAmmoRed();
        }
    }


    protected override void CreatePopup()
    {
        base.CreatePopup();
        Gun gun = GameAssets.g.PlayerCombat.GetGunFromID(GunID);
        bps.ChangeSideMessage(gun.Name());
        bps.ChangeImage(gun.GunImage);
        if (ToBuyGun) // If player doesnt have gun
        {
            bps.ChangeMainMessage("BUY GUN");
            bps.ChangePrice(gun.GunCost());
            CurrentCost = gun.GunCost();
        }
        else // If player does have gun in inventory
        {
            bps.ChangeMainMessage("BUY AMMO");
            bps.ChangePrice(gun.AmmoCost());
            CurrentCost = gun.AmmoCost();
        }
    }

}
