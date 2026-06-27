using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject PanelRoot;
    public TMP_Text TitleText;
    public Button RestartButton;
    public Button MainMenuButton;

    public string DefeatTitle = "You Died";
    public string VictoryTitle = "Victory!";

    private void Awake()
    {
        PanelRoot.SetActive(false);
    }

    private void Start()
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        GameStateManager.Instance.OnStateChanged += HandleStateChanged;
        RestartButton.onClick.AddListener(HandleRestartClicked);
        MainMenuButton.onClick.AddListener(HandleMainMenuClicked);
    }

    private void HandleStateChanged(GameState newState)
    {
        if (newState != GameState.GameOver) return;

        var reason = GameStateManager.Instance.LastGameOverReason;
        TitleText.text = reason == GameOverReason.Victory ? VictoryTitle : DefeatTitle;
        PanelRoot.SetActive(true);
    }

    private void HandleRestartClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void HandleMainMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= HandleStateChanged;
    }
}