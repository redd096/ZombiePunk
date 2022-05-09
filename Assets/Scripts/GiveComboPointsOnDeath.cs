using UnityEngine;
using redd096.GameTopDown2D;

public class GiveComboPointsOnDeath : MonoBehaviour
{
    [Header("Necessary components - default get from parent")]
    [SerializeField] HealthComponent healthComponent = default;

    [Header("Combo")]
    [SerializeField] int pointsToGive = 10;

    private void OnEnable()
    {
        //get references
        if (healthComponent == null) healthComponent = GetComponentInParent<HealthComponent>();

        //add events
        if (healthComponent)
            healthComponent.onDie += OnDie;
    }

    private void OnDisable()
    {
        //remove events
        if (healthComponent)
            healthComponent.onDie -= OnDie;
    }

    private void OnDie(HealthComponent obj, Character whoHit)
    {
        //add combo points on die
        if (whoHit && whoHit.GetSavedComponent<ComboComponent>())
            whoHit.GetSavedComponent<ComboComponent>().AddCombo(pointsToGive);
    }
}
