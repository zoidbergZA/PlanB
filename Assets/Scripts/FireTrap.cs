using UnityEngine;
using System.Collections;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private float damage = 35f;

    void OnTriggerEnter(Collider other)
    {
        Vulnerable vulnerable = other.transform.root.GetComponent<Vulnerable>();

        if (vulnerable)
        {
            vulnerable.TakeDamage(damage, null, vulnerable.transform.position);
        }
    }
}
