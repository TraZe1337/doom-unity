using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public BotHealth botHealth;
    
    public void OnRaycastHit(RaycastWeapon weapon, Vector3 direction)
    {
        botHealth.TakeDamage(weapon.damage, direction);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
