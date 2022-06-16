using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;

public abstract class EffectBASE : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] protected List<Character.ECharacterType> charactersToHit = new List<Character.ECharacterType>() { Character.ECharacterType.Player };
    [Tooltip("Can hit also props")] [SerializeField] protected bool hitAlsoNotCharacters = true;
    [Tooltip("Can hit owner")] [SerializeField] protected bool hitAlsoWhoCreatedThis = false;
    [Tooltip("Can hit same type of enemy (if owner is an enemy)")] [SerializeField] protected bool hitAlsoEnemiesOfSameType = false;

    [Header("Timer Disappear (0 = no disappear)")]
    [SerializeField] protected float duration = 2;

    //events
    public System.Action onDisappear { get; set; }

    protected Character owner;

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="owner"></param>
    public virtual void Init(Character owner)
    {
        this.owner = owner;

        //disappear coroutine
        if (duration > 0)
        {
            StartCoroutine(DisappearCoroutine());
        }
    }

    IEnumerator DisappearCoroutine()
    {
        //wait
        yield return new WaitForSeconds(duration);

        //call event
        onDisappear?.Invoke();

        //and destroy
        redd096.Pooling.Destroy(gameObject);
    }
}
