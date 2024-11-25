using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public BotHealth botHealth;
    
    public void OnRaycastHit(RaycastWeapon weapon, Vector3 direction)
    {
        botHealth.TakeDamage(weapon.damage, direction);
    }
}
