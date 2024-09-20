using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;

    [SerializeField] private TMP_Text Volume;

    [SerializeField] private TMP_Dropdown ResolutionDropdown;

    [SerializeField] private Toggle FullscreenToggle;

    [SerializeField] private Button BackButton;

    [SerializeField] private GameObject Main, Settings;

    private void Start()
    {
        VolumeSlider.onValueChanged.AddListener(AudioManager.g.ChangeMasterVolume); // OBJECTIVE 36

        VolumeSlider.onValueChanged.AddListener(OnVolumeChange);

        ResolutionDropdown.onValueChanged.AddListener(ResolutionChanged);

        BackButton.onClick.AddListener(Back);

        FullscreenToggle.onValueChanged.AddListener(FullScreenChanged);

        SetupSettings();
    }

    private void Back()
    {
        AudioManager.g.Play("Click");
        Main.SetActive(true);


        SaveLoadSystem.SaveData(AudioManager.g.GetMasterVolume(), Screen.width, Screen.height, Screen.fullScreen);

        Settings.SetActive(false);
    }

    public void SetupSettings()
    {
        ResolutionDropdown.value = GetResolutionOption(Screen.width, Screen.height);
        FullscreenToggle.isOn = Screen.fullScreen;
        VolumeSlider.value = AudioManager.g.GetMasterVolume();
        OnVolumeChange(AudioManager.g.GetMasterVolume());
    }

    private int GetResolutionOption(int width, int height)
    {
        if (width == 1920 && height == 1080)
        {
            return 0;
        }
        else if (width == 1280 && height == 720)
        {
            return 1;
        }
        else if (width == 640 && height == 480)
        {
            return 2;
        }
        else
        {
            Debug.LogError($"Width {width} or/and height {height} not applicable");
            return -1;
        }

    }

    private void ResolutionChanged(int option)
    {
        int[] widths = { 1920, 1280, 640 };
        int[] heights = { 1080, 720, 480 };

        Screen.SetResolution(widths[option], heights[option], FullscreenToggle.isOn);
        AudioManager.g.Play("Toggle");
    }

    private void FullScreenChanged(bool option)
    {
        AudioManager.g.Play("Toggle");
        Screen.fullScreen = option;
    }

    private void OnVolumeChange(float v)
    {
        Volume.text = ((int)(v * 100)).ToString();
    }
}
