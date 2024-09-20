using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    [Header("PlayerScripts")]

    private EntityColourSetter PlayerColourSetter;


    [Header("Initial Values")]

    [SerializeField] private Vector2 InitialPlayerPosition;


    [Header("Colors")]

    [SerializeField] private Color StartingMainPlayerColor;
    [SerializeField] private Color StartingUnderPlayerColor;

    [Header("Game Over")]

    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject MainGameUI;
    [SerializeField] private GameObject PauseUI;

    [Header("Music")]

    private bool intro;

    private AudioClip CurrentClip;



    private void Awake() // Similar to constructor
    {
        PlayerColourSetter = GameObject.Find("Player").GetComponent<EntityColourSetter>();
    }
    private void Start()
    {
        PlayerColourSetter.ChangePlayerColour(StartingMainPlayerColor, StartingUnderPlayerColor);

        intro = true;
        AudioManager.g.Play("Load In Music");

    }

    private void Update()
    {
        Music();
    }

    private void Music()
    {
        if ((intro && AudioManager.g.IsPlaying("Load In Music")) || (!intro && AudioManager.g.IsPlaying("Main Music")))
        {
            return; // If playing music continue
        }
        else if (intro && !AudioManager.g.IsPlaying("Load In Music")) // If intro music ended
        {
            intro = false;
            AudioManager.g.Play("Main Music");
            CurrentClip = AudioManager.g.GetClip("Main Music");
            return;
        }
        else if (!intro && !AudioManager.g.IsPlaying("Main Music")) // Play new main menu music
        {
            do
            {
                AudioManager.g.PlayNewClip("Main Music");
            }
            while (AudioManager.g.GetClip("Main Music") == CurrentClip);
            CurrentClip = AudioManager.g.GetClip("Main Music");

        }

    }



    public IEnumerator GameOver()
    {
        GameAssets.g.PlayerMovement.enabled = false;
        GameAssets.g.PlayerRB.velocity = Vector2.zero;
        GameAssets.g.PlayerAim.enabled = false;
        GameAssets.g.PlayerCombat.enabled = false;

        GameAssets.g.UIMethods.TurnOnGameOverUI();
        GameAssets.g.UIMethods.ChangeGameOverScore(GameAssets.g.PlayerMoneySystem.TotalMoney); // OBJECTIVE 12.2.1
        

        SaveLoadSystem.SaveData(GameAssets.g.PlayerMoneySystem.TotalMoney);

        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}