using System;
using UnityEngine;
using System.Collections;

public class Player : Vulnerable 
{
    public AbilityManager abilityManager;

    [SerializeField] private float respawnTime = 5f;
	[SerializeField] private float movementSpeed;
    [SerializeField] private float shootCooldown = 0.35f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private int damageDie = 1;
    [SerializeField] private int damageDieFaces = 4;
    [SerializeField] private float shootRange = 30f;
    [SerializeField] private float bulletRadius = 1.05f;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private GameObject model;
	[SerializeField] private Transform turret;
    [SerializeField] private Transform[] shoot_refs;
    [SerializeField] private ParticleSystem[] shootParticles;
    [SerializeField] private Light muzzleLight;
    [SerializeField] private Light flashLight;
    [SerializeField] private LayerMask bulletMask;
	[SerializeField] private GameObject bulletImpactEnemyPrefab;
	[SerializeField] private GameObject bulletImpactShieldPrefab;
    [SerializeField] private GameObject bulletMaxRangePrefab;
    [SerializeField] private Transform dead_ref;
	[SerializeField] private ParticleSystem damagePrefab;
	[SerializeField] private float smoking;
//    [SerializeField] private ParticleSystem shootParticleR;
//    [SerializeField] private ParticleSystem shootParticleL;

    private int activeGun;
	private CharacterController cc;
	private Vector3 movement;
	private PlayerInput playerInput;
	public PlayerInput.PlayerInputType myInputType;
	private float lastFireAt;
	private bool shieldHit;
    private bool IsWalking = false;
    private float moveSoundCooldown = 0.519f;
    private float lastMoveSoundPlayedAt;
    private float lastShootSoundPlayedAt;
    public bool MoveSoundIsOnCooldown
    {
        get
        {
            if (Time.time < lastMoveSoundPlayedAt + moveSoundCooldown)
                return true;
            return false;
        }
    }
    public bool ShootSoundIsOnCooldown
    {
        get
        {
            if (Time.time < lastShootSoundPlayedAt + shootCooldown)
                return true;
            return false;
        }
    }
    public AudioSource[] AudioSource;
    public float Gold { get; set; }
    public float CritChance { get; set; }
    public float CritMultiplier { get; set; }
    public PlayerInput PlayerInput { get { return playerInput; }}

    public override void Awake()
    {
        base.Awake();

		cc = gameObject.GetComponent<CharacterController>();
		playerInput = gameObject.GetComponent<PlayerInput>();
    }

    void Start()
    {
        muzzleLight.enabled = false;
        flashLight.enabled = false;
        //////////////////// DEBUG ////////////////////
//        Gold = 99;
        //////////////////// DEBUG ////////////////////
    }

    public override void Update()
    {
        base.Update();

        if (IsDead)
            return;

		UpdateMovement ();
		UpdateRotation ();

        if (muzzleLight.enabled)
        {
            if (Time.time > lastFireAt + 0.1f)
                muzzleLight.enabled = false;
        }
		Damaged ();
	}

    public override void TakeDamage(float amount, Vulnerable attacker, Vector3 point)
    {
        base.TakeDamage(amount, attacker, point);

        GameManager.Instance.Hud.ShowPlayerDamage();
    }

    public override void Die()
    {
        IsWalking = false;
        cc.enabled = false;
        playerInput.enabled = false;
//        model.SetActive(false);
        model.transform.localPosition = dead_ref.localPosition;
        model.transform.localRotation = dead_ref.localRotation;

        GameManager.Instance.GameDefeat();

//        GameManager.Instance.Hud.ShowToast("Player down!", 3f, NewHud.AlertType.WARNING);

//        StartCoroutine(RespawnSequence());
    }

    public void ToggleInputEnabled(bool enable)
    {
        if (enable)
            playerInput.enabled = true;
        else
            playerInput.enabled = false;
    }

    public void CollectUpgrade(int abilitySlot)
    {
        abilityManager.Abilities[abilitySlot].Activate();
    }

    public void ModifyDamage(float amount)
    {
        baseDamage += amount;
    }

    public void IncreaseMovespeed(float amount)
    {
        movementSpeed += amount;
    }

    public void ReduceCooldown(float amount)
    {
        shootCooldown -= amount;

        if (shootCooldown < 0.01f)
            shootCooldown = 0.01f;
    }

    public void ToggleFlashLight(bool on)
    {
        flashLight.enabled = on;
    }

    private void TryFire()
    {
        if (Time.time < lastFireAt + shootCooldown)
            return;

        FireRay(shoot_refs[activeGun]);
        shootParticles[activeGun].Play();

        activeGun++;

        if (activeGun >= shoot_refs.Length)
            activeGun = 0;

//        for (int i = 0; i < shoot_refs.Length; i++)
//        {
//            FireRay(shoot_refs[i]);    
//        }
        
        lastFireAt = Time.time;
    }

	private void Damaged()
	{
        if (!damagePrefab)
            return;

		if (Hitpoints < smoking) {
//			Debug.Log("Smoking");
			damagePrefab.Play();
		} else {
//			Debug.Log("Not Smoking");
			damagePrefab.Stop();
		}
	}

