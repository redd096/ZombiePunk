using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Weapon")]
[Description("Attack for few seconds using WeaponComponent")]
public class AttackWithTimer : ActionTask<WeaponComponent>
{
    public BBParameter<float> durationAttack = 1;
    public bool repeat;

    float timer;
    bool attacking;

    protected override void OnExecute()
    {
        timer = Time.time + durationAttack.value;
        attacking = false;
    }

    protected override void OnStop()
    {
        //be sure to stop attack
        agent.CurrentWeapon?.ReleaseAttack();
    }

    protected override void OnUpdate()
    {
        //when press, attack
        if (timer > Time.time && attacking == false)
        {
            attacking = true;
            agent.CurrentWeapon?.PressAttack();
        }
        //on release, stop it if automatic shoot
        else if (timer <= Time.time && attacking)
        {
            attacking = false;
            agent.CurrentWeapon?.ReleaseAttack();
        }

        //end action if necessary
        if(timer <= Time.time)
            if (!repeat) { EndAction(); }
    }
}
