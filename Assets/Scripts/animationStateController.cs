using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class animationStateController : MonoBehaviour
{
    NavMeshAgent _agent;
    public Transform playerTransform;
    Animator animator;
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            float sqrDistance = (playerTransform.position - _agent.destination).sqrMagnitude;
            if(sqrDistance > maxDistance * maxDistance)
            {
                _agent.destination = playerTransform.position;
            }

            timer = maxTime;
        }
        _agent.destination = playerTransform.position;
        animator.SetFloat("Speed", _agent.velocity.magnitude); 
    }
}