    private void FireRay(Transform shootRef)
    {       
		RaycastHit hitInfo;
        Vector3 endPoint = shootRef.position + shootRef.forward * shootRange;
        if (Physics.SphereCast(shootRef.position, bulletRadius, shootRef.transform.forward, out hitInfo, shootRange, bulletMask))
        {
//            Debug.DrawLine(shoot_ref.position, hitInfo.point, Color.red);

            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Bullets"))
            {
                Destroy(hitInfo.transform.gameObject);
            }

            else
            {
                //todo: improve this
                Drone drone = hitInfo.transform.root.GetComponent<Drone>();

                float dmg = baseDamage + Utils.RollDie(damageDie, damageDieFaces);

                if (drone)
                {

					if (drone is HeavyDrone){
						HeavyDrone hDrone = (HeavyDrone) drone;
						if(hDrone.shielImpact == true){
							shieldHit = true;
						}
					} else {
						shieldHit = false;
					}

					if (UnityEngine.Random.value < CritChance)
                    {
                        dmg = dmg + dmg * CritMultiplier;
                        Instantiate(GameManager.Instance.CritTextPrefab, drone.FloatingTextRef.position, drone.floatingText_ref.rotation);
					}

                    drone.TakeDamage(dmg, this, hitInfo.point);
                }

                DroneSpawner droneSpawner = hitInfo.transform.root.GetComponent<DroneSpawner>();

                if (droneSpawner)
                {
                    droneSpawner.TakeDamage(dmg, this, hitInfo.point);
                }

//                ForceWall forceWall = hitInfo.transform.root.GetComponent<ForceWall>();
//
//                if (forceWall)
//                {
//                    forceWall.TakeDamage(baseDamage, this, hitInfo.point);
//                }
            }

            endPoint = hitInfo.point;

			if (shieldHit == false)
            {
//            	Instantiate(bulletImpactEnemyPrefab, hitInfo.point, Quaternion.identity);
			    GameManager.Instance.ParticlesHelper.ShowParticles(ParticlesHelper.Effects.BulletImpactEnemy, hitInfo.point, Quaternion.identity);
            } 
            
            else 
            {
				Instantiate(bulletImpactShieldPrefab, hitInfo.point, Quaternion.identity);
			}

        }

        else
        {
            Instantiate(bulletMaxRangePrefab, endPoint, Quaternion.identity);
        }

        GameManager.Instance.TracerHelper.ShowTracer(shootRef.position, endPoint);


        if (shootSound && !ShootSoundIsOnCooldown)
            PlayShootSound(AudioSource[0], shootSound);

        muzzleLight.enabled = true;
    }

    private void PlayShootSound(AudioSource source, AudioClip clip)
    {
        lastShootSoundPlayedAt = Time.time;
        source.PlayOneShot(clip);
    }


    private void PlayMoveSound(AudioSource source, AudioClip clip)
    {
        lastMoveSoundPlayedAt = Time.time;
        source.PlayOneShot(clip);
    }

	void SetPlayerInputType(PlayerInput.PlayerInputType pi)
    {
		myInputType = pi;
	}

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(respawnTime);

        Revive();

        cc.enabled = true;
        playerInput.enabled = true;
        model.SetActive(true);

        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
    }

	void UpdateMovement()
    {
		movement.x = playerInput.MoveXaxis;
		movement.z = playerInput.MoveZaxis;
		movement.y = 0;

		if (movement.magnitude < 0.1f)
		{
		    cc.SimpleMove(Vector3.zero);
            myAudioSource.Pause();
            IsWalking = false;
		}
		else
		{
            movement.Normalize();
            movement *= movementSpeed;
            cc.SimpleMove(movement);
            IsWalking = true;
		}
        //if (IsWalking && !MoveSoundIsOnCooldown) PlayMoveSound(AudioSource[1], moveSound);
        //update rotation of the lower part of the player
        if ((playerInput.MoveXaxis >= 0.2f || playerInput.MoveXaxis <= -0.2f) || (playerInput.MoveZaxis >= 0.2f || playerInput.MoveZaxis <= -0.2f))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
             Mathf.Atan2(playerInput.MoveXaxis, playerInput.MoveZaxis) * Mathf.Rad2Deg, transform.eulerAngles.z);
        }
    }

	void UpdateRotation()
    {
        if ((playerInput.MouseXaxis >= 0.2f || playerInput.MouseXaxis <= -0.2f) || (playerInput.MouseYaxis >= 0.2f || playerInput.MouseYaxis <= -0.2f))
        {
            turret.transform.eulerAngles = new Vector3(turret.transform.eulerAngles.x,
             Mathf.Atan2(-playerInput.MouseXaxis, -playerInput.MouseYaxis) * Mathf.Rad2Deg, turret.transform.eulerAngles.z);

            if (playerInput.Controls == "KEYBOARD")
            {
                if (Input.GetMouseButton(0))
                    TryFire();
            }
            else
                TryFire();
        }

        else
        {
            turret.transform.eulerAngles = new Vector3(transform.eulerAngles.x,
             Mathf.Atan2(playerInput.MoveXaxis, playerInput.MoveZaxis) * Mathf.Rad2Deg + 180, transform.eulerAngles.z);
//            shootParticleL.Stop();
//            shootParticleR.Stop();
        }
	}
	
	
	
}
