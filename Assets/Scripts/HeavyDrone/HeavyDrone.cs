using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeavyDrone : Drone 
{
	private enum States
	{
		IDLE,
		ATTACKBASE,
		CHASEPLAYER
	}

    [SerializeField] private float shieldAngle = 70f;               // angle from transform forward that the shield protects
    [SerializeField] private float shieldDamageReduction = 0.8f;    // damage reduction when shield is hit ( 0 == no protection, 1 == full protection)
    [SerializeField] private GameObject shieldImpactPrefab;         // effects that spawn when shield is hit
    //[SerializeField] private float searchRadius = 40f; //geve warning 'assigned but never used'

	[SerializeField] private float meleeRadius = 4f;
	[SerializeField] private float meleeDamage = 10f;
    //[SerializeField] private float shootCooldown = 2f; //geve warning 'assigned but never used'
	[SerializeField] private float meleeCooldown = 2.1f;
	[SerializeField] private AudioClip meleeSound;
	[SerializeField] private ParticleSystem meleeParticleSystem;

	public bool shielImpact = false;

	private float lastMeleeAt;
	
	private States state;
	private Vulnerable target;
    
	void Start () 
	{
		GoToState(States.ATTACKBASE);
	}

	public override void Update()
	{
		base.Update();
		
		if (state == States.ATTACKBASE)
		{
			LookForPlayer();
		}
	}

    public override void TakeDamage(float amount, Vulnerable attacker, Vector3 point)
    {
        if (attacker is Player)
        {
			Vector3 distance = attacker.transform.position - this.transform.position;
			Vector3 forward = this.transform.forward;
			float angle = Vector3.Angle(distance, forward);

			if(angle < shieldAngle)
			{
				amount *= 1 - shieldDamageReduction;
				Instantiate(shieldImpactPrefab);
				shielImpact = true;
			} 
        }

        //finally call base.takeDamage with modified amount
        base.TakeDamage(amount, attacker, point);
    }
    
	private void LookForPlayer()
	{
		if (GameManager.Instance.Player)
		{
			Deffending();
		}
	}

	private void Deffending()
	{
		Vector3 DiffVector = new Vector3();
		Vector3 defpos = new Vector3();

		if (FindClosestAlly () != null) 
		{ 
			DiffVector = (FindClosestAlly ().transform.position - GameManager.Instance.Player.transform.position);
			defpos = GameManager.Instance.Player.transform.position + DiffVector * 0.5f;
			navMeshAgent.SetDestination(defpos);
		}

	    else
	    {
	        if (GameManager.Instance.ForcewallsDown)
	        {
                DiffVector = (FindClosestSpawner().transform.position - GameManager.Instance.Player.transform.position);
                defpos = GameManager.Instance.Player.transform.position + DiffVector * 0.75f;
                navMeshAgent.SetDestination(defpos);
	        }

	        else
	        {
                target = GameManager.Instance.HomeTower;
                if (!IsStunned) navMeshAgent.SetDestination(GameManager.Instance.HomeTower.transform.position);
                HandleMelee();
	        }
	    }

//		if (FindClosestAlly () == null && GameManager.Instance.ForcewallsDown == true) 
//		{
//			DiffVector = (FindClosestSpawner().transform.position - GameManager.Instance.Player.transform.position);
//			defpos = GameManager.Instance.Player.transform.position + DiffVector * 0.75f;
//			navMeshAgent.SetDestination(defpos);
//		}
//		if (FindClosestAlly () == null && GameManager.Instance.ForcewallsDown == false) 
//		{
//			target = GameManager.Instance.HomeTower;
//			if(!IsStunned) navMeshAgent.SetDestination(GameManager.Instance.HomeTower.transform.position);
//			HandleMelee();
//		}

//		Vector3 targetDir = GameManager.Instance.Player.transform.position - this.transform.position;
//		Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, 0.5f, 0.0f);
//		transform.rotation = Quaternion.LookRotation(newDir);

//		var q = Quaternion.LookRotation(GameManager.Instance.Player.transform.position - this.transform.position);
//		this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, q, 5 * Time.deltaTime);

//		Quaternion.RotateTowards (this.transform.rotation, GameManager.Instance.Player.transform.rotation, 5f * Time.deltaTime);

//		Quaternion lookAtRotation = Quaternion.LookRotation(GameManager.Instance.Player.transform.position - this.transform.position);
//		transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtRotation, Time.deltaTime / 5);

		var lookPos = GameManager.Instance.Player.transform.position - this.transform.position;
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation(lookPos);
		rotation *= Quaternion.Euler (0, 360, 0);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 0.8f);
	}

	private Drone FindClosestAlly() 
	{
		List<Drone> drones = new List<Drone> ();

		foreach(Drone drone in GameManager.Instance.WaveManager.SpawnedList){
			if (!(drone is HeavyDrone))
            {
				drones.Add(drone);
			}
		}

		if (drones.Count == 0) {
			return null;
		}

		Drone closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = this.transform.position;
		foreach (Drone d in drones) {
			Vector3 diff = d.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = d;
				distance = curDistance;
			}
		}
		return closest;
	}

	private DroneSpawner FindClosestSpawner() 
	{
		List<DroneSpawner> spawners = new List<DroneSpawner> ();

		foreach(DroneSpawner spawner in GameManager.Instance.DroneSpawners){
			spawners.Add(spawner);
		}
		
		if (spawners.Count == 0) {
			return null;
		}

		DroneSpawner closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = this.transform.position;
		foreach (DroneSpawner sp in spawners) {
			Vector3 diff = sp.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = sp;
				distance = curDistance;
			}
		}
		return closest;
	}
	
	private void GoToState(States newState)
	{
		state = newState;
		
		switch (state)
		{
		case States.ATTACKBASE:
			target = GameManager.Instance.HomeTower;
			if(!IsStunned) navMeshAgent.SetDestination(GameManager.Instance.HomeTower.transform.position);
			HandleMelee();
			break;
			
		case States.CHASEPLAYER:
			break;
		}
	}

	public override void Die()
	{
		base.Die();

		Destroy(gameObject);
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
