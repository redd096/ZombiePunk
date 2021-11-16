using UnityEngine;
using Sirenix.OdinInspector;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    public bool Invincible = false;
    public float CurrentHealth = 100;

    [Header("DEBUG")]
    [ReadOnly] public float MaxHealth = 100;
    [ReadOnly] [ShowInInspector] bool alreadyDead = false;

    //events
    public System.Action onGetDamage { get; set; }
    public System.Action onDie { get; set; }
    public System.Action onGetHealth { get; set; }

    void OnValidate()
    {
        //set max health
        MaxHealth = CurrentHealth;
    }

    /// <summary>
    /// Get damage and check if dead
    /// </summary>
    /// <param name="damage"></param>
    public void GetDamage(float damage)
    {
        if (alreadyDead)
            return;

        //set health, only if not invincible
        if(Invincible == false)
            CurrentHealth -= damage;

        //call event
        onGetDamage?.Invoke();

        //check if dead
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Call it when health reach 0
    /// </summary>
    public void Die()
    {
        if (alreadyDead)
            return;

        alreadyDead = true;

        //call event
        onDie?.Invoke();

        //destroy object
        Destroy(gameObject);
    }

    /// <summary>
    /// Get health and clamp to max health
    /// </summary>
    /// <param name="healthGiven"></param>
    /// <param name="clampMaxHealth">Clamp or can exceed max health?</param>
    public void GetHealth(float healthGiven, bool clampMaxHealth = true)
    {
        if (alreadyDead)
            return;

        //add health
        CurrentHealth += healthGiven;

        //clamp to max health if necessary
        if (clampMaxHealth && CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;

        //call event
        onGetHealth?.Invoke();
    }
}
