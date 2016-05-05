using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Text textObject;

    [SerializeField] private float riseSpeed = 3f;

    void Awake()
    {
        transform.rotation = Camera.main.transform.rotation;

        //randomize rise speed
        riseSpeed += Random.Range(-0.5f, 1.9f);
    }

	void Update () 
    {
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
        transform.rotation = Camera.main.transform.rotation;

	    riseSpeed *= 0.95f;
    }
}
