using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// OBJECTIVE 20.1
public class RoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoints
    {
        [SerializeField] public List<GameObject> SP;
    }

    [Header("Spawning")]

    [SerializeField] private List<SpawnPoints> AllSpawnPoints = new List<SpawnPoints>();
    // OBJECTIVE 20.3

    [SerializeField] private int StartingRound;

    private List<Transform> ValidSpawnPoints;

    [SerializeField] private float MinSpawnRadius, MaxSpawnRadius;

    private int Round;

    private int EnemiesToSpawn; // OBJECTIVE 20.2

    private int MaxEnemiesAlive; // OBJECTIVE 20.4



    [Header("Times")]

    [SerializeField] private float TimeBetweenSpawns;

    private float CurrentTimeBetweenSpawns;



    [Header("UI")]

    [SerializeField] private Text RoundNumber;



    [Header("Enemie GOs")]

    private List<GameObject> EnemiesInRound;



    [Header("Enemy Speed")]

    [SerializeField] private float MaxSpeedMultiple;

    [SerializeField] private int RoundMaxSpeedReached;



    [Header("Singleton")]

    private static RoundManager instance;
    public static RoundManager g { get; private set;  } 



    [Header("Areas")]

    public bool[] AreasOpen = new bool[5];



    [Header("Player")]

    [SerializeField] private GameObject Player;

    [Header("Money")]

    [SerializeField] private int MoneyPerRound;

    /*
    0: Starting area
    1: Blue area
    2: Green area
    3: Orange area
    4: Purple area
    */

    private void Awake()
    {
        if (g == null)
        {
            g = GetComponent<RoundManager>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    void Start()
    {
        Round = StartingRound - 1;
        EnemiesToSpawn = 0;
        EnemiesInRound = new List<GameObject>();
        ValidSpawnPoints = new List<Transform>();
        AreasOpen[0] = true;
    }

    void Update()
    {
        gameObject.transform.position = GameAssets.g.Player.transform.position;

        if (EnemiesToSpawn == 0)
        {
            CurrentTimeBetweenSpawns = TimeBetweenSpawns;
            if (EnemiesInRound.Count == 0)
            {
                OnNewWave();
            }
        }
        else if (EnemiesToSpawn > 0 && EnemiesInRound.Count < MaxEnemiesAlive)
        {
            if (CurrentTimeBetweenSpawns <= 0)
            {
                SpawnEnemy();
                EnemiesToSpawn--;
                CurrentTimeBetweenSpawns = TimeBetweenSpawns;
            }
            else
            {
                CurrentTimeBetweenSpawns -= Time.deltaTime;
            }
        }


    }

    private Transform FindEnemySpawnPoint()
    {
        ValidSpawnPoints.Clear();
        bool InRangeFound = false;
        float MinDistance = float.MaxValue;
        Transform MinDisSP = null;

        for (int i = 0; i < AllSpawnPoints.Count; i++)
        {
            if (AreasOpen[i])
            {
                for (int j = 0; j < AllSpawnPoints[i].SP.Count; j++)
                {
                    float distance = Vector2.Distance(GameAssets.g.Player.transform.position, AllSpawnPoints[i].SP[j].transform.position);
                    if (distance >= MinSpawnRadius && distance <= MaxSpawnRadius)
                    {
                        // If SP is in range
                        ValidSpawnPoints.Add(AllSpawnPoints[i].SP[j].transform);
                        InRangeFound = true;
                    }
                    if (!InRangeFound && distance < MinDistance)
                    {
                        MinDistance = distance;
                        MinDisSP = AllSpawnPoints[i].SP[j].transform;
                    }
                }
            }
        }
        if (!InRangeFound)
        {
            return MinDisSP;
        }
        if (ValidSpawnPoints.Count == 0)
        {
            Debug.LogError($"Error occured whilst getting spawn points in range of player");
            return null;
        }
        else
        {
            return ValidSpawnPoints[UnityEngine.Random.Range(0, ValidSpawnPoints.Count)];
        }
    }

    private void OnNewWave() // OBJECTIVE 20.5
    {
        Round++;
        GameAssets.g.UIMethods.ChangeRoundText(Round);
        GameAssets.g.UIMethods.FlashNewRound();
        EnemiesToSpawn = CalcualteEnemiesToSpawnInRound(); // enemies in round
        MaxEnemiesAlive = CalculateMaxZombiesAtOnce(); // max number of enemies in round
        TimeBetweenSpawns = CalculateTimeBetweenEnemySpawns(); // time between spawns
        GameAssets.g.PlayerMoneySystem.ChangeMoney(MoneyPerRound); // OBJECTIVE 21.3

    }

    #region Calculate Methods

    private float CalculateEnemySpeed() // OBJECTIVE 19.1
    {
        if (Round >= RoundMaxSpeedReached)
        {
            return MaxSpeedMultiple;
        }
        // using points (1,1) & (RoundMaxSpeedReached, MaxSpeedMultiple)
        float m = (MaxSpeedMultiple - 1) / (RoundMaxSpeedReached - 1); // dy/dx
        float c = 1 - m; //c = y - mx

        return m * Round + c;
    }

    private int CalculateEnemyHealth() // OBJECTIVE 19.3
    {
        if (Round >= 1 && Round <= 9)
        {
            return 100 * Round + 50;
        }
        else if (Round >= 10 && Round <= 45)
        {
            return (int)(950 * Mathf.Pow(1.1f, Round - 9));
        }
        else if (Round >= 46)
        {
            return 30000;
        }
        Debug.LogError($"Round value: {Round} is outside bounds of health calculation");
        return 0;

    }

    private int CalcualteEnemiesToSpawnInRound() // OBJECTIVE 19.2
    {
        if (Round >= 1 && Round <= 30)
        {
            return 3 * Round;
        }
        else if (Round > 30)
        {
            return 10 * Round - 210;
        }
        Debug.LogError($"Round value: {Round} is outside bounds of enemies to spawn calculation");
        return 0;
    }

    private float CalculateTimeBetweenEnemySpawns() // OBJECTIVE 19
    {
        if (Round >= 1)
        {
            return 0.0625f * Mathf.Pow((4 - Round / 8), 2) + 0.25f;
        }
        Debug.LogError($"Round value: {Round} is outside bounds of enemy spawn rate calculation");
        return 0;
    }

    private int CalculateMaxZombiesAtOnce() // OBJECTIVE 19.2
    {
        if (Round >= 1 && Round <= 26)
        {
            return Round / 2 + 2;
        }
        else if (Round > 26)
        {
            return 15;
        }

        Debug.LogError($"Round value: {Round} is outside bounds of enemy spawn rate calculation");
        return 0;
    }

    private int CalculateEnemyDamage() // OBJECTIVE 19.4
    {
        if (Round >= 1)
        {
            return 49 + (int)(1 - 50f * Mathf.Exp(-0.25f * Round));
        }
        Debug.LogError($"Round value: {Round} is outside bounds of enemy damage calculation");
        return 0;
    }

    private float CalculateTimeBeforeEnemyAttacks() // OBJECTIVE 19.4
    {
        if (Round >= 1)
        {
            return 0.5f + 0.75f * Mathf.Exp(-Round / 5);
        }
        Debug.LogError($"Round value: {Round} is outside bounds of enemy time before attack calculation");
        return 0;
    }

    private float CalculateTimeBetweenEnemyAttacks() // OBJECTIVE 19.4
    {
        if (Round >= 1)
        {
            return 0.6f + 0.75f * Mathf.Exp(-Round / 4);
        }

        Debug.LogError($"Round value: {Round} is outside bounds of enemy time between attack calculation");
        return 0;
    }

    #endregion

    private void SpawnEnemy()
    {
        GameObject SpawnedEnemy = Instantiate(GameAssets.g.Enemy, FindEnemySpawnPoint().position, Quaternion.identity);
        EnemiesInRound.Add(SpawnedEnemy);


        //speed
        SpawnedEnemy.GetComponent<EnemyFollow>().SetSpeedToPercent(CalculateEnemySpeed());
        SpawnedEnemy.GetComponent<EnemyFollow>().SetTarget(Player);


        //damage, tBeforeA, tBetweenA, health
        SpawnedEnemy.GetComponent<EnemyCombat>().InitiliaseProperties(CalculateEnemyDamage(), CalculateTimeBeforeEnemyAttacks(), CalculateTimeBetweenEnemyAttacks(), CalculateEnemyHealth());

        
    }

    public void RemoveEnemy(GameObject go)
    {
        if (!EnemiesInRound.Contains(go))
        {
            Debug.LogWarning("Trying to remove element which doesn't exist");
            return;
        }
        EnemiesInRound.Remove(go);
    }

    public void DoorOpen(int door) // OBJECTIVE 28.3
    {
        /*
        0: Starting -> blue
        1: blue -> green
        2: blue -> orange
        3: orange -> purple
        4: green -> purple
        */
        if (door == 0)
        {
            AreasOpen[0] = AreasOpen[1] = true;
        }
        else if (door == 1)
        {
            AreasOpen[1] = AreasOpen[2] = true;
        }
        else if (door == 2)
        {
            AreasOpen[1] = AreasOpen[3] = true;
        }
        else if (door == 3)
        {
            AreasOpen[3] = AreasOpen[4] = true;
        }
        else if (door == 4)
        {
            AreasOpen[2] = AreasOpen[4] = true;
        }
        else
        {
            Debug.LogError($"Invalid door ID: {door}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (gameObject.transform.position == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Player.transform.position, MinSpawnRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Player.transform.position, MaxSpawnRadius);

    }
}
