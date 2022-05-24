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
    public RandomAttackStruct[] RandomAttacks = default;
    [ReadOnly] public int SelectedAttack;

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

        //TODO
        //V entra e seleziona il player (FindPlayer State)
        //V scegli un attacco random tra quelli disponibili (in base alla vita)
        //- una volta selezionato l'attacco, questo avrà la metà della possibilità di essere scelto al prossimo giro
        //
        //Attacco 1
        //- si muove al centro dell'arena e sta fermo, mirando al player con tanto di linea per mostrare dove sta mirando
        //- carica come il charger, verso il giocatore e danneggiando anche gli altri zombie
        //- quando sbatte, si ferma per tot secondi
        //- poi si sceglie un altro ATTACCO RANDOM
        //
        //Attacco 2
        //- si muove verso un punto random, su un lato dell'arena (carica finché non arriva al muro)
        //- una volta raggiunto, calcolerà un altro punto random, su un altro lato dell'arena (deve caricare, senza sbattere contro il muro dove già si trova)
        //- danneggia tutto ciò che colpisce
        //- dopo aver raggiunto tot punti (aver fatto tot caricate, decise da inspector), calcolerà il prossimo ATTACCO RANDOM
        //
        //Attacco 3
        //- si muove al centro dell'arena e sta fermo, iniziando ad urlare
        //- questo farà spawnare vari zombie (lista di prefab con % e numero min-max?)
        //- una volta spawnati, inizierà a chaseare il player come i follower zombie
        //- ma quando arriva a una certa distanza dal player, si ferma e carica come un charger
        //- dopo tot secondi che continua a inseguire il player, cerca un altro ATTACCO RANDOM
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
                if (healthComponent.CurrentHealth / healthComponent.MaxHealth <= RandomAttacks[i].NecessaryHealthPercentage)
                    possibleAttacksIndex.Add(i);
            }

            //selected random attack
            if (possibleAttacksIndex.Count > 0)
                SelectedAttack = possibleAttacksIndex[Random.Range(0, possibleAttacksIndex.Count)];
        }

        //complete task
        CompleteTask();
    }
}
