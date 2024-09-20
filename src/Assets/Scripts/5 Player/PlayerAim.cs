using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour // OBJECTIVE 14
{
    [Header("Aiming")]

    private Vector2 MousePosition;
    private float PlayerRotationAngle;
    private Rigidbody2D PlayerRB; // Reference to collider attribute of the player
    public bool Dead { private get; set; }


    [Header("Camera")]

    public Camera MainCamera; // Reference to MainCamera

    private void Awake()
    {
        PlayerRB = GetComponent<Rigidbody2D>();
        Dead = false;
    }

    private void Update() // Called once per frame
    {
        ProcessMouseInputs();
    }

    private void FixedUpdate()
    {
        if (!Dead)
        {
            RotatePlayer();
        }
    }

    private void ProcessMouseInputs() // OBJECTIVE 14.1/2
    {
        MousePosition = MainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 PlayerToMouse = MousePosition - PlayerRB.position;
        float angleRad = Mathf.Atan2(PlayerToMouse.y, PlayerToMouse.x);
        PlayerRotationAngle = angleRad * Mathf.Rad2Deg - 90;
    }

    private void RotatePlayer()
    {
        PlayerRB.rotation = PlayerRotationAngle;
    }
}
