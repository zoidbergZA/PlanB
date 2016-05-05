using UnityEngine;
using System.Collections;

public class MiddlePoint : MonoBehaviour {

	[SerializeField]
	Transform p1;
	[SerializeField]
	Transform p2;
	
	[SerializeField]
	private float maxClose = 3;
	[SerializeField]
	private float maxAppart = 10;

	public float distance;

	Vector3 DiffVector;

	void Start () {
		transform.position = new Vector3 (0, 0, 0);
		transform.rotation = Quaternion.Euler (0, 0, 0);
		transform.localScale = new Vector3 (1, 1, distance);
	}

	void Update () {
		DiffVector = p2.transform.position - p1.transform.position;
		distance = Mathf.Clamp(DiffVector.magnitude / 2, maxClose, maxAppart);
		transform.localScale = new Vector3 (1, 1, distance);
		this.transform.position = p1.transform.position + DiffVector / 2;
		this.transform.forward =  Quaternion.Euler(0, 135, 0) * DiffVector; 
	}
}
