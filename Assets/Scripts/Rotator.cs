using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateSpeed = 3f;

	void Update () 
    {
        target.Rotate(Vector3.up, rotateSpeed*Time.deltaTime);	    
	}
}
