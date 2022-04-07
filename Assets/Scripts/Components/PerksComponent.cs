using UnityEngine;
using redd096.GameTopDown2D;

public class PerksComponent : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] PerkData equippedPerk = default;
    public PerkData EquippedPerk => equippedPerk;

    //events
    public System.Action<bool> onUsePerk { get; set; }

    Character owner;

    void Awake()
    {
        owner = GetComponent<Character>();
        if (owner == null) Debug.LogWarning("Miss Character on " + name);
    }

    /// <summary>
    /// Use equipped perk
    /// </summary>
    public void UsePerk()
    {
        bool usedPerk = false;

        //use equipped perk
        if (equippedPerk)
            usedPerk = equippedPerk.UsePerk();

        //call event
        onUsePerk?.Invoke(usedPerk);
    }

    /// <summary>
    /// Add perk. Remove previous if necessary
    /// </summary>
    /// <param name="perk"></param>
    public void AddPerk(PerkData perk)
    {
        if (perk == null)
            return;

        //remove previous perk if there is one
        if (equippedPerk != null)
            RemovePerk(equippedPerk);

        //equip new perk
        equippedPerk = perk;
        if (equippedPerk) equippedPerk.Equip(owner);
    }

    /// <summary>
    /// Remove perk if equipped
    /// </summary>
    /// <param name="perk"></param>
    public void RemovePerk(PerkData perk)
    {
        //check if perk is equipped
        if (equippedPerk != null && equippedPerk == perk)
        {
            //unequip it
            equippedPerk = null;
            perk.Unequip();
        }
    }

    /// <summary>
    /// Set equipped perk. Normally is better to use AddPerk.
    /// </summary>
    /// <param name="perk"></param>
    public void ForceSetPerk(PerkData perk)
    {
        equippedPerk = perk;
    }
}
