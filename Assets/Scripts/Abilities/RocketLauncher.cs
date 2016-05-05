using UnityEngine;
using System.Collections;

public class RocketLauncher : Ability
{
    [SerializeField] private GuidedRocket rocketPrefab;
    [SerializeField] private Transform launcher_ref;
    [SerializeField] private int amount = 3;

    protected override void Use()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 offset = launcher_ref.transform.forward*i*2f;

            Instantiate(rocketPrefab, launcher_ref.position + offset, launcher_ref.rotation);
        }
    }

    private void FireRocket()
    {
        //GuidedRocket rocket = Instantiate(rocketPrefab, launcher_ref.position, launcher_ref.rotation) as GuidedRocket;
        Instantiate(rocketPrefab, launcher_ref.position, launcher_ref.rotation); //this fixed warning 'rocket variable created but never used'
//        //ignore bullet collisions with self
//        Collider[] myColliders = transform.root.GetComponentsInChildren<Collider>();
//
//        foreach (Collider col in myColliders)
//        {
//            Physics.IgnoreCollision(rocket.GetComponent<Collider>(), col, true);
//        }
    }
}
