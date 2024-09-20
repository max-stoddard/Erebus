using System;
using System.Collections;
using UnityEngine;

public class UpgradeStation : Interactable
{
    [Header("Upgrades")]
    [SerializeField] private int[] Costs = new int[3];
    private int stage; // 0 means collecting coins, 1, 2, 3 corraspond to upgrade levels 4 corraponds to max level

    [Header("Glow")]
    [SerializeField] private Gradient gradient;
    [SerializeField] private Color defaultColor;

    [SerializeField] private float TimeToGlow;
    private float CurrentTimeToGlow;


    [SerializeField] private SpriteRenderer[] GlowSprites = new SpriteRenderer[4];
    
    private bool[] GlowsActive;


    protected override void Start()
    {
        base.Start();
        PopupActive = false;
        CurrentTimeToGlow = 0;
        GlowsActive = new bool[4];
        stage = 0;
        
    }

    protected override void Update()
    {
        base.Update();
        UpdateGlow();
    }


    protected override void Interact()
    {
        if (stage == 0 && !StructuralComparisons.StructuralEqualityComparer.Equals(GlowsActive, GameAssets.g.PlayerMoneySystem.Coins)) // StructuralComparisons compares values whereas Array.Equals compares memory locations
            // Fixed test [3.15]
        {
            GameAssets.g.PlayerMoneySystem.Coins.CopyTo(GlowsActive, 0);
            if (StructuralComparisons.StructuralEqualityComparer.Equals(GlowsActive, new bool[4] { true, true, true, true })) // If all coins in upgrade machine
            {
                AudioManager.g.Play("UpgradeComplete");
                PopupActive = true;
                stage = 1;
                GameEvents.g.OnGunChanged += GunChanged;
                GunChanged();

            }
            else // If only 1 2 or 3 coins placed into machine
            {
                AudioManager.g.Play("UpgradeDing");
            }
        }
        else if (stage >= 1 && stage <= 3)
        {
            SpendMoneyCurrentCost(); // OBJECTIVE 22.2
            GameAssets.g.PlayerCombat.UpgradeCurrentGun(); // OBJECTIVE 26
            GunChanged();
        }
        else if (stage > 4 || stage < 0)
        {
            Debug.LogError($"Stage {stage}; out of range");
        }
    }

    private void GunChanged()
    {
        if (stage == 0)
        {
            return;
        }
        Destroy(Popup);
        stage = GameAssets.g.PlayerCombat.GetCurrentGun().UpgradeLevel + 1;

        CreatePopup();
    }

    protected override void CreatePopup()
    {
        base.CreatePopup();
        Gun gun = GameAssets.g.PlayerCombat.GetCurrentGun();
        if (stage == 4)
        {
            bps.ChangeMainMessage($"Max Level");
            bps.ChangePrice(0);
            CurrentCost = 0;
        }
        else
        {
            bps.ChangeMainMessage($"UPGRADE {stage}");
            bps.ChangePrice(Costs[stage - 1]);
            CurrentCost = Costs[stage - 1];
        }

        bps.ChangeSideMessage(gun.Name());
        bps.ChangeImage(gun.GunImage);


    }

    #region Glow Methods

    private void UpdateGlow() // Algorithm to cycle the glowing animation
    {
        CurrentTimeToGlow += Time.deltaTime;
        if (CurrentTimeToGlow > TimeToGlow) CurrentTimeToGlow = 0f;
        Color c = gradient.Evaluate(CurrentTimeToGlow / TimeToGlow);

        for (int i = 0; i < GlowSprites.Length; i++)
        {
            if (GlowsActive[i])
            {
                GlowSprites[i].color = c;
            }
            else
            {
                GlowSprites[i].color = defaultColor;
            }
        }
    }


    #endregion
}
