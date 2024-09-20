using UnityEngine;

public class EnemyCombat : HealthSystem
{
    [Header("Health Bar")]

    private GameObject HealthBar;

    [Header("Attacking")]


    [SerializeField] private float damageDealt;

    // Time taken before the enemy actually hits a player whilst player is in vicinity of enemy
    [SerializeField] private float TimeBeforeAttack;
    private float CurrentTimeBeforeAttack;

    // Time between attacks goes to positive value after attack
    [SerializeField] private float TimeBetweenAttacks;
    private float CurrentTimeBetweenAttacks;

    [SerializeField] private float AttackRange;
    [SerializeField] private Transform AttackPoint;

    [Header("Death")]
    bool Dying = false;

    [Header("Player")]

    private bool PlayerInRange;
    [SerializeField] LayerMask PlayerLayer;

    [Header("Money")]
    [SerializeField] private int MoneyPerHit;
    [SerializeField] private int MoneyPerKill;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private int AttackTurn = 0;


    protected override void Start()
    {
        base.Start();
        SetupHealthBar();
    }

    private void Update()
    {
        if (CurrentTimeBetweenAttacks > 0) { CurrentTimeBetweenAttacks -= Time.deltaTime; } // OBJECTIVE 17.1
        else { CurrentTimeBetweenAttacks = 0; }
        PlayerInRange = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, PlayerLayer).Length > 0; // OBJECTIVE 17
        CheckAttackPlayer();
    }

    public void InitiliaseProperties(float Damage, float TimeBeforeAttacks, float TimeBetweenAttacks, int _maxHealth)
    {
        if (Damage > 0 && TimeBeforeAttack > 0 && TimeBetweenAttacks > 0 && _maxHealth > 0)
        {
            this.damageDealt = Damage;
            this.TimeBeforeAttack = TimeBeforeAttacks;
            this.TimeBetweenAttacks = TimeBetweenAttacks;
            SetMaxHealth(_maxHealth);
        }
        else
        {
            Debug.LogError($"Properties incorrect: damage: {Damage}, TimeBeforeAttack: {TimeBeforeAttack}, TimeBetweenAttacks: {TimeBetweenAttacks}, MaxHealth: {_maxHealth}");
        }
    }

    private void CheckAttackPlayer()
    {
        if (!PlayerInRange) { CurrentTimeBeforeAttack = TimeBeforeAttack; }
        if (CurrentTimeBetweenAttacks <= 0 && PlayerInRange && !Dying)
        {
            if (CurrentTimeBeforeAttack > 0) { CurrentTimeBeforeAttack -= Time.deltaTime; }
            else { AttackPlayer(); }
        }
    }

    private void AttackPlayer()
    {
        CurrentTimeBetweenAttacks = TimeBetweenAttacks;


        if (AttackTurn == 0)
        {
            animator.SetTrigger("AttackRight");
            AttackTurn = 1;
        }
        else if (AttackTurn == 1)
        {
            animator.SetTrigger("AttackLeft");
            AttackTurn = 0;
        }
        else
        {
            Debug.LogWarning($"Invalid AttackTurn: {AttackTurn}");
        }
        GameAssets.g.PlayerHealthSystem.Damage(damageDealt);
    }

    public override void Damage(float Damage)
    {
        base.Damage(Damage);
        GameAssets.g.PlayerMoneySystem.ChangeMoney(MoneyPerHit); // OBJECTIVE 21.1
        if (Health <= 0f)
        {
            Health = 0f;
            Die();
        }

        HealthBar.GetComponent<HealthBar>().Activate(Health, maxHealth);
        AudioManager.g.Play("Enemy hit");
    }

    private void Die()
    {
        if (RoundManager.g != null)
        {
            RoundManager.g.RemoveEnemy(gameObject);
        }

        gameObject.GetComponent<EnemyFollow>().Die();
        Dying = true;
        Destroy(HealthBar);
        Destroy(gameObject.GetComponent<Collider2D>());
        GameAssets.g.PlayerMoneySystem.ChangeMoney(MoneyPerKill); // OBJECTIVE 21.2

        animator.SetTrigger("Death");
        Destroy(gameObject, 3f);
    }

    private void SetupHealthBar()
    {
        HealthBar = Instantiate(GameAssets.g.HealthBar, gameObject.transform);
        HealthBar.GetComponent<HealthBar>().SetAttachedTo(gameObject.transform);
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint == null) return;
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }
}
