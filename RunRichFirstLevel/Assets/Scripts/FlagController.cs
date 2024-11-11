using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    Animator animator;

    public void Active()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Active");
    }
}
