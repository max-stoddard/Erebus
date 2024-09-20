using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*

Scipt made by Max Stoddard
Script deals with:
- Shooting gun
- Organising guns in inventory
- Gun UI
- 

Slot(UI)    Index(for all code)
1           0
2           1
3           2
4           3
5           4

*/
public class PlayerCombat : MonoBehaviour
{
    [Header("Guns")]
    // Current gun
    private int CurrentGunSlot; // The int value of the gun
    private Gun CurrentGunScript;
    // Starting
    [SerializeField] private int StartingGunID; // OBJECTIVE 25

    // All guns in inventory
    private GameObject[] GunSlots; // All the gun gameobjects
    [SerializeField] private Transform GunsFolder; // Folder where gun gameobjects are stored

    private int MaxNoGuns;

    [SerializeField] private Color GunNotSelectedColor, GunSelectedColor;


    [Header("UI")]

    [SerializeField] private Text UIGunName;
    [SerializeField] private Text UIMagAmmo, UIResAmmo;
    private UIMethods UI;


    [Header("All Guns")]

    public List<GameObject> AllGuns;

    [Header("Powerups")]

    private bool ReloadSpeed, Firerate, Accuracy;


    #region Initialiase Methods

    private void Awake()
    {
        UI = GameAssets.g.UIMethods;
        CheckAllGunsValid();
        InitialiseProperties();
        InitialiseStartingGun();
    }

    private void Start()
    {

        InitialiseInput();
        UpdateUI();

    }

    private void InitialiseInput()
    {
        // Enables player input map
        GameAssets.g.PlayerInputActions.Player.Shoot.Enable();
        GameAssets.g.PlayerInputActions.Player.Shoot.started += ShootInput;
        GameAssets.g.PlayerInputActions.Player.Shoot.canceled += ShootInput;

        GameAssets.g.PlayerInputActions.Player.WeaponSwap.Enable();
        GameAssets.g.PlayerInputActions.Player.WeaponSwap.performed += WeaponSwapInput;

        GameAssets.g.PlayerInputActions.Player.Reload.Enable();
        GameAssets.g.PlayerInputActions.Player.Reload.performed += ReloadInput;
    } //Initalises/enables all of the input required for player combat

    private void InitialiseProperties()
    {
        MaxNoGuns = 5;

        GunSlots = new GameObject[MaxNoGuns]; // Creates 4 spaces for weapons all null
        CurrentGunSlot = 0;
        ReloadSpeed = Firerate = Accuracy = false;
    }

    public void DisableInput()
    {
        GameAssets.g.PlayerInputActions.Player.Shoot.started -= ShootInput;
        GameAssets.g.PlayerInputActions.Player.Shoot.canceled -= ShootInput;
        GameAssets.g.PlayerInputActions.Player.Reload.performed -= ReloadInput;
        GameAssets.g.PlayerInputActions.Player.WeaponSwap.performed -= WeaponSwapInput;

        GameAssets.g.PlayerInputActions.Player.Shoot.Disable();
        GameAssets.g.PlayerInputActions.Player.WeaponSwap.Disable();
        GameAssets.g.PlayerInputActions.Player.Reload.Disable();
    }

    #endregion


    #region UI Methods

    public void UpdateUI()
    {
        for (int i = 0; i < MaxNoGuns; i++)
        {
            if (GunSlots[i] == null)
            {
                UI.ChangeGunInfoEmpty(i);
            }
            else
            {
                Gun gunscript = GunSlots[i].GetComponent<Gun>();
                UI.ChangeGunInfo(i, gunscript.GunImage, gunscript.Name());
                if (CurrentGunSlot == i)
                {
                    UI.ChangeSlotColor(GunSelectedColor, i);
                    UI.ChangeGunName(gunscript.Name());
                    UI.ChangeMainAmmoText(gunscript.BulletsMagazine);
                    UI.ChangeResAmmoText(gunscript.BulletsReserve);
                }
                else
                {
                    UI.ChangeSlotColor(GunNotSelectedColor, i);
                }
            }
        }
    }

    #endregion


    #region Gun Methods

    private void CheckAllGunsValid()
    {
        if (AllGuns != null)
        {
            if (AllGuns.GroupBy(g => g.GetComponent<Gun>().ID()).Where(x => x.Count() > 1).Count() > 0) // If there is a group of guns such that they have the same ID
            {
                Debug.LogError("Gun IDs not unique");
            }
        }
        else
        {
            Debug.LogError("Guns list null");
        }
    }

    private void InitialiseStartingGun() // OBJECTIVE 25
    {
        AddGunToSlot(StartingGunID, CurrentGunSlot);
    }

    public void AddGun(int GunID)
    {
        int SlotToFill;
        if (FindSpaceInInventory() == -1)
        {
            SlotToFill = CurrentGunSlot;
        }
        else
        {
            SlotToFill = FindSpaceInInventory();
        }
        AddGunToSlot(GunID, SlotToFill);
    }

