using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro TMP;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float DisapearTime;
    [SerializeField] private float DisappearSpeed;
    [SerializeField] private Gradient ColorGradient;
    [SerializeField] private int MaxDamage;


    public static GameObject Create(Vector2 pos, int Damage)
    {
        GameObject Popup = Instantiate(GameAssets.g.DamagePopup, pos, Quaternion.identity);
        DamagePopup damagePopup = Popup.GetComponent<DamagePopup>();
        damagePopup.Setup(Damage);
        return Popup;
    }

    private void Awake()
    {
        TMP = gameObject.GetComponent<TextMeshPro>();
    }

    public void Setup(int Damage)
    {
        TMP.text = Damage.ToString();
        float p = (float)Damage / (float)MaxDamage;
        TMP.color = ColorGradient.Evaluate(p);
    }

    private void Update()
    {
        DisapearTime -= Time.deltaTime;
        if (DisapearTime < 0)
        {
            TMP.alpha -= DisappearSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(0, moveSpeed * Time.deltaTime);
        }
        if (TMP.alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
