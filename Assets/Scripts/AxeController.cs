using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class AxeController : MonoBehaviour
{
    public GameObject player; 
    public GameObject socket;
    Rigidbody rigidBody;

    public float damage;

    public float cooldownDuration;
    private float cooldown;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateCooldown();
    }
    
    public void LaunchAxe(Vector3 force, float spinRate)
    {
        transform.parent = null;
        rigidBody.AddForce(force, ForceMode.Impulse);
        rigidBody.AddTorque(new Vector3(spinRate, 0, 0), ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollidingWithRoot(other))
        {
            return;
        }

        Health health = other.GetComponent<Health>();
        if (health)
        {            
            if (cooldown == 0.0f && (IsDetached() || IsPlayerAttackingWithAxe()))
            {
                health.ApplyDamage(damage);
                StartCooldown();
            }
        }

        if (IsDetached())
        {
            StickTo(other.transform);
        }

    }

    private void StartCooldown()
    {
        cooldown = cooldownDuration;
    }

    private void UpdateCooldown()
    {
        if (cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            cooldown = 0.0f;
        }
    }

    private bool IsCollidingWithRoot(Collider other)
    {
        return (transform.root.gameObject == other.gameObject) ? true : false;
    }

    private bool IsPlayerAttackingWithAxe()
    {
        PlayerAnimationManager playerAnimation = player.GetComponent<PlayerAnimationManager>();
        return (IsAttachedToPlayer() && playerAnimation.IsAttacking()) ? true : false;
    }

    private bool IsDetached()
    {
        return (transform.parent == null) ? true : false;
    }

    private bool IsAttachedToPlayer()
    {
        return (transform.root.gameObject == player) ? true : false;
    }

    public void StickTo(Transform other)
    {
        StopMovement();
        transform.SetParent(other);
    }

    public void ReturnToPlayer(Vector3 curvePoint, float duration)
    {
        StopMovement();
        StartCoroutine(ReturnMovement(curvePoint, duration));
    }

    IEnumerator ReturnMovement(Vector3 curvePoint, float duration)
    {
        float timeElapsed = 0;

        Vector3 initialPosition = rigidBody.position;
        Vector3 initialRotation = rigidBody.transform.eulerAngles;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float percentComplete = timeElapsed / duration;

            Vector3 socketPosition = socket.transform.position;
            Vector3 socketRotation = socket.transform.eulerAngles;

            // The axe follows a quadratic bezier curve created with initialPosition, curvePoint, and socketPosition
            rigidBody.position = Mathf.Pow(1 - percentComplete, 2) * initialPosition
                + 2 * (1 - percentComplete) * percentComplete * curvePoint
                + Mathf.Pow(percentComplete, 2) * socketPosition;

            rigidBody.transform.eulerAngles = Vector3.Lerp(initialRotation, socketRotation, percentComplete);

            yield return null;
        }

        transform.SetParent(socket.transform);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

    }

    private void StopMovement()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
    }

}
