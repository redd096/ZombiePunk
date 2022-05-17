using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using UnityEngine.UI;
using redd096.Attributes;

public class LifeTest : MonoBehaviour
{
    [Header("Necessary Components")]
    [SerializeField] HealthComponent life = default;
    [SerializeField] Image image = default;

    [Header("Predict")]
    [SerializeField] bool enablePredict = true;
    [EnableIf("enablePredict")] [SerializeField] Image predictionImage = default;
    [EnableIf("enablePredict")] [Tooltip("For how much time prediction will remain still")] [SerializeField] float durationPrediction = 0.5f;
    [EnableIf("enablePredict")] [Tooltip("Duration animation from start value to end value")] [SerializeField] float timeToReachValue = 0.5f;
    [EnableIf("enablePredict")] [SerializeField] Color colorOnIncrease = Color.green;
    [EnableIf("enablePredict")] [SerializeField] Color colorOnDecrease = Color.red;

    Coroutine updateHealthBarCoroutine;

    void OnEnable()
    {
        //find components
        if (image == null) image = gameObject.GetComponent<Image>();
        if (life == null) life = GameObject.Find("Player").GetComponent<HealthComponent>();

        //set prediction above health bar (to render it behind)
        if (image && predictionImage)
        {
            predictionImage.transform.SetSiblingIndex(image.transform.GetSiblingIndex() - 1);
        }

        //add events
        if (life)
        {
            life.onChangeHealth += UpdateLife;
            UpdateLife(); //set default value
        }
    }

    void OnDisable()
    {
        //remove events
        if (life)
        {
            life.onChangeHealth -= UpdateLife;
        }
    }

    void UpdateLife()
    {
        //do nothing if image is not setted
        if (image == null)
            return;

        //if predict is not enabled or there isn't prediction image, just set health value
        if (enablePredict == false || predictionImage == null)
        {
            image.fillAmount = life.CurrentHealth / life.MaxHealth;
        }
        //else start coroutine to update health using prediction too
        else
        {
            if (updateHealthBarCoroutine != null)
                StopCoroutine(updateHealthBarCoroutine);

            updateHealthBarCoroutine = StartCoroutine(UpdateHealthBarCoroutine());
        }
    }

    IEnumerator UpdateHealthBarCoroutine()
    {
        //check if increase or decrease
        float valueToReach = life.CurrentHealth / life.MaxHealth;
        bool increase = valueToReach >= predictionImage.fillAmount;

        //set prediction color (increase or decrease)
        predictionImage.color = increase ? colorOnIncrease : colorOnDecrease;

        //if increase, move prediction first, else move healthBar
        if (increase)
            predictionImage.fillAmount = valueToReach;
        else
            image.fillAmount = valueToReach;

        //wait duration
        yield return new WaitForSeconds(durationPrediction);

        //then start animation
        float startValue = increase ? image.fillAmount : predictionImage.fillAmount;
        float delta = 0;
        while (delta < 1)
        {
            delta += Time.deltaTime / timeToReachValue;

            if (increase)
                image.fillAmount = Mathf.Lerp(startValue, valueToReach, delta);
            else
                predictionImage.fillAmount = Mathf.Lerp(startValue, valueToReach, delta);

            yield return null;
        }
    }
}
