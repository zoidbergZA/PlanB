using UnityEngine;
using System.Collections;

public class CameraScript1 : MonoBehaviour {

	[SerializeField]
	Transform middlePoint;
	[SerializeField]
	Transform p1;

	[SerializeField]
	float angle;

	[SerializeField]
	GameObject middle;

	MiddlePoint mid;

	[SerializeField]
	private float camHeight = 7;
	
	void Start() {

		mid = middle.GetComponent<MiddlePoint>();
	}
	
	void Update () {

		if (mid.distance > 3) {
			this.transform.LookAt(middlePoint);
		} else {
			this.transform.LookAt(p1);
		}

		this.transform.rotation = Quaternion.Euler (angle, 0, 0);
		this.transform.position = new Vector3 (transform.position.x, mid.distance * camHeight, transform.position.z);
	}

}