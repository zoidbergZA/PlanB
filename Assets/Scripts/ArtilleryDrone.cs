using UnityEngine;
using System.Collections;

public class ArtilleryDrone : Drone 
{
    public enum States
    {
        IDLE,
        REPOSITION
    }

    [SerializeField] private ArtilleryShell artilleryShellPrefab;
    [SerializeField] private float idealRange = 40f;
    [SerializeField] private float fireCooldown = 3f;
    [SerializeField] private float errorDistance = 4f;
    [SerializeField] private ParticleSystem fireSystem;
    [SerializeField] private LayerMask hitMask;
    
    //private States state; //geve warning 'assigned but never used'
    private float lastFireAt;

    void Start()
    {
        //state = States.REPOSITION; //not used anywhere
    }

    public override void Update()
    {
        base.Update();

        if (IsStunned)
            return;

        if (GameManager.Instance.Player)
        {
            Vector3 deltaVector = GameManager.Instance.Player.transform.position - transform.position;
            float dist = deltaVector.magnitude;
            //bool chase = false; //not used anywhere, gave warning 'assigned but never used'

            Vector3 moveDir = Vector3.zero;

            if (dist >= idealRange + 2f)
            {
                //chase = true; //not used anywhere, gave warning 'assigned but never used'
                moveDir = deltaVector.normalized;
            }
            else if (dist <= idealRange - 2f)
            {
                moveDir = -deltaVector.normalized;
            }

            navMeshAgent.SetDestination(transform.position + moveDir * 40f);

            HandleShoot(dist);
        }
    }

    void LateUpdate()
    {
        if (GameManager.Instance.Player)
        {
            model.transform.LookAt(GameManager.Instance.Player.transform);
        }
    }

    public override void Die()
    {
        base.Die();

        Destroy(gameObject);
    }

    private void HandleShoot(float targetDistance)
    {
        if (targetDistance > idealRange + 9f)
            return;

        if (Time.time < lastFireAt + fireCooldown)
            return;

        if (GameManager.Instance.Player)
        {
            if (GameManager.Instance.Player.IsDead)
                return;
            
            Vector3 targetPosition = GameManager.Instance.Player.transform.position + GameManager.Instance.Player.transform.forward * 7f + (Vector3)(Random.insideUnitCircle * errorDistance);
            RaycastHit hit;

            if (Physics.Raycast(targetPosition + Vector3.up * 50, Vector3.down, out hit, hitMask))
            {
                //layermask bug fix
                if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "MousePlane")
                    return;

                GameObject shell = Instantiate(artilleryShellPrefab, new Vector3(hit.point.x, 0, hit.point.z), Quaternion.identity) as GameObject;
            }

            lastFireAt = Time.time;

            myAudioSource.PlayOneShot(shootSound);
            fireSystem.Play();
            GameManager.Instance.TracerHelper.ShowTracer(shoot_ref.position, shoot_ref.position + Vector3.up * 30f);
        }
    }
}
