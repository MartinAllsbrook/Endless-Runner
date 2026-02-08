using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }
    private @InputSystemActions controls;

    // Input events
    public static event Action<float> Move = delegate { };
    public static event Action<bool> Shoot = delegate { };
    public static event Action<float> Throttle = delegate { };
    public static event Action OnToggleMap = delegate { };

    void Awake()
    {
        Debug.Log("Initializing InputReader Singleton");

        if (Instance == null) 
            Instance = this;
        
        controls = new @InputSystemActions();

        LinkEvents();
    }

    void OnEnable() 
    {
        controls.Enable();
    }

    void OnDisable() 
    {
        controls.Disable();
    }

    void LinkEvents()
    {
        controls.Default.Move.performed += ctx => Move(ctx.ReadValue<float>());
        controls.Default.Move.canceled += ctx => Move(0f);

        controls.Default.Shoot.performed += ctx => Shoot(true);
        controls.Default.Shoot.canceled += ctx => Shoot(false);

        controls.Default.Throttle.performed += ctx => Throttle(ctx.ReadValue<float>());
        controls.Default.Throttle.canceled += ctx => Throttle(0f);
    
        controls.Default.ToggleMap.performed += ctx => OnToggleMap();
    }
}
