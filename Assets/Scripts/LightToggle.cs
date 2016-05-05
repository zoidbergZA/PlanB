using UnityEngine;
using System.Collections;

public class LightToggle : MonoBehaviour
{
    [SerializeField] private float toggleTime = 2f;

    private float timer;
    private Light toggleLight; //renamed because it gave warning

    void Awake()
    {
        toggleLight = GetComponent<Light>();
    }

	void Update ()
	{
	    timer -= Time.deltaTime;

	    if (timer <= 0)
	    {
	        timer = toggleTime;
	        toggleLight.enabled = !toggleLight.enabled;
	    }
	}
}
