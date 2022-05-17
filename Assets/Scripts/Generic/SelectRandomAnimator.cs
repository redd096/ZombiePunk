using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;

public class SelectRandomAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct AnimStruct
    {
        public RuntimeAnimatorController Anim;
        [Range(0, 100)] public int Percentage;
    }

    [Header("Necessary Components - default get in children")]
    [SerializeField] Animator animToReplace = default;

    [Header("Possible Animators")]
    [ReadOnly] [SerializeField] int totalPercentage = 0;
    [SerializeField] bool usePercentage = true;
    [SerializeField] AnimStruct[] anims = default;

    void OnValidate()
    {
        //editor - show total percentage
        totalPercentage = 0;
        if (anims != null && anims.Length > 0)
        {
            foreach (AnimStruct animStruct in anims)
                totalPercentage += animStruct.Percentage;
        }
    }

    void Start()
    {
        if (usePercentage)
        {
            //get random value from 0 to 100
            int random = Mathf.RoundToInt(Random.value * 100);

            //cycle every element
            int percentage = 0;
            for (int i = 0; i < anims.Length; i++)
            {
                //if reach percentage, set it
                percentage += anims[i].Percentage;
                if (percentage >= random)
                {
                    if (anims[i].Anim && animToReplace)
                    {
                        animToReplace.runtimeAnimatorController = anims[i].Anim;
                    }

                    break;
                }
            }
        }
        else
        {
            //or select random anim without percentage
            int random = Random.Range(0, anims.Length);

            //and set it
            if (anims[random].Anim && animToReplace)
            {
                animToReplace.runtimeAnimatorController = anims[random].Anim;
            }
        }
    }
}
