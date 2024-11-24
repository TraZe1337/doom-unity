using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float baseBounceForce = 3f;
    public float forceMultiplier = 1.3f;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float impactForce = collision.relativeVelocity.magnitude;
                float adjustedBounceForce = baseBounceForce + (impactForce * forceMultiplier);
                rb.AddForce(Vector3.up * adjustedBounceForce, ForceMode.Impulse);
            }
        }
    }
}