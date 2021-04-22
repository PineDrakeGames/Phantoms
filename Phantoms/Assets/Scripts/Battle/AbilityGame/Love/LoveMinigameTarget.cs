using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveMinigameTarget : MonoBehaviour
{
    public LoveMinigame Minigame = null;
    
    public void Hit()
    {
        this.gameObject.SetActive(false);
    }
}
