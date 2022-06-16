using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCaveaux : MonoBehaviour
{
    public BoxCollider2D boxCollider2D;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        animator.SetBool("isOpen", true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        animator.SetBool("isOpen", false);
    }
}
