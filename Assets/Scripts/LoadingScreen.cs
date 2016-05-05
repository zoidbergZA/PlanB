using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    void Start()
    {
        Application.LoadLevelAsync(sceneToLoad);
    }
}
