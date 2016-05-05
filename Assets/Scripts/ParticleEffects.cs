using UnityEngine;
using System.Collections;

public class ParticleEffects : MonoBehaviour
{
    private ParticleSystem[] particleSystems;

    void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();

    }

    public void Play()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }
    }
}
