using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class POIInteractionUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject waitingToStopPanel;
    [SerializeField] GameObject interactionPanel;

    [Header("Buttons")]
    [SerializeField] Button enterInteractionButton;
    [SerializeField] Button exitInteractionButton;

    bool playerMoving = false;
    bool inInteraction = false;

    CarMovement playerMovement;

    void OnEnable()
    {
        waitingToStopPanel.SetActive(true);
        interactionPanel.SetActive(false);

        exitInteractionButton.onClick.AddListener(ExitInteraction);
        enterInteractionButton.onClick.AddListener(EnterInteraction);

        if (Player.Instance != null)
        {
            playerMovement = Player.Instance.GetComponent<CarMovement>();
        }
    }

    void OnDisable()
    {
        exitInteractionButton.onClick.RemoveListener(ExitInteraction);
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
        interactionPanel.SetActive(true);
    }

    void ExitInteraction()
    {
        inInteraction = false;
        playerMovement.EnableMovement(true);

        waitingToStopPanel.SetActive(true);
        interactionPanel.SetActive(false);
    }
}