using UnityEngine;

public class BotHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float dieForce;
    [HideInInspector] 
    public float currentHealth;
    private Ragdoll _ragdoll;
    private SkinnedMeshRenderer _meshRenderer;
    private UIHealthBar _healthBar;
    private AnimationStateController _animationStateController;


    public float blinkIntensity = 10f;
    public float blinkDuration = 0.05f;
    private float _blinkTimer;

    private void Start()
    {
        _ragdoll = GetComponent<Ragdoll>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _healthBar = GetComponentInChildren<UIHealthBar>();
        _animationStateController = GetComponent<AnimationStateController>();
        Debug.Log(_healthBar);
        currentHealth = maxHealth;

        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            Hitbox hitbox = rb.gameObject.AddComponent<Hitbox>();
            hitbox.botHealth = this;
        }
    }

    private void Update()
    {
        _blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(_blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1f;
        _meshRenderer.material.color = Color.gray * intensity;
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        currentHealth -= damage;
        _healthBar.SetHealthBarPercentage(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            Die(direction);
        }

        _blinkTimer = blinkDuration;
    }

    private void Die(Vector3 direction)
    {
        _ragdoll.ActivateRagdoll(); 
        direction.y = 1;
        _healthBar.gameObject.SetActive(false);
        _animationStateController.isDead = true;
        _ragdoll.ApplyForce(direction * dieForce);
    }
}