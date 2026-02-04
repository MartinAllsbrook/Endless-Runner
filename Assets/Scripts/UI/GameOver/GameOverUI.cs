using UnityEngine;
using UnityEngine.UI;

class GameOverUI : MonoBehaviour
{
    [SerializeField] Button restartButton;

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
    }

    async void RestartGame()
    {
        Debug.Log("Restarting Game from GameOverUI");
        await GameManager.Instance.RestartGame();
    }    

}