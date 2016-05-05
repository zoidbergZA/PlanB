using UnityEngine;
using System.Collections;

public class TracerHelper : MonoBehaviour 
{
    [SerializeField] private Tracer TracerPrefab;
    [SerializeField] private Tracer HealTracerPrefab; //Chiggy

    private Tracer[] tracers = new Tracer[20];
    private Tracer[] healTracers = new Tracer[20]; //Chiggy
    private int tracerIndex;
    private int healTracerIndex; //Chiggy

    void Awake()
    {
        for (int i = 0; i < tracers.Length; i++)
        {
            tracers[i] = (Tracer)Instantiate(TracerPrefab, Vector3.zero, Quaternion.identity);
            tracers[i].Reset();
            tracers[i].transform.parent = transform;
            healTracers[i] = (Tracer)Instantiate(HealTracerPrefab, Vector3.zero, Quaternion.identity); //Chiggy
            healTracers[i].Reset(); //Chiggy
            healTracers[i].transform.parent = transform;
        }
    }

    public void ShowTracer(Vector3 start, Vector3 end)
    {
        tracers[tracerIndex].Activate(start, end);

        tracerIndex++;

        if (tracerIndex >= tracers.Length)
            tracerIndex = 0;
    }

    //Chiggy
    public void ShowHealTracer(Vector3 start, Vector3 end)
    {
        healTracers[healTracerIndex].Activate(start, end);

        healTracerIndex++;

        if (healTracerIndex >= healTracers.Length)
            healTracerIndex = 0;
    }
}
