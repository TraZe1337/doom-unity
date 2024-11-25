using UnityEngine;
using UnityEngine.AI;


public class AnimationStateController : MonoBehaviour
{
    NavMeshAgent _agent;
    public Transform playerTransform;
    Animator _animator;
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    private float _timer;
    public bool isDead;
    public LayerMask whatIsPlayer;
    public float sightRange, attackRange, healthBarRange;
    public bool playerInSightRange, playerInHealthBarRange ,playerInAttackRange;
    public UIHealthBar healthBar;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        healthBar.GetComponentInChildren<UIHealthBar>();
        playerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (isDead) return;
        _timer -= Time.deltaTime;
        
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInHealthBarRange = Physics.CheckSphere(transform.position, healthBarRange, whatIsPlayer);
        
        if (playerInSightRange && playerInHealthBarRange) DisplayUIHealthBar();
        if (!playerInSightRange && !playerInHealthBarRange) HideUIHealthBar();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        if (!playerInSightRange && !playerInAttackRange && playerInHealthBarRange) StandStill();
        
    }

    void DisplayUIHealthBar()
    {
        FollowPlayer();
        _animator.SetBool("isAttacking", false);
        healthBar.gameObject.SetActive(true);
    }

    void HideUIHealthBar()
    {
        healthBar.gameObject.SetActive(false);
        _animator.SetBool("isAttacking", false);
    }
    
    void AttackPlayer()
    {
        FollowPlayer();
        healthBar.gameObject.SetActive(true);
        _animator.SetBool("isAttacking", true);
    }

    void StandStill()
    {
        _agent.destination = _agent.transform.position;
        _animator.SetBool("isAttacking", false);
        _animator.SetFloat("Speed", 0); 

    }

    void FollowPlayer()
    {
        if (_timer <= 0.0f)
        {
            float sqrDistance = (playerTransform.position - _agent.destination).sqrMagnitude;
            if(sqrDistance > maxDistance * maxDistance)
            {
                _agent.destination = playerTransform.position;
            }

            _timer = maxTime;
        }
        _animator.SetFloat("Speed", _agent.velocity.magnitude); 
    }
}