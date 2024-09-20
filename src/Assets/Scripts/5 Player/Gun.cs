using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Info")]

    [SerializeField] private string _Name; // Name used for gun

    [SerializeField] private int _ID; // Unique ID for gun

    [SerializeField] private int _GunPrice, _AmmoCost;


    [Header("Automatic Settings")]

    [SerializeField] private bool[] Automatic = new bool[2];
    private bool IsAutomaticTriggerDown;

    [Header("Damage Settings")]

    public int[] Damage = new int[4];

    [Header("Time settings")]

    [SerializeField] private float[] TimeBetweenShots = new float[2];
    private float CurrentTimeBetweenShots;

    [SerializeField] private float[] ReloadTime = new float[2];
    private float CurrentReloadTime;
    private bool CurrentlyReloading;

    [Header("Spread Settings")]

    [SerializeField] private float[] Spread = new float[2];


    [Header("Ammo")]
    [SerializeField] private int[] MaxAmmoMag = new int[2];
    [SerializeField] private int[] MaxAmmoReserve = new int[2];
    public int BulletsMagazine { get; private set; } // OBJECTIVE 24.1
    public int BulletsReserve { get; private set; } // OBJECTIVE 24.2


    [Header("Default Positions")]

    [SerializeField] private Vector2 LocalGunPosition;

    [SerializeField] private Vector2 LeftHandPosition;


    [Header("Two Hand Positions")]

    [SerializeField] private bool TwoHands; // Only left if false

    [SerializeField] private Vector2 RightHandPosition;

    private GameObject LeftHand, RightHand;

    [Header("Bullet")]

    [SerializeField] private Transform FirePoint;

    [SerializeField] private float bulletForce = 20f;


    [Header("UI")]
    public Sprite GunImage;


    [Header("Upgrades")]

    [SerializeField] private Color UpgradeColor;
    [SerializeField] private SpriteRenderer ColorLocation;

    public int UpgradeLevel { get; private set; } // 0 1 2 3
    private int IsUpgraded; // 0 or 1 used instead of bool because can access arrays

    private bool FastReload;
    private bool AccuracyIncreased;
    private bool FireRateIncrease;

    #region Getters

    public int ID()
    {
        return _ID;
    }

    public string Name()
    {
        return _Name;
    }

    public int GunCost()
    {
        return _GunPrice;
    }

    public int AmmoCost()
    {
        return _AmmoCost;
    }

    #endregion

    #region Initilize Classes

    private void Awake()
    {
        InitialiseGunPos();
        InitialiseHandPos();
        InitiliseAmmo();
    }

    private void InitialiseGunPos()
    {
        gameObject.transform.localPosition = LocalGunPosition;
        gameObject.transform.localRotation = Quaternion.identity;
    }

    private void InitialiseHandPos()
    {
        LeftHand = GameAssets.g.Player.transform.GetChild(1).gameObject;
        RightHand = GameAssets.g.Player.transform.GetChild(2).gameObject;
        LeftHand.transform.localPosition = LeftHandPosition;
        if (!TwoHands) { RightHand.SetActive(false); }
        else
        {
            RightHand.SetActive(true);
            RightHand.transform.localPosition = RightHandPosition;
        }
    }

    private void InitiliseAmmo()
    {
        SetAmmo(MaxAmmoMag[IsUpgraded], MaxAmmoReserve[IsUpgraded]);
    }

    public void UpdateGunPos()
    {
        InitialiseHandPos();
        InitialiseGunPos();
    }

    #endregion

    #region Update
    private void Update()
    {
        if (CurrentTimeBetweenShots > 0)
        {
            CurrentTimeBetweenShots -= FireRateIncrease ? Time.deltaTime * 1.5f : Time.deltaTime;
        }
        else
        {
            CurrentTimeBetweenShots = 0;
        }
        if (IsAutomaticTriggerDown && CurrentTimeBetweenShots <= 0 && BulletsMagazine > 0)
        {
            FireBullet();
            CurrentTimeBetweenShots = TimeBetweenShots[IsUpgraded];
        }

        if (CurrentlyReloading)
        {
            if (FastReload)
            {
                CurrentReloadTime -= Time.deltaTime * 2;
            }
            else
            {
                CurrentReloadTime -= Time.deltaTime;
            }

            if (CurrentReloadTime <= 0)
            {
                Reload();
                CurrentReloadTime = ReloadTime[IsUpgraded];
                CurrentlyReloading = false;
            }
        }
        else
        {
            CurrentReloadTime = ReloadTime[IsUpgraded];
        }

    }

    #endregion

    #region Bullet Fire Methods

    public void StartFiring(bool FireratePowerup, bool AccuracyPowerup)
    {
        AccuracyIncreased = AccuracyPowerup;
        FireRateIncrease = FireratePowerup;

        if (BulletsMagazine > 0 && !CurrentlyReloading) // OBJECTIVE 24.4
        {
            if (Automatic[IsUpgraded])
            {
                IsAutomaticTriggerDown = true;
            }
            else if (CurrentTimeBetweenShots <= 0)
            {
                FireBullet();
            }
        }
        else if (BulletsMagazine == 0 && !CurrentlyReloading)
        {
            AudioManager.g.Play("Dryfire");
            GameAssets.g.UIMethods.FlashMagAmmoRed(); // OBJECTIVE 32.2
        } // OBJECTIVE 24.4
    }

    public void StopFiring()
    {
        if (Automatic[IsUpgraded])
        {
            IsAutomaticTriggerDown = false;
        }
    }

    private void FireBullet()
    {

        GameObject bullet = Instantiate(GameAssets.g.Bullet, FirePoint.position, gameObject.transform.rotation);

        bullet.GetComponent<Bullet>().ChangeDamage((int)(Damage[UpgradeLevel] * UnityEngine.Random.Range(1.0f, 2.0f)));


        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        Vector2 fireForce = FirePoint.up;
        Vector2 spread = new Vector2(UnityEngine.Random.Range(AccuracyIncreased ? -Spread[IsUpgraded] * 0.5f : Spread[IsUpgraded], AccuracyIncreased ? Spread[IsUpgraded] * 0.5f : -Spread[IsUpgraded]), 0f);
        Vector2 Force = (fireForce + spread).normalized * bulletForce;

        rb.AddForce(Force, ForceMode2D.Impulse);
        BulletsMagazine -= 1; // OBJECTIVE 24.3
        GameAssets.g.PlayerCombat.UpdateUI();
        AudioManager.g.Play("Gunfire");
    }

    #endregion

    #region Ammo Methods

    public void TryToReload(bool Powerup)
    {
        FastReload = Powerup;
        if (BulletsReserve != 0 && BulletsMagazine != MaxAmmoMag[IsUpgraded] && !CurrentlyReloading)
        {
            CurrentlyReloading = true;
            AudioManager.g.Play("Reload Start");
        }
        else if (BulletsReserve == 0 && !CurrentlyReloading && BulletsMagazine != MaxAmmoMag[IsUpgraded])
        {
            GameAssets.g.UIMethods.FlashResAmmoRed(); // OBJECTIVE 32.2
        }
    }

    private void Reload() // OBJECTIVE 24.2
    {
        int BulletsNeededForFullMag = MaxAmmoMag[IsUpgraded] - BulletsMagazine;
        if (BulletsReserve >= BulletsNeededForFullMag)
        {
            BulletsReserve -= BulletsNeededForFullMag;
            BulletsMagazine += BulletsNeededForFullMag;
        }
        else
        {
            BulletsMagazine += BulletsReserve;
            BulletsReserve = 0;
        }
        GameAssets.g.PlayerCombat.UpdateUI();
        AudioManager.g.Play("Reload End");
    }

    public void SetAmmo(int MagazineAmmo, int ReserveAmmo)
    {
        if (MagazineAmmo <= MaxAmmoMag[IsUpgraded] && MagazineAmmo >= 0) { BulletsMagazine = MagazineAmmo; }
        else { Debug.LogWarning("Magazine bullet input out of range"); }
        if (ReserveAmmo <= MaxAmmoReserve[IsUpgraded] && ReserveAmmo >= 0) { BulletsReserve = ReserveAmmo; }
        else { Debug.LogWarning("Reserve bullet input out of range"); }
    }

    public void SetMaxAmmo()
    {
        SetAmmo(MaxAmmoMag[IsUpgraded], MaxAmmoReserve[IsUpgraded]);
    }

    public bool IsAtMaxAmmo()
    {
        return BulletsReserve == MaxAmmoReserve[IsUpgraded] && BulletsMagazine == MaxAmmoMag[IsUpgraded];
    }

    #endregion

    #region Upgrade Methods

    private void UpdateIsUpgraded()
    {
        if (UpgradeLevel == 0)
        {
            IsUpgraded = 0;
        }
        else if (UpgradeLevel >= 1 && UpgradeLevel <= 3)
        {
            IsUpgraded = 1;
        }
        else
        {
            Debug.LogError($"Upgrade level {UpgradeLevel} out of range");
        }

    }

    public void Upgrade()
    {
        UpgradeLevel += 1;
        UpdateIsUpgraded();
        SetMaxAmmo();
        ColorLocation.color = UpgradeColor;
    }

    #endregion
}
