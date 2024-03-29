using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{

    public Action<PointerEventData> OnClickHandler;
    public Action<PointerEventData> OnDragHandler;



    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointer Click");
        if(OnClickHandler != null)
            OnClickHandler.Invoke(eventData);    

    }

}
