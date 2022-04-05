using UnityEngine;
using redd096.GameTopDown2D;

public class PerksComponent : MonoBehaviour
{
    [Header("DEBUG")]
    public PerkData EquippedPerk = default;

    //events
    public System.Action<bool> onUsePerk { get; set; }

    Character owner;

    void Awake()
    {
        owner = GetComponent<Character>();
        if (owner == null) Debug.LogWarning("Miss Character on " + name);
    }

    public void UsePerk()
    {
        bool usedPerk = false;

        //use equipped perk
        if (EquippedPerk)
            usedPerk = EquippedPerk.UsePerk(owner);

        //call event
        onUsePerk?.Invoke(usedPerk);
    }

    /// <summary>
    /// Add perk. Replace previous
    /// </summary>
    /// <param name="perk"></param>
    public void AddPerk(PerkData perk)
    {
        EquippedPerk = perk;

        //initialize perk
        if (EquippedPerk)
            EquippedPerk.Init();
    }
}
