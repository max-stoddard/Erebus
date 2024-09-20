using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    private bool Paused = false;

    InputActions PlayerInputActions;

    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject PauseUIButtons;
    [SerializeField] private GameObject MainUI;
    [SerializeField] private GameObject SettingUI;

    [SerializeField] private Button QuitButton; // OBJECTIVE 11
    [SerializeField] private Button ResumeButton; // OBJECTIVE 10
    [SerializeField] private Button SettingButton;


    private void Start()
    {
        PlayerInputActions = GameAssets.g.PlayerInputActions;

        PlayerInputActions.UI.Pause.Enable();
        PlayerInputActions.UI.Pause.started += PauseInput;

        ResumeButton.onClick.AddListener(Unpause);
        QuitButton.onClick.AddListener(Quit);
        SettingButton.onClick.AddListener(Setting);
    }

    private void PauseInput(InputAction.CallbackContext context) // OBJECTIVE 9
    {
        if (!Paused) Pause();
        else Unpause();
    }

    private void Pause()
    {
        GameAssets.g.UIMethods.TurnOnPauseUI();
        PlayerInputActions.Player.Disable();

        Time.timeScale = 0.0f;
        Paused = true;
        AudioManager.g.Play("Toggle");
    }

    private void Unpause()
    {
        GameAssets.g.UIMethods.TurnOnMainUI();
        PlayerInputActions.Player.Enable();

        Time.timeScale = 1.0f;
        Paused = false;

        AudioManager.g.Play("Toggle");
    }

    private void Quit() // OBJECTIVE 11.1
    {
        GameAssets.g.PlayerHealthSystem.Kill();
        Unpause();
        AudioManager.g.Play("Click");
    }

    private void Setting()
    {
        GameAssets.g.UIMethods.TurnOnSettingsUI();

        AudioManager.g.Play("Click");
    }
}
