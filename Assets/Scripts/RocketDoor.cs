using UnityEngine;
using System.Collections;

public class RocketDoor : MonoBehaviour 
{
    void OnTriggerEnter(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if (player)
        {
            GameManager.Instance.GameVictory();
        }
    }
}
