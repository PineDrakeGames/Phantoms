using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTrigger : MonoBehaviour
{
    [Scene]
    [SerializeField]
    private string scene = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LoadingManager.LoadSceneByPath(scene);
        }
    }
}
