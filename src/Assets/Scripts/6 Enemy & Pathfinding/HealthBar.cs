using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("Transform")]

    private Transform AttachedTo;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private Transform HealthBarFill;


    [Header("Timers")]

    [SerializeField] private float HealthBarTimer;
    private float CurrentHealthBarTimer;
    private bool Active;

    [SerializeField] private float TimeToDisappear;
    private float CurrentTimeToDisappear;
    private bool Disappearing;


    [Header("Color")]

    [SerializeField] private float MaxAlpha;
    [SerializeField] private SpriteRenderer[] AllSprites;

    // Health bar will activate, go through health bar timer then go through time to disappear which will change alpha

    private void Start()
    {
        Active = false;
        Disappearing = false;
        CurrentTimeToDisappear = CurrentHealthBarTimer = 0;
        Disappear();
    }

    private void Update()
    {
        gameObject.transform.SetPositionAndRotation(AttachedTo.position + Offset, Quaternion.identity);

        if (Active)
        {
            CurrentHealthBarTimer -= Time.deltaTime;
            if (CurrentHealthBarTimer <= 0)
            {
                CurrentHealthBarTimer = 0;
                Disappearing = true;
                CurrentTimeToDisappear = TimeToDisappear;
                Active = false;
            }
        }

        if (Disappearing)
        {
            CurrentTimeToDisappear -= Time.deltaTime;
            Disappear();
            if (CurrentTimeToDisappear <= 0)
            {
                Disappearing = false;
                CurrentTimeToDisappear = 0;
                Disappear();
            }
        }
    }

    public void Activate(float Health, float MaxHealth)
    {
        Active = true;
        Disappearing = false;
        CurrentHealthBarTimer = HealthBarTimer;
        SetColorNormal();
        SetSlider(Health / MaxHealth);
    }

    private void Disappear()
    {
        float alpha = (CurrentTimeToDisappear / TimeToDisappear) * (MaxAlpha / 255);
        foreach (SpriteRenderer s in AllSprites)
        {
            s.color = new Color(s.color.r, s.color.g, s.color.b, alpha);
        }
    }

    private void SetColorNormal()
    {
        foreach (SpriteRenderer s in AllSprites)
        {
            s.color = new Color(s.color.r, s.color.g, s.color.b, MaxAlpha / 255);
        }
    }

    public void SetSlider(float percent)
    {
        if (percent >= 0 && percent <= 1)
        {
            HealthBarFill.localScale = new Vector3(percent, 1, 1);
        }
    }

    public void SetAttachedTo(Transform tf)
    {
        if (tf != null)
        {
            AttachedTo = tf;
        }
    }
}
