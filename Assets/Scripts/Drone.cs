using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Drone : Vulnerable
{
    public NavMeshAgent navMeshAgent;
    public Bullet bulletPrefab;
    public AudioClip shootSound;
    public Transform shoot_ref;
    public Transform stun_ref;

    [SerializeField] protected GameObject model;

    //modifiable stats
    public float bulletRange = 30f;
    public float bulletSpeed = 10f;
    public float bulletDamage = 10f;
    private float tempSpeed;
    private Vector3 tempVelocity;
    private Quaternion tempRotation;

    public float GoldValue { get; set; }
    public float StunnedTimer { get; set; }
    public bool IsStunned { get; private set; }



    void Start()
    {
        IsStunned = false;
        RecordCurrentSpeed();
    }

    public override void Update()
    {
        base.Update();
        if(IsStunned)
        {
            HandleStunning();
        }
    }

    private void HandleStunning()
    {
        if (StunnedTimer <= 0)
        {
            RevertToSavedSpeed();
            IsStunned = false;
        }
        else
        {
            FreezeDroneSpeed();
        }
        StunnedTimer -= Time.deltaTime;
    }

    public void Stun(float duration)
    {
        RecordCurrentSpeed();
        IsStunned = true;
        StunnedTimer = duration;
    }

    private void RecordCurrentSpeed()
    {
        if (!IsStunned)
        {
            tempSpeed = this.navMeshAgent.speed;
            tempVelocity = this.navMeshAgent.velocity;
            tempRotation = this.transform.rotation;
        }
    }

    private void RevertToSavedSpeed()
    {
        this.navMeshAgent.speed = tempSpeed;
        this.model.transform.localPosition = Vector3.zero;
        this.model.transform.localRotation = Quaternion.identity;
    }

    private void FreezeDroneSpeed()
    {
        this.navMeshAgent.speed = 0;
        this.navMeshAgent.velocity = Vector3.zero;
        this.model.transform.localPosition = stun_ref.localPosition;
        this.model.transform.localRotation = stun_ref.localRotation;
    }

    public override void Die()
    {
        if (GameManager.Instance.WaveManager.SpawnedList.Contains(this))
            GameManager.Instance.WaveManager.SpawnedList.Remove(this);

        GameManager.Instance.HomeTower.DecreaseClock();
        GameManager.Instance.ParticlesHelper.ShowParticles(ParticlesHelper.Effects.DroneDeath, transform.position, transform.rotation);

//        GameManager.Instance.Player.Gold += GoldValue;
//
//        FloatingText goldEarnedText = Instantiate(GameManager.Instance.GoldTextPrefab, floatingText_ref.position, floatingText_ref.rotation) as FloatingText;
//
//        goldEarnedText.textObject.text = "+" + Mathf.RoundToInt(GoldValue);
    }

    public virtual void Fire(Vector3 direction)
    {
        if (shootSound)
            myAudioSource.PlayOneShot(shootSound);

        Bullet b = Instantiate(bulletPrefab, shoot_ref.position, shoot_ref.rotation) as Bullet;
        b.transform.LookAt(shoot_ref.forward);
        Rigidbody bulletBody = b.GetComponent<Rigidbody>();

        direction.y = 0;
        direction *= bulletSpeed;

        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController)
        {
            direction += characterController.velocity * 0.5f;
        }

        //ignore bullet collisions with self
        Collider[] myColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in myColliders)
        {
            Physics.IgnoreCollision(b.GetComponent<Collider>(), col, true);

        }

        bulletBody.AddForce(direction, ForceMode.VelocityChange);

        //set bullet stats
        b.Damage = bulletDamage;

        b.GetComponent<SelfDestruct>().timer = bulletRange / bulletSpeed;
    }

    public void IncreaseSpeed(float amount)
    {
        navMeshAgent.speed += amount;
    }

    public void IncreaseDamage(float amount)
    {
        bulletDamage += amount;
    }

    public void IncreaseBulletRange(float amount)
    {
        bulletRange += amount;
    }

    public void IncreaseBulletSpeed(float amount)
    {
        bulletSpeed += amount;
    }

}