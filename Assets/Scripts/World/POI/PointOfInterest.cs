using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

struct ConnectionPoint
{
    public Transform Point;
    public bool Connected;
}

[RequireComponent(typeof(Collider2D))]
abstract class PointOfInterest : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] int countInWorld = 3;
    [SerializeField] float radius = 10f;

    [Header("General References")]
    [SerializeField] Sprite mapIcon;
    [SerializeField] Transform[] roadConnectionPoints;
    [SerializeField] POIInteractionUI interactionUI;

    ConnectionPoint[] connectionPoints;
    public int CountInWorld => countInWorld;
    public float Radius => radius;
    public Sprite MapIcon => mapIcon;

    List<PointOfInterest> connectedPOIs = new List<PointOfInterest>();
    public List<PointOfInterest> ConnectedPOIs => connectedPOIs;
    
    bool playerInside = false;

    public bool AllPointsConnected
    {
        get
        {
            foreach (var cp in connectionPoints)
            {
                if (!cp.Connected)
                    return false;
            }
            return true;
        }
    }

    public ConnectionPoint[] ConnectionPoints => connectionPoints;

    #region Unity

    void Awake()
    {
        interactionUI.gameObject.SetActive(false);

        connectionPoints = new ConnectionPoint[roadConnectionPoints.Length];
        for (int i = 0; i < roadConnectionPoints.Length; i++)
        {
            connectionPoints[i] = new ConnectionPoint { Point = roadConnectionPoints[i], Connected = false };
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerExit();
        }
    }

    #endregion

    protected virtual void OnPlayerEnter()
    {
        if (interactionUI != null)
        {
            interactionUI.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(interactionUI.gameObject);
        }
    }

    protected virtual void OnPlayerExit()
    {
        if (interactionUI != null)
        {
            interactionUI.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}