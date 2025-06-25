using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 3;
    private int CurrentHealth;

    public HealthUI HealthUI;
    private SpriteRenderer mSpriteRenderer;

    public static event Action OnPlayerDied;

    // Start is called before the first frame update
    void Start()
    {
        ResetHealth();

        mSpriteRenderer = GetComponent<SpriteRenderer>();
        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(enemy.Damage);
        }

        Trap trap = collision.GetComponent<Trap>();
        if (trap && trap.Damage > 0)
        {
            TakeDamage(trap.Damage);
        }
    }

    private void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth >MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        HealthUI.UpdateHearts(CurrentHealth);
    }

    private void TakeDamage (int damage)
    {
        CurrentHealth -= damage;
        HealthUI.UpdateHearts(CurrentHealth);

        StartCoroutine(FlashRed());

        if (CurrentHealth <= 0)
        {
            OnPlayerDied.Invoke();
        }
    }

    private void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        HealthUI.SetMaxHearts(CurrentHealth);
    }

    private IEnumerator FlashRed()
    {
        mSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        mSpriteRenderer.color = Color.white;
    }
}
