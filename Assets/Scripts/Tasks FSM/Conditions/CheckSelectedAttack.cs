using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.Attributes;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Selected Attack")]
public class CheckSelectedAttack : ConditionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] SelectRandomAttack selectRandomAttack = default;

    [Header("Check if selected this attack")]
    [Dropdown("GetAttackNames")] [SerializeField] string attackName = "";

    #region editor
#if UNITY_EDITOR

    string[] GetAttackNames()
    {
        //find every attack name
        if (selectRandomAttack && selectRandomAttack.RandomAttacks != null)
        {
            List<string> possibleAttacks = new List<string>();
            for (int i = 0; i < selectRandomAttack.RandomAttacks.Length; i++)
                possibleAttacks.Add(selectRandomAttack.RandomAttacks[i].AttackName);

            return possibleAttacks.ToArray();
        }

        return new string[1] { "BE SURE SelectRandomAttack ISN'T NULL" };
    }

#endif
    #endregion

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (selectRandomAttack == null) selectRandomAttack = GetStateMachineComponent<SelectRandomAttack>();
    }

    public override bool OnCheckTask()
    {
        //return if selected this attack
        return SelectAttackByName();
    }

    bool SelectAttackByName()
    {
        //if there is the component
        if (selectRandomAttack && selectRandomAttack.RandomAttacks != null)
        {
            if (selectRandomAttack.SelectedAttack < selectRandomAttack.RandomAttacks.Length)
            {
                //check selected attack name
                return selectRandomAttack.RandomAttacks[selectRandomAttack.SelectedAttack].AttackName == attackName;
            }
        }

        return false;
    }
}
