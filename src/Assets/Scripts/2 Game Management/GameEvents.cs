using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents g { get; private set; }

    private void Awake()
    {
        g = this;

    }


    public event Action OnPlayerHealthChange;
    public void PlayerHealthChanged()
    {
        OnPlayerHealthChange?.Invoke();
    }

    public event Action OnGunChanged;
    public void GunChanged()
    {
        OnGunChanged?.Invoke();
    }

}
