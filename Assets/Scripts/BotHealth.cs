using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float dieForce;
    [HideInInspector] public float currentHealth;
    Ragdoll ragdoll;
    SkinnedMeshRenderer meshRenderer;
    UIHealthBar healthBar;
    animationStateController animationStateController;


    public float blinkIntensity = 10f;
    public float blinkDuration = 0.05f;
    private float blinkTimer;

    // Start is called before the first frame update
    void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        healthBar = GetComponentInChildren<UIHealthBar>();
        animationStateController = GetComponent<animationStateController>();
        Debug.Log(healthBar);
        currentHealth = maxHealth;

        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            Hitbox hitbox = rb.gameObject.AddComponent<Hitbox>();
            hitbox.botHealth = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1f;
        meshRenderer.material.color = Color.red * intensity;
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        currentHealth -= damage;
        healthBar.setHealthBarPercentage(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            Die(direction);
        }

        blinkTimer = blinkDuration;
    }

    public void Die(Vector3 direction)
    {
        ragdoll.ActivateRagdoll(); 
        direction.y = 1;
        healthBar.gameObject.SetActive(false);
        animationStateController.isDead = true;
        ragdoll.applyForce(direction * dieForce);
    }
}