using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;

    private Animator animator; 
    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        
        DeactivateRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    public void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        animator.enabled = true;
    }
    
    public void ActivateRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        animator.enabled = false;
    }
    
    public void applyForce(Vector3 force)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddForce(force);
        }
    }
}
