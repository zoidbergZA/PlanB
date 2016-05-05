using UnityEngine;
using System.Collections.Generic;

//[RequireComponent(typeof(Collider))]
public class SawBlade : MonoBehaviour
{


    [SerializeField] private float stunTime = 2f;
    [SerializeField] private float damage = 3f;

    private ParticleCollisionEvent[] m_CollisionEvents = new ParticleCollisionEvent[16];
    private ParticleSystem m_ParticleSystem;
    public static float lastSoundTime;
    public float force = 1;
    //public float speed = 5f;
    //private float startHeight;

    void Start()
    {
        //startHeight = transform.position.y;
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void LateUpdate()
    {
        //maintain same height
        //transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
    }

    private void OnParticleCollision(GameObject other)
    {
        int safeLength = m_ParticleSystem.GetSafeCollisionEventSize();

        if (m_CollisionEvents.Length < safeLength)
        {
            m_CollisionEvents = new ParticleCollisionEvent[safeLength];
        }

        int numCollisionEvents = m_ParticleSystem.GetCollisionEvents(other, m_CollisionEvents);
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (Time.time > lastSoundTime + 0.2f)
            {
                lastSoundTime = Time.time;
            }

            var col = m_CollisionEvents[i].collider;

            if (col.attachedRigidbody != null)
            {
                Vector3 vel = m_CollisionEvents[i].velocity;
                col.attachedRigidbody.AddForce(vel * force);

                Drone drone = other.transform.root.GetComponent<Drone>();
                if (drone)
                {
                    Vector3 direction = (this.transform.position - drone.transform.position);
                    drone.TakeDamage(damage, null, drone.transform.position);
                    Rigidbody rb = drone.GetComponent<Rigidbody>();
                    //rb.AddRelativeForce(direction.normalized * pushForce);
                    drone.Stun(stunTime);
                } else if (other.transform.root.gameObject.layer == LayerMask.NameToLayer("Bullets"))
                {
                    Destroy(other.gameObject);
                }
            }

            other.BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);

            i++;
        }
    }
}
