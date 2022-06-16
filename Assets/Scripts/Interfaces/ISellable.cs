using UnityEngine;

public interface ISellable
{
    string SellName { get; }
    int SellPrice { get; }
    Sprite SellSprite { get; }
}
