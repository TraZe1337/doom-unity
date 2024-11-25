using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public TMP_Text healthText;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
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
            health = 100f;
        }
    }

    private void OnGUI()
    {
        healthText.text = "" + health;
    }
}