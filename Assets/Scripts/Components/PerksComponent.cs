using UnityEngine;
using redd096.GameTopDown2D;

public class PerksComponent : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] PerkData equippedPerk = default;
    public PerkData EquippedPerk => equippedPerk;

    //events
    public System.Action<bool> onUsePerk { get; set; }
    public System.Action<PerkData> onEquipPerk { get; set; }
    public System.Action<PerkData> onUnequipPerk { get; set; }

    Character owner;

    void Awake()
    {
        if (owner == null) owner = GetComponent<Character>();
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

        //call UIManager when use perk
        if (usedPerk)
        {
            if (GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.SetPerkUsed(equippedPerk);
        }
    }

    /// <summary>
    /// Add perk. Remove previous if necessary
    /// </summary>
    /// <param name="perk"></param>
    public void AddPerk(PerkData perk)
    {
        if (perk == null)
            return;

        //in case is still not setted (for example when GameManager call it)
        if (owner == null) owner = GetComponent<Character>();

        //remove previous perk if there is one
        if (equippedPerk != null)
            RemovePerk(equippedPerk);

        //equip new perk
        equippedPerk = perk;
        if (equippedPerk) equippedPerk.Equip(owner);

        //call event
        onEquipPerk?.Invoke(equippedPerk);

        //set UIManager
        if (GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetPerkImage(equippedPerk);
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

            //call event
            onUnequipPerk?.Invoke(perk);

            //set UIManager
            if (GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.SetPerkImage(equippedPerk);
        }
    }
}
