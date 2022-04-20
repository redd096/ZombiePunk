using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

public class TemporaryInvincibleFeedback : FeedbackRedd096<HealthComponent>
{
    [Header("Feedback - default get sprites in children")]
    [SerializeField] Color colorWhenTemporaryInvincible = Color.green;
    [SerializeField] SpriteRenderer[] spritesToColor = default;

    Coroutine temporaryInvincibleCoroutine;
    Dictionary<SpriteRenderer, Color> defaultColors = new Dictionary<SpriteRenderer, Color>();

    protected override void OnEnable()
    {
        base.OnEnable();

        //get references
        if (spritesToColor == null || spritesToColor.Length <= 0)
            spritesToColor = GetComponentsInChildren<SpriteRenderer>();

        //save default colors
        foreach (SpriteRenderer sprite in spritesToColor)
            defaultColors.Add(sprite, sprite.color);
    }

    protected override void AddEvents()
    {
        owner.onSetTemporaryInvincible += OnSetTemporaryInvincible;
    }

    protected override void RemoveEvents()
    {
        owner.onSetTemporaryInvincible -= OnSetTemporaryInvincible;
    }

    private void OnSetTemporaryInvincible()
    {
        //start coroutine
        if (temporaryInvincibleCoroutine != null)
            StopCoroutine(temporaryInvincibleCoroutine);

        temporaryInvincibleCoroutine = StartCoroutine(TemporaryInvincibleCoroutine());
    }

    IEnumerator TemporaryInvincibleCoroutine()
    {
        //change colors
        foreach (SpriteRenderer sprite in defaultColors.Keys)
            sprite.color = colorWhenTemporaryInvincible;

        //wait to finish invincibility
        while (owner && owner.IsCurrentlyTemporaryInvincible)
            yield return null;

        //reset color
        foreach (SpriteRenderer sprite in defaultColors.Keys)
            sprite.color = defaultColors[sprite];
    }
}
