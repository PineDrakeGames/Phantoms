using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSavePoint : Interactable
{
    public override void Interact()
    {
        DataManager.Instance.FullHeal();
        NotificationManager.SetBottomNotification("<b>Checkpoint!</b>\nYou take a moment to rest up and heal!\n(Normally this would also save, but that's not set up quite yet.");
    }
}
