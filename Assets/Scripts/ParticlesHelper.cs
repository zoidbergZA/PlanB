using UnityEngine;
using System.Collections;

public class ParticlesHelper : MonoBehaviour 
{
    public enum Effects
    {
        BulletImpactEnemy,
        BulletImpactShield,
        DroneDeath
    }

    [SerializeField] private ParticleEffects bulletImpactEnemyPrefab;
    [SerializeField] private ParticleEffects DroneDeathPrefab;

    private ParticleEffects[] bulletImpactEnemyCache = new ParticleEffects[30];
    private ParticleEffects[] droneDeathCache = new ParticleEffects[20];

    private int impactEnemyIndex;
    private int droneDeathIndex;

    void Awake()
    {
        for (int i = 0; i < bulletImpactEnemyCache.Length; i++)
        {
            bulletImpactEnemyCache[i] = Instantiate(bulletImpactEnemyPrefab, Vector3.zero, Quaternion.identity) as ParticleEffects;
            bulletImpactEnemyCache[i].transform.SetParent(transform);
        }

        for (int i = 0; i < droneDeathCache.Length; i++)
        {
            droneDeathCache[i] = Instantiate(DroneDeathPrefab, Vector3.zero, Quaternion.identity) as ParticleEffects;
            droneDeathCache[i].transform.SetParent(transform);
        }
    }

    public void ShowParticles(Effects effect, Vector3 position, Quaternion rotation)
    {
        switch (effect)
        {
            case Effects.BulletImpactEnemy:
                HandleBulletImpactPlayer(position, rotation);
                break;

            case Effects.DroneDeath:
                HandleDroneDeath(position, rotation);
                break;
        }

        
    }

    private void HandleBulletImpactPlayer(Vector3 position, Quaternion rotation)
    {
        bulletImpactEnemyCache[impactEnemyIndex].transform.position = position;
        bulletImpactEnemyCache[impactEnemyIndex].transform.rotation = rotation;
        bulletImpactEnemyCache[impactEnemyIndex].Play();

        impactEnemyIndex++;

        if (impactEnemyIndex >= bulletImpactEnemyCache.Length)
            impactEnemyIndex = 0;
    }

    private void HandleDroneDeath(Vector3 position, Quaternion rotation)
    {
        droneDeathCache[droneDeathIndex].transform.position = position;
        droneDeathCache[droneDeathIndex].transform.rotation = rotation;
        droneDeathCache[droneDeathIndex].Play();

        droneDeathIndex++;

        if (droneDeathIndex >= droneDeathCache.Length)
            droneDeathIndex = 0;
    }
}
