using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLoadingTrigger : MonoBehaviour
{
    [Scene]
    [SerializeField]
    private string scene = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LoadingManager.LoadScene(scene, LoadingManager.SceneType.BATTLE);
        }
    }
}
