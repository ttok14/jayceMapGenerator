using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEventDelegator : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum EventType
    {
        PointerEnter,
        PointerExit,
        OneClick,
        DoubleClick
    }

    const float doubleClickValidInterval = 0.2f;

    public object param;
    public Dictionary<EventType, Action<object>> callbacks = new Dictionary<EventType, Action<object>>();

    float timeElapsedFromLastClick;
    bool readyTriggerDoubleClick;

    public void AddListener(EventType eventType, Action<object> listener)
    {
        callbacks[eventType] += listener;
    }

    public void RemoveListener(EventType eventType, Action<object> listener)
    {
        if (callbacks.ContainsKey(eventType))
        {
            callbacks[eventType] -= listener;
        }
    }

    public void ClearListener(EventType eventType)
    {
        if (callbacks.ContainsKey(eventType))
        {
            callbacks[eventType] = null;
        }
    }

    public void ClearListener()
    {
        foreach (var t in callbacks)
        {
            callbacks[t.Key] = null;
        }
    }

    void Update()
    {
        if (readyTriggerDoubleClick)
        {
            timeElapsedFromLastClick += Time.deltaTime;

            if (timeElapsedFromLastClick > doubleClickValidInterval)
            {
                readyTriggerDoubleClick = false;
                timeElapsedFromLastClick = 0;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (readyTriggerDoubleClick)
        {
            readyTriggerDoubleClick = false;

            if (callbacks.ContainsKey(EventType.DoubleClick) &&
                callbacks[EventType.DoubleClick] != null)
            {
                callbacks[EventType.DoubleClick](param);
            }
        }
        else
        {
            readyTriggerDoubleClick = true;
            timeElapsedFromLastClick = 0;

            if (callbacks.ContainsKey(EventType.OneClick) &&
                callbacks[EventType.OneClick] != null)
            {
                callbacks[EventType.OneClick](param);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (callbacks.ContainsKey(EventType.PointerEnter) &&
            callbacks[EventType.PointerEnter] != null)
        {
            callbacks[EventType.PointerEnter](param);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (callbacks.ContainsKey(EventType.PointerExit) &&
            callbacks[EventType.PointerExit] != null)
        {
            callbacks[EventType.PointerExit](param);
        }
    }
}
