using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class SImpleAI : MonoBehaviour
{
    NavMeshAgent _agent;
    public Transform playerTransform;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _agent.destination = playerTransform.position;
        animator.SetFloat("Speed", _agent.velocity.magnitude);
        Debug.Log(animator.GetFloat("Speed"));

    } 
}
