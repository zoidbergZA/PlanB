using UnityEngine;
using System.Collections;

public class PhysicsHelper : MonoBehaviour 
{
    public void AreaDamage(Vector3 location, float aoeDamage, float aoeRadius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(location, aoeRadius);

        int i = 0;
        while (i < hitColliders.Length)
        {
            Drone drone = hitColliders[i].transform.root.GetComponent<Drone>();

            if (drone)
            {
                float distance = (drone.transform.position - location).magnitude;
                
                if (distance <= aoeRadius)
                {
                    float dmg = 1f - distance / aoeRadius;
                    dmg = (dmg < 0.2f) ? 0.2f : dmg;
                    dmg *= aoeDamage;

                    drone.TakeDamage(dmg, null, hitColliders[i].transform.position);
                }
            }

            i++;
        }
    }
}
