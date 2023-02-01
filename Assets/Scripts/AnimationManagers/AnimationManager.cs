using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationManager : MonoBehaviour
{
    public float animationSmoothTime = 0.3f;
    public Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    public bool IsAttacking()
    {
        return (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) ? true : false;
    }

    public void PlayDeath()
    {
        animator.SetTrigger("Death");
    }
}
