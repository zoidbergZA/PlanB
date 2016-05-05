using UnityEngine;
using System.Collections;

public class ArtilleryShell : MonoBehaviour
{
    [SerializeField] private GameObject hitPreview;
    [SerializeField] private float explodeDelay = 2f;
    [SerializeField] private GameObject explodeFxPrefab;
    [SerializeField] private float aoeRange = 15f;
    [SerializeField] private float aoeDamage = 32f;

    void Start()
    {
        GetComponentInChildren<Animator>().speed = 1f/explodeDelay;
    }

    void Update()
    {
        explodeDelay -= Time.deltaTime;

        if (explodeDelay <= 0)
            Explode();
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRange);

        int i = 0;
        while (i < hitColliders.Length)
        {
            Player player = hitColliders[i].transform.root.GetComponent<Player>();

            if (player)
            {
                player.TakeDamage(aoeDamage, null, hitColliders[i].transform.position);
            }

            i++;
        }

        Instantiate(explodeFxPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
