using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Weapon")]
[Description("CheckAttack using WeaponComponent")]
public class AttackByWeaponComponent : ActionTask<WeaponComponent>
{
    [BlackboardOnly] public BBParameter<bool> inputAttack;
    public bool repeat;

    bool attacking;

    protected override void OnUpdate()
    {
        //when press, attack
        if (inputAttack.value && attacking == false)
        {
            attacking = true;
            agent.CurrentWeapon?.PressAttack();
        }
        //on release, stop it if automatic shoot
        else if (inputAttack.value == false && attacking)
        {
            attacking = false;
            agent.CurrentWeapon?.ReleaseAttack();
        }

        //end action if necessary
        if (!repeat) { EndAction(); }
    }
}
