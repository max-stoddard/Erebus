using UnityEngine;
public class HealthSystem : MonoBehaviour
{
    [SerializeField] protected float maxHealth; // The maximum health of the entity
    // OBJECTIVE 12.1

    protected float Health { get; set; } // The current health of the entity


    protected virtual void Start()
    {
        Health = maxHealth;
    }

    public virtual void Damage(float Damage) // When an enemy is damaged; when Damage input is postiive health is taken away from entity
    {
        Health -= Damage;
        if (Health < 0f)
        {
            Health = 0f;
        }
        DamagePopup.Create(gameObject.transform.position, (int)Damage);
    }

    public virtual void Heal(float Heal) // Increment health by heal
    {
        Health += Heal;
        if (Health > maxHealth)
        {
            Health = maxHealth;
        }
    }

    public virtual void SetMaxHealth(int NewMaxHealth)
    {
        if (NewMaxHealth > 0)
        {
            maxHealth = NewMaxHealth;
        }
        else
        {
            Debug.LogError($"Trying to set MaxHealth to {NewMaxHealth}");
        }
    }

    public float GetHealthPercent()
    {
        return (Health / maxHealth);
    }
}
