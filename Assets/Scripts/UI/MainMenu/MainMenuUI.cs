using UnityEngine;
using UnityEngine.UI;

class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    async void StartGame()
    {
        Debug.Log("Starting Game from MainMenuUI");
        await GameManager.Instance.StartGame();
    }
}