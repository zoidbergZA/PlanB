using UnityEngine;
using System.Collections;

public class Charger : Drone 
{
    private enum States
    {
        IDLE,
        ATTACKBASE,
        CHASEPLAYER
    }

    [SerializeField] private AudioClip meleeSound;
    [SerializeField] private ParticleSystem meleeParticleSystem;
    [SerializeField] private float meleeCooldown = 2.1f;
    [SerializeField] private float searchRadius = 40f;
    [SerializeField] private float meleeRadius = 4f;
    [SerializeField] private float meleeDamage = 22f;

    private States state;
    private Vulnerable target;
    private float lastMeleeAt;

    void Start()
    {
        GoToState(States.ATTACKBASE);
    }

    public override void Update()
    {
        base.Update();
        
        if (state == States.ATTACKBASE)
        {
            if (!target || target.IsDead)
                GoToState(States.IDLE);
            HandleMelee();
            LookForPlayer();
        }

        if (state == States.CHASEPLAYER)
        {
            if (!target || target.IsDead)
            {
                GoToState(States.ATTACKBASE);
                return;
            }
            
            navMeshAgent.SetDestination(target.transform.position);
        }

        HandleMelee();
    }

    public override void Die()
    {
        base.Die();

        Destroy(gameObject);
    }

    private void GoToState(States newState)
    {
        state = newState;

        switch (state)
        {
            case States.ATTACKBASE:
                target = GameManager.Instance.HomeTower;
                navMeshAgent.SetDestination(GameManager.Instance.HomeTower.transform.position);
                break;

            case States.CHASEPLAYER:
                break;
        }
    }

    private void HandleMelee()
    {
        float distance = (target.transform.position - transform.position).magnitude;
        
        if (distance <= meleeRadius)
        {
            if (Time.time >= lastMeleeAt + meleeCooldown)
            {
                lastMeleeAt = Time.time;

                if (!IsStunned) MeleeAttack();
            }
        }
    }

    private void LookForPlayer()
    {
        if (GameManager.Instance.Player)
        {
            float distance = (GameManager.Instance.Player.transform.position - transform.position).magnitude;

            if (distance <= searchRadius)
            {
                target = GameManager.Instance.Player;
                GoToState(States.CHASEPLAYER);
            }
        }
    }

    private void MeleeAttack()
    {
        Collider myCollider = GetComponent<Collider>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, meleeRadius);

        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i] == myCollider)
                break;

            Vulnerable vulnerable = hitColliders[i].transform.root.GetComponent<Vulnerable>();

            if (vulnerable)
            {
                if (vulnerable is Player || vulnerable is HomeTower)
                {
                    vulnerable.TakeDamage(meleeDamage, null, hitColliders[i].transform.position);
                }
            }

            i++;
        }

        myAudioSource.PlayOneShot(meleeSound);
        meleeParticleSystem.Play();
    }
}
