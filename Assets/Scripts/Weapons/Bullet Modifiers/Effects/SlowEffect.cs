using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096.Attributes;

[AddComponentMenu("redd096/Weapons/Bullet Modifiers/Effects/Slow Effect")]
public class SlowEffect : EffectAreaBASE
{
    struct DefaultValuesHitStruct
    {
        public float MaxSpeed;
        public float Drag;

        public DefaultValuesHitStruct(float MaxSpeed, float Drag)
        {
            this.MaxSpeed = MaxSpeed;
            this.Drag = Drag;
        }
    }

    [Header("Slow")]
    [SerializeField] bool setMaxSpeed = true;
    [EnableIf("setMaxSpeed")] [SerializeField] float newMaxSpeed = 1;
    [SerializeField] bool setDrag = false;
    [EnableIf("setDrag")] [SerializeField] float newDrag = 1;
    [SerializeField] float durationEffect = 0.1f;

    //set at 0 to restart coroutine at every trigger stay
    protected override float delayOnTriggerStay => 0;

    //dictionary
    Dictionary<Redd096Main, Coroutine> removeEffectCoroutines = new Dictionary<Redd096Main, Coroutine>();
    Dictionary<Redd096Main, DefaultValuesHitStruct> defaultValues = new Dictionary<Redd096Main, DefaultValuesHitStruct>();

    protected override void OnDisable()
    {
        base.OnDisable();

        //reset speeds
        foreach (Redd096Main hit in defaultValues.Keys)
        {
            if (hit && hit.GetSavedComponent<MovementComponent>())
            {
                if (setMaxSpeed) hit.GetSavedComponent<MovementComponent>().SetMaxSpeed(defaultValues[hit].MaxSpeed);
                if (setDrag) hit.GetSavedComponent<MovementComponent>().SetDrag(defaultValues[hit].Drag);
            }
        }

        //reset vars
        removeEffectCoroutines.Clear();
        defaultValues.Clear();
    }

    protected override void OnHit(Collider2D collision, Redd096Main hit)
    {
        //if there is no hit, do nothing
        if (hit == null)
            return;

        //else, who get hit will start coroutine (so if this puddle will be destroyed, coroutine will not stop)
        if (removeEffectCoroutines.ContainsKey(hit))
        {
            StopCoroutine(removeEffectCoroutines[hit]);
            removeEffectCoroutines.Remove(hit);
        }

        removeEffectCoroutines.Add(hit, StartCoroutine(RemoveEffectCoroutine(hit)));
    }

    IEnumerator RemoveEffectCoroutine(Redd096Main hit)
    {
        //set default values (if not already setted)
        if (defaultValues.ContainsKey(hit) == false)
        {
            if (hit && hit.GetSavedComponent<MovementComponent>())
                defaultValues.Add(hit, new DefaultValuesHitStruct(hit.GetSavedComponent<MovementComponent>().MaxSpeed, hit.GetSavedComponent<MovementComponent>().Drag));
        }

        //set max speed and/or drag
        if (hit && hit.GetSavedComponent<MovementComponent>())
        {
            if (setMaxSpeed)
                hit.GetSavedComponent<MovementComponent>().SetMaxSpeed(newMaxSpeed);
            if (setDrag)
                hit.GetSavedComponent<MovementComponent>().SetDrag(newDrag);
        }

        //wait
        yield return new WaitForSeconds(durationEffect);

        //reset max speed and/or drag
        if (hit && hit.GetSavedComponent<MovementComponent>())
        {
            if (defaultValues.ContainsKey(hit))
            {
                if (setMaxSpeed)
                    hit.GetSavedComponent<MovementComponent>().SetMaxSpeed(defaultValues[hit].MaxSpeed);
                if (setDrag)
                    hit.GetSavedComponent<MovementComponent>().SetDrag(defaultValues[hit].Drag);
            }
        }
    }
}
