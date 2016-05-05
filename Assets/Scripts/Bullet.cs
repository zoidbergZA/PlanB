using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Bullet : MonoBehaviour 
{
    public enum BulletType
    {
        PlayerBullet,
        DroneBullet
    }

    [SerializeField] private BulletType bulletType;
    [SerializeField] private GameObject impactPrefab;
	
	public float Damage { get; set; }
    
    void Update()
    {
//        transform.Translate(Direction * Speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Vulnerable vulnerable = collision.transform.root.GetComponent<Vulnerable>();

        if (vulnerable)
        {
            if (bulletType == BulletType.DroneBullet)
            {
                if (vulnerable is Player)
                {
                    vulnerable.TakeDamage(Damage, null, collision.contacts[0].point);
                }   

                if (vulnerable is HomeTower)
                    vulnerable.TakeDamage(Damage, null, collision.contacts[0].point);
            }

            if (bulletType == BulletType.PlayerBullet)
            {
                if (vulnerable is Drone || vulnerable is DroneSpawner)
                {
                    vulnerable.TakeDamage(Damage, null, collision.contacts[0].point);
                }
            }
        }

        Instantiate(impactPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

	
	
	
	
	
	
	
}
