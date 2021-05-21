using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;// Required when using Event data.

public class UIItemHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    public UnityEvent<GameObject> PointerEnter = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> PointerExit = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> Select = new UnityEvent<GameObject>();

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter.Invoke(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit.Invoke(this.gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Select.Invoke(this.gameObject);
    }
}
