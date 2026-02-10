using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class POIInteractionButton : MonoBehaviour
{
    [SerializeField] UnityEvent onInteract;
    Button button;
    PointOfInterest currentPOI;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(DoInteraction);
    }

    void DoInteraction()
    {
        onInteract?.Invoke();
    }
}