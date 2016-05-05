using UnityEngine;
using System.Collections;

public class LineCrusher : Ability 
{
    /*
    [SerializeField] private SawBlade sawPrefab;
    [SerializeField] private Transform[] launcher_refs;
    [SerializeField] private float range = 30f;
    [SerializeField] private float speed = 13f;.
    private SawBlade sawBlade;
    */
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private float jetBlastDuration = 0.1f;

    void Update()
    {
        //particlesPlay();
    }

    private void particlesPlay()
    {
        if (this.CooldownProgress <= jetBlastDuration)
        {
            Debug.Log("play plz" + Time.time);
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
       
            }
            //particleSystems.Play();
        }
        else
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Stop();
            }
            //particleSystems.Stop();
        }
    }

    protected override void Use()
    {
        StartCoroutine(WaitForStop());

        //fire rocket
        /*
        for (int i = 0; i < launcher_refs.Length; i++)
        {
            sawBlade = Instantiate(sawPrefab, launcher_refs[i].position, topPivot.transform.rotation) as SawBlade;
        }

        //ignore bullet collisions with self
        Collider[] myColliders = transform.root.GetComponentsInChildren<Collider>();

        foreach (Collider col in myColliders)
        {
            Physics.IgnoreCollision(sawBlade.GetComponent<Collider>(), col, true);
        }

        //add range limit
        float t = range/speed;

        sawBlade.speed = speed;
        sawBlade.gameObject.AddComponent<SelfDestruct>().timer = t;
        */
    }

    private IEnumerator WaitForStop()
    {
        //Debug.Log("started at: " + Time.time);
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();

        }

        yield return new WaitForSeconds(jetBlastDuration);

        //Debug.Log("ended at: " + Time.time);
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Stop();
        }
    }
}
