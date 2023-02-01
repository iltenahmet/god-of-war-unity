using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : AnimationManager
{
    PlayerController player;

    public float animationDirectionChangeSpeed;

    private Vector3 animationMoveDirection;
    private Vector3 targetDirection;

    protected override void Start()
    {
        base.Start();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        animator.SetBool("Attack1", Input.GetButton("Fire1"));
        animator.SetBool("AxeThrow", Input.GetButton("Fire2"));

        UpdateTargetMoveDirection();
        UpdateAnimationMoveDirection();
        UpdateMovementAnimation();
    }

    private void UpdateTargetMoveDirection()
    {
        float targetAngle = Mathf.Atan2(player.moveDirection.x, player.moveDirection.z) * Mathf.Rad2Deg - player.cam.eulerAngles.y;
        targetDirection = (Quaternion.Euler(0f, targetAngle, 0.0f) * Vector3.forward).normalized;
    }

    private void UpdateAnimationMoveDirection()
    {
        animationMoveDirection = Vector3.MoveTowards(animationMoveDirection, targetDirection, animationDirectionChangeSpeed * Time.deltaTime);
    }

    private void UpdateMovementAnimation()
    {
        float multiplier = player.speed / player.maxSpeed;

        animator.SetFloat("VelocityX", animationMoveDirection.x * multiplier);
        animator.SetFloat("VelocityZ", animationMoveDirection.z * multiplier);
    }

}
