using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public TMP_Text healthText;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            health -= 10f;
            OnGUI();
        }
    }

    private void Start()
    {
        OnGUI();
    }

    private void Update()
    {
        if (health <= 0)
        {
            // Destroy the player
            //Destroy(gameObject);
        }
    }

    private void OnGUI()
    {
        healthText.text = "" + health;
    }
}