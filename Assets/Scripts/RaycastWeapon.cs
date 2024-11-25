using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float Time;
        public Vector3 InitialPosition;
        public Vector3 InitialVelocity;
        public TrailRenderer Tracer;
    }

    public bool isFiring = false;
    public int fireRate = 25;
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 0.0f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastOrigin;
    public Transform raycastDestination;
    public TrailRenderer tracerEffect;
    
    public float damage = 10.0f;

    private Ray _ray;
    private RaycastHit _hitInfo;
    private float _accumulatedTime;
    private List<Bullet> _bullets = new List<Bullet>();
    public float maxLifetime = 3.0f;

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.InitialPosition + bullet.InitialVelocity * bullet.Time +
               0.5f * gravity * bullet.Time * bullet.Time;
    }


    Bullet CreateBullet(Vector3 initialPosition, Vector3 initialVelocity)
    {
        Bullet bullet = new Bullet();
        bullet.Time = 0.0f;
        bullet.InitialPosition = initialPosition;
        bullet.InitialVelocity = initialVelocity;
        bullet.Tracer = Instantiate(tracerEffect, initialPosition, Quaternion.identity);
        bullet.Tracer.AddPosition(initialPosition);
        return bullet;
    }

    public void StartFiring()
    {
        isFiring = true;
        _accumulatedTime = 0;
        FireBullet();
    }

    public void UpdateFiring(float deltaTime)
    {
        _accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (_accumulatedTime >= fireInterval)
        {
            FireBullet();
            _accumulatedTime -= fireInterval;
        }
    }

    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void SimulateBullets(float deltaTime)
    {
        _bullets.ForEach(bullet =>
        {
            Vector3 oldPosition = GetPosition(bullet);
            bullet.Time += deltaTime;
            Vector3 newPosition = GetPosition(bullet);
            RaycastSegment(oldPosition, newPosition, bullet);
        });
    }

    private void DestroyBullets()
    {
        _bullets.RemoveAll(bullets => bullets.Time >= 3.0f);
    }
    
    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        _ray.origin = start;
        _ray.direction = direction;
        if (Physics.Raycast(_ray, out _hitInfo, distance))
        {
            hitEffect.transform.position = _hitInfo.point;
            hitEffect.transform.forward = _hitInfo.normal;
            hitEffect.Emit(1);
        
            bullet.Tracer.transform.position = _hitInfo.point;
            bullet.Time = maxLifetime;
            
            var rb2d = _hitInfo.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(_ray.direction * 20, _hitInfo.point, ForceMode.Impulse);
            }
            
            var hitbox = _hitInfo.collider.GetComponent<Hitbox>();
            if (hitbox)
            {
                hitbox.OnRaycastHit(this, _ray.direction);
            }
        }
        else
        {
            bullet.Tracer.transform.position = end;
        }
    }

    private void FireBullet()
    {
        muzzleFlash.Emit(1);

        Vector3 fireDirection = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, fireDirection);
        _bullets.Add(bullet);
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}