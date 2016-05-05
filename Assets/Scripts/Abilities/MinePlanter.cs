using UnityEngine;
using System.Collections;

public class MinePlanter : Ability 
{
    [SerializeField] private Mine minePrefab;
    [SerializeField] private Transform launcher_ref;

    protected override void Use()
    {
        //plant mine
        Mine mine = Instantiate(minePrefab, launcher_ref.position, launcher_ref.rotation) as Mine;

        //ignore bullet collisions with self
        Collider[] myColliders = transform.root.GetComponentsInChildren<Collider>();

        foreach (Collider col in myColliders)
        {
            Physics.IgnoreCollision(mine.GetComponent<Collider>(), col, true);
        }
    }
}
