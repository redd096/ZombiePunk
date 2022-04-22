using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Weapons/Bullet Modifiers/Puddle Bullet Modifier")]
public class PuddleBulletModifier : MonoBehaviour
{
    [Header("Necessary Components - default get from this gameObject")]
    [SerializeField] Bullet bullet = default;

    [Header("Puddle to spawn")]
    [SerializeField] EffectAreaBASE puddle = default;

    Pooling<EffectAreaBASE> poolingPuddle = new Pooling<EffectAreaBASE>();

    void OnEnable()
    {
        //get references
        if (bullet == null) bullet = GetComponent<Bullet>();

        //add events
        if (bullet)
        {
            bullet.onDie += OnDie;
        }
    }

    void OnDisable()
    {
        //remove events
        if (bullet)
        {
            bullet.onDie -= OnDie;
        }
    }

    void OnDie()
    {
        //spawn puddle at position
        if (puddle)
        {
            EffectAreaBASE puddleInstantiated = poolingPuddle.Instantiate(puddle, transform.position, Quaternion.identity);
            puddleInstantiated.Init(bullet.Owner);
        }
    }
}
