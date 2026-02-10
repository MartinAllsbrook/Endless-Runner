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

    [Header("Map")]
    [SerializeField] Sprite mapIcon;
    
    [Header("Road Connections")]
    [SerializeField] Transform[] roadConnectionPoints;
    
    [Header("Interaction")]
    [SerializeField] Canvas interactionCanvas;
    [SerializeField] Transform interactionCanvasPivot;

    ConnectionPoint[] connectionPoints;
    public int CountInWorld => countInWorld;
    public float Radius => radius;
    public Sprite MapIcon => mapIcon;

    List<PointOfInterest> connectedPOIs = new List<PointOfInterest>();
    public List<PointOfInterest> ConnectedPOIs => connectedPOIs;
    
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

    void Awake()
    {
        connectionPoints = new ConnectionPoint[roadConnectionPoints.Length];
        for (int i = 0; i < roadConnectionPoints.Length; i++)
        {
            connectionPoints[i] = new ConnectionPoint { Point = roadConnectionPoints[i], Connected = false };
        }
    }

    void Update()
    {
        if (interactionCanvasPivot != null)
        {
            interactionCanvasPivot.rotation = Quaternion.identity; // Keep canvas upright
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

    protected virtual void OnPlayerEnter()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(true);
            // EventSystem.current.SetSelectedGameObject(interactionCanvas.gameObject); // IDK what this does
        }
    }

    protected virtual void OnPlayerExit()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
        }
    }
}