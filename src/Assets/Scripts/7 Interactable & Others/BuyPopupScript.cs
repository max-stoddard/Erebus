using TMPro;
using UnityEngine;

public class BuyPopupScript : MonoBehaviour
{
    [SerializeField] private TextMeshPro MainMessage, SideMessage, Key, Price;
    [SerializeField] private SpriteRenderer GunImage;

    public void ChangeMainMessage(string input)
    {
        MainMessage.text = input;
    }
    public void ChangeSideMessage(string input)
    {
        SideMessage.text = input;
    }

    public void ChangePrice(int input) // If positive buy, if negative sell
    {
        if (input == 0) // If item is free
        {
            Price.text = "";
        }
        else if (input > 0) // If item is to buy
        {
            Price.text = $"${input}";
        }
        else // If item is giving you money
        {
            Price.text = $"+${-input}";
        }

    }

    public void ChangeImage(Sprite s)
    {
        GunImage.sprite = s;
    }

}
