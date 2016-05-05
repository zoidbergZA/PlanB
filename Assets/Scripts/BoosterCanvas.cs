using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoosterCanvas : MonoBehaviour 
{
    public Text NameText;

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
