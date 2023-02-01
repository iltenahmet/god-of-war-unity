using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Image healthBarImage;
    AnimationManager animationManger;

    public float maxHealth;
    public float health { get; private set; }

    private bool hasPlayedDeathAnimation = false;

    private void Start()
    {
        health = maxHealth;
        animationManger = GetComponent<AnimationManager>();
    }

    private void Update()
    {
        if (healthBarImage)
        {
            UpdateHealthBar();
        }

        if (!IsAlive() && !hasPlayedDeathAnimation && animationManger)
        {
            animationManger.PlayDeath();
            hasPlayedDeathAnimation = true;
        }
    }

    public void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;
    }

    public void OnDamage()
    {
        if (!IsAlive() && animationManger)
        {
            animationManger.PlayDeath();
        }
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);
    }

    public bool IsAlive()
    {
        if (health > 0)
        {
            return true;
        }
        else
        {
            health = 0; //prevents negative health values
            return false;
        }
    }
}