    public void RefillGunAmmo(int slot)
    {
        GunSlots[slot].GetComponent<Gun>().SetMaxAmmo();
        UpdateUI();
    }

    public bool IsGunAmmoFull(int gunID)
    {
        Gun gun = GunSlots[GetSlotFromGunID(gunID)].GetComponent<Gun>();
        return gun.IsAtMaxAmmo();

    }

    private void AddGunToSlot(int GunID, int slot)
    {
        if (!SlotInRange(slot))
        {
            Debug.LogError($"Slot out of range: {slot}");
            return;
        }
        else
        {
            GunSlots[slot] = Instantiate(AllGuns.Where(gun => gun.GetComponent<Gun>().ID() == GunID).ToArray()[0], GunsFolder);
            SwitchWeapon(slot);
        }
    } // Adds a selected gun to a selected slot

    public void UpgradeCurrentGun() // OBJECTIVE 26
    {
        CurrentGunScript.Upgrade();
        UpdateUI();
    }

    private void Update()
    {
        if (!ReloadSpeed && GameAssets.g.PowerupManager.IsPowerupActive(2))
        {
            ReloadSpeed = true;

        }
        if (!Firerate && GameAssets.g.PowerupManager.IsPowerupActive(3))
        {
            Firerate = true;

        }
        if (!Accuracy && GameAssets.g.PowerupManager.IsPowerupActive(4))
        {
            Accuracy = true;
        }

    }

    public Gun GetCurrentGun()
    {
        if (!GunSlots[CurrentGunSlot].TryGetComponent<Gun>(out var g))
        {
            Debug.LogError("Problem finding current gun");
            return null;
        }
        return g;
    }

    #endregion


    #region Inventory Methods

    private int FindSpaceInInventory() // Returns next available space in inventory or -1 if no space
    {
        for (int i = 0; i < MaxNoGuns; i++)
        {
            if (GunSlots[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
    private void SwitchWeapon(int slot)
    {
        if (!SlotInRange(slot) || !IsGunInSlot(slot))
        {
            return;
        }

        GunSlots[CurrentGunSlot].SetActive(false);
        CurrentGunSlot = slot;
        GunSlots[CurrentGunSlot].SetActive(true);
        CurrentGunScript = GunSlots[CurrentGunSlot].GetComponent<Gun>();
        CurrentGunScript.UpdateGunPos();

        UpdateUI();
        GameEvents.g.GunChanged();

    } // Switches to the gun in the slot input

    public bool SlotInRange(int slot)
    {
        return slot >= 0 && slot < MaxNoGuns;
    } // returns true if input slot is within range of all slots

    public bool IsGunInSlot(int slot)
    {
        return GunSlots[slot] != null;
    } // Returns true if slot has a gun in it

    public bool PlayerHasGunID(int GunID)
    {
        if (GunSlots == null)
        {
            Debug.LogError("GunSlots empty");
            return false;
        }
        return GunSlots.Where(g => g != null).Where(g => g.GetComponent<Gun>().ID() == GunID).Count() > 0;
    }

    public Gun GetGunFromID(int GunID)
    {
        return AllGuns.Where(g => g.GetComponent<Gun>().ID() == GunID).First().GetComponent<Gun>(); // Use first as all elements in AllGuns has checked unique ID
    }

    public int GetSlotFromGunID(int GunID)
    {
        for (int i = 0; i < GunSlots.Length; i++)
        {
            if (GunSlots[i].GetComponent<Gun>().ID() == GunID) return i;
        }
        return -1;
    }

    #endregion


    #region Input Methods

    private void WeaponSwapInput(InputAction.CallbackContext context)
    {
        int SwitchTo = CurrentGunSlot;
        if (context.control.valueType == typeof(System.Single))
        {
            // Number buttons
            SwitchTo = Convert.ToInt32(context.control.displayName) - 1; // Example of displayName is "3"
        }
        else if (context.control.valueType == typeof(Vector2))
        {
            // Scroll wheel
            int ScrollDirection = context.ReadValue<Vector2>().y > 0 ? -1 : 1; // If want to scroll up: -1, if down +1 
            do
            {
                SwitchTo += ScrollDirection;
                if (SwitchTo < 0) { SwitchTo += MaxNoGuns; }
                else if (SwitchTo >= MaxNoGuns) { SwitchTo -= MaxNoGuns; }
            }
            while (GunSlots[SwitchTo] == null);
        }
        SwitchWeapon(SwitchTo);

    } // Occurs when there is input to change weapon

    private void ShootInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        if (context.started)
        {
            CurrentGunScript.StartFiring(Firerate, Accuracy);
        }
        else if (context.canceled)
        {
            CurrentGunScript.StopFiring();
        }
    } // Occurs when there is input to shoot

    private void ReloadInput(InputAction.CallbackContext context)
    {
        CurrentGunScript.TryToReload(ReloadSpeed);
    }

    #endregion


}
