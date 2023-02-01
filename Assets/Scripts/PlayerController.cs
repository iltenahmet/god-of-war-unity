using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public GameObject axe;
    private AnimationManager playerAnimation;

    public float maxSpeed = 6f;
    public float gravity = -10f;

    public float decelerationSpeed;
    public float accelerationSpeed;

    public float turnSmoothTime = 0.1f;

    private Vector3 axeThrowDirection;
    public float axeThrowSpeed;
    public float axeSpinRate;
    public float axeRecallTime;
    public Transform axeRecallCurvePoint;

    public Vector3 moveDirection { get; private set; }
    private Vector3 moveDirectionNSecondsAgo;
    private float tempTime = 0.0f;
    private Queue<Vector3> moveDirections = new Queue<Vector3>();

    public float speed { get; private set; }
    private float rotationAngle;
    
    private float horizontalInput;
    private float verticalInput;
    private Vector3 inputDirection;

    private float verticalVelocity;
    private float currentVelocity;
    private float turnVelocity;


    private void Start()
    {
        playerAnimation = GetComponent<AnimationManager>();
    }


    private void Update()
    {
        UpdateMovementInput();
        UpdateInputDirection();

        if (Input.GetButton("Fire3"))
        {
            RecallAxe();
        }

        if (Input.GetButton("Cancel"))
        {
            SceneManager.LoadScene(0);
        }
    }


    private void FixedUpdate()
    {
        UpdateSpeedBasedOnInput();

        if (playerAnimation.IsAttacking())
        {
            speed = 0.0f;
        }

        UpdateMoveDirection();
        RetainMoveDirectionNSecondsAgo(0.3f);
        UpdateVerticalVelocity(); //gravity
        controller.Move(moveDirection * (speed * Time.deltaTime) + (new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime));

        if (IsMoveInputDetected() || playerAnimation.IsAttacking())
        {
            UpdateRotationAngle();
            transform.rotation = Quaternion.Euler(0f, rotationAngle, 0.0f);
        }
    }

    private void UpdateMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void UpdateInputDirection()
    {
        inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
    }

    private bool IsMoveInputDetected()
    {
        return (inputDirection.magnitude > 0.1f) ? true : false;
    }

    private void UpdateMoveDirection()
    {
        if (IsMoveInputDetected())
        {
            float moveAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            moveDirection = (Quaternion.Euler(0.0f, moveAngle, 0.0f) * Vector3.forward).normalized;
        }
        else
        {
            // this prevents sudden direction change if the player releases one movement input key slightly before releasing the other
            moveDirection = moveDirectionNSecondsAgo;
        }  
    }


    private void RetainMoveDirectionNSecondsAgo(float seconds)
    {
        if (moveDirections.Count == 0)
        {
            moveDirections.Enqueue(moveDirection);
            moveDirectionNSecondsAgo = moveDirection;
        }

        tempTime += Time.deltaTime;

        if (tempTime > seconds)
        {
            tempTime = 0.0f;
            moveDirections.Enqueue(moveDirection);
            moveDirectionNSecondsAgo = moveDirections.Dequeue();
        }
    }

    private void UpdateVerticalVelocity()
    {
        if (controller.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -1.0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void UpdateRotationAngle()
    {
        rotationAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnVelocity, turnSmoothTime);
    }


    private void UpdateSpeedBasedOnInput()
    {
        if (IsMoveInputDetected())
        {
            speed = Mathf.MoveTowards(speed, maxSpeed, accelerationSpeed * Time.deltaTime);
        }
        else
        {
            speed = Mathf.MoveTowards(speed, 0, decelerationSpeed * Time.deltaTime);
        }
    }

    //This function is called from an animation event
    private void ThrowAxe()
    {
        SetAxeThrowDirection();

        if (IsAxeAttachedToPlayer())
        {
            axe.GetComponent<AxeController>().LaunchAxe(axeThrowDirection * axeThrowSpeed, axeSpinRate);
        }        
    }

    private void SetAxeThrowDirection()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit))
        {
            axeThrowDirection = (hit.point - axe.transform.position).normalized;
        }
        else
        {
            axeThrowDirection = cam.forward;
        }
    }

    private bool IsAxeAttachedToPlayer()
    {
        return (axe.transform.root == this.transform) ? true : false;
    }

    private void RecallAxe()
    {
        axe.GetComponent<AxeController>()
            .ReturnToPlayer(axeRecallCurvePoint.position, axeRecallTime);
    }

}
