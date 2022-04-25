using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;

[CreateAssetMenu(menuName = "Zombie Punk/Slow-mo Perk")]
public class SlowMoPerk : PerkData
{
    [Header("Slow motion")]
    [SerializeField] float timeScale = 0.2f;
    [SerializeField] float delay = 1;
    [SerializeField] float duration = 0.2f;

    float cooldownTime;
    Coroutine durationCoroutine;

    public override float GetPerkDeltaCooldown() => 1 - (cooldownTime - Time.time) / delay;

    public override void Equip(Redd096Main owner)
    {
        //reset vars (because in scriptable object will remain saved also if not serialized)
        cooldownTime = 0;

        //set owner
        base.Equip(owner);
    }

    public override void Unequip()
    {
        //reset vars (because in scriptable object will remain saved also if not serialized)
        cooldownTime = 0;
        if (durationCoroutine != null && owner) owner.StopCoroutine(durationCoroutine);

        //remove owner
        base.Unequip();
    }

    public override bool UsePerk()
    {
        if (owner == null)
            return false;

        //check cooldown
        if (Time.time > cooldownTime)
        {
            cooldownTime = Time.time + delay;

            //start coroutine
            if (durationCoroutine != null) owner.StopCoroutine(durationCoroutine);
            durationCoroutine = owner.StartCoroutine(DurationCoroutine());

            return true;
        }

        return false;
    }

    IEnumerator DurationCoroutine()
    {
        //set timeScale
        float previousTimeScale = Time.timeScale;
        Time.timeScale = timeScale;

        //wait
        float time = Time.realtimeSinceStartup + duration;      //use realtime because timeScale is lower
        while (Time.realtimeSinceStartup < time)
            yield return null;

        //restore previous timeScale
        Time.timeScale = previousTimeScale;
    }
}
