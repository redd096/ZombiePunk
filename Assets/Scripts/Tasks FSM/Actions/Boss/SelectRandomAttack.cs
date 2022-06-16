using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.Attributes;
using redd096.GameTopDown2D;

[System.Serializable]
public struct RandomAttackStruct
{
    [ReadOnly] public string InspectorString;
    public string AttackName;
    public float NecessaryHealthPercentage;
}

[AddComponentMenu("redd096/Tasks FSM/Action/Boss/Select Random Attack")]
public class SelectRandomAttack : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] HealthComponent healthComponent = default;

    [Header("Random Attacks")]
    [Range(0, 100)] [SerializeField] int percentageDoSameAttackTwoTimes = 50;
    public RandomAttackStruct[] RandomAttacks = default;
    [ReadOnly] public int SelectedAttack = -1;

    void OnValidate()
    {
        //update names in inspector
        if (RandomAttacks != null)
            for (int i = 0; i < RandomAttacks.Length; i++)
                RandomAttacks[i].InspectorString = RandomAttacks[i].AttackName + " when life <= " + RandomAttacks[i].NecessaryHealthPercentage + "%";
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (healthComponent == null) healthComponent = GetStateMachineComponent<HealthComponent>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        if (healthComponent)
        {
            //find possible attacks using necessary health
            List<int> possibleAttacksIndex = new List<int>();
            for (int i = 0; i < RandomAttacks.Length; i++)
            {
                if (healthComponent.CurrentHealth / healthComponent.MaxHealth <= RandomAttacks[i].NecessaryHealthPercentage / 100f)
                {
                    //if it's the previous attack, check percentageDoSameAttackTwoTimes
                    int r = i == SelectedAttack ? Mathf.RoundToInt(Random.value * 100) : 100;
                    if (r >= percentageDoSameAttackTwoTimes)
                        possibleAttacksIndex.Add(i);
                }
            }

            //selected random attack
            if (possibleAttacksIndex.Count > 0)
                SelectedAttack = possibleAttacksIndex[Random.Range(0, possibleAttacksIndex.Count)];
        }

        //complete task
        CompleteTask();
    }
}
