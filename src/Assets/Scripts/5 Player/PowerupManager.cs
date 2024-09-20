using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [System.Serializable]
    public class Powerup
    {
        [HideInInspector] public bool active;
        public string name;
        public Sprite sprite;
        public int price;

    }

    /*

    0: Speed
    1: Health * 
    2: Reload speed *
    3: Firerate *
    4: Accuracy *

    */

    [SerializeField] private Powerup[] Powerups;

    public void PowerupActive(int powerup)
    {
        Powerups[powerup].active = true;
        GameAssets.g.UIMethods.IncreaseOpacity(powerup);
    }

    public bool IsInRange(int powerup)
    {
        if (powerup >= 0 && powerup < Powerups.Length)
        {
            return true;
        }
        return false;
    }

    public bool IsPowerupActive(int powerup)
    {
        if (!IsInRange(powerup))
        {
            Debug.LogError($"Powerup int: {powerup} is out of range");
            return false;
        }

        return Powerups[powerup].active;
    }

    public Powerup GetPowerup(int powerup)
    {
        if (!IsInRange(powerup))
        {
            Debug.LogError($"Powerup int: {powerup} is out of range");
            return null;
        }

        return Powerups[powerup];
    }
}
