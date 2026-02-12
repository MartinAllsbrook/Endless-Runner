using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class POIInteractionUI : MonoBehaviour
{
    [SerializeField] GameObject waitingToStopPanel;
    [SerializeField] InteractionUI interactionUIPrefab;
    [SerializeField] Button enterInteractionButton;

    bool playerMoving = false;
    bool inInteraction = false;

    CarMovement playerMovement;

    void OnEnable()
    {
        waitingToStopPanel.SetActive(true);

        enterInteractionButton.onClick.AddListener(EnterInteraction);

        if (Player.Instance != null)
        {
            playerMovement = Player.Instance.GetComponent<CarMovement>();
        }
    }

    void OnDisable()
    {
        enterInteractionButton.onClick.RemoveListener(EnterInteraction);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity; // Keep canvas upright

        CheckIfMoving();
    }

    void CheckIfMoving()
    {
        if (playerMovement == null) return;
    
        if (playerMovement.IsMoving() && !playerMoving)
        {
            playerMoving = true;
            AllowInteraction(false);
        }
        else if (!playerMovement.IsMoving() && playerMoving)
        {
            playerMoving = false;
            AllowInteraction(true);
        }
    }

    void AllowInteraction(bool enable)
    {
        if (enable)
        {
            enterInteractionButton.interactable = true;
            enterInteractionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Interact";
        }
        else
        {
            enterInteractionButton.interactable = false;
            enterInteractionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop to Interact";
        }
    }

    void EnterInteraction()
    {
        inInteraction = true;
        playerMovement.EnableMovement(false);
        waitingToStopPanel.SetActive(false);

        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.EnterInteraction(interactionUIPrefab);
        }
        PlayerHUD.OnExitInteraction += ExitInteraction;
    }

    void ExitInteraction()
    {
        inInteraction = false;
        playerMovement.EnableMovement(true);
        waitingToStopPanel.SetActive(true);

        PlayerHUD.OnExitInteraction -= ExitInteraction;
    }
}