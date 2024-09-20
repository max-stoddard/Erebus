using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerHealthSystem : HealthSystem // OBJECTIVE 12
{
    [Header("Death")]

    private bool Dying;

    [SerializeField] private float TimeToDie;


    [Header("Healing")]

    [SerializeField] private float TimeTillHeal;

    private float CurrentTimeTillHeal;

    [SerializeField] private float HealSpeed;


    [Header("Powerup")]

    private bool Powerup;


    new public void Start()
    {
        //UIHealthBar = GameObject.Find("Player Bar Fill").GetComponent<Image>();
        base.Start();
        OnPlayerHealthChanged();
        CurrentTimeTillHeal = TimeTillHeal;
        Powerup = false;
    }

    private void Update()
    {
        if (Dying)
        {
            TimeToDie -= Time.deltaTime;
        }
        if (TimeToDie <= 0)
        {
            Dying = false;
            TimeToDie = 0;
            PlayerDie();
        }

        if (CurrentTimeTillHeal > 0) // OBJECTIVE 12.3.1
        {
            CurrentTimeTillHeal -= Time.deltaTime;
        }
        else // OBJECTIVE 12.3
        {
            CurrentTimeTillHeal = 0;
            if (Health < maxHealth && !Dying)
            {
                Heal(Time.deltaTime * HealSpeed);
            }
        }

        if (!Powerup && GameAssets.g.PowerupManager.IsPowerupActive(1))
        {
            Powerup = true;
            SetMaxHealth(250); // OBJECTIVE 12.1.1
        }

    }

    public override void Damage(float Damage) // OBJECTIVE 12.4
    {
        base.Damage(Damage);
        OnPlayerHealthChanged();
        Instantiate(GameAssets.g.PlayerHitEffect, gameObject.transform.position, Quaternion.identity);
        AudioManager.g.Play("Player hit");
        CurrentTimeTillHeal = TimeTillHeal;
    }

    public void Kill()
    {
        base.Health = 0;
        OnPlayerHealthChanged();
    }

    public override void Heal(float Heal)
    {
        base.Heal(Heal);
        OnPlayerHealthChanged();
    }

    public override void SetMaxHealth(int NewMaxHealth) // OBJECTIVE 12.1.1
    {
        base.SetMaxHealth(NewMaxHealth);
        Health = NewMaxHealth;
        OnPlayerHealthChanged();
    }

    private void OnPlayerHealthChanged()
    {
        GameAssets.g.UIMethods.ChangeHealth((int)Health, GetHealthPercent());
        if (base.Health == 0f)
        {
            PlayerDying();
        }
        GameEvents.g.PlayerHealthChanged();
    }

    private void PlayerDying() // OBJECTIVE 12.2
    {
        Dying = true;
        GameAssets.g.RoundManager.enabled = false;
        GameAssets.g.PlayerCombat.DisableInput();
        GameAssets.g.PlayerCombat.enabled = false;
        GameAssets.g.PlayerMovement.MultiplyMoveSpeed(0.1f);
        GameAssets.g.PlayerAim.Dead = true;
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy").Where(c => c.GetComponent<EnemyFollow>() != null).ToArray();

        foreach (GameObject enemy in Enemies)
        {
            Destroy(enemy);
        }


    }

    private void PlayerDie()
    {
        StartCoroutine(GameAssets.g.GameManager.GameOver());
    }

}
