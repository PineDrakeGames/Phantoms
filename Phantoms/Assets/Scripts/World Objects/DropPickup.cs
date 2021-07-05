using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPickup : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The amount that the player collects when they get this drop (most will be 1, but can be larger for bigger ones)")]
    private int m_dropAmount = 1;

    [SerializeField]
    private AudioClip m_dropPickupSoundEffect;

    
    // Called when the drop should be picked up
    public void Collect()
    {
        DataManager.CurrentDrops += m_dropAmount;

        AudioManager.PlaySound(m_dropPickupSoundEffect);

        // TODO: Play an animation instead
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            Collect();
        }
    }
}
