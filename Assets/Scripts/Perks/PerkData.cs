using UnityEngine;
using redd096.Attributes;
using redd096.GameTopDown2D;

public abstract class PerkData : ScriptableObject, ISellable
{
    [Header("Perk Data")]
    public string PerkName = "Perk Name";
    public int PerkPrice = 10;
    [ShowAssetPreview] public Sprite PerkSprite = default;

    //ISellable
    public string SellName => PerkName;
    public int SellPrice => PerkPrice;
    public Sprite SellSprite => PerkSprite;

    protected Redd096Main owner;

    public virtual void Equip(Redd096Main owner) { this.owner = owner; }
    public virtual void Unequip() { owner = null; }
    public abstract bool UsePerk();
}
