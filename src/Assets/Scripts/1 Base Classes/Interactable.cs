using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private float PopupRadius; // Radius where popup will come up and player will be able to interact

    protected bool InRadius; // Whether the player is currently within the popup radius

    protected int CurrentCost; // Cost to interact

    protected GameObject Popup; // GameObject where the popup is held

    protected BuyPopupScript bps; // The script attached to the popup

    protected bool PopupActive; // Whether the popup is allowed to popup if the player is within radius (the upgrade table will intitrally not be accessible)

    protected virtual void Start()
    {
        GameAssets.g.PlayerInputActions.Player.Interact.Enable();
        GameAssets.g.PlayerInputActions.Player.Interact.performed += TryToInteract;

        InRadius = false;
        PopupActive = true;
    }

    protected virtual void Update()
    {
        float Distance = Vector2.Distance(GameAssets.g.Player.transform.position, gameObject.transform.position);
        if ((Distance < PopupRadius && !InRadius) || (Distance > PopupRadius && InRadius))
        {
            InRadius = !InRadius;
            if (PopupActive)
            {
                Popup.SetActive(InRadius);
            }

        }
    }

    abstract protected void Interact();

    protected virtual void CreatePopup() // Method to intilise the popup
    {
        Popup = Instantiate(GameAssets.g.BuyPopup, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 2), Quaternion.identity, gameObject.transform);
        bps = Popup.GetComponent<BuyPopupScript>();
        Popup.SetActive(InRadius);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, PopupRadius);
    }

    protected void TryToInteract(InputAction.CallbackContext context) // When player presses the interact input
    {
        if (context.performed && InRadius)
        {
            if (GameAssets.g.PlayerMoneySystem.CanBuy(CurrentCost))
            {
                Interact();
            }
            else
            {
                GameAssets.g.UIMethods.FlashMoneyRed();
            }
        }
    }

    protected void DestoryInteractable() // Used when gameobject shouldbe destroyed entirely
    {
        DisableInteractable();
        Destroy(gameObject);
    }

    protected void DisableInteractable() // Used when gameobject should be kept in place but should not be usuable
    {
        GameAssets.g.PlayerInputActions.Player.Interact.performed -= TryToInteract;
        if (Popup != null)
        {
            Destroy(Popup);
        }
        Destroy(this);
    }

    protected void SpendMoneyCurrentCost() // Spend the money assocaited with the interactable
    {
        AudioManager.g.Play("Buy");
        GameAssets.g.PlayerMoneySystem.ChangeMoney(-CurrentCost);
    }
}
