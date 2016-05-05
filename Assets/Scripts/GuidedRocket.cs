using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class GuidedRocket : MonoBehaviour
{
    public enum State
    {
        SEEKING,
        TRACKING
    }

    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private float seekRange = 100f;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float aoeRange = 15f;
    [SerializeField] private float aoeDamage = 32f;

    private State state;
    private float seekFrequence = 1f;
    private float lastSeekAt;
    private Drone target;
    private float startHeight;

    void Start ()
    {
        startHeight = transform.position.y;
    }
	
	void Update () 
    {
	    switch (state)
	    {
	        case State.SEEKING:
                HandleSeeking();
                break;

            case State.TRACKING:
                HandleTracking();
                break;
	    }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void LateUpdate()
    {
        //maintain same height
//        transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
    }

    private void HandleSeeking()
    {
        if (Time.time >= lastSeekAt + (1/seekFrequence))
        {
            lastSeekAt = Time.time;
            SeekTarget();
        }
    }

    private void HandleTracking()
    {
        if (!target)
        {
            state = State.SEEKING;
            return;
        }

        transform.LookAt(target.transform);
    }

    private void SeekTarget()
    {
        List<Drone> potentialTargets = new List<Drone>();
        //float closestDistance = 0;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, seekRange);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Drone drone = hitColliders[i].transform.root.GetComponent<Drone>();

            if (drone)
            {
                float distance = (drone.transform.position - transform.position).magnitude;
                if (distance <= seekRange)
                {
                    potentialTargets.Add(drone);

//                    if (bestTarget)
//                    {
//                        if (distance < closestDistance)
//                        {
//                            bestTarget = drone;
//                            closestDistance = distance;
//                        }
//                    }
//                    else
//                    {
//                        bestTarget = drone;
//                        closestDistance = distance;
//                    }
                }
            }

            i++;
        }

        if (potentialTargets.Count > 0)
        {
            target = potentialTargets[Random.Range(0, potentialTargets.Count)];

            state = State.TRACKING;
            GetComponent<AudioSource>().PlayOneShot(lockedSound);
        }
    }

    private void Explode()
    {
        GameManager.Instance.PhysicsHelper.AreaDamage(transform.position, aoeDamage, aoeRange);
   
        Instantiate(impactPrefab, transform.position, transform.rotation);
        Camera.main.SendMessage("Shake");
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
