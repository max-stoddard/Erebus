using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _play; // OBJECTIVE 5
    [SerializeField] private Button _settings; // OBJECTIVE 6
    [SerializeField] private Button _exit; // OBJECTIVE 7

    [Header("Game Objects")]
    [SerializeField] private GameObject Main;
    [SerializeField] private GameObject SettingsMenu;

    [Header("Text")]
    [SerializeField] private TMP_Text HighScoreText;

    private void Start()
    {
        _play.onClick.AddListener(Play);
        _settings.onClick.AddListener(Settings);
        _exit.onClick.AddListener(Quit);

        UpdateHighScore();
        AudioManager.g.Play("Main Menu Music");

        LoadSavedOptions();
    }

    private void Play()
    {
        AudioManager.g.Play("Click");
        AudioManager.g.Stop("Main Menu Music");
        Debug.Log("Done");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // OBJECTIVE 5.1
    }

    private void Settings()
    {
        Main.SetActive(false);
        SettingsMenu.SetActive(true);
        SettingsMenu.GetComponent<SettingsMenu>().SetupSettings();
        AudioManager.g.Play("Click");
    }

    private void Quit()
    {
        AudioManager.g.Play("Click");
        Application.Quit(); // OBJECTIVE 7.1
    }

    public void UpdateHighScore()
    {
        int HighScore = SaveLoadSystem.LoadData().Score; // OBJECTIVE 8
        HighScoreText.text = HighScore.ToString();
    }

    private void LoadSavedOptions()
    {
        PlayerData data = SaveLoadSystem.LoadData();

        Screen.SetResolution(data.Width, data.Height, data.FullScreen);
        AudioManager.g.ChangeMasterVolume(data.Volume);
    }
}
