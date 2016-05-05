using UnityEngine;
using System.Collections;

public class VerticalTracer : MonoBehaviour 
{
    void Start()
    {
        GameManager.Instance.TracerHelper.ShowTracer(transform.position, transform.position + Vector3.up * 30f);
    }
}
