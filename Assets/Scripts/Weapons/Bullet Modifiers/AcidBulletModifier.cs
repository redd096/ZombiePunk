using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Weapons/Bullet Modifiers/Acid Bullet Modifier")]
public class AcidBulletModifier : MonoBehaviour
{
    [Header("Necessary Components - default get from this gameObject")]
    [SerializeField] Bullet bullet = default;

    [Header("Acid to spawn")]
    [SerializeField] AcidEffect acid = default;

    Pooling<AcidEffect> poolingAcid = new Pooling<AcidEffect>();

    void OnEnable()
    {
        //get references
        if (bullet == null) bullet = GetComponent<Bullet>();

        //add events
        if(bullet)
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
        //spawn acid at position
        if (acid)
        {
            AcidEffect acidInstantiated = poolingAcid.Instantiate(acid, transform.position, Quaternion.identity);
            acidInstantiated.Init(bullet.Owner);
        }
    }
}
