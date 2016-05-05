using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class FloorLight : MonoBehaviour
{
//    [SerializeField] private float startIntensity;
    [SerializeField] private float endIntensity;

    private Light floorLight;

    void Awake()
    {
        floorLight = GetComponent<Light>();
    }

    void Update()
    {
        float healthFrac = GameManager.Instance.HomeTower.Hitpoints/GameManager.Instance.HomeTower.MaxHP;

        floorLight.intensity = endIntensity*healthFrac;

//        if (healthFrac >= activateAt)
//            floorLight.enabled = true;
//        else
//            floorLight.enabled = false;
    }
}
