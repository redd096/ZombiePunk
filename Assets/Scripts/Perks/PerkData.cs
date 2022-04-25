using UnityEngine;
using redd096.Attributes;
using redd096.GameTopDown2D;
using redd096;

public abstract class PerkData : ScriptableObject, ISellable
{
    [Header("Perk Data")]
    public string PerkName = "Perk Name";
    public int PerkPrice = 10;
    [ShowAssetPreview] public Sprite PerkSprite = default;
    [Tooltip("Used by UIManager")] [ShowAssetPreview] public Sprite PerkBackgroundSprite = default;

    [Header("Feedbacks on use")]
    public FeedbackStructRedd096 Feedback = default;
    public CameraShakeStruct CameraShake = default;
    public GamepadVibrationStruct GamepadVibration = default;

    [Header("Animation on use")]
    public bool UseAnimation = false;
    [EnableIf("UseAnimation")] public string AnimatorTrigger = "";

    //ISellable
    public string SellName => PerkName;
    public int SellPrice => PerkPrice;
    public Sprite SellSprite => PerkSprite;

    protected Redd096Main owner;

    public virtual void Equip(Redd096Main owner) { this.owner = owner; }
    public virtual void Unequip() { owner = null; }
    public abstract bool UsePerk();

    /// <summary>
    /// Used by UIManager
    /// </summary>
    /// <returns></returns>
    public abstract float GetPerkDeltaCooldown();
}
