using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] _rigidbodies;
    private Animator _animator; 
    void Start()
    {
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        
        DeactivateRagdoll();
    }
    
    
    private void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = true;
        }
        _animator.enabled = true;
    }
    
    public void ActivateRagdoll()
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = false;
        }
        _animator.enabled = false;
    }
    
    public void ApplyForce(Vector3 force)
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.AddForce(force);
        }
    }
}
