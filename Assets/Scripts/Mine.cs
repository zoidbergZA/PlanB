using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour 
{
	[SerializeField] private GameObject explodePrefab;
    [SerializeField] private float aoeDamageRange = 15f;
    [SerializeField] private float aoeDamage = 32f;

    private void Explode()
    {
//        Collider myCollider = GetComponent<Collider>();

        GameManager.Instance.PhysicsHelper.AreaDamage(transform.position, aoeDamage, aoeDamageRange);

        Instantiate(explodePrefab, transform.position, transform.rotation);
        Camera.main.SendMessage("Shake");
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Drone drone = collision.transform.root.GetComponent<Drone>();

        if (drone)
            Explode();
    }
}
