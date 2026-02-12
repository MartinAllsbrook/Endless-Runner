using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] Button exitButton;
    public event Action OnExit = delegate { };

    protected virtual void OnEnable()
    {
        exitButton.onClick.AddListener(Exit);
    }

    protected virtual void OnDisable()
    {
        exitButton.onClick.RemoveListener(Exit);
    }

    protected void Exit()
    {
        OnExit.Invoke();
    }    
}