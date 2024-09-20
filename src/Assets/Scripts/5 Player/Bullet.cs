using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int Damage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject effect;
        if (collision.collider.gameObject.CompareTag("Enemy"))
        {
            collision.collider.gameObject.GetComponent<EnemyCombat>().Damage(Damage); // OBJECTIVE 23
            effect = Instantiate(GameAssets.g.EnemyHitEffect, transform.position, Quaternion.identity);
        }
        else
        {
            effect = Instantiate(GameAssets.g.ObjectHitEffect, transform.position, Quaternion.identity);
        }
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.startLifetime.constant);
        Destroy(gameObject);
    }

    public void ChangeDamage(int Damage)
    {
        if (Damage <= 0)
        {
            Debug.LogWarning($"Damage too low: {Damage}");
        }
        else
        {
            this.Damage = Damage;
        }
    }
}
