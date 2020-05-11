using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneHover : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void OnPointerEnter()
    {
        animator.SetBool("Hover", true);
    }

    public void OnPointerExit()
    {
        animator.SetBool("Hover", false);
    }
}
