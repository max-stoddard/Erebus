using UnityEngine;

public class EntityColourSetter : MonoBehaviour
{
    [Header("Sprites")]

    [SerializeField] private SpriteRenderer[] PrimaryObjects;
    [SerializeField] private SpriteRenderer[] SecondaryObjects;

    public Color PrimaryColor;
    public Color SecondaryColor;

    private void Start()
    {
        ChangePlayerColour(PrimaryColor, SecondaryColor);
    }

    public void ChangePlayerColour(Color MainColour, Color UnderColour)
    {
        foreach (SpriteRenderer obj in PrimaryObjects)
        {
            obj.color = MainColour;
        }
        foreach (SpriteRenderer obj in SecondaryObjects)
        {
            obj.color = UnderColour;
        }
        PrimaryColor = MainColour;
        SecondaryColor = UnderColour;
    }


}
