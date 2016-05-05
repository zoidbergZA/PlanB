using UnityEngine;
using System.Collections;

public class DroneSpawner : Vulnerable
{
    [SerializeField] private Transform spawnRef;
    [SerializeField] private NavMeshObstacle[] navBlockers;
    [SerializeField] private GameObject aliveModel;
    [SerializeField] private GameObject deadModel;
    [SerializeField] private Rigidbody fanRigidbody;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip dieSound;
    [SerializeField] private Animator animator;
    [SerializeField] private Light spawnerLight;

    private int currentExit = -1;
//    private Rigidbody fan;

    public override void Awake()
    {
        base.Awake();

//        InitFan();
    }

    void Start ()
    {
        Invulnerable = true;
        deadModel.SetActive(false);
	}

    void FixedUpdate()
    {
        if (!IsDead)
        {
            fanRigidbody.AddRelativeTorque(Vector3.up * 50f);
        }
    }

    public Drone SpawnDrone(Drone dronePrefab, float goldValue)
    {
        NextExit();

        Drone drone = Instantiate(dronePrefab, spawnRef.position, spawnRef.rotation) as Drone;
        drone.GoldValue = goldValue;

        myAudioSource.PlayOneShot(spawnSound);

        return drone;
    }

    public override void Die()
    {
        //deadModel.SetActive(true);
        if (spawnerLight)
            spawnerLight.enabled = false;

        myAudioSource.PlayOneShot(dieSound);
        animator.SetBool("IsDefeated", true);
        StartCoroutine(DieSequence());
        GameManager.Instance.CheckAllSpawners();
    }

    private IEnumerator DieSequence()
    {
        yield return new WaitForSeconds(0.9f);
        aliveModel.SetActive(false);
    }

    private void NextExit()
    {
        currentExit++;

        if (currentExit >= navBlockers.Length)
            currentExit = 0;

        for (int i = 0; i < navBlockers.Length; i++)
        {
            if (i == currentExit)
                navBlockers[i].gameObject.SetActive(false);
            else
                navBlockers[i].gameObject.SetActive(true);
        }
    }

//    private void InitFan()
//    {
//        fan = fanTransform.gameObject.AddComponent<Rigidbody>();
//
//        fan.useGravity = false;
//        fan.angularDrag = 0.65f;
//        fan.constraints = RigidbodyConstraints.FreezePosition;
//    }
}
