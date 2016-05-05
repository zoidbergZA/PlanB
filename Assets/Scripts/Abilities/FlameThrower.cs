using UnityEngine;
using System.Collections;

public class FlameThrower : Ability
{
    [SerializeField] private Transform launcher_ref;
    [SerializeField] private ParticleSystem flamesParticleSystem;
    [SerializeField] private float flameTime;
    [SerializeField] private float flameRadius = 2f;
    [SerializeField] private float flameDistance = 7f;
    [SerializeField] private float damagePerFlame = 8f;
    [SerializeField] private float flameEvery = 0.125f;

    private float flameTimer;
    private bool flaming;
    private float lastFlameRayAt;

    public override void Update()
    {
        base.Update();

        if (flaming && Time.time >= lastFlameRayAt + flameEvery)
        {
            ShootFlameRay();
            lastFlameRayAt = Time.time;
        }

        flameTimer -= Time.deltaTime;

        if (flameTimer <= 0)
            StopFlaming();
    }
    
    protected override void Use()
    {
        //todo: fix use base from activating if already flaming
        if (flaming)
            return;

        StartFlaming();
    }

    private void StartFlaming()
    {
        flaming = true;
        flameTimer = flameTime;
        flamesParticleSystem.Play();
    }

    private void StopFlaming()
    {
        flaming = false;
        flamesParticleSystem.Stop();
    }

    private void ShootFlameRay()
    {
        RaycastHit hitInfo;
        
        if (Physics.SphereCast(launcher_ref.position, flameRadius, launcher_ref.transform.forward, out hitInfo, flameDistance))
        {
            Debug.DrawLine(launcher_ref.position, hitInfo.point, Color.red);

            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Bullets"))
            {
                Destroy(hitInfo.transform.gameObject);
            }

            else
            {
                Drone drone = hitInfo.transform.root.GetComponent<Drone>();

                if (drone)
                {
                    drone.TakeDamage(damagePerFlame, Player, hitInfo.point);
                }
            }

        }

//        Debug.DrawLine(launcher_ref.position, launcher_ref.position + launcher_ref.transform.forward * flameDistance, Color.green);
    }
}
