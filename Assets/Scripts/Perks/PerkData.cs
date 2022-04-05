using UnityEngine;
using redd096.Attributes;
using redd096.GameTopDown2D;

public abstract class PerkData : ScriptableObject, ISellable
{
    [Header("Perk Data")]
    public string PerkName = "Perk Name";
    public int PerkPrice = 10;
    [ShowAssetPreview] public Sprite PerkSprite = default;

    public string SellName => PerkName;
    public int SellPrice => PerkPrice;
    public Sprite SellSprite => PerkSprite;

    public abstract void Init();
    public abstract bool UsePerk(Character owner);
}
