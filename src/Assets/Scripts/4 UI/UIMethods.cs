using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMethods : MonoBehaviour
{
    [Header("Colors")]

    [SerializeField] private Color MainColor;
    [SerializeField] private Color LightColor;
    [SerializeField] private Color DarkColor;

    [Header("UI GameObjects")]

    [SerializeField] private GameObject MainUI;
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject PauseMenuUI;
    [SerializeField] private GameObject SettingMenuUI;
    [SerializeField] private GameObject PauseUIButtons;

    [Header("Main UI Objects")]

    [SerializeField] private Text Money;
    [SerializeField] private Text Score;
    [SerializeField] private Text Round;
    [SerializeField] private Text MagAmmo;
    [SerializeField] private Text ResAmmo;
    [SerializeField] private Text GunName;
    [SerializeField] private Image UIHealthBar;
    [SerializeField] private Text HealthText;

    [SerializeField] private Text[] UIGunNumbers;
    [SerializeField] private Image[] UIGunImages;
    [SerializeField] private Text[] UIGunNames;

    [SerializeField] private Image[] Powerups;

    [Header("Game Over UI Objects")]

    [SerializeField] private TMP_Text GameOverScore;

    #region ChangeUIElement
    public void ChangeMoneyText(int money)
    {
        if (money < 0)
        {
            Debug.LogError($"Negative money {money}");
            return;
        }
        Money.text = money.ToString();
    }

    public void ChangeScoreText(int score)
    {
        if (score < 0)
        {
            Debug.LogError($"Negative score {score}");
            return;
        }
        Score.text = score.ToString();
    }

    public void ChangeRoundText(int round) // OBJECTIVE 34.1
    {
        if (round < 0)
        {
            Debug.LogError($"Negative round {round}");
            return;
        }
        Round.text = round.ToString();
    }

    public void ChangeMainAmmoText(int mAmmo) // OBJECTIVE 32.1
    {
        if (mAmmo < 0)
        {
            Debug.LogError($"Negative main ammo {mAmmo}");
            return;
        }
        MagAmmo.text = mAmmo.ToString();
    }

    public void ChangeResAmmoText(int rAmmo) // OBJECTIVE 32.1
    {
        if (rAmmo < 0)
        {
            Debug.LogError($"Negative res ammo {rAmmo}");
            return;
        }
        ResAmmo.text = rAmmo.ToString();
    }

    public void ChangeGunName(string name) // OBJECTIVE 33.1
    {
        if (name == "")
        {
            Debug.LogError($"Empty gun name {name}");
            return;
        }
        GunName.text = name.ToString();
    }

    public void ChangeHealth(int health, float percent) // OBJECTIVE 30.1/2/3
    {
        if (percent < 0f || percent > 1f || health < 0)
        {
            Debug.LogError($"Health out of range %: {percent} & health: {health}");
            return;
        }
        UIHealthBar.fillAmount = percent;
        HealthText.text = health.ToString();
    }

    public void ChangeSlotColor(Color color, int slot)
    {
        if (color == null || !GameAssets.g.PlayerCombat.IsGunInSlot(slot) || !GameAssets.g.PlayerCombat.SlotInRange(slot))
        {
            Debug.LogError($"Error; color: {color}, GunInSlot: {GameAssets.g.PlayerCombat.IsGunInSlot(slot)}, SlotInRange: {GameAssets.g.PlayerCombat.SlotInRange(slot)}");
            return;
        }
        UIGunNames[slot].color = color;
        UIGunImages[slot].color = color;
        UIGunNumbers[slot].color = color;

    }

    public void ChangeGunInfo(int slot, Sprite sprite, string Name)
    {
        UIGunImages[slot].sprite = sprite;
        UIGunNames[slot].text = Name;
    }

    public void ChangeGunInfoEmpty(int slot) // Sets the gun info section if slot is empty
    {
        UIGunImages[slot].color = Color.clear;
        UIGunNames[slot].text = "";
    }

    public void ChangeGameOverScore(int score)
    {
        GameOverScore.text = score.ToString();
    }

    #endregion

    #region Flash Methods

    public void FlashMagAmmoRed()
    {
        StartCoroutine(FlashText(MagAmmo, .3f, LightColor, Color.red));
    }

    public void FlashResAmmoRed()
    {
        StartCoroutine(FlashText(ResAmmo, .3f, LightColor, Color.red));
    }

    public void FlashMoneyRed()
    {
        StartCoroutine(FlashText(Money, .3f, DarkColor, Color.red));
    }

    public void FlashNewRound()
    {
        StartCoroutine(FlashText(Round, 1.5f, LightColor, new Color(255f, 255f, 255f, 0f)));
    }


    public static IEnumerator FlashText(Text text, float time, Color startColour, Color flashColour)
    { // OBJECTIVE 32.2
        float t = 0;

        while (t < time / 2)
        {

            text.color = Color.Lerp(startColour, flashColour, t / (time / 2));
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < time / 2)
        {
            text.color = Color.Lerp(flashColour, startColour, t / (time / 2));
            t += Time.deltaTime;
            yield return null;
        }

        text.color = startColour;
    }

    #endregion

    #region Powerups

    public void IncreaseOpacity(int powerup) // OBJECTIVE 31.1
    {
        Color c = Powerups[powerup].color;
        c.a = 255f;
        Powerups[powerup].color = c;
    }


    #endregion

    #region Toggle UI 

    public void TurnOnGameOverUI()
    {
        MainUI.SetActive(false);
        GameOverUI.SetActive(true);
        PauseMenuUI.SetActive(false);
        SettingMenuUI.SetActive(false);
        PauseUIButtons.SetActive(false);
    }

    public void TurnOnPauseUI()
    {
        MainUI.SetActive(false);
        GameOverUI.SetActive(false);
        SettingMenuUI.SetActive(false);
        PauseMenuUI.SetActive(true);
        PauseUIButtons.SetActive(true);
    }

    public void TurnOnMainUI()
    {
        MainUI.SetActive(true);
        GameOverUI.SetActive(false);
        SettingMenuUI.SetActive(false);
        PauseMenuUI.SetActive(false);
        PauseUIButtons.SetActive(false);
    }

    public void TurnOnSettingsUI()
    {
        MainUI.SetActive(false);
        GameOverUI.SetActive(false);
        SettingMenuUI.SetActive(true);
        PauseMenuUI.SetActive(true);
        PauseUIButtons.SetActive(false);
        SettingMenuUI.GetComponent<SettingsMenu>().SetupSettings();
    }


    #endregion
}
