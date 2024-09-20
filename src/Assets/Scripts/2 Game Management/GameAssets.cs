using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameAssets : MonoBehaviour
{
    private string SceneName; // Current scene

    
    public static GameAssets g { get; private set; }
    

    private void Awake()
    {
        if (g == null)
        {
            g = GetComponent<GameAssets>();
        }
        else
        {
            Debug.LogError("Multiple instances");
            Destroy(gameObject);
            return;
        }

        SceneName = SceneManager.GetActiveScene().name;

        if (SceneName == "Game")
        {
            Player = GameObject.Find("Player");

            MainGameUI = GameObject.Find("Main Game Canvas");
            UIMethods = GameObject.Find("UI").GetComponent<UIMethods>();

            PlayerMovement = Player.GetComponent<PlayerMovement>();
            PlayerAim = Player.GetComponent<PlayerAim>();
            PlayerCombat = Player.GetComponent<PlayerCombat>();
            PlayerCollider = Player.GetComponent<CircleCollider2D>();
            PlayerRB = Player.GetComponent<Rigidbody2D>();
            PlayerMoneySystem = Player.GetComponent<PlayerMoneySystem>();
            PlayerInputActions = new InputActions(); // Creates instance of our PlayerInputMap for our script
            PowerupManager = Player.GetComponent<PowerupManager>();
            PlayerHealthSystem = Player.GetComponent<PlayerHealthSystem>();
            
            RoundManager = GameObject.Find("Wave Manager").GetComponent<RoundManager>();

            GameManager = GameObject.Find("Game Manager").GetComponent<Game_Manager>();
        }

    }

    [Header("Player")]

    [NonSerialized] public GameObject Player;

    [NonSerialized] public PlayerMovement PlayerMovement;

    [NonSerialized] public PlayerAim PlayerAim;

    [NonSerialized] public PlayerCombat PlayerCombat;

    [NonSerialized] public CircleCollider2D PlayerCollider;

    [NonSerialized] public Rigidbody2D PlayerRB;

    [NonSerialized] public PlayerMoneySystem PlayerMoneySystem;

    [NonSerialized] public PowerupManager PowerupManager;

    [NonSerialized] public PlayerHealthSystem PlayerHealthSystem;


    [Header("Game")]

    [NonSerialized] public Game_Manager GameManager;


    [Header("Enemies")]

    public GameObject Enemy;

    public GameObject HealthBar;

    [NonSerialized] public RoundManager RoundManager;


    [Header("Combat")]

    public GameObject DamagePopup;

    public GameObject Bullet;


    [Header("Hit Effects")]

    public GameObject EnemyHitEffect;

    public GameObject ObjectHitEffect;

    public GameObject PlayerHitEffect;


    [Header("UI")]

    [NonSerialized] public GameObject MainGameUI;

    [NonSerialized] public UIMethods UIMethods;


    [Header("Interactables")]

    public GameObject BuyPopup;


    [Header("Input")]

    public InputActions PlayerInputActions; // Instance of PlayerInputActions to handle all input


}